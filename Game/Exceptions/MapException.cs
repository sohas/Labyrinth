﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game
{
    [Serializable]
    public class MapException : Exception
    {
        public MapException(string message) : base(message) { }
    }
}
