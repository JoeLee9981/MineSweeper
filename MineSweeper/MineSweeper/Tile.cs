// Joe Lee

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MineSweeper
{
    class Tile : Button
    {

        public int NumberOfMines;
        public int Row { get; private set; }
        public int Column { get; private set; }
        public bool IsViewed = false;
        public string SoundFilePath { protected set; get; }
        public int RightClickCount { private set; get; }

        private string[] imageName = { "tile", "flag", "questionmark" };


        public Tile(int numOfMines, int row, int col)
        {
            RightClickCount = 0;
            NumberOfMines = numOfMines;
            Row = row;
            Column = col;
            Height = 60;
            Width = 60;
            FontSize = 32;
            Foreground = GetColor();
            FontWeight = FontWeights.Bold;
            Background = new ImageBrush((ImageSource)FindResource("tile"));
            SoundFilePath = "Sounds/MineSweeperMove.mp3";
                
        }

        //methods

        private Brush GetColor()
        {
            switch (NumberOfMines)
            {
                case 1:
                    return Brushes.Blue;
                case 2:
                    return Brushes.Green;
                case 3:
                    return Brushes.Red;
                case 4:
                    return Brushes.Purple;
                case 5:
                    return Brushes.Orange;
                case 6:
                    return Brushes.Olive;
                case 7:
                    return Brushes.Pink;
                case 8:
                    return Brushes.YellowGreen;
                default:
                    return Brushes.White;
            }
        }

        public void RightClick()
        {
                switch (++RightClickCount % 3)
                {
                    case 0:
                        Background = new ImageBrush((ImageSource)FindResource(imageName[0]));
                        break;
                    case 1:
                        Background = new ImageBrush((ImageSource)FindResource(imageName[1]));
                        break;
                    case 2:
                        Background = new ImageBrush((ImageSource)FindResource(imageName[2]));
                        break;
                    default:
                        Background = new ImageBrush((ImageSource)FindResource(imageName[0]));
                        break;
                }
        }

        public virtual bool LeftClick()
        {
            bool gameOver = false;
            SetBackground("viewedTile");
            if (NumberOfMines > 0)
            {
                Content = NumberOfMines;
            }
            IsViewed = true;
            return gameOver;
        }

        public void SetBackground(string imageName)
        {
            Background = new ImageBrush((ImageSource)FindResource(imageName));
        }
    }
}
