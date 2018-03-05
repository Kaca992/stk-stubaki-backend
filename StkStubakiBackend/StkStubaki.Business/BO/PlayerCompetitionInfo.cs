using StkStubaki.Business.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StkStubaki.Business.BO
{
    public class PlayerCompetitionInfo : ICompetitionData
    {
        public int ID { get; set; }
        public int Points => Wins;
        public int Wins { get; set; }
        public int Loses { get; set; }
        public int SetsLost { get; set; }
        public int SetsWon { get; set; }
        public int PointsLost { get; set; }
        public int PointsWon { get; set; }

        public PlayerCompetitionInfo()
        {

        }

        public PlayerCompetitionInfo(int playerId)
        {
            ID = playerId;
        }

        public int CompareTo(object obj)
        {
            if (obj == null) return 1;

            PlayerCompetitionInfo otherInfo = obj as PlayerCompetitionInfo;
            var order = compareValues(Wins, otherInfo.Wins);
            if (order != 0)
            {
                return order;
            }

            order = -1 * compareValues(Loses, otherInfo.Loses);
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
            var info = data as PlayerCompetitionInfo;
            if (info == null)
            {
                return;
            }

            Wins += info.Wins;
            Loses += info.Loses;
            SetsWon += info.SetsWon;
            SetsLost += info.SetsLost;
            PointsWon += info.PointsWon;
            PointsLost += info.PointsLost;
        }
    }
}
