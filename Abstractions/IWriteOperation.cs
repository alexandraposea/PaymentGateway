using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abstractions
{
    public interface IWriteOperation<T>
    {
        public void PerformOperation(T operation);
    }
}
