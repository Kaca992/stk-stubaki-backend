using StkStubaki.Business.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StkStubaki.Business.BO
{
    public class TeamCompetitionInfo : ICompetitionData
    {
        public int ID { get; set; }
        public int Points { get; set; }
        public int MatchesWon { get; set; }
        public int MatchesLost { get; set; }
        public int SetsWon { get; set; }
        public int SetsLost { get; set; }
        public int PointsWon { get; set; }
        public int PointsLost { get; set; }

        public TeamCompetitionInfo()
        {
            
        }

        public TeamCompetitionInfo(int teamId)
        {
            ID = teamId;
        }

        public int CompareTo(object obj)
        {
            if (obj == null) return 1;

            TeamCompetitionInfo otherInfo = obj as TeamCompetitionInfo;
            var order = compareValues(Points, otherInfo.Points);
            if (order != 0)
            {
                return order;
            }

            order = compareValues(MatchesWon, otherInfo.MatchesWon);
            if (order != 0)
            {
                return order;
            }

            order = -1 * compareValues(MatchesLost, otherInfo.MatchesLost);
            if (order != 0)
            {
                return order;
            }

            order = compareValues(SetsWon, otherInfo.SetsWon);
            if (order != 0)
            {
                return order;
            }

            order = -1 * compareValues(SetsLost, otherInfo.SetsLost);
            if (order != 0)
            {
                return order;
            }

            order = compareValues(PointsWon, otherInfo.PointsWon);
            if (order != 0)
            {
                return order;
            }

            order = -1 * compareValues(PointsLost, otherInfo.PointsLost);
            if (order != 0)
            {
                return order;
            }

            return 0;
        }

        private int compareValues(int value1, int value2)
        {
            if (value1 > value2)
            {
                return 1;
            }
            else if (value1 < value2)
            {
                return -1;
            }

            return 0;
        }

        public void Aggregate<T>(T data)
        {
            var info = data as TeamCompetitionInfo;
            if (info == null)
            {
                return;
            }

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
