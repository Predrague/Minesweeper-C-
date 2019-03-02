using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using static Minesweeper.MainWindow;

namespace Minesweeper
{
    [Serializable]
    class Score
    {
        private string name;
        private int points;
        public static List<Score> scoresEasy = new List<Score>();
        public static List<Score> scoresMedium = new List<Score>();
        public static List<Score> scoresHard = new List<Score>();
        static BinaryFormatter bin = new BinaryFormatter();

        public int Points { get => points; set => points = value; }
        public string Name { get => name; set => name = value; }

        public Score(string name, int points)
        {
            this.Name = name;
            this.Points = points;
        }

        public static List<Score> SortScores(List<Score> r)
        {
            r = r.OrderBy(x => x.Points).ToList();
            return r;
        }

      
        public static void Save(List<Score> r, Difficulty difficulty)
        {
            switch (difficulty)
            {
                case Difficulty.EASY:
                    scoresEasy = r;
                    break;

                case Difficulty.MEDIUM:
                    scoresMedium = r;
                    break;

                case Difficulty.HARD:
                    scoresHard = r;
                    break;
            }
        }

        override public string ToString()
        {
            return this.Name + " " + this.Points + "\n";
        }

        public static void SaveScores()
        {
            FileStream fs = new FileStream("rekordLako.bin", FileMode.Create);
            bin.Serialize(fs, scoresEasy);
            fs.Close();
            fs = new FileStream("rekordSrednje.bin", FileMode.Create);
            bin.Serialize(fs, scoresMedium);
            fs.Close();
            fs = new FileStream("rekordTesko.bin", FileMode.Create);
            bin.Serialize(fs, scoresHard);
            fs.Close();
        }

        public static void LoadScores()
        {
            try
            {
                FileStream fs = new FileStream("rekordLako.bin", FileMode.Open);
                scoresEasy = (List<Score>)bin.Deserialize(fs);
                fs.Close();
                fs = new FileStream("rekordSrednje.bin", FileMode.Open);
                scoresMedium = (List<Score>)bin.Deserialize(fs);
                fs.Close();
                fs = new FileStream("rekordTesko.bin", FileMode.Open);
                scoresHard = (List<Score>)bin.Deserialize(fs);
                fs.Close();
            }
            catch (FileNotFoundException e) { }
        }
    }
}
