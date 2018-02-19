using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StkStubaki.Business.DTO
{
    public class TableTeamInfoDTO
    {
        public int TeamId { get; set; }
        public string Name { get; set; }

        public int GamesPlayed { get; set; }
        public int Won { get; set; }
        public int Lost { get; set; }
        public int Draw { get; set; }

        public int WonMatches { get; set; }
        public int LostMatches { get; set; }
        public string Matches => $"{(WonMatches)} : {(LostMatches)}";

        public int Points { get; set; }
        public int NegativePoints { get; set; }
        public string PenaltyDesc { get; set; }
    }
}