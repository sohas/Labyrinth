using System;
using System.Collections.Generic;
using static Game.Direction;

namespace Game
{
    public class Walk
    {
        #region private fields

        private readonly Map _map;
        public static readonly Dictionary<ConsoleKey, Direction> _keyDictionary;

        #endregion

        #region public properties

        public Map Map => _map;
        public static Dictionary<ConsoleKey, Direction> KeyDictionary => _keyDictionary;

        #endregion


        #region ctors

        static Walk()
        {
            _keyDictionary = new Dictionary<ConsoleKey, Direction>
            {
                [ConsoleKey.LeftArrow] = Left,
                [ConsoleKey.RightArrow] = Right,
                [ConsoleKey.UpArrow] = Up,
                [ConsoleKey.DownArrow] = Down
            };
        }

        public Walk(Map map)
        {
            _map = map;
        }

        #endregion


        #region public methods

        public Exploring Step(Direction direction)
        {
            Cell currentCell = _map.Player.Cell;
            int column = currentCell.Column;
            int row = currentCell.Row;

            if (currentCell.Wall(direction) == WallState.Present)
            {
                return Exploring.Walled;
            }
            else
            {
                switch (direction)
                {
                    case Left:
                        column--;
                        break;
                    case Right:
                        column++;
                        break;
                    case Up:
                        row--;
                        break;
                    case Down:
                        row++;
                        break;
                    default:
                        break;
                }
            }

            if (column < 0 || column >= _map.Width || row < 0 || row >= _map.Height)
            {
                _map.ExtractPlayer();
                return Exploring.Out;
            }
            else
            {
                Cell nextCell = _map.Cells[row, column];

                if (nextCell.Hole != null)
                {
                    column = nextCell.Hole.ColumnTarget;
                    row = nextCell.Hole.RowTarget;
                    _map.Player.TakeCell(_map.Cells[row, column]);
                    return Exploring.Holed;
                }
                else
                {
                    _map.Player.TakeCell(nextCell);
                    return Exploring.Passed;
                }
            }
        }

        public void Move()
        {

            if (_map.Player == null)
            {
                _map.PrintMap(Exploring.Empty.ToName());
                return;
            }

            _map.PrintMap($"it is a labyrinth. you can move with arrows. esc - exit");

            while (true)
            {
                ConsoleKey key = Console.ReadKey().Key;

                if (key == ConsoleKey.Escape || _map.Player == null)
                {
                    return;
                }

                else if (_keyDictionary.ContainsKey(key))
                {
                    _map.PrintMap(Step(_keyDictionary[key]).ToName());
                }
            }
        }

        #endregion
    }
}
