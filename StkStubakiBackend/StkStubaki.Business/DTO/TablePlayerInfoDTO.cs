using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StkStubaki.Business.DTO
{
    public class TablePlayerInfoDTO
    {
        public int PlayerId { get; set; }
        public string PlayerName { get; set; }
        public string TeamName { get; set; }
        public int Position { get; set; }
        public int Won { get; set; }
        public int Lost { get; set; }
        public string Matches => string.Format("{0} : {1}", Won, Lost);
    }
}
