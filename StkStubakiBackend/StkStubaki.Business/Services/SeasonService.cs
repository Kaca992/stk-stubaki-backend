using StkStubaki.Business.DTO;
using StkStubaki.DatabaseModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StkStubaki.Business.Services
{
    public class SeasonService
    {
        public SeasonService()
        {

        }

        public Task<List<SeasonInfo>> GetAllSeasons()
        {
            using (var db = new StkStubakiEntities())
            {
                List<SeasonInfo> seasonInfos = new List<SeasonInfo>();
                foreach (var season in db.Sezonas.OrderByDescending(x => x.Godina).AsEnumerable())
                {
                    seasonInfos.Add(new SeasonInfo(season));
                }

                return Task.FromResult(seasonInfos);
            }
        }

        public Task<SeasonInfo> GetSeason(int id)
        {
            using (var db = new StkStubakiEntities())
            {
                var season = db.Sezonas.FirstOrDefault((s) => s.SifraSezona == id);

                if (season == null)
                {
                    throw new Exception("test");
                }
                return Task.FromResult(new SeasonInfo(season));
            }
        }
    }
}
