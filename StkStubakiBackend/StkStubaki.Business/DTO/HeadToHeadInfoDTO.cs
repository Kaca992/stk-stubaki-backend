using StkStubaki.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StkStubaki.Business.DTO
{
    public class HeadToHeadInfoDTO
    {
        public int OpponentId { get;set; }
        public HeadToHeadStatusEnum ScoreStatus { get; set; }

        public HeadToHeadInfoDTO()
        {

        }

        public HeadToHeadInfoDTO(int opponentId, HeadToHeadStatusEnum scoreStatus)
        {
            OpponentId = opponentId;
            ScoreStatus = scoreStatus;
        }
    }
}
