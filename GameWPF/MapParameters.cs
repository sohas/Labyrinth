using Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace GameWPF
{
    internal static class MapParameters
    {
        internal static Brush wallColor = new SolidColorBrush(Color.FromArgb(255, 150, 100, 50));
        internal static Brush playerColor = new SolidColorBrush(Color.FromArgb(255, 20, 200, 100));
        internal static Brush startColor = new SolidColorBrush(Color.FromArgb(255, 200, 20, 50));
        internal static Brush unvisitedColor = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
        internal static Brush visitedColor = new SolidColorBrush(Color.FromArgb(255, 50, 50, 50));
        internal static Brush alertColor = new SolidColorBrush(Color.FromArgb(255, 150, 150, 150));
        internal static int wallSize = 2;
        internal static int cellSize = 8;

        internal static Brush GetColor(MapSymbol symbol)
        {
            return symbol switch
            {
                MapSymbol.CrossAbsent => visitedColor,
                MapSymbol.CrossUnsertain => unvisitedColor,
                MapSymbol.CrossPresent => wallColor,
                MapSymbol.Player => visitedColor,
                MapSymbol.Hole => visitedColor,
                MapSymbol.Start => visitedColor,
                MapSymbol.WallPresentVertical => wallColor,
                MapSymbol.WallPresentHorizontal => wallColor,
                MapSymbol.WallUnsertainVertical => unvisitedColor,
                MapSymbol.WallUnsertainHorizontal => unvisitedColor,
                MapSymbol.WallAbsentVertical => visitedColor,
                MapSymbol.WallAbsentHorizontal => visitedColor,
                MapSymbol.DiodeRight => wallColor,
                MapSymbol.DiodeLeft => wallColor,
                MapSymbol.DiodeUp => wallColor,
                MapSymbol.DiodeDown => wallColor,
                MapSymbol.Unvisited => unvisitedColor,
                MapSymbol.Visited => visitedColor,
                _ => unvisitedColor
            };
        }
    }
}
