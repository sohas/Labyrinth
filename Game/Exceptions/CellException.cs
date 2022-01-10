using System;

namespace Game
{
    [Serializable]
    public class CellException : Exception
    {
        public CellException(string message) : base(message) { }
    }
}
