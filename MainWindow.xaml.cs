using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Minesweeper
{
    public partial class MainWindow : Window
    {
        public enum Difficulty { EASY, MEDIUM, HARD };
        int dimensions;
        int numberOfMines;
        int minesUsed;
        int activeFieldCount;
        int time;
        public Difficulty difficulty;
        bool gameStarted;
        DispatcherTimer timer;
        Random random = new Random();
        Grid playGrid;
        List<Field> mines = new List<Field>();
        List<Score> score = new List<Score>();
        Field[,] fields;

        String name;


        delegate void FieldDelegate(Field clicked);
        FieldDelegate fieldMethod;             

        public MainWindow(String name, Difficulty difficulty)
        {
            this.name = name;
            this.difficulty = difficulty;
            InitializeComponent();
            SetUpGame();
        }

        //Setting up basic game parameters
        public void SetUpGame()
        {
            gameStarted = false;
            timer = new DispatcherTimer();
            time = 0;
            timer.Tick += TimeTick;
            btnFlag.Checked += FlagsOn;
            btnFlag.Unchecked += FlagsOff;
            fieldMethod = new FieldDelegate(FieldClick);

            switch (difficulty)
            {
                case Difficulty.EASY:
                    {
                        dimensions = 10;
                        numberOfMines = 20;
                        score = Score.scoresEasy;
                        break;
                    }
                case Difficulty.MEDIUM:
                    {
                        dimensions = 12;
                        numberOfMines = 45;
                        score = Score.scoresMedium;
                        break;
                    }
                case Difficulty.HARD:
                    {
                        dimensions = 14;
                        numberOfMines = 60;
                        score = Score.scoresHard;
                        break;
                    }
            }
            fields = new Field[dimensions, dimensions];
            minesUsed = numberOfMines;
            activeFieldCount = (dimensions * dimensions) - numberOfMines;
            AddControls();
        }

        /// Adding minefield  
        private void AddControls()
        {
            lblTimer.Content = time.ToString();
            lblMines.Content = minesUsed;
            for (int i = 0; i < dimensions; i++)
            {
                for (int j = 0; j < dimensions; j++)
                {
                    AddField(i, j);
                }
            }
            playGrid = new Grid();

            for (int i = 0; i < dimensions; i++)
            {
                RowDefinition row = new RowDefinition();
                ColumnDefinition col = new ColumnDefinition();
                playGrid.RowDefinitions.Add(row);
                playGrid.ColumnDefinitions.Add(col);
            }
            for (int i = 0; i < dimensions; i++)
            {
                for (int j = 0; j < dimensions; j++)
                {
                    Grid.SetRow(fields[i, j], i);
                    Grid.SetColumn(fields[i, j], j);
                    playGrid.Children.Add(fields[i, j]);
                }
            }
            for (int i = 0; i < numberOfMines; i++)
            {
                AddMine();
            }

            CountFieldsValues();
            pnlPlayField.Children.Add(playGrid);
            pnlPlayField.IsEnabled = true;
            btnNewGame.IsEnabled = true;
            btnFlag.IsEnabled = true;
            this.SizeToContent = SizeToContent.WidthAndHeight;
            ChangeImage("new");
        }

        //Make and add single field
        public void AddField(int i, int j)
        {
            Field p = new Field(i, j, FieldState.EMPTY);
            Style stil = new Style();
            p.FontSize = 14;
            p.FontWeight = FontWeights.Bold;
            p.Width = 30;
            p.Height = 30;
            fields[i, j] = p;
            fields[i, j].Click += FieldClickedMethod;
        }

        //Make and add single mine
        private void AddMine()
        {
            int x = random.Next(dimensions);
            int y = random.Next(dimensions);

            if (fields[x, y].State == FieldState.MINE)
            {
                AddMine();
                return;
            }
            fields[x, y].State = FieldState.MINE;
            mines.Add(fields[x, y]);
        }

        //Reseting field for new game
        private void ResetField()
        {
            timer.Stop();
            time = 0;
            pnlPlayField.Children.Clear();
        }

        public void ResetGame(object sender, RoutedEventArgs e)
        {
            ResetField();
            SetUpGame();
        }

        public void StartGame()
        {
            timer.Interval = new TimeSpan(0, 0, 1);
            timer.Start();
        }

        //Working with options from menubar
        public void MenuClicked(object sender, RoutedEventArgs e)
        {
            MenuItem clicked = sender as MenuItem;
            if (MessageBox.Show("Do you want to start new game?", "Question", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No) return;
            else
            {
                switch (clicked.Name)
                {
                    case "menuEasy":              
                        difficulty = Difficulty.EASY;
                        break;
                    case "menuMedium":
                        difficulty = Difficulty.MEDIUM;
                        break;
                    case "menuHard":
                        difficulty = Difficulty.HARD;
                        break;
                }
                ResetField();
                SetUpGame();
            }
        }

        public void FlagsOn(Object sender, EventArgs e)
        {
            BitmapImage bmp = (BitmapImage)this.Resources["flag2"];
            imgFlag.Source = bmp;
            fieldMethod = SetFlag;
        }

        public void FlagsOff(Object sender, EventArgs e)
        {
            BitmapImage bmp = (BitmapImage)this.Resources["flag"];
            imgFlag.Source = bmp;
            fieldMethod = FieldClick;
        }

        //Setting flag to clicked field
        private void SetFlag(Field clicked)
        {
            if (clicked.Flag)
            {
                clicked.Content = "";
                minesUsed++;
                clicked.Flag = false;
                lblMines.Content = minesUsed.ToString();
                return;
            }

            if (minesUsed == 0)
            {
                MessageBox.Show("No flags remaining");
                return;
            }

            SetImage(clicked,"flag");
            clicked.Flag = true;
            minesUsed--;
            lblMines.Content = minesUsed.ToString();
        }

        //Calls right method depending on flags on/off button
        public void FieldClickedMethod(Object sender, RoutedEventArgs e)
        {
            Field clicked = sender as Field;
            fieldMethod(clicked);
        }

        //Called when flags button is off
        private void FieldClick(Field clicked)
        {
            if (!gameStarted)
            {
                StartGame();
                gameStarted = true;
            }
            if (clicked.Flag) return;
            switch (clicked.State)
            {
                case FieldState.EMPTY:
                    clicked.Content = clicked.MineCount.ToString();
                    clicked.State = FieldState.INACTIVE;
                    clicked.IsEnabled = false;
                    activeFieldCount--;

                    if (clicked.MineCount == 0)
                    {
                        clicked.Content = "";
                        clicked.State = FieldState.INACTIVE;
                        RevealAdjacent(clicked.PosX, clicked.PosY);
                    }
                    if (activeFieldCount == 0) Victory();
                    break;

                case FieldState.MINE:
                    MarkMines(0); 
                    Defeat();
                    break;
            }
        }

        //Counting MineCount for all fields
        private void CountFieldsValues()
        {
            for (int i = 0; i < dimensions; i++)
            {
                for (int j = 0; j < dimensions; j++)
                {
                    if (fields[i, j].State == FieldState.MINE)
                    {
                        fields[i, j].MineCount = -1;
                    }
                    if (fields[i, j].State == FieldState.EMPTY)
                    {
                        fields[i, j].MineCount = CountMineCount(i, j);
                        fields[i, j].ColourTheField();
                    }
                }
            }
        }

        //Counts MineCount for single field
        public int CountMineCount(int x, int y)
        {
            int counter = 0;
            for (int i = x - 1; i <= x + 1; i++)
            {
                if (i < 0 || i >= dimensions) continue;
                for (int j = y - 1; j <= y + 1; j++)
                {
                    if (j < 0 || j >= dimensions) continue;
                    if (fields[i, j].State == FieldState.MINE)
                    {
                        counter++;
                    }
                }
            }
            return counter;
        }

        //Reveals blank fields
        public void RevealAdjacent(int x, int y)
        {
            for (int i = x - 1; i <= x + 1; i++)
            {
                if (i < 0 || i >= dimensions) continue;
                for (int j = y - 1; j <= y + 1; j++)
                {
                    if (j < 0 || j >= dimensions) continue;
                    if (i == x && j == y) continue;
                    Field adjacent = fields[i, j];
                    if (adjacent.State == FieldState.INACTIVE) continue;
                    if (adjacent.Flag) continue;
                    adjacent.Content = adjacent.MineCount.ToString();
                    adjacent.IsEnabled = false;
                    adjacent.State = FieldState.INACTIVE;
                    activeFieldCount--;
                    if (adjacent.MineCount == 0)
                    {
                        adjacent.Content = "";
                        adjacent.State = FieldState.INACTIVE;
                        RevealAdjacent(i, j);
                    }
                }
            }
        }


        public void TimeTick(Object sender, EventArgs e)
        {
            time++;
            lblTimer.Content = time.ToString();
        }

        //Called when game ends
        public void MarkMines(int option)
        {
            if (option == 0)
            {
                mines.ForEach(x => SetImage(x,"mine"));
                mines.ForEach(x => x.IsEnabled = false);
            }
            else if (option == 1)
            {
                mines.ForEach(x => SetImage(x, "flag2"));
                mines.ForEach(x => x.IsEnabled = false);
            }
        }

        //Setting image on field (mine or flag)
        private void SetImage(Field clicked, String src)
        {
            Image img = new Image();
            BitmapImage bmp = (BitmapImage)this.Resources[src];
            img.Source = bmp;
            clicked.Content = img;
        }

        //Changing new button image
        public void ChangeImage(string option)
        {
            BitmapImage bmp = new BitmapImage();
            switch (option)
            {
                case "new":
                    bmp = (BitmapImage)this.Resources["smiley"];
                    break;
                case "victory":
                    bmp = (BitmapImage)this.Resources["smileyVictory"];
                    break;
                case "defeat":
                    bmp = (BitmapImage)this.Resources["smileyDefeat"];
                    break;
            }
            Image image = new Image();
            image.Stretch = Stretch.Fill;
            image.Source = bmp;
            btnNewGame.Content = image;
        }


        public void CheckScores()
        {
            if(score.Count < 10) score.Add(new Score(name, time));
            else if(score.Count == 10)
            {
                int last = score.Count - 1;

                if (time < score[last].Points)
                {
                    score.RemoveAt(9);
                    score.Add(new Score(name, time));
                }
            }
            score = Score.SortScores(score);
            Score.Save(score, difficulty);
        }

        public void ShowScores(object sender, RoutedEventArgs e)
        {
            string message ="";
            if (score.Count == 0)
            {
                MessageBox.Show("No records!");
                return;
            }
            foreach (Score r in score)
            {
                message += r.ToString();
            }
            MessageBox.Show(message);
        }

        public void Victory()
        {
            MessageBox.Show("VICTORY");
            MarkMines(1);
            lblMines.Content = "0";
            ChangeImage("victory");
            CheckScores();
            timer.Stop();
        }

        public void Defeat() 
        {
            btnFlag.IsEnabled = false;
            timer.Stop();
            ChangeImage("defeat");
            pnlPlayField.IsEnabled = false;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Score.SaveScores();
        }
    }
}
