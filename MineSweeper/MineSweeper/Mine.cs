// Joe Lee
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MineSweeper
{
    class Mine : Tile
    {

        public Mine(int row, int column)
            : base(-1, row, column)
        {
            SoundFilePath = "Sounds/MineSweeperMines.mp3";
        }

        public override bool LeftClick()
        {
            bool gameOver = true;
            return gameOver;
        }


    }

}
