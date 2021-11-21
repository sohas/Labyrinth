using System;
using System.Collections.Generic;
using static Game.Direction;

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
            _counter = 1;
            _exploredMaps = new List<Map>();
            _walk = new Walk(map);
            _exploringMap = new Map($"{map.Name} explore {_counter}", (map.Width * 2) - 1, (map.Height * 2) - 1, true);
            _explorer = new Player();
            _exploringMap.TakePlayer(_explorer, map.Width - 1, map.Height - 1);
        }

        #endregion

        #region private methods

        private static Direction TakeOpposite(Direction direction)
        {
            return direction switch
            {
                Left => Right,
                Right => Left,
                Up => Down,
                Down => Up,
                _ => throw new NotImplementedException(),
            };
        }

        #endregion



        #region public methods

        public void Step(Direction direction, Exploring exploring)
        {
            Cell currentCell = _exploringMap.Player.Cell;
            int column = currentCell.Column;
            int row = currentCell.Row;

            switch (exploring)
            {
                case Exploring.Out:
                    _exploringMap.ExtractPlayer();
                    _exploredMaps.Add(_exploringMap);
                    return;

                case Exploring.Walled:
                    _exploringMap.Cells[row, column].SetWalls(direction);
                    return;

                case Exploring.Passed:
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
                    currentCell.BreakWalls(direction);
                    currentCell = _exploringMap.Cells[row, column];
                    _exploringMap.Player.TakeCell(currentCell);
                    break;

                case Exploring.Holed:
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
                    currentCell.BreakWalls(direction);
                    currentCell = _exploringMap.Cells[row, column];
                    currentCell.BreakWalls(TakeOpposite(direction));
                    currentCell.SetHole(new Hole(-1, -1));
                    _exploringMap.Player.TakeCell(currentCell);
                    _exploredMaps.Add(_exploringMap);
                    _counter++;
                    Map map = _walk.Map;
                    _exploringMap = new Map($"{map.Name} explore {_counter}", (map.Width * 2) - 1, (map.Height * 2) - 1, true);
                    _explorer = new Player();
                    _exploringMap.TakePlayer(_explorer, map.Width - 1, map.Height - 1);
                    return;

                case Exploring.Empty:
                    break;

                default:
                    break;
            }
        }

        public void Move()
        {
            _exploringMap.PrintMap($"it is a labyrinth. you can move with arrows. esc - exit");

            while (true)
            {
                ConsoleKey key = Console.ReadKey().Key;

                if (key == ConsoleKey.Escape || _exploringMap.Player == null)
                {
                    return;
                }

                if (key == ConsoleKey.P)
                {
                    Console.WriteLine();
                    Console.WriteLine();

                    foreach (Map map in _exploredMaps)
                    {
                        map.PrintMap(map.Name);
                    }
                }

                else if (Walk.KeyDictionary.ContainsKey(key))
                {
                    Direction direction = Walk.KeyDictionary[key];
                    Exploring exploring = _walk.Step(direction);
                    Step(direction, exploring);
                    _exploringMap.PrintMap(exploring.ToName());
                }
            }
        }

        #endregion
    }
}
