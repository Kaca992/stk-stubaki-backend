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
        public int Id { get; private set; }
        public string DisplayName { get; private set; }
        public string Godina { get; private set; }
        public SeasonTypeEnum Type { get; private set; }

        public SeasonInfo()
        {

        }

        public SeasonInfo(Sezona season)
        {
            Id = season.SifraSezona;           
            Godina = season.Godina;
            Type = season.Liga == "Kup" ? SeasonTypeEnum.Kup : season.Liga == "1" ? SeasonTypeEnum.PrvaLiga : SeasonTypeEnum.DrugaLiga;
            DisplayName = Type == SeasonTypeEnum.Kup ? $"{season.Liga} {season.Godina}" : $"{season.Liga}.liga {season.Godina}";
        }
    }
}