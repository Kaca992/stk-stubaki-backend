using StkStubaki.Business.DTO;
using StkStubaki.DatabaseModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using StkStubaki.Business.BO;
using StkStubaki.Business.Interfaces;

namespace StkStubaki.Business.Services
{
    public class CompetitionService
    {
        public readonly Dictionary<int, TeamCompetitionInfo> TeamCompetitionInfos = new Dictionary<int, TeamCompetitionInfo>();
        public readonly Dictionary<int, PlayerCompetitionInfo> PlayerCompetitionInfos = new Dictionary<int, PlayerCompetitionInfo>();

        public readonly Dictionary<HeadToHeadKey, HeadToHeadInfo<TeamCompetitionInfo>> TeamHeadToHeadInfos = new Dictionary<HeadToHeadKey, HeadToHeadInfo<TeamCompetitionInfo>>();
        public readonly Dictionary<HeadToHeadKey, HeadToHeadInfo<PlayerCompetitionInfo>> PlayerHeadToHeadInfos = new Dictionary<HeadToHeadKey, HeadToHeadInfo<PlayerCompetitionInfo>>();

        public CompetitionService()
        {

        }

        #region Data Fetchers
        public async Task LoadCompetitionData(int competitionId)
        {
            using (var db = new StkStubakiEntities())
            {
                var allPlayedGames = await db.Utakmicas.Where(x => x.SifraSezona == competitionId && (x.RezDomacin != 0 || x.RezGost != 0))
                    .Include(x => x.Mecs.Select(m => m.SetMecs))
                    .Include(x => x.Pars.Select(p => p.SetPars))
                    .ToListAsync();

                foreach (var game in allPlayedGames)
                {
                    insertTeamInfo(game);
                    populateHeadToHead(game);
                    insertPlayerInfo(game);
                }
            }
        }

        public async Task<List<TableTeamInfoDTO>> GetTeamInfos(int competitionId)
        {
            using (var db = new StkStubakiEntities())
            {
                var teamInfos = await db.Natjeces.Include("Momcad")
                    .Where(x => x.SifraSezona == competitionId)
                    .Select(x => new TableTeamInfoDTO()
                    {
                        TeamId = x.IdMomcad,
                        Name = x.Momcad.Naziv,
                        GamesPlayed = x.BrUtakmica ?? 0,
                        Won = x.BrPobjeda ?? 0,
                        Lost = x.BrPoraza ?? 0,
                        Draw = x.BrNerijesenih ?? 0,
                        WonMatches = x.DobiveniMecevi ?? 0,
                        LostMatches = x.IzgubljeniMecevi ?? 0,
                        Points = x.BrBodova ?? 0,
                        NegativePoints = x.Kazna ?? 0,
                        PenaltyDesc = x.OpisKazne
                    }).ToListAsync();

               return teamInfos;
            }
        }
        public async Task<List<TablePlayerInfoDTO>> GetPlayerInfos(int competitionId)
        {
            using (var db = new StkStubakiEntities())
            {
                var playerInfos = await db.IgraZzas.Include(x => x.Momcad).Include(x => x.Igrac)
                    .Where(x => x.SifraSezona == competitionId && (x.BrPobjeda > 0 || x.BrPoraza > 0))
                    .Select(x => new TablePlayerInfoDTO()
                    {
                        PlayerId = x.IdIgrac,
                        PlayerName = x.Igrac.Prezime + ", " + x.Igrac.Ime,
                        TeamName = x.Momcad.Naziv,
                        Won = x.BrPobjeda ?? 0,
                        Lost = x.BrPoraza ?? 0
                    }).ToListAsync();

                return playerInfos;
            }
        }
        #endregion

        #region Sortings
        public Task<List<TableTeamInfoDTO>> SortTeams(List<TableTeamInfoDTO> teamInfosDTO)
        {
            return Task.Run(() =>
            {
                var teams = TeamCompetitionInfos.Values.ToList();
                List<int> sortedTeamIds = sortCompetitionData(teams, TeamHeadToHeadInfos);

                var sortedTeamInfosDTO = new List<TableTeamInfoDTO>();
                foreach (var teamId in sortedTeamIds)
                {
                    sortedTeamInfosDTO.Add(teamInfosDTO.First(x => x.TeamId == teamId));
                }

                return sortedTeamInfosDTO;
            });
        }

