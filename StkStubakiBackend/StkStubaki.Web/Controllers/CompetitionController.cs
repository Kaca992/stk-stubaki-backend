using StkStubaki.Business.DTO;
using StkStubaki.Business.Services;
using StkStubaki.Business.Utils;
using StkStubaki.DatabaseModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace StkStubaki.Web.Controllers
{
    [RoutePrefix("api/competition")]
    public class CompetitionController : ApiController
    {
        [Route("{id:int}/teams")]
        [HttpGet]
        public async Task<IHttpActionResult> GetTeamInfos(int id)
        {
            var competitionService = new CompetitionService();
            var teams = competitionService.GetTeamInfos(id);
            var players = competitionService.GetPlayerInfos(id);
            var winRatios = competitionService.LoadCompetitionData(id);

            // 2 Liga, sezona 2014/2015

            // TODO sortirati prema head to head + onim info detaljnim, ulaz su ovi gore teams
            // TODO teamsHeadToHead Dict<int, {int, int=>-1,0,1}>
            // TODO isto za igrace
            // TODO Izvuci info za parove

            // HeadToHead klasa => Id, Dict<ProtivnikId, {-1, 0 , 1}>

            // REFACTOR:
            //  1. Izdvojiti sorter u posebnu klasu da se lako moze zamijeniti
            //  2. Dohvat i popunjavanje info-a i head to head
            //  3. Sortiranje momcadi i mapiranje u DTO
            //  4. Sortiranje igraca i mapiranje u DTO
            //  5. HeadToHead mapiranje u DTO

            await Task.WhenAll(teams, players, winRatios);

            var sortedTeams = competitionService.SortTeams(teams.Result);
            var sortedPlayers = competitionService.SortPlayers(players.Result);
            await Task.WhenAll(sortedTeams, sortedPlayers);

            var teamHeadToHeads = HeadToHeadHelper.GenerateHeadToHeadInfoDTO(competitionService.TeamHeadToHeadInfos);
            var playerHeadToHeads = HeadToHeadHelper.GenerateHeadToHeadInfoDTO(competitionService.PlayerHeadToHeadInfos);

            return Ok(new { teams = sortedTeams.Result, players = sortedPlayers.Result, teamHeadToHeads = teamHeadToHeads, playerHeadToHeads = playerHeadToHeads });
        }
    }
}
