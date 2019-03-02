using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Minesweeper
{
    public partial class StartingWindow : Window
    {
        MainWindow.Difficulty difficulty;

        public StartingWindow()
        {
            InitializeComponent();
            Score.LoadScores();
        }

        private void StartGame(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrWhiteSpace(tbName.Text))
            {
                MessageBox.Show("Enter name");
                return;
            }
            string name = tbName.Text;

            if (rbEasy.IsChecked == true)
            {
                difficulty = MainWindow.Difficulty.EASY;
            }
            else if(rbMedium.IsChecked == true)
            {
                difficulty = MainWindow.Difficulty.MEDIUM;
            }else if(rbHard.IsChecked == true)
            {
                difficulty = MainWindow.Difficulty.HARD;
            }

            new MainWindow(name, difficulty).Show();
            this.Close();
        }
    }
}
