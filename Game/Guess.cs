using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Game.Direction;

namespace Game
{
    public class Guess
    {
        #region private fields

        private readonly Map _basicMap;
        private readonly Map _map;
        private MapSymbol[,] _mapVisual;
        private readonly int _height;
        private readonly int _width;

        #endregion

        #region public properties

        public Map Map => _map;
        public MapSymbol[,] MapVisual => _mapVisual;
        public bool Equity => Map.CheckEquity(_map, _basicMap, Cell.SoftEquity);

        #endregion


        #region ctors

        public Guess(Map basicMap)
        {
            _basicMap = basicMap;
            _height = basicMap.Height;
            _width = basicMap.Width;
            _map = new Map($"{basicMap.Name} guess", basicMap.Height, basicMap.Width, false, true);
            _map.MakePerimetr();
            _mapVisual = _map.GetVisual();
        }

        #endregion


        #region public methods

        public void ChangeFromMapvisual(int y, int x)
        {
            if (
                y % 2 == 0 && x % 2 == 0 ||
                y < 0 ||
                y > 2 * _height ||
                x < 0 ||
                x > 2 * _width
               )
            {
                return;
            }

            MapSymbol symbol = _mapVisual[y, x];

            if (y % 2 == 0)
            {
                var cellUp = y == 0 ? new Cell(-1, -1) : _map.Cells[y / 2 - 1, (x - 1) / 2];
                var cellDown = y == _height * 2 ? new Cell(-1, -1) : _map.Cells[y / 2, (x - 1) / 2];

                if (symbol == MapSymbol.WallPresentHorizontal)
                {
                    cellUp.SetWalls(Down);
                    cellDown.SetWalls(Up);
                }
                if (symbol == MapSymbol.WallAbsentHorizontal)
                {
                    cellUp.BreakWalls(Down);
                    cellDown.BreakWalls(Up);
                }
                if (symbol == MapSymbol.DiodeUp)
                {
                    cellUp.SetWalls(Down);
                    cellDown.BreakWalls(Up);
                }
                if (symbol == MapSymbol.DiodeDown)
                {
                    cellUp.BreakWalls(Down);
                    cellDown.SetWalls(Up);
                }

                return;
            }

            if (x % 2 == 0)
            {
                var cellLeft = x == 0 ? new Cell(-1, -1) : _map.Cells[(y - 1) / 2, x / 2 - 1];
                var cellRight = x == _width * 2 ? new Cell(-1, -1) : _map.Cells[(y - 1) / 2, x / 2];

                if (symbol == MapSymbol.WallPresentVertical)
                {
                    cellLeft.SetWalls(Right);
                    cellRight.SetWalls(Left);
                }
                if (symbol == MapSymbol.WallAbsentVertical)
                {
                    cellLeft.BreakWalls(Right);
                    cellRight.BreakWalls(Left);
                }
                if (symbol == MapSymbol.DiodeLeft)
                {
                    cellLeft.SetWalls(Right);
                    cellRight.BreakWalls(Left);
                }
                if (symbol == MapSymbol.DiodeRight)
                {
                    cellLeft.BreakWalls(Right);
                    cellRight.SetWalls(Left);
                }

                return;
            }

            if (symbol == MapSymbol.Visited)
            {
                var cell = _map.Cells[ (y - 1) / 2, (x - 1) / 2 ];
                cell.SetHole(null);
                
                return;
            }
        }

        public void SetHoleTarget(int holeY, int holeX, int targetY, int targetX) 
        {
            if (
                holeY < 0 ||
                holeY > 2 * _height ||
                holeX < 0 ||
                holeX > 2 * _width ||
                targetY < 0 ||
                targetY > 2 * _height ||
                targetX < 0 ||
                targetX > 2 * _width ||
                holeY % 2 == 0 ||
                holeX % 2 == 0 ||
                targetY % 2 == 0 ||
                targetX % 2 == 0
               )
            {
                return;
            }

            Cell cell = _map.Cells[(holeY - 1) / 2, (holeX - 1) / 2];
            cell.SetHole(new Hole((targetY - 1) / 2, (targetX - 1) / 2));
        }

        #endregion

    }
}
