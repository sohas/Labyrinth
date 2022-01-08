using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game
{
    [Serializable]
    public class CellException : Exception
    {
        public CellException(string message) : base(message) { }
    }
}
