using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StkStubaki.Business.BO
{
    public class TeamCompetitionInfo : IComparable
    {
        public int TeamId { get; set; }
        public int Points { get; set; }
        public int MatchesWon { get; set; }
        public int MatchesLost { get; set; }
        public int SetsWon { get; set; }
        public int SetsLost { get; set; }
        public int PointsWon { get; set; }
        public int PointsLost { get; set; }

        public TeamCompetitionInfo(int teamId)
        {
            TeamId = teamId;
        }

        public int CompareTo(object obj)
        {
            if (obj == null) return 1;

            TeamCompetitionInfo otherInfo = obj as TeamCompetitionInfo;
            

        }

        public void Aggregate(TeamCompetitionInfo info)
        {
            Points += info.Points;
            MatchesWon += info.MatchesWon;
            MatchesLost += info.MatchesLost;
            SetsWon += info.SetsWon;
            SetsLost += info.SetsLost;
            PointsWon += info.PointsWon;
            PointsLost += info.PointsLost;
        }
    }
}
