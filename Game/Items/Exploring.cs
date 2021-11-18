using System;
using System.ComponentModel;

namespace Game
{
    public enum Exploring
    {
        [Description("nobody in the labyrinth")]
        Empty,
        [Description("no path")]
        Walled,
        [Description("you left the labyrinth")]
        Out,
        [Description("you fell in a hole")]
        Holed,
        [Description("you passed")]
        Passed
    }
}
