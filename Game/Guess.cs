using static Game.Direction;

namespace Game
{
    public class Guess
    {
        #region private fields

        private readonly Map _basicMap;
        private readonly Map _map;
        private readonly int _height;
        private readonly int _width;

        #endregion

        #region public properties

        public Map Map => _map;
        public MapSymbol[,] MapVisual => _map.GetVisual();
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
        }

        public Guess(int height, int width, string name)
        {
            _height = height;
            _width = width;
            _map = new Map(name, height, width, false, true);
            _map.MakePerimetr();
        }

        #endregion

        #region public methods

        public void ChangeMapFromMapvisual(int y, int x)
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

            MapSymbol symbol = _map.GetVisual()[y, x];

            if (y % 2 == 0)
            {
                Cell cellUp = y == 0 ? new Cell(-1, -1) : _map.Cells[y / 2 - 1, (x - 1) / 2];
                Cell cellDown = y == _height * 2 ? new Cell(-1, -1) : _map.Cells[y / 2, (x - 1) / 2];

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
                Cell cellLeft = x == 0 ? new Cell(-1, -1) : _map.Cells[(y - 1) / 2, x / 2 - 1];
                Cell cellRight = x == _width * 2 ? new Cell(-1, -1) : _map.Cells[(y - 1) / 2, x / 2];

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
                Cell cell = _map.Cells[(y - 1) / 2, (x - 1) / 2];
                cell.SetHole(null);

                return;
            }
        }

        public void ChangeMapFromMapVisualSymbol(int y, int x, MapSymbol symbol)
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

            if (y % 2 == 0)
            {
                Cell cellUp = y == 0 ? new Cell(-1, -1) : _map.Cells[y / 2 - 1, (x - 1) / 2];
                Cell cellDown = y == _height * 2 ? new Cell(-1, -1) : _map.Cells[y / 2, (x - 1) / 2];

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
                Cell cellLeft = x == 0 ? new Cell(-1, -1) : _map.Cells[(y - 1) / 2, x / 2 - 1];
                Cell cellRight = x == _width * 2 ? new Cell(-1, -1) : _map.Cells[(y - 1) / 2, x / 2];

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
                Cell cell = _map.Cells[(y - 1) / 2, (x - 1) / 2];
                cell.SetHole(null);

                return;
            }
            else if (symbol == MapSymbol.Hole) 
            {
                Cell cell = _map.Cells[(y - 1) / 2, (x - 1) / 2];
                cell.SetHole(new Hole(-1, -1));

                return;
            }
        }

        public void SetHoleTarget(int holeRow, int holeColumn, int holeTargetRow, int holeTargetColumn) 
        {
            Cell cell = _map.Cells[holeRow, holeColumn];
            cell.SetHole(new Hole(holeTargetRow, holeTargetColumn));
        }

        #endregion
    }
}
