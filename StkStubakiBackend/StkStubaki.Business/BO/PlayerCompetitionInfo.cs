using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StkStubaki.Business.BO
{
    public class PlayerCompetitionInfo
    {
        int PlayerId { get; set; }
        int Wins { get; set; }
        int Loses { get; set; }
        int SetsLost { get; set; }
        int SetsWon { get; set; }
        int PointsLost { get; set; }
        int PointsWon { get; set; }

        public PlayerCompetitionInfo(int playerId)
        {
            PlayerId = playerId;
        }
    }
}
