// Joe Lee

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace MineSweeper
{
    class Game
    {
        // fields
        private UniformGrid grid;
        private Tile[,] board;

        private Label titleLabel;
        private StackPanel stackPanel;
        private MediaElement soundElement;

        private int length = 9;
        private int width = 9;
        public bool gameOver = false;

        // constructor
        public Game(UniformGrid g, Label label, StackPanel panel)
        {
            // TODO
            grid = g;
            titleLabel = label;
            stackPanel = panel;
            soundElement = InitializeMediaElement();
            stackPanel.Children.Add(soundElement);
            soundElement.Play();
            board = new Tile[grid.Rows, grid.Columns];
            CreateBoard();
            AddToGrid();
        }

        // methods
        #region setup
  
        private MediaElement InitializeMediaElement()
        {
            MediaElement element = new MediaElement();
            element.LoadedBehavior = MediaState.Manual;
            element.UnloadedBehavior = MediaState.Stop;
            element.MediaEnded += new System.Windows.RoutedEventHandler(
                (o, e) => element.Stop());
            element.Source = new System.Uri("Sounds/MineSweeper.mp3", UriKind.Relative);
            return element;
        }

        private void CreateBoard()
        {
            AddMines(board);
            AddTiles(board);
        }

        private void AddMines(Tile[,] board)
        {
            Random random = new Random();
            for (int count = 0; count < 10; )
            {
                int row = random.Next(length);
                int col = random.Next(width);
                if (board[row, col] == null)
                {
                    Mine mine = new Mine(row, col);
                    mine.MouseRightButtonDown += Tile_RightClick;
                    mine.Click += Tile_Click;
                    board[row, col] = mine;
                    count++;
                }
            }
        }

        private void AddTiles(Tile[,] board)
        {
            for (int row = 0; row <= board.GetUpperBound(0); row++)
            {
                for (int col = 0; col <= board.GetUpperBound(1); col++)
                {
                    if (board[row, col] == null)
                    {
                        Tile tile = new Tile(GetNumberOfMines(board, row, col), row, col);
                        tile.MouseRightButtonDown += Tile_RightClick;
                        tile.Click += Tile_Click;
                        board[row, col] = tile;
                    }
                }
            }
        }

        private void AddToGrid()
        {
            for (int row = 0; row <= board.GetUpperBound(0); row++)
            {
                for (int col = 0; col <= board.GetUpperBound(1); col++)
                {
                    grid.Children.Add(board[row, col]);
                }
            }
        }

        #endregion

        #region events

        public void Tile_RightClick(Object sender, MouseButtonEventArgs e)
        {
            Tile tile = sender as Tile;
            if (tile.IsViewed == false)
            {
                tile.RightClick();
            }
        }

        public void Tile_Click(Object sender, EventArgs e)
        {
            Tile tile = sender as Tile;

            if (tile.IsViewed == false && gameOver == false && tile.RightClickCount % 3 != 1)
            {
                soundElement.Source = new System.Uri(tile.SoundFilePath, UriKind.Relative);
                soundElement.Play();
                gameOver = tile.LeftClick();
                if (gameOver)
                {
                    GameOver();
                }
                else if (gameOver = CheckGameWon())
                {
                    Winner();

                }
                else
                {
                    ClickNeighbors(tile);
                }
            }
        }

        private void Timer_Tick(Object sender, EventArgs e)
        {
            ShowMines("blownMine");
            titleLabel.Foreground = Brushes.DarkRed;
            titleLabel.Content = "Game Over";
            SetFocusable();

            DispatcherTimer t = sender as DispatcherTimer;
            t.Stop();
        }

        #endregion

        #region internalMethods

        private int GetNumberOfMines(Tile[,] board, int row, int col)
        {
            int mineCount = 0;
            int rowStart = row - 1;
            int rowStop = row + 1;
            int colStart = col - 1;
            int colStop = col + 1;

            for (int rowIndex = rowStart; rowIndex <= rowStop; rowIndex++)
            {
                for (int colIndex = colStart; colIndex <= colStop; colIndex++)
                {
                    if (OutOfBounds(rowIndex, colIndex))
                    {
                        continue;
                    }
                    if (board[rowIndex, colIndex] is Mine)
                    {
                        mineCount++;
                    }
                }
            }
            return mineCount;
        }

        private bool OutOfBounds(int row, int col)
        {
            return row < 0 || row > board.GetUpperBound(0) ||
                col < 0 || col > board.GetUpperBound(1);
        }

        private void ShowMines(string imageName)
        {
            for (int row = 0; row <= board.GetUpperBound(0); row++)
            {
                for (int col = 0; col <= board.GetUpperBound(1); col++)
                {
                    if (board[row, col] is Mine)
                    {
                        board[row, col].SetBackground(imageName);
                    }
                }
            }
        }

        private void ClickNeighbors(Tile tile)
        {
            int rowStart = tile.Row - 1;
            int rowStop = tile.Row + 1;
            int colStart = tile.Column - 1;
            int colStop = tile.Column + 1;

            for (int rowIndex = rowStart; rowIndex <= rowStop; rowIndex++)
            {
                for (int colIndex = colStart; colIndex <= colStop; colIndex++)
                {
                    if (OutOfBounds(rowIndex, colIndex))
                    {
                        continue;
                    }
                    if (!(board[rowIndex, colIndex] is Mine) && board[rowIndex, colIndex].NumberOfMines == 0)
                    {
                        ClickNeighborsInternal(board[rowIndex, colIndex]);
                    }
                }
            }
        }

        private void ClickNeighborsInternal(Tile tile)
        {
            if (tile.IsViewed)
            {
                return;
            }
            if (tile.NumberOfMines > 0)
            {
                tile.LeftClick();
                return;
            }
            tile.LeftClick();
            int rowStart = tile.Row - 1;
            int rowStop = tile.Row + 1;
            int colStart = tile.Column - 1;
            int colStop = tile.Column + 1;

            for (int rowIndex = rowStart; rowIndex <= rowStop; rowIndex++)
            {
                for (int colIndex = colStart; colIndex <= colStop; colIndex++)
                {
                    if (OutOfBounds(rowIndex, colIndex))
                    {
                        continue;
                    }
                    if (!(board[rowIndex, colIndex] is Mine))
                    {
                        ClickNeighborsInternal(board[rowIndex, colIndex]);
                    }
                }
            }
        }
        #endregion

        #region EndGameMethods

        private bool CheckGameWon()
        {
            for (int row = 0; row <= board.GetUpperBound(0); row++)
            {
                for (int col = 0; col <= board.GetUpperBound(0); col++)
                {
                    if (board[row, col] is Mine)
                    {
                        continue;
                    }
                    if (board[row, col].IsViewed == false)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private void GameOver()
        {
            ShowMines("mine");
            //2 second delay
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 2);
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void Winner()
        {
            System.Media.SystemSounds.Exclamation.Play();
            ShowMines("mine");
            titleLabel.Foreground = Brushes.DarkGreen;
            titleLabel.Content = "Winner";
            SetFocusable();
        }

        public void SetFocusable()
        {
            for (int row = 0; row <= board.GetUpperBound(0); row++)
            {
                for (int col = 0; col <= board.GetUpperBound(0); col++)
                {
                    board[row, col].Focusable = false;
                }
            }
        }

        #endregion
    }
}
