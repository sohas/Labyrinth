using System;
using static Game.Direction;


namespace Game
{
    [Serializable]
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
        private bool _started;

        #endregion

        #region public properties

        public int Column => _column;
        public int Row => _row;
        public bool Occupied => _player != null;
        public bool Unvisited => _unvisited;
        public Hole Hole => _hole;
        public bool Started => _started;

        #endregion

        #region ctors

        public Cell(int row, int column, bool unvisited = false, bool allWallsAbsent = false, params Direction[] directions)
        {
            _column = column;
            _row = row;
            _unvisited = unvisited;
            SetWalls(directions);
            if (allWallsAbsent) 
            {
                BreakWalls(Left, Right, Up, Down);
            }
        }

        public Cell(int row, int column, string walls)
        {
            _column = column;
            _row = row;

            if (walls.Length != 4)
            {
                throw new CellException($"string {walls} has wrong format. must be 4 difits of 0 or 1");
            }

            for (int i = 0; i < 4; i++)
            {
                char ch = walls[i];
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
            WallState wallState = destroy ? WallState.Absent : WallState.Present;

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
            foreach (Direction dir in directions)
            {
                SetWall(dir);
            }
        }

        public void BreakWalls(params Direction[] directions)
        {
            foreach (Direction dir in directions)
            {
                SetWall(dir, true);
            }
        }

        public void SetHole(Hole hole)
        {
            _hole = hole;
        }

        public void MarkStarted()
        {
            _started = true;
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

        public static bool StrongEquity(Cell first, Cell second)
        {
            return
                first._column == second._column &&
                first._row == second.Row &&
                first._wallL == second._wallL &&
                first._wallR == second._wallR &&
                first._wallU == second._wallU &&
                first._wallD == second._wallD &&
                first._unvisited == second._unvisited &&
                (
                    (first._hole == null && second.Hole == null) ||
                    (
                        first._hole != null &&
                        second._hole != null &&
                        first._hole.ColumnTarget == second._hole.ColumnTarget &&
                        first._hole.RowTarget == second._hole.RowTarget
                     )
                ) &&
                first._player.Cell.Column == second._player.Cell.Column &&
                first._started == second._started &&
                true;
        }

        public static bool SoftEquity(Cell first, Cell second)
        {
            bool res =
                first._wallL == second._wallL &&
                first._wallR == second._wallR &&
                first._wallU == second._wallU &&
                first._wallD == second._wallD &&
                (
                    (first._hole == null && second.Hole == null) ||
                    (
                        first._hole != null &&
                        second._hole != null &&
                        first._hole.ColumnTarget == second._hole.ColumnTarget &&
                        first._hole.RowTarget == second._hole.RowTarget
                     )
                ) &&
                true;

            return res;
        }

        #endregion
    }
}