        public Task<List<TablePlayerInfoDTO>> SortPlayers(List<TablePlayerInfoDTO> playerInfosDTO)
        {
            return Task.Run(() =>
            {
                var players = PlayerCompetitionInfos.Values.ToList();
                List<int> sortedPlayerIds = sortCompetitionData(players, PlayerHeadToHeadInfos);

                var sortedPlayerInfosDTO = new List<TablePlayerInfoDTO>();
                foreach (var playerId in sortedPlayerIds)
                {
                    if (playerId == 1)
                    {
                        continue;
                    }
                    sortedPlayerInfosDTO.Add(playerInfosDTO.First(x => x.PlayerId == playerId));
                }

                return sortedPlayerInfosDTO;
            });
        }

        private List<int> sortCompetitionData<T>(List<T> competitionData, Dictionary<HeadToHeadKey, HeadToHeadInfo<T>> headToHeadInfos) where T: ICompetitionData, new()
        {
            var sortedDataIds = new List<int>();
            competitionData.Sort();
            competitionData.Reverse();

            int lastPoints = -1;
            var dataWithSamePoints = new List<T>();
            foreach (var data in competitionData)
            {
                if (data.Points != lastPoints)
                {
                    sortedDataIds.AddRange(sortDataWithSamePoints(dataWithSamePoints, headToHeadInfos));
                    dataWithSamePoints.Clear();

                    lastPoints = data.Points;
                    dataWithSamePoints.Add(data);
                }
                else
                {
                    dataWithSamePoints.Add(data);
                }
            }

            sortedDataIds.AddRange(sortDataWithSamePoints(dataWithSamePoints, headToHeadInfos));
            return sortedDataIds;
        }

        private List<int> sortDataWithSamePoints<T>(List<T> dataWithSamePoints, Dictionary<HeadToHeadKey, HeadToHeadInfo<T>> headToHeadInfos) where T : ICompetitionData, new()
        {
            if (dataWithSamePoints.Count == 1)
            {
                return new List<int>() { dataWithSamePoints.FirstOrDefault().ID };
            }

            Dictionary<int, T> headToHeadInfo = new Dictionary<int, T>();
            foreach(var data in dataWithSamePoints)
            {
                headToHeadInfo.Add(data.ID, new T());
                headToHeadInfo[data.ID].ID = data.ID;
            }

            // if none than we just return as was sorted in start
            bool hasHeadToHead = false;
            for (int i = 0; i < dataWithSamePoints.Count - 1; i++)
            {
                for (int j = i + 1; j < dataWithSamePoints.Count; j++)
                {
                    var headToHeadKey = new HeadToHeadKey(dataWithSamePoints[i].ID, dataWithSamePoints[j].ID);
                    if (headToHeadInfos.ContainsKey(headToHeadKey))
                    {
                        hasHeadToHead = true;
                        var headToHead = headToHeadInfos[headToHeadKey];
                        headToHeadInfo[headToHead.Info1.ID].Aggregate(headToHead.Info1);
                        headToHeadInfo[headToHead.Info2.ID].Aggregate(headToHead.Info2);
                    }
                }
            }

            if (!hasHeadToHead)
            {
                return dataWithSamePoints.Select(x => x.ID).ToList();
            }

            var headToHeadTeamList = headToHeadInfo.Values.ToList();
            headToHeadTeamList.Sort();
            headToHeadTeamList.Reverse();

            return headToHeadTeamList.Select(x => x.ID).ToList();
        }

        #endregion

        #region Helpers
        private void insertTeamInfo(Utakmica game)
        {
            if (!TeamCompetitionInfos.ContainsKey(game.IdDomacin))
            {
                TeamCompetitionInfos.Add(game.IdDomacin, new TeamCompetitionInfo(game.IdDomacin));
            }

            if (!TeamCompetitionInfos.ContainsKey(game.IdGost))
            {
                TeamCompetitionInfos.Add(game.IdGost, new TeamCompetitionInfo(game.IdGost));
            }

            populateTeamCompetitionInfo(TeamCompetitionInfos[game.IdDomacin], TeamCompetitionInfos[game.IdGost], game);
        }

        private void insertPlayerInfo(Utakmica game)
        {
            foreach (var mec in game.Mecs)
            {
                populatePlayerCompetitionInfo(mec);
            }
        }

