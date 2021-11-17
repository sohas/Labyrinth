using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Game.Direction;
using static Game.Exploring;

namespace Game
{
    public class Explore
    {
        #region private fields

        private readonly List<Map> _exploredMaps;
        private readonly Walk _walk;
        private Map _exploringMap;
        private Player _explorer;
        private int _counter;

        #endregion


        #region ctors

        public Explore(Map map)
        {
            _counter = 0;
            _exploredMaps = new List<Map>();
            _walk = new Walk(map);
            _exploringMap = new Map($"{map.Name}ex{_counter}", map.Width * 2 - 1, map.Height * 2 - 1, true, true);
            _explorer = new Player();
            _exploringMap.TakePlayer(_explorer, map.Width - 1, map.Height - 1);
        }

        #endregion

        #region private methods

        private static Direction TakeOpposite(Direction direction)
        {
            switch (direction)
            {
                case Left:
                    return Right;
                case Right:
                    return Left;
                case Up:
                    return Down;
                case Down:
                    return Up;
                default:
                    throw new Exception("");
            }
        }

        #endregion



        #region public methods

        public void Step(Direction direction, Exploring exploring)
        {
            var currentCell = _exploringMap.Player.Cell;
            var column = currentCell.Column;
            var row = currentCell.Row;

            switch (exploring) 
            {
                case Out:
                    _exploringMap.ExtractPlayer();
                    _exploredMaps.Add(_exploringMap);
                    return;

                case Walled:
                    _exploringMap.Cells[row, column].SetWalls(direction);
                    return;

                case Occupied:
                    return;

                case Passed:
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
                    currentCell.BreakWalls(direction);
                    currentCell = _exploringMap.Cells[row, column];
                    currentCell.BreakWalls(TakeOpposite(direction));
                    _exploringMap.Player.TakeCell(currentCell);
                    break;

                case Holed:
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
                    currentCell = _exploringMap.Cells[row, column];
                    currentCell.SetHole(new Hole(-1, -1));
                    _exploringMap.Player.TakeCell(currentCell);
                    _exploredMaps.Add(_exploringMap);
                    _counter++;
                    Map map = _walk.Map;
                    _exploringMap = new Map($"{map.Name}ex{_counter}", map.Width * 2 - 1, map.Height * 2 - 1, true, true);
                    _explorer = new Player();
                    _exploringMap.TakePlayer(_explorer, map.Width - 1, map.Height - 1);
                    return;
            }
        }

        public void Move()
        {
            _exploringMap.PrintWithComment($"it is a labyrinth. you can move with arrows. esc - exit");

            while (true)
            {
                var key = Console.ReadKey().Key;

                if (key == ConsoleKey.Escape || _exploringMap.Player == null)
                {
                    return;
                }

                else if (_walk.KeyDictionary.ContainsKey(key))
                {
                    var direction = _walk.KeyDictionary[key];
                    var exploring = _walk.Step(direction);
                    Step(direction, exploring);
                    _exploringMap.PrintWithComment(exploring.ToName());
                }
            }
        }

        #endregion
    }
}
