using System;
using System.Collections.Generic;
using static Game.Direction;
using static Game.WallState;

namespace Game
{
    public class Cell
    {
        #region private fields

        private readonly int _column;
        private readonly int _row;
        private WallState _wallL;
        private WallState _wallR;
        private WallState _wallU;
        private WallState _wallD;
        private bool _unvisited;
        private Hole _hole;
        private Player _player;

        #endregion

        #region public properties

        public int Column => _column;
        public int Row => _row;
        public bool Occupied => _player != null;
        public bool Unvisited => _unvisited;
        public Hole Hole => _hole;

        #endregion

        #region ctors

        public Cell(int column, int row, bool unvisited = false, params Direction[] directions) 
        {
            _column = column;
            _row = row;
            _unvisited = unvisited;
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

            for (var i = 0; i < 4; i++) 
            {
                var ch = walls[i];
                if (ch == '1')
                {
                    SetWall((Direction)i);
                }
                else if (ch == '0') 
                {
                    SetWall((Direction)i, true);
                }
                else 
                {
                    throw new CellException($"string {walls} has wrong format. it must be 4 difits of 0 or 1");
                }
            }
        }

        #endregion

        #region private methods
        private void SetWall(Direction direction, bool destroy = false)
        {
            WallState wallState = destroy ? Absent : Present;

            switch (direction)
            {
                case Left:
                    _wallL = wallState;
                    break;
                case Right:
                    _wallR = wallState;
                    break;
                case Up:
                    _wallU = wallState;
                    break;
                case Down:
                    _wallD = wallState;
                    break;
                default:
                    break;
            }
        }

        #endregion

        #region public methods

        public WallState Wall(Direction direction)
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
                SetWall(dir, true);
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

        #endregion
    }
}
