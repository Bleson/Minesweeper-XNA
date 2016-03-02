using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Minesweeper
{
    class Square
    {
        public bool isClicked = false;
        public bool isMine = false;
        public bool isFlagged = false;
        public int minesNearby = 0;
        //public int gridPositionX;
        //public int gridPositionY;
        

        public void AddMine()
        {
            isMine = true;
        }

        public void Open()
        {
            isClicked = true;
            if(isMine)
            {
                //lose
            }
        }

        public void Flag()
        {
            if (!isClicked)
            {
                isFlagged = !isFlagged;
                
            }
        }
    }
}
