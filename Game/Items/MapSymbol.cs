using System.ComponentModel;


namespace Game
{
    public enum MapSymbol
    {
        [Description("+")]
        CrossPresent,

        [Description(" ")]
        CrossAbsent,

        [Description("i")]
        Player,

        [Description("o")]
        Hole,

        [Description("|")]
        WallPresentVertical,

        [Description("-")]
        WallPresentHorizontal,

        [Description(" ")]
        WallUnsertainVertical,

        [Description(" ")]
        WallUnsertainHorizontal,

        [Description(" ")]
        WallAbsentVertical,

        [Description(" ")]
        WallAbsentHorizontal,

        [Description(">")]
        DiodeRight,

        [Description("<")]
        DiodeLeft,

        [Description("^")]
        DiodeUp,

        [Description("v")]
        DiodeDown,

        [Description(" ")]
        Unvisited,

        [Description(".")]
        Visited
    }
}
