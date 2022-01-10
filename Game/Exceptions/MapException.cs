using System;

namespace Game
{
    [Serializable]
    public class MapException : Exception
    {
        public MapException(string message) : base(message) { }
    }
}