        private void populateTeamCompetitionInfo(TeamCompetitionInfo info1, TeamCompetitionInfo info2, Utakmica game)
        {
            var infoDomacin = info1.ID == game.IdDomacin ? info1 : info2;
            var infoGost = info1.ID == game.IdDomacin ? info2 : info1;

            infoDomacin.MatchesWon += game.RezDomacin ?? 0;
            infoDomacin.MatchesLost += game.RezGost ?? 0;
            infoDomacin.SetsWon += game.BrSetDomacin ?? 0;
            infoDomacin.SetsLost += game.BrSetGost ?? 0;

            infoGost.MatchesLost += game.RezDomacin ?? 0;
            infoGost.MatchesWon += game.RezGost ?? 0;
            infoGost.SetsLost += game.BrSetDomacin ?? 0;
            infoGost.SetsWon += game.BrSetGost ?? 0;

            if (game.RezDomacin == game.RezGost)
            {
                infoGost.Points += 1;
                infoDomacin.Points += 1;
            } else if (game.RezDomacin > game.RezGost)
            {
                infoDomacin.Points += 2;
            } else
            {
                infoGost.Points += 2;
            }

            foreach (var mec in game.Mecs)
            {
                foreach (var set in mec.SetMecs)
                {
                    infoDomacin.PointsWon += set.PoenDom;
                    infoDomacin.PointsLost += set.PoenGost;
                    infoGost.PointsWon += set.PoenGost;
                    infoGost.PointsLost += set.PoenDom;
                }
            }
        }

        private void populatePlayerCompetitionInfo(Mec game)
        {
            if (!PlayerCompetitionInfos.ContainsKey(game.IdDomaciIgrac ?? 0))
            {
                PlayerCompetitionInfos.Add(game.IdDomaciIgrac ?? 0, new PlayerCompetitionInfo(game.IdDomaciIgrac ?? 0));
            }

            if (!PlayerCompetitionInfos.ContainsKey(game.IdGostujuciIgrac ?? 0))
            {
                PlayerCompetitionInfos.Add(game.IdGostujuciIgrac ?? 0, new PlayerCompetitionInfo(game.IdGostujuciIgrac ?? 0));
            }

            var infoDomacin = PlayerCompetitionInfos[game.IdDomaciIgrac ?? 0];
            var infoGost = PlayerCompetitionInfos[game.IdGostujuciIgrac ?? 0];
            populatePlayerCompetitionInfo(game, infoDomacin, infoGost);

            HeadToHeadKey key = new HeadToHeadKey(game.IdDomaciIgrac ?? 0, game.IdGostujuciIgrac ?? 0);
            if (!PlayerHeadToHeadInfos.ContainsKey(key))
            {
                PlayerHeadToHeadInfos.Add(key, new HeadToHeadInfo<PlayerCompetitionInfo>(game.IdDomaciIgrac ?? 0, game.IdGostujuciIgrac ?? 0));
            }

            populatePlayerCompetitionInfo(game, PlayerHeadToHeadInfos[key].Info1, PlayerHeadToHeadInfos[key].Info2);
        }

        private static void populatePlayerCompetitionInfo(Mec game, PlayerCompetitionInfo info1, PlayerCompetitionInfo info2)
        {
            var infoDomacin = info1.ID == game.IdDomaciIgrac ? info1 : info2;
            var infoGost = info1.ID == game.IdDomaciIgrac ? info2 : info1;

            if (game.DobSetoviDom > game.DobSetoviGost)
            {
                infoDomacin.Wins++;
                infoGost.Loses++;
            }
            else
            {
                infoGost.Wins++;
                infoDomacin.Loses++;
            }

            infoDomacin.SetsWon += game.DobSetoviDom ?? 0;
            infoDomacin.SetsLost += game.DobSetoviGost ?? 0;
            infoGost.SetsWon += game.DobSetoviGost ?? 0;
            infoGost.SetsLost += game.DobSetoviDom ?? 0;

            foreach (var set in game.SetMecs)
            {
                infoDomacin.PointsWon += set.PoenDom;
                infoDomacin.PointsLost += set.PoenGost;
                infoGost.PointsWon += set.PoenGost;
                infoGost.PointsLost += set.PoenDom;
            }
        }

        private void populateHeadToHead(Utakmica game)
        {
            HeadToHeadKey key = new HeadToHeadKey(game.IdDomacin, game.IdGost);
            if (!TeamHeadToHeadInfos.ContainsKey(key))
            {
                TeamHeadToHeadInfos.Add(key, new HeadToHeadInfo<TeamCompetitionInfo>(game.IdDomacin, game.IdGost));
            }

            populateTeamCompetitionInfo(TeamHeadToHeadInfos[key].Info1, TeamHeadToHeadInfos[key].Info2, game);
        }
        #endregion
    }
}
