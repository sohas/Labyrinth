using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using static Game.Direction;

namespace Game
{
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

        #endregion


        #region ctors
        public Map(string name, int width, int height, bool allUnvisited = false, bool allWalls = false) 
        {
            _name = name;
            _width = width;
            _height = height;
            _cells = new Cell[height, width];
            for (var i = 0; i < height; i++) 
            {
                for (var j = 0; j < width; j++) 
                {
                    _cells[i, j] = new Cell(j, i, allUnvisited);
                    if (allWalls) 
                    { 
                        _cells[i, j].SetWalls(Left, Right, Up, Down); 
                    }
                }
            }
        }

        public Map(string filename)
        {
            var lines = File.ReadAllLines(filename);
            var dict = new Dictionary<char, string>();

            if (lines.Length < 20) 
            {
                throw new MapException($"very few lines in the map file ({lines.Length}). " +
                    $"it must contain at least 1 line of name, 1 empty line, 16 lines of mask," +
                    $"1 empty line and 1 line of map size.");
            }
            else
            {
                _name = lines[0];

                for (var i = 2; i < 18; i++)
                {
                    if (lines[i].Length != 5)
                    {
                        throw new MapException($"string { lines[i] } has wrong format. it must be symbol and then 4 difits of 0 or 1:" +
                            $"symbol/wall_left/wall_right/wall_up/wall_down. if 1 - wall presents, if 0 - absents");
                    }
                    dict[lines[i][0]] = lines[i][1..];
                }
            }

            var mapSize = lines[19].Split(',');

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

            for (var i = 0; i < height; i++) 
            {
                var row = lines[i + 21];
                if (row.Length != width) 
                {
                    throw new MapException($"line of map descripion ({row}) has wrong format. " +
                        $"it must consist of {width} symbols described in lines 3-18)");
                }
                for (var j = 0; j < width; j++) 
                {
                    if (dict.ContainsKey(row[j]))
                    {
                        _cells[i, j] = new Cell(j, i, dict[row[j]]);
                    }
                    else 
                    {
                        throw new MapException($"line of map descripion ({row}) has wrong format. " +
                            $"it must consist of {width} symbols described in lines 3-18)");
                    }
                }
            }

            int holesCount;

            if (lines.Length > 22 + height && int.TryParse(lines[22 + height], out holesCount))
            {
                for (var i = 24 + height; i < 24 + height + holesCount; i++) 
                {
                    var holeDescription = lines[i].Split(',');
                    
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
                        _cells[rowStart, columnStart].SetHole(new Hole(columnTarget, rowTarget));
                    }
                    else 
                    {
                        throw new MapException($"in this line ({i}) must be 4 positive digits (or 0), devided by comma: " +
                            $"column start, row start, column target, row target. column must be < {width}, row must be < {height}");
                    }
                }
            }
        }

        #endregion


        #region public methods

        public void MakePerimetr() 
        {
            for (var i = 0; i < Height; i++)
            {
                _cells[i, 0].SetWalls(Left);
                _cells[i, Width - 1].SetWalls(Right);
            }
            for (var j = 0; j < Width; j++)
            {
                _cells[0, j].SetWalls(Up);
                _cells[Height - 1, j].SetWalls(Down);
            }

        }

        public void TakePlayer(Player player, int column, int row)
        {
            if (column < 0 || column >= Width || row < 0 || row >= Height) 
            {
                throw new MapException($"position of player (column:{column}, row:{row}) is out of map (width:{Width}, height:{Height})");
            }

            _player = player;
            _cells[row, column].Occupy(_player);
        }

        public void ExtractPlayer() 
        {
            _player.LeaveCell();
            _player = null;
        }

        public void TakeHole(int column, int row, Hole hole) 
        {
            _cells[row, column].SetHole(hole);
        }

        public override string ToString()
        {
            var res = new StringBuilder();
            for (var i = 0; i < Height; i++) 
            {
                var fstStr = new StringBuilder();
                var sndStr = new StringBuilder();
                for (var j = 0; j < Width; j++) 
                {
                    fstStr.Append('+');
                    
                    bool wallUp, wallDown, wallLeft, wallRight, occupied, hole, unvisited;

                    wallUp = i==0 || _cells[i - 1, j].Wall(Down);
                    wallDown = _cells[i, j].Wall(Up);
                    wallLeft = j==0 || _cells[i, j - 1].Wall(Right);
                    wallRight = _cells[i, j].Wall(Left);
                    occupied = _cells[i, j].Occupied;
                    hole = _cells[i, j].Hole != null;
                    unvisited = _cells[i, j].Unvisited;

                    if (wallUp && wallDown)
                    {
                        fstStr.Append('-');
                    }
                    else if (wallUp && !wallDown)
                    {
                        fstStr.Append('^');
                    }
                    else if (!wallUp && wallDown)
                    {
                        fstStr.Append('v');
                    }
                    else
                    {
                        fstStr.Append(' ');
                    }

                    if (wallLeft && wallRight)
                    {
                        sndStr.Append('|');
                    }
                    else if (wallLeft && !wallRight)
                    {
                        sndStr.Append('<');
                    }
                    else if (!wallLeft && wallRight)
                    {
                        sndStr.Append('>');
                    }
                    else
                    {
                        sndStr.Append(' ');
                    }

                    if (occupied && hole)
                    {
                        sndStr.Append('x');
                    }
                    else if (occupied)
                    {
                        sndStr.Append('i');
                    }
                    else if (hole)
                    {
                        sndStr.Append('o');
                    }
                    else if (unvisited) 
                    {
                        sndStr.Append('X');
                    }
                    else
                    {
                        sndStr.Append(' ');
                    }
                }
                fstStr.Append("+\n");
                sndStr.Append(_cells[i, Width - 1].Wall(Right) ? "|\n" : ">\n");
                res.Append(fstStr);
                res.Append(sndStr);
            }
            for (var j = 0; j < Width; j++) 
            {
                res.Append(_cells[Height - 1, j].Wall(Down) ? "+-" : "+v");
            }
            res.Append('+');

            return res.ToString();
        }

        public void PrintWithComment(string comment) 
        {
            Console.WriteLine(comment);
            Console.WriteLine();
            Console.WriteLine(ToString());
            Console.WriteLine();
        }

        #endregion
    }
}
