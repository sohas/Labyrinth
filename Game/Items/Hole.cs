using System;

namespace Game
{
    public class Hole
    {
        #region private fields

        private readonly int _columnTarget;
        private readonly int _rowTarget;

        #endregion


        #region public properties

        public int ColumnTarget => _columnTarget;
        public int RowTarget => _rowTarget;

        #endregion


        #region ctors

        public Hole(int rowTarget, int columnTarget)
        {
            _columnTarget = columnTarget;
            _rowTarget = rowTarget;
        }

        #endregion
    }
}
