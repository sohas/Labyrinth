﻿using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using static Game.Direction;
using System.Linq;

namespace Game
{
    [Serializable]
    public class Map
    {
        #region private fields

        private readonly string _name;
        private readonly int _width;
        private readonly int _height;
        private readonly Cell[,] _cells; // [vertical down, horizontal to right]
        private Player _player;

        #endregion

        #region public properties
        public string Name => _name;
        public int Width => _width;
        public int Height => _height;
        public Cell[,] Cells => _cells;
        public Player Player => _player;
        public bool IsReachable => CheckReachable();

        #endregion

        #region ctors
        public Map(string name, int height, int width, bool allUnvisited = false, bool allWallsAbsent = false)
        {
            _name = name;
            _width = width;
            _height = height;
            _cells = new Cell[height, width];
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    _cells[i, j] = new Cell(i, j, allUnvisited, allWallsAbsent);
                }
            }
        }

        public Map(string filename)
        {
            string[] lines = File.ReadAllLines(filename);
            Dictionary<char, string> dict = new();

            if (lines.Length < 20)
            {
                throw new MapException($"very few lines in the map file ({lines.Length}). " +
                    $"it must contain at least 1 line of name, 1 empty line, 16 lines of mask," +
                    $"1 empty line and 1 line of map size.");
            }
            else
            {
                _name = lines[0];

                for (int i = 2; i < 18; i++)
                {
                    if (lines[i].Length != 5)
                    {
                        throw new MapException($"string { lines[i] } has wrong format. it must be symbol and then 4 difits of 0 or 1:" +
                            $"symbol/wall_left/wall_right/wall_up/wall_down. if 1 - wall presents, if 0 - absents");
                    }
                    dict[lines[i][0]] = lines[i][1..];
                }
            }

            string[] mapSize = lines[19].Split(',');

            if (mapSize.Length != 2)
            {
                throw new MapException($"in the 20th line of the file must be two positive integer digits, divided by comma");
            }

            int width, height;

            if (!int.TryParse(mapSize[0], out width) || !int.TryParse(mapSize[1], out height))
            {
                throw new MapException($"in the 20th line of the file must be two positive integer digits, divided by comma");
            }

            _height = height;
            _width = width;

            if (lines.Length < 21 + height)
            {
                throw new MapException($"very few lines in the map file ({lines.Length}). " +
                    $"it must contain at least 1 line of name, 1 empty line, 16 lines of mask, 1 empty line," +
                    $"line of map size ({height},{width} in this case), 1 empty line and {height} lines of map description.");
            }

            _cells = new Cell[height, width];

            for (int i = 0; i < height; i++)
            {
                string row = lines[i + 21];

                if (row.Length != width)
                {
                    throw new MapException($"line of map descripion ({row}) has wrong format. " +
                        $"it must consist of {width} symbols described in lines 3-18)");
                }
                
                for (int j = 0; j < width; j++)
                {
                    _cells[i, j] = dict.ContainsKey(row[j]) ?
                        new Cell(i, j, dict[row[j]]) :
                        throw new MapException($"line of map descripion ({row}) has wrong format. " +
                            $"it must consist of {width} symbols described in lines 3-18)");
                }
            }

            if (lines.Length < 23 + height)
            {
                throw new MapException($"very few lines in the map file ({lines.Length}). " +
                    $"it must contain at least 1 line of name, 1 empty line, 16 lines of mask, 1 empty line," +
                    $"line of map size ({height},{width} in this case), 1 empty line, {height} lines of map description," +
                    $"1 empty line and line to describe player.");
            }

            string[] playerDescription = lines[22 + height].Split(',');

            if (playerDescription.Length != 2)
            {
                throw new MapException($"in this line ({22 + height}) must be 2 positive digits (or 0), devided by comma: " +
                    $"column, row. these ara player coordinates. column must be < {width}, row must be < {height}");
            }

            int playerColumn, playerRow;

            if (int.TryParse(playerDescription[0], out playerColumn) &&
                        int.TryParse(playerDescription[1], out playerRow) &&
                        playerColumn >= 0 && playerColumn < width &&
                        playerRow >= 0 && playerRow < height)
            {
                _player = new Player();
                _cells[playerRow, playerColumn].Occupy(_player);
                _cells[playerRow, playerColumn].MarkStarted();
            }
            else
            {
                throw new MapException($"in this line ({22 + height}) must be 2 positive digits (or 0), devided by comma: " +
                    $"column, row. these ara player coordinates. column must be < {width}, row must be < {height}");
            }

            int holesCount;

            if (lines.Length > 24 + height && int.TryParse(lines[24 + height], out holesCount))
            {
                for (int i = 26 + height; i < 26 + height + holesCount; i++)
                {
                    string[] holeDescription = lines[i].Split(',');

                    if (holeDescription.Length != 4)
                    {
                        throw new MapException($"in this line ({i}) must be 4 positive digits (or 0), devided by comma: " +
                            $"column start, row start, column target, row target. column must be < {width}, row must be < {height}");
                    }

                    int columnStart, rowStart, columnTarget, rowTarget;

                    if (int.TryParse(holeDescription[0], out columnStart) &&
                        int.TryParse(holeDescription[1], out rowStart) &&
                        int.TryParse(holeDescription[2], out columnTarget) &&
                        int.TryParse(holeDescription[3], out rowTarget) &&
                        columnStart >= 0 && columnStart < width &&
                        rowStart >= 0 && rowStart < height &&
                        columnTarget >= 0 && columnTarget < width &&
                        rowTarget >= 0 && rowTarget < height)
                    {
                        _cells[rowStart, columnStart].SetHole(new Hole(rowTarget, columnTarget));
                    }
                    else
                    {
                        throw new MapException($"in this line ({i}) must be 4 positive digits (or 0), devided by comma: " +
                            $"column start, row start, column target, row target. column must be < {width}, row must be < {height}");
                    }
                }
            }
        }

        public Map(MapSymbol[,] symbols) //////////////////////////////// не используется
        {
            int h = symbols.GetLength(0);
            int w = symbols.GetLength(1);
            if (h % 2 != 1 || w % 2 != 1) 
            {
                throw new MapException($"wrong symbols size {h} {w}. they both must be odd and >=3");
            }

            _height = (h - 1) / 2;
            _width = (w - 1) / 2;
            _cells = new Cell[_height, _width];

            for (int i = 0; i < _height; i++)
            {
                for (int j = 0; j < _width; j++)
                {
                    MapSymbol cellSymbol = symbols[2 * i + 1, 2 * j + 1];
                    MapSymbol wallLSymbol = j == 0 ? MapSymbol.WallPresentVertical : symbols[2 * i + 1, 2 * j];
                    MapSymbol wallRSymbol = j == _width - 1 ? MapSymbol.WallPresentVertical : symbols[2 * i + 1, 2 * j + 2];
                    MapSymbol wallUSymbol = i == 0 ? MapSymbol.WallPresentHorizontal : symbols[2 * i, 2 * j + 1];
                    MapSymbol wallDSymbol = i == _height - 1 ? MapSymbol.WallPresentVertical : symbols[2 * i + 2, 2 * j + 1];

                    Cell cell = new Cell(i, j);

                    if (cellSymbol == MapSymbol.Hole) 
                    {
                        cell.SetHole(new Hole(0, 0)); ///////////////////////////////////////////////////////
                    }
                    if (wallLSymbol == MapSymbol.WallPresentVertical || wallLSymbol == MapSymbol.DiodeRight)
                    {
                        cell.SetWalls(Left);
                    }
                    if (wallRSymbol == MapSymbol.WallPresentVertical || wallRSymbol == MapSymbol.DiodeLeft)
                    {
                        cell.SetWalls(Right);
                    }
                    if (wallUSymbol == MapSymbol.WallPresentHorizontal || wallUSymbol == MapSymbol.DiodeDown)
                    {
                        cell.SetWalls(Up);
                    }
                    if (wallDSymbol == MapSymbol.WallPresentHorizontal || wallDSymbol == MapSymbol.DiodeUp)
                    {
                        cell.SetWalls(Down);
                    }

                    _cells[i, j] = cell;
                }
            }
        }

        #endregion

        #region private methods

        private static MapSymbol GetCrossType(int y, int x, MapSymbol[,] res) 
        {
            MapSymbol current = res[y, x];

            int resHeight = res.GetLength(0);
            int resWidth = res.GetLength(1);

            if (x < 0 ||
                y < 0 ||
                x >= resWidth ||
                y >= resHeight ||
                (current != MapSymbol.CrossPresent && current != MapSymbol.CrossUnsertain && current != MapSymbol.CrossAbsent))
            {
                return current;
            }

            MapSymbol wallLeft = x == 0 ? MapSymbol.WallAbsentHorizontal : res[y, x - 1];
            MapSymbol wallRight = x == resWidth - 1 ? MapSymbol.WallAbsentHorizontal : res[y, x + 1];
            MapSymbol wallUp = y == 0 ? MapSymbol.WallAbsentVertical : res[y - 1, x];
            MapSymbol wallDown = y == resHeight - 1 ? MapSymbol.WallAbsentVertical : res[y + 1, x];

            bool unsertain = false;

            if (wallLeft == MapSymbol.WallUnsertainHorizontal)
            {
                unsertain = true;
            }
            else if (wallLeft != MapSymbol.WallAbsentHorizontal) 
            {
                return MapSymbol.CrossPresent;
            }

            if (wallRight == MapSymbol.WallUnsertainHorizontal)
            {
                unsertain = true;
            }
            else if (wallRight != MapSymbol.WallAbsentHorizontal)
            {
                return MapSymbol.CrossPresent;
            }

            if (wallUp == MapSymbol.WallUnsertainVertical)
            {
                unsertain = true;
            }
            else if (wallUp != MapSymbol.WallAbsentVertical)
            {
                return MapSymbol.CrossPresent;
            }

            if (wallDown == MapSymbol.WallUnsertainVertical)
            {
                unsertain = true;
            }
            else if (wallDown != MapSymbol.WallAbsentVertical)
            {
                return MapSymbol.CrossPresent;
            }

            return unsertain ? MapSymbol.CrossUnsertain : MapSymbol.CrossAbsent;
        }

        private HashSet<Cell> GetNextCells(Cell cell, bool notHoled)
        {
            HashSet<Cell> res = new();

            int row;
            int column;

            if (notHoled)
            {
                row = cell.Row;
                column = cell.Column;

                if (cell.Wall(Left) == WallState.Absent && column > 0)
                {
                    res.Add(_cells[row, column - 1]);
                }
                if (cell.Wall(Right) == WallState.Absent && column < _width - 1)
                {
                    res.Add(_cells[row, column + 1]);
                }
                if (cell.Wall(Up) == WallState.Absent && row > 0)
                {
                    res.Add(_cells[row - 1, column]);
                }
                if (cell.Wall(Down) == WallState.Absent && row < _height - 1)
                {
                    res.Add(_cells[row + 1, column]);
                }
            }
            else
            {
                row = cell.Hole.RowTarget;
                column = cell.Hole.ColumnTarget;

                if (
                    row >= 0 &&
                    row < _height &&
                    column >= 0 &&
                    column < _width
                   )
                {
                    res.Add(_cells[row, column]);
                }
            }

            return res;
        }

        private HashSet<Cell> AllCells() 
        {
            HashSet<Cell> res = new();

            for (int i = 0; i < _height; i++) 
            {
                for (int j = 0; j < _width; j++) 
                {
                    res.Add(_cells[i, j]);
                }
            }

            return res;
        }

        private bool CheckReachable(Cell cell) 
        {
            HashSet<Cell> unvisited = AllCells();
            HashSet<(Cell, bool)> visited = new();
            Stack<(Cell, bool)> toVisit = new();
            toVisit.Push((cell, true));
            visited.Add((cell, true));
            unvisited.Remove(cell);

            while (toVisit.Count > 0)
            {
                (Cell, bool) stackElement = toVisit.Pop();
                Cell currentCell = stackElement.Item1;
                bool currentNotHoled = stackElement.Item2;

                HashSet<Cell> nextCells = GetNextCells(currentCell, currentNotHoled);
                foreach (Cell nextCell in nextCells)
                {
                    bool nextNotHoled = nextCell.Hole == null || !currentNotHoled;

                    if (!visited.Contains((nextCell, nextNotHoled)))
                    {
                        toVisit.Push((nextCell, nextNotHoled));
                        unvisited.Remove(nextCell);
                        visited.Add((nextCell, nextNotHoled));
                    }
                }
            }
            return unvisited.Count == 0;
        }

        private bool CheckReachable() 
        {
            HashSet<Cell> cells = AllCells();
            return !cells.Any(x => !CheckReachable(x));
        }

        #endregion

        #region public methods

        public void MakePerimetr()
        {
            for (int i = 0; i < _height; i++)
            {
                _cells[i, 0].SetWalls(Left);
                _cells[i, _width - 1].SetWalls(Right);
            }
            for (int j = 0; j < _width; j++)
            {
                _cells[0, j].SetWalls(Up);
                _cells[_height - 1, j].SetWalls(Down);
            }

        }

        public void TakePlayer(Player player, int row, int column)
        {
            if (row < 0 || row >= _height || column < 0 || column >= _width)
            {
                throw new MapException($"position of player (row:{row}, column:{column}) is out of map (height:{_height}, width:{_width})");
            }

            _player = player;
            _cells[row, column].Occupy(_player);
        }

        public void ExtractPlayer()
        {
            _player.LeaveCell();
            _player = null;
        }

        public void TakeHole(int row, int column, Hole hole)
        {
            _cells[row, column].SetHole(hole);
        }
        
        public void MarkStart(int row, int column)
        {
            _cells[row, column].MarkStarted();
        }

        public MapSymbol[,] GetVisual()
        {
            if (_height == 0 || _width == 0)
            {
                throw new MapException($"one of the map dimention is of 0 size (width {_width} x height {_height})");
            }

            MapSymbol[,] res = new MapSymbol[(_height * 2) + 1, (_width * 2) + 1];

            for (int i = 0; i < _height; i++)
            {
                for (int j = 0; j < _width; j++)
                {
                    MapSymbol visualCrossLeftUp = MapSymbol.CrossAbsent;

                    res[2 * i, 2 * j] = visualCrossLeftUp;

                    WallState wallUp, wallDown, wallLeft, wallRight;
                    bool occupied, hole, unvisited, started;

                    wallUp = i == 0 ? _cells[i, j].Wall(Up) : _cells[i - 1, j].Wall(Down);
                    wallDown = _cells[i, j].Wall(Up);
                    wallLeft = j == 0 ? _cells[i, j].Wall(Left) : _cells[i, j - 1].Wall(Right);
                    wallRight = _cells[i, j].Wall(Left);
                    occupied = _cells[i, j].Occupied;
                    hole = _cells[i, j].Hole != null;
                    unvisited = _cells[i, j].Unvisited;
                    started = _cells[i, j].Started;

                    MapSymbol visualWallUp;

                    if (wallUp == WallState.Present && wallDown == WallState.Absent)
                    {
                        visualWallUp = MapSymbol.DiodeUp;
                    }
                    else if (wallUp == WallState.Absent && wallDown == WallState.Present)
                    {
                        visualWallUp = MapSymbol.DiodeDown;
                    }
                    else if (wallUp == WallState.Present || wallDown == WallState.Present)
                    {
                        visualWallUp = MapSymbol.WallPresentHorizontal;
                    }
                    else if (wallUp == WallState.Uncertain && wallDown == WallState.Uncertain)
                    {
                        visualWallUp = MapSymbol.WallUnsertainHorizontal;
                    }
                    else
                    {
                        visualWallUp = MapSymbol.WallAbsentHorizontal;
                    }

                    res[i * 2, (j * 2) + 1] = visualWallUp;

                    MapSymbol visualWallLeft;

                    if (wallLeft == WallState.Present && wallRight == WallState.Absent)
                    {
                        visualWallLeft = MapSymbol.DiodeLeft;
                    }
                    else if (wallLeft == WallState.Absent && wallRight == WallState.Present)
                    {
                        visualWallLeft = MapSymbol.DiodeRight;
                    }
                    else if (wallLeft == WallState.Present || wallRight == WallState.Present)
                    {
                        visualWallLeft = MapSymbol.WallPresentVertical;
                    }
                    else if (wallLeft == WallState.Uncertain && wallRight == WallState.Uncertain)
                    {
                        visualWallLeft = MapSymbol.WallUnsertainVertical;
                    }
                    else
                    {
                        visualWallLeft = MapSymbol.WallAbsentVertical;
                    }

                    res[(i * 2) + 1, j * 2] = visualWallLeft;

                    MapSymbol visualCell;

                    if (unvisited)
                    {
                        visualCell = MapSymbol.Unvisited;
                    }
                    else if (occupied)
                    {
                        visualCell = MapSymbol.Player;
                    }
                    else if (hole)
                    {
                        visualCell = MapSymbol.Hole;
                    }
                    else if (started)
                    {
                        visualCell = MapSymbol.Start;
                    }
                    else
                    {
                        visualCell = MapSymbol.Visited;
                    }

                    res[(i * 2) + 1, (j * 2) + 1] = visualCell;
                }

                MapSymbol visualCrossEndLine = MapSymbol.CrossAbsent;

                res[2 * i, 2 * _width] = visualCrossEndLine;

                WallState lastWallR = _cells[i, Width - 1].Wall(Right);

                MapSymbol visualWallEndLine =
                    lastWallR == WallState.Present ?
                    MapSymbol.WallPresentVertical :
                    lastWallR == WallState.Absent ?
                    MapSymbol.DiodeRight :
                    MapSymbol.WallUnsertainVertical;

                res[(2 * i) + 1, 2 * _width] = visualWallEndLine;
            }

            for (int j = 0; j < Width; j++)
            {
                MapSymbol visualCrossLeftDawn = MapSymbol.CrossAbsent;

                res[2 * _height, 2 * j] = visualCrossLeftDawn;

                WallState lastWallD = _cells[Height - 1, j].Wall(Down);

                MapSymbol visualWallDawn =
                    lastWallD == WallState.Present ?
                    MapSymbol.WallPresentHorizontal :
                    lastWallD == WallState.Absent ?
                    MapSymbol.WallAbsentHorizontal :
                    MapSymbol.WallUnsertainHorizontal;

                res[2 * _height, (2 * j) + 1] = visualWallDawn;
            }

            MapSymbol lastCross = MapSymbol.CrossAbsent;

            res[2 * _height, 2 * _width] = lastCross;

            for (int i = 0; i <= 2 * _height; i++)
            {
                for (int j = 0; j <= 2 * _width; j++)
                {
                    res[i, j] = GetCrossType(i, j, res);
                }
            }

            return res;
        }

        public static bool CheckEquity(Map first, Map second, Func<Cell, Cell, bool> cellEquityKind)
        {

            if (
                first == null ||
                second == null ||
                first.Height != second.Height ||
                first.Width != second.Width ||
                false
               )
            {
                return false;
            }

            for (int i = 0; i < first.Height; i++)
            {
                for (int j = 0; j < first.Width; j++)
                {
                    if (!cellEquityKind(first.Cells[i, j], second.Cells[i, j]))
                    {
                        return false;
                    };
                }
            }

            return true;
        }

        public override string ToString()
        {
            MapSymbol[,] visual = GetVisual();

            int visualHigh = visual.GetLength(0);
            int visualWidth = visual.GetLength(1);

            StringBuilder res = new();

            for (int i = 0; i < visualHigh; i++)
            {
                for (int j = 0; j < visualWidth; j++)
                {
                    res.Append(visual[i, j].ToName());
                }
                res.Append('\n');
            }
            res.Remove(res.Length - 1, 1);

            return res.ToString();
        }

        public void PrintMap(string comment = null)//Console Method
        {
            if (comment != null)
            {
                Console.WriteLine(comment);
                Console.WriteLine();
            }
            Console.WriteLine(ToString());
            Console.WriteLine();
        }

        #endregion
    }
}
