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
        private Player _player;
        private int _counter;

        #endregion

        #region public properties

        public Map Map => _exploringMap;
        public List<Map> ExploredMaps => _exploredMaps;
        public MapSymbol[,] MapVisual => _exploringMap.GetVisual();

        #endregion

        #region ctors

        public Explore(Map basicMap)
        {
            _counter = 1;
            _exploredMaps = new List<Map>();
            _walk = new Walk(basicMap);
            _exploringMap = new Map($"{basicMap.Name} explore {_counter}", (basicMap.Height * 2) - 1, (basicMap.Width * 2) - 1, true);
            _player = new Player();
            _exploringMap.TakePlayer(_player, basicMap.Height - 1, basicMap.Width - 1);
            _exploringMap.MarkStart(basicMap.Height - 1, basicMap.Width - 1);
        }

        #endregion

        #region private methods

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
                    currentCell.SetHole(new Hole(-1, -1));
                    currentCell.Leave();
                    _exploredMaps.Add(_exploringMap);
                    _counter++;
                    Map map = _walk.Map;
                    _exploringMap = new Map($"{map.Name} explore {_counter}", (map.Height * 2) - 1, (map.Width * 2) - 1, true);
                    _player = new Player();
                    _exploringMap.TakePlayer(_player, map.Height - 1, map.Width - 1);
                    _exploringMap.MarkStart(map.Height - 1, map.Width - 1);
                    return;

                case Exploring.Empty:
                    break;

                default:
                    break;
            }
        }

        public void Step(Direction direction)
        {
            Exploring exploring = _walk.Step(direction);
            Step(direction, exploring);
            return;
        }

        public void ConsoleMove()
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
