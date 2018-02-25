using StkStubaki.Business.DTO;
using StkStubaki.Business.Services;
using StkStubaki.DatabaseModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace StkStubaki.Web.Controllers
{
    [RoutePrefix("api/competition")]
    public class CompetitionController : ApiController
    {
        [Route("{id:int}/teams")]
        [HttpGet]
        public IHttpActionResult GetTeamInfos(int id)
        {
            var competitionService = new CompetitionService();
            return Ok(new { teams = competitionService.GetTeamInfos(id) });
        }
    }
}
