namespace _LetsQuiz
{
    [System.Serializable]
    public class OngoingGamesData
    {
        public int gameNumber;
        public string player;
        public string opponent;
        public string askedQuestions;
        public string Round1Catagory;
        public string questionsLeftInCat;
        public string Round2Catagory;
        public string questionsLeftInCat2;
        public string round3Cat;
        public string questionsLeftInCat3;
        public int playerScore;
        public int opponentScore;
        public bool gameRequiresOppoent;
        public int turnNumber;
        public int overAllScore;
    }
}