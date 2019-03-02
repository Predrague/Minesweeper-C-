using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Minesweeper
{
    public enum FieldState { EMPTY, MINE, INACTIVE };
    class Field : Button
    {
        private int posX;
        private int posY;       
        private FieldState state;
        private bool flag;
        private int mineCount;

        public int PosX { get => posX; set => posX = value; }
        public int PosY { get => posY; set => posY = value; }
        public FieldState State { get => state; set => state = value; }
        public int MineCount { get => mineCount; set => mineCount = value; }
        public bool Flag { get => flag; set => flag = value; }

        public Field(int posX, int posY, FieldState state)
        {
            this.posX = posX;
            this.posY = posY;
            this.state = state;
        }


        //Change button font color depending on mineCount
        public void ColourTheField()
        {
            switch (this.mineCount)
            {
                case 1:
                    this.Foreground = Brushes.Blue;
                    break;
                case 2:
                    this.Foreground = Brushes.Green;
                    break;
                case 3:
                    this.Foreground = Brushes.Red;
                    break;
                case 4:
                    this.Foreground = Brushes.Blue;
                    break;
                case 5:
                    this.Foreground = Brushes.DarkRed;
                    break;
                case 6:
                    this.Foreground = Brushes.Cyan;
                    break;
                case 7:
                    this.Foreground = Brushes.Black;
                    break;
                case 8:
                    this.Foreground = Brushes.Gray;
                    break;
            }
        }
    }
}
