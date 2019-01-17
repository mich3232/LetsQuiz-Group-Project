namespace _LetsQuiz
{
    [System.Serializable]
    public class HighScoresObject
    {
        public string userID;
        public string userName;
        public string gamesWon;
        public string questionsRight;
        public string totalScore;

        public int getTotalScoreInt()
        {
            int integer = int.Parse(totalScore);
            return integer;
        }

        public int getTotalCorrect()
        {
            int integer = int.Parse(questionsRight);
            return integer;
        }
    }
}