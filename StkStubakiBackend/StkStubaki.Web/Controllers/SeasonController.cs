using StkStubaki.DatabaseModel;
using StkStubaki.Web.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace StkStubaki.Web.Controllers
{
    [RoutePrefix("api/seasons")]
    public class SeasonController : ApiController
    {
        [Route("")]
        [HttpPost]
        public IHttpActionResult GetAllSeasons()
        {
            using(var db = new StkStubakiEntities())
            {
                List<SeasonInfo> seasonInfos = new List<SeasonInfo>();
                foreach(var season in db.Sezonas.OrderByDescending(x => x.Godina).AsEnumerable())
                {
                    seasonInfos.Add(new SeasonInfo(season));
                }

                return Ok(seasonInfos);
            }
        }

        [Route("{id:int}")]
        [HttpGet]
        public IHttpActionResult GetSeason(int id)
        {
            using (var db = new StkStubakiEntities())
            {               
                var season = db.Sezonas.FirstOrDefault((s) => s.SifraSezona == id);

                if (season == null)
                {
                    return NotFound();
                }
                return Ok(new SeasonInfo(season));
            }           
        }
    }
}
