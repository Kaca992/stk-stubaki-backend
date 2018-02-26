using StkStubaki.Business.DTO;
using StkStubaki.Business.Services;
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
            competitionService.GetWinRatio(id);

            // TODO sortirati prema head to head + onim info detaljnim, ulaz su ovi gore teams
            // TODO teamsHeadToHead Dict<int, {int, int=>-1,0,1}>
            // TODO isto za igrace
            // TODO Izvuci info za parove

            await Task.WhenAll(teams, players);

            return Ok(new { teams = teams.Result, players = players.Result });
        }
    }
}
