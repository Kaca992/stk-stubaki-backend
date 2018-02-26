using StkStubaki.Business.DTO;
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

        public readonly Dictionary<HeadToHeadKey, HeadToHeadTeamInfo> TeamHeadToHeadInfos = new Dictionary<HeadToHeadKey, HeadToHeadTeamInfo>();

        public CompetitionService()
        {

        }

        public Task<List<TableTeamInfoDTO>> GetTeamInfos(int competitionId)
        {
            using (var db = new StkStubakiEntities())
            {
                var teamInfos = db.Natjeces.Include("Momcad")
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
                    }).OrderByDescending(x => x.Points).ToList();

               return Task.FromResult(teamInfos);
            }
        }

        public Task<List<TablePlayerInfoDTO>> GetPlayerInfos(int competitionId)
        {
            using (var db = new StkStubakiEntities())
            {
                var teamInfos = db.IgraZzas.Include(x => x.Momcad).Include(x => x.Igrac)
                    .Where(x => x.SifraSezona == competitionId && (x.BrPobjeda > 0 || x.BrPoraza > 0))
                    .Select(x => new TablePlayerInfoDTO()
                    {
                        PlayerId = x.IdIgrac,
                        PlayerName = x.Igrac.Prezime + ", " + x.Igrac.Ime,
                        TeamName = x.Momcad.Naziv,
                        Won = x.BrPobjeda ?? 0,
                        Lost = x.BrPoraza ?? 0
                    }).OrderByDescending(x => x.Won).ThenBy(x => x.Lost).ToList();

                return Task.FromResult(teamInfos);
            }
        }

        public void GetWinRatio(int competitionId)
        {
            using (var db = new StkStubakiEntities())
            {
                var allPlayedGames = db.Utakmicas.Where(x => x.SifraSezona == competitionId && (x.RezDomacin != 0 || x.RezGost != 0))
                    .Include(x => x.Mecs.Select(m => m.SetMecs))
                    .Include(x => x.Pars.Select(p => p.SetPars))
                    .ToList();

                foreach(var game in allPlayedGames)
                {
                    insertTeamInfo(game);
                    populateHeadToHead(game);
                }
            }
        }

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
                TeamHeadToHeadInfos.Add(key, new HeadToHeadTeamInfo(game.IdDomacin, game.IdGost));
            }

            populateTeamCompetitionInfo(TeamHeadToHeadInfos[key].Team1, TeamHeadToHeadInfos[key].Team2, game);
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

        public class HeadToHeadKey
        {
            public int Id1 { get; set; }
            public int Id2 { get; set; }

            public HeadToHeadKey(int teamId1, int teamId2)
            {
                Id1 = teamId1 > teamId2 ? teamId1 : teamId2;
                Id2 = teamId1 > teamId2 ? teamId2 : teamId1;
            }

            public override bool Equals(object obj)
            {
                HeadToHeadKey h2 = obj as HeadToHeadKey;
                if (h2 == null)
                    return false;
                return Id1 == h2.Id1 && Id2 == h2.Id2;
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    int hash = 17;
                    hash = hash * 23 + Id1.GetHashCode();
                    hash = hash * 23 + Id2.GetHashCode();
                    return hash;
                }
            }
        }

        public class HeadToHeadTeamInfo
        {
            public TeamCompetitionInfo Team1 { get; set; }
            public TeamCompetitionInfo Team2 { get; set; }

            public HeadToHeadTeamInfo(int teamId1, int teamId2)
            {
                var id1 = teamId1 > teamId2 ? teamId1 : teamId2;
                var id2 = teamId1 > teamId2 ? teamId2 : teamId1;

                Team1 = new TeamCompetitionInfo(id1);
                Team2 = new TeamCompetitionInfo(id2);
            }
        }
    }
}
