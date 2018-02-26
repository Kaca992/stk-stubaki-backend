using StkStubaki.Business.DTO;
using StkStubaki.DatabaseModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace StkStubaki.Business.Services
{
    public class CompetitionService
    {
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

        public void GetTeamWinRatio(int competitionId)
        {
            using (var db = new StkStubakiEntities())
            {
                var allPlayedGames = db.Utakmicas.Where(x => x.SifraSezona == competitionId && (x.RezDomacin != 0 || x.RezGost != 0))
                    .Include(x => x.Mecs.Select(m => m.SetMecs))
                    .Include(x => x.Pars.Select(p => p.SetPars))
                    .ToList();

                foreach(var game in allPlayedGames)
                {
                    var test = game;
                }
            }
        }

        class TeamCompetitionInfo
        {
            int TeamId { get; set; }
            int Points { get; set; }
            int MatchesDifference { get; set; }
            int MatchesWon { get; set; }
            int SetsDifference { get; set; }
            int SetsWon { get; set; }
            int PointsDifference { get; set; }
            int PointsWon { get; set; }
        }

        class PlayerCompetitionInfo
        {
            int PlayerId { get; set; }
            int Wins { get; set; }
            int Loses { get; set; }
            int SetsDifference { get; set; }
            int SetsWon { get; set; }
            int PointsDifference { get; set; }
            int PointsWon { get; set; }
        }
    }
}
