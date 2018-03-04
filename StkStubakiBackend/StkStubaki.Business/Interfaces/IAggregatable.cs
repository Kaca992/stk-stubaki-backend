using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StkStubaki.Business.Interfaces
{
    public interface IAggregatable
    {
        void Aggregate<T>(T data);
    }
}
