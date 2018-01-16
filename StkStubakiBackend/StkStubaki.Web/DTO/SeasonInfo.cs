using StkStubaki.Common.Enums;
using StkStubaki.DatabaseModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StkStubaki.Web.DTO
{
    public class SeasonInfo
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public SeasonTypeEnum Type { get; set; }

        public SeasonInfo()
        {

        }

        public SeasonInfo(Sezona season)
        {
            Id = season.SifraSezona;
            Name = $"{season.Liga} {season.Godina}";
            Type = season.Liga == "Kup" ? SeasonTypeEnum.Kup : season.Liga == "1" ? SeasonTypeEnum.PrvaLiga : SeasonTypeEnum.DrugaLiga;
        }
    }
}