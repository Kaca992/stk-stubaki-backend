﻿using StkStubaki.Business.DTO;
using StkStubaki.DatabaseModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using StkStubaki.Business.BO;

namespace StkStubaki.Business.Services
{
    public class CompetitionService
    {
        public readonly Dictionary<int, TeamCompetitionInfo> TeamCompetitionInfos = new Dictionary<int, TeamCompetitionInfo>();
        public readonly Dictionary<int, PlayerCompetitionInfo> PlayerCompetitionInfos = new Dictionary<int, PlayerCompetitionInfo>();

        public readonly Dictionary<HeadToHeadKey, HeadToHeadInfo<TeamCompetitionInfo>> TeamHeadToHeadInfos = new Dictionary<HeadToHeadKey, HeadToHeadInfo<TeamCompetitionInfo>>();

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

                    // TODO populate player infos
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
            return Task.Run(() => {
                var teams = TeamCompetitionInfos.Values.ToList();
                var sortedTeamIds = new List<int>();
                teams.Sort();
                teams.Reverse();

                int lastPoints = -1;
                var teamsWithSamePoints = new List<TeamCompetitionInfo>();
                foreach (var team in teams)
                {
                    if (team.Points != lastPoints)
                    {
                        sortedTeamIds.AddRange(sortTeamsWithSamePoints(teamsWithSamePoints));
                        teamsWithSamePoints.Clear();

                        lastPoints = team.Points;
                        teamsWithSamePoints.Add(team);
                    }
                    else
                    {
                        teamsWithSamePoints.Add(team);
                    }
                }

                sortedTeamIds.AddRange(sortTeamsWithSamePoints(teamsWithSamePoints));

                var sortedTeamInfosDTO = new List<TableTeamInfoDTO>();
                foreach (var teamId in sortedTeamIds)
                {
                    sortedTeamInfosDTO.Add(teamInfosDTO.First(x => x.TeamId == teamId));
                }

                return sortedTeamInfosDTO;
            });
        }

        private List<int> sortTeamsWithSamePoints(List<TeamCompetitionInfo> teamsWithSamePoints)
        {
            if (teamsWithSamePoints.Count == 1)
            {
                return new List<int>() { teamsWithSamePoints.FirstOrDefault().TeamId };
            }

            Dictionary<int, TeamCompetitionInfo> headToHeadInfo = new Dictionary<int, TeamCompetitionInfo>();
            foreach(var team in teamsWithSamePoints)
            {
                headToHeadInfo.Add(team.TeamId, new TeamCompetitionInfo(team.TeamId));
            }

            // if none than we just return as was sorted in start
            bool hasHeadToHead = false;
            for (int i = 0; i < teamsWithSamePoints.Count - 1; i++)
            {
                for (int j = i + 1; j < teamsWithSamePoints.Count; j++)
                {
                    var headToHeadKey = new HeadToHeadKey(teamsWithSamePoints[i].TeamId, teamsWithSamePoints[j].TeamId);
                    if (TeamHeadToHeadInfos.ContainsKey(headToHeadKey))
                    {
                        hasHeadToHead = true;
                        var headToHead = TeamHeadToHeadInfos[headToHeadKey];
                        headToHeadInfo[headToHead.Info1.TeamId].Aggregate(headToHead.Info1);
                        headToHeadInfo[headToHead.Info2.TeamId].Aggregate(headToHead.Info2);
                    }
                }
            }

            if (!hasHeadToHead)
            {
                return teamsWithSamePoints.Select(x => x.TeamId).ToList();
            }

            var headToHeadTeamList = headToHeadInfo.Values.ToList();
            headToHeadTeamList.Sort();
            headToHeadTeamList.Reverse();

            return headToHeadTeamList.Select(x => x.TeamId).ToList();
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
        private void populateHeadToHead(Utakmica game)
        {
            HeadToHeadKey key = new HeadToHeadKey(game.IdDomacin, game.IdGost);
            if (!TeamHeadToHeadInfos.ContainsKey(key))
            {
                TeamHeadToHeadInfos.Add(key, new HeadToHeadInfo<TeamCompetitionInfo>(game.IdDomacin, game.IdGost));
            }

            populateTeamCompetitionInfo(TeamHeadToHeadInfos[key].Info1, TeamHeadToHeadInfos[key].Info2, game);
        }
        private void populateTeamCompetitionInfo(TeamCompetitionInfo info1, TeamCompetitionInfo info2, Utakmica game)
        {
            var infoDomacin = info1.TeamId == game.IdDomacin ? info1 : info2;
            var infoGost = info1.TeamId == game.IdDomacin ? info2 : info1;

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
                foreach(var set in mec.SetMecs)
                {
                    infoDomacin.PointsWon += set.PoenDom;
                    infoDomacin.PointsLost += set.PoenGost;
                    infoGost.PointsWon += set.PoenGost;
                    infoGost.PointsLost += set.PoenDom;
                }
            }
        }
        #endregion
    }
}
