using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game
{
    [Serializable]
    public class Player
    {
        #region private fields

        private Cell _cell;

        #endregion

        #region public properties

        public Cell Cell => _cell;

        #endregion

        #region ctors

        public Player() { }

        #endregion


        #region public methods
        public void TakeCell(Cell cell)
        {
            if (_cell == cell)
            {
                return;
            }

            if (_cell != null)
            {
                _cell.Leave();
            }

            _cell = cell;
            _cell.Occupy(this);
        }

        public void LeaveCell()
        {
            if (_cell != null)
            {
                _cell.Leave();
            }
            _cell = null;
        }

        #endregion
    }
}
