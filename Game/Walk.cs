using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Game.Direction;
using static Game.Exploring;
using static Game.WallState;

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
        public Dictionary<ConsoleKey, Direction> KeyDictionary =>_keyDictionary;

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
            var currentCell = _map.Player.Cell;
            var column = currentCell.Column;
            var row = currentCell.Row;

            if (currentCell.Wall(direction) == Present)
            {
                return Walled;
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
                }
            }

            if (column < 0 || column >= _map.Width || row < 0 || row >= _map.Height)
            {
                _map.ExtractPlayer();
                return Out;
            }
            else 
            {
                var nextCell = _map.Cells[row, column];

                if (nextCell.Hole != null)
                {
                    column = nextCell.Hole.ColumnTarget;
                    row = nextCell.Hole.RowTarget;
                    _map.Player.TakeCell(_map.Cells[row, column]);
                    return Holed;
                }
                else 
                {
                    _map.Player.TakeCell(nextCell);
                    return Passed;
                }
            }
        }

        public void Move() 
        {

            if (_map.Player == null) 
            {
                _map.PrintMap(Empty.ToName());
                return;
            }

            _map.PrintMap($"it is a labyrinth. you can move with arrows. esc - exit");

            while (true)
            {
                var key = Console.ReadKey().Key;

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
