namespace _LetsQuiz
{
    [System.Serializable]
    public class HighScoreData
    {
        public HighScore[] HighScore;
    }

    [System.Serializable]
    public class HighScore
    {
        public int userID;
        public string userName;
        public int gamesWon;
        public int questionRight;
        public int totalScore;
    }
}