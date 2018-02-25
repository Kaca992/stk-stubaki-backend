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
    [RoutePrefix("api/seasons")]
    public class SeasonController : ApiController
    {
        [Route("")]
        [HttpGet]
        public async Task<IHttpActionResult> GetAllSeasons()
        {
            var seasonService = new SeasonService();
            var seasonInfos = await seasonService.GetAllSeasons();

            return Ok(seasonInfos);
        }

        [Route("{id:int}")]
        [HttpGet]
        public async Task<IHttpActionResult> GetSeason(int id)
        {
            var seasonService = new SeasonService();
            var season = await seasonService.GetSeason(id);

            return Ok(season);
        }
    }
}
