using StkStubaki.DatabaseModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StkStubaki.Web.DTO
{
    public class SeasonInfo
    {
        public int SeasonId { get; set; }
        public string SeasonName { get; set; }

        public SeasonInfo()
        {

        }

        public SeasonInfo(Sezona season)
        {
            SeasonId = season.SifraSezona;
            SeasonName = $"{season.Liga} {season.Godina}";
        }
    }
}