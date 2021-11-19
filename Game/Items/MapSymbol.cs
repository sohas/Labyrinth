using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game
{
    public enum MapSymbol
    {
        [Description("+")]
        Cross,

        [Description("i")]
        Player,
        
        [Description("o")]
        Hole,
        
        [Description("|")]
        WallPresentVertical,
        
        [Description("-")]
        WallPresentHorizontal,
        
        [Description(">")]
        DiodeRight,
        
        [Description("<")]
        DiodeLeft,
        
        [Description("^")]
        DiodeUp,
        
        [Description("v")]
        DiodeDown,
        
        [Description(" ")]
        WallUnsertainVertical,
        
        [Description(" ")]
        WallUnsertainHorizontal,
        
        [Description(" ")]
        WallAbsentVertical,
        
        [Description(" ")]
        WallAbsentHorizontal,
        
        [Description("x")]
        Unvisited,
        
        [Description(" ")]
        Visited
    }
}
