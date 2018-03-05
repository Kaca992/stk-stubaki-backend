using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StkStubaki.Business.Interfaces
{
    public interface ICompetitionData: IAggregatable, IComparable
    {
        int ID { get; set; }
        int Points { get; }
    }
}
