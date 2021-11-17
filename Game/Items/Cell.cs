using System;
using System.Collections.Generic;
using static Game.Direction;

namespace Game
{
    public class Cell
    {
        #region private fields

        private readonly int _column = -1;
        private readonly int _row = -1;
        private bool _wallL;
        private bool _wallR;
        private bool _wallU;
        private bool _wallD;
        private bool _unvisited;
        private Hole _hole;
        private Player _player;

        #endregion

        #region public properties

        public int Column => _column;
        public int Row => _row;
        public Player Player => _player;
        public bool Occupied => _player != null;
        public bool Unvisited => _unvisited;
        public Hole Hole => _hole;

        #endregion

        #region ctors

        public Cell(int column, int row, bool unvusited = false, params Direction[] directions) 
        {
            _column = column;
            _row = row;
            _unvisited = unvusited;
            SetWalls(directions);
        }

        public Cell(int column, int row, string walls) 
        {
            _column = column;
            _row = row;

            if (walls.Length != 4) 
            {
                throw new CellException($"string {walls} has wrong format. must be 4 difits of 0 or 1");
            }

            var res = new List<Direction>();

            for (var i = 0; i < 4; i++) 
            {
                if (walls[i] == '1')
                {
                    res.Add((Direction)i);
                }
                else if (walls[i] != '0') 
                {
                    throw new CellException($"string {walls} has wrong format. it must be 4 difits of 0 or 1");
                }
            }

            SetWalls(res.ToArray());
        }

        #endregion

        #region private methods
        private void SetWall(Direction direction)
        {
            switch (direction)
            {
                case Left:
                    _wallL = true;
                    break;
                case Right:
                    _wallR = true;
                    break;
                case Up:
                    _wallU = true;
                    break;
                case Down:
                    _wallD = true;
                    break;
                default:
                    break;
            }
        }

        private void BreakWall(Direction direction)
        {
            switch (direction)
            {
                case Left:
                    _wallL = false;
                    break;
                case Right:
                    _wallR = false;
                    break;
                case Up:
                    _wallU = false;
                    break;
                case Down:
                    _wallD = false;
                    break;
                default:
                    break;
            }
        }

        #endregion

        #region public methods

        public bool Wall(Direction direction)
        {
            switch (direction)
            {
                case Left:
                    return _wallL;
                case Right:
                    return _wallR;
                case Up:
                    return _wallU;
                case Down:
                    return _wallD;
                default:
                    throw new Exception("wrong direction)");
            }
        }

        public void SetWalls(params Direction[] directions)
        {
            foreach (var dir in directions)
            {
                SetWall(dir);
            }
        }

        public void BreakWalls(params Direction[] directions)
        {
            foreach (var dir in directions)
            {
                BreakWall(dir);
            }
        }

        public void SetHole(Hole hole) 
        {
            _hole = hole;
        }
        
        public void Occupy(Player player) 
        {
            if (_player == player) 
            {
                return;
            }
            _unvisited = false;
            _player = player;
            player.TakeCell(this);
        }

        public void Leave()
        {
            _player = null;
        }

        public bool CanPath(Direction direction) => !Wall(direction);

        #endregion
    }
}
