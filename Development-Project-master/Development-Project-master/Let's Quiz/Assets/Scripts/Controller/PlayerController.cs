using UnityEngine;

namespace _LetsQuiz
{
    public class PlayerController : Singleton<PlayerController>
    {
        #region variables

        [Header("Player Content")]
        [SerializeField] private string _questionData = "";
        [SerializeField] private string _highScoreData = "";

        private QuestAndSub[] _questandSub;

        #endregion variables

        #region properties

        public Player Player { get; set; }
        public int PlayerType { get; set; }
        public string HighScoreJSON { get; set; }
		public int NumberOfCorrectQuestions { get; set;}
        public int UserScore { get; set; }
        public string ScoreStatus { get; set; }
        public SavedGameContainer SavedGames { get; set; }

        #endregion properties

        #region methods

        #region unity

        protected override void OnEnable()
        {
            base.OnEnable();
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            Debug.Log("[PlayerController] Start() : Current saved games: " + SavedGames);
        }

        #endregion unity

        #region PlayerController specific

        #region id

        // set the player id value
        public void SetId(int id)
        {
            if (id != Player.ID)
            {
                Player.ID = id;
                SaveId();
            }
        }

        // get the player id value
        public int GetId()
        {
            return Player.ID;
        }

        // save the player id value in playerprefs
        private void SaveId()
        {
            PlayerPrefs.SetInt(DataHelper.PlayerDataKey.ID, Player.ID);
            PlayerPrefs.Save();
        }

        #endregion id

        #region status

        // set the player status value
        public void SetPlayerType(int type)
        {
            if (type != PlayerType)
            {
                PlayerType = type;
                SavePlayerType();
            }
        }

        // get the player status value
        public int GetPlayerType()
        {
            return PlayerType;
        }

        // save the player status value in playerprefs
        private void SavePlayerType()
        {
            PlayerPrefs.SetInt(DataHelper.PlayerDataKey.TYPE, PlayerType);
            PlayerPrefs.Save();
        }

        #endregion status

        #region username

        // set the player username value
        public void SetUsername(string username)
        {
            if (username != Player.username)
            {
                Player.username = username;
                SaveUsername();
            }
        }

        // get the player username value
        public string GetUsername()
        {
            return Player.username;
        }

        // save the player username value in playerprefs
        private void SaveUsername()
        {
            PlayerPrefs.SetString(DataHelper.PlayerDataKey.USERNAME, Player.username);
            PlayerPrefs.Save();
        }

        #endregion username

        #region email

        // set the player email value
        public void SetEmail(string email)
        {
            if (email != Player.email)
            {
                Player.email = email;
                SaveEmail();
            }
        }

        // get the player email value
        public string GetEmail()
        {
            return Player.email;
        }

        // save the player email value in playerprefs
        private void SaveEmail()
        {
            PlayerPrefs.SetString(DataHelper.PlayerDataKey.EMAIL, Player.email);
            PlayerPrefs.Save();
        }

        #endregion email

        #region password

        // set the player password value
        public void SetPassword(string password)
        {
            if (password != Player.password)
            {
                Player.password = password;
                SavePassword();
            }
        }

        // get the player password value
        public string GetPassword()
        {
            return Player.password;
        }

        // save the player password value in playerprefs
        private void SavePassword()
        {
            PlayerPrefs.SetString(DataHelper.PlayerDataKey.PASSWORD, Player.password);
            PlayerPrefs.Save();
        }

        #endregion password

        #region dob

        // set the player dob value
        public void SetDOB(string date)
        {
            if (date != Player.DOB)
            {
                Player.DOB = date;
                SaveDOB();
            }
        }

        // get the player dob value
        public string GetDOB()
        {
            return Player.DOB;
        }

        // save the player dob value in playerprefs
        private void SaveDOB()
        {
            PlayerPrefs.SetString(DataHelper.PlayerDataKey.DOB, Player.DOB);
            PlayerPrefs.Save();
        }

        #endregion dob

        #region questions submitted

        // set the player questions submitted value
        public void SetQuestionsSubmitted(string questions)
        {
            if (questions != Player.questionsSubmitted)
            {
                Player.questionsSubmitted = questions;
                SaveQuestionsSubmitted();
            }
        }

        // get the player questions submitted value
        public string GetQuestionsSubmitted()
        {
            return Player.questionsSubmitted;
        }

        // save the player questions submitted value in playerprefs
        private void SaveQuestionsSubmitted()
        {
            PlayerPrefs.SetString(DataHelper.PlayerDataKey.QUESTIONS_SUBMITTED, Player.questionsSubmitted);
            PlayerPrefs.Save();
        }

        #endregion questions submitted

        #region number questions submitted

        // set the player number questions submitted value
        public void SetNumberQuestionsSubmitted(int questionsSubmitted)
        {
            if (questionsSubmitted != Player.numQuestionsSubmitted)
            {
                Player.numQuestionsSubmitted = questionsSubmitted;
                SaveNumberQuestionsSubmitted();
            }
        }

        // get the player number questions submitted value
        public int GetNumberQuestionsSubmitted()
        {
            return Player.numQuestionsSubmitted;
        }

        // save the player number questions submitted value in playerprefs
        private void SaveNumberQuestionsSubmitted()
        {
            PlayerPrefs.SetInt(DataHelper.PlayerDataKey.QUESTIONS_SUBMITTED_NUMBER, Player.numQuestionsSubmitted);
            PlayerPrefs.Save();
        }

        #endregion number questions submitted

        #region number games played

        // set the player games played value
        public void SetGamesPlayed(int gamesPlayed)
        {
            if (gamesPlayed != Player.numGamesPlayed)
            {
                SaveGamesPlayed(gamesPlayed);
            }
        }

        public void AddToGamesPlayed()
        {
            Player.numGamesPlayed = Player.numGamesPlayed++;
            Debug.Log("[PlayerController] AddToGamesPlayed() Games played: " + Player.numGamesPlayed);
            SaveGamesPlayed(Player.numGamesPlayed);
        }

        // get the player games played value
        public int GetGamesPlayed()
        {
            return Player.numGamesPlayed;
        }

        // save the player games played value in playerprefs
        private void SaveGamesPlayed(int num)
        {
            Player.numGamesPlayed = num;
            PlayerPrefs.SetInt(DataHelper.PlayerDataKey.GAMES_PLAYED_NUMBER, Player.numGamesPlayed);
            PlayerPrefs.Save();
        }

        #endregion number games played

        #region highest score

        private void SetTotalPointsScored(int points)
        {
            Player.totalPointsScore = points;
        }

        // set the player highest score value
        public void SetHighestScore(int score)
        {
            Player.HighestScore = score;
        }

        // get the player number questions submitted value
        public int GetHighestScore()
        {
            return Player.HighestScore;
        }

        // save the player number questions submitted value in playerprefs
        private void SaveHighestScore()
        {
            PlayerPrefs.SetInt(DataHelper.PlayerDataKey.HIGHEST_SCORE, Player.HighestScore);
            PlayerPrefs.Save();
        }

        #endregion highest score

        #region number correct answers

        // set the player correct answers value
        //public void SetNumberCorrectAnswers(int answers)
        //{
        //    if (answers > Player.numCorrectAnswers)
        //    {
        //        Player.numCorrectAnswers = answers;
        //        SaveNumberCorrectAnswers(answers);
        //    }
        //}

        private void SetTotalCorrectAnswers(int total)
        {
            Player.TotalCorrectAnswers = total;
        }

        public void AddToNumberCorrectAnswers()
        {
            Player.TotalCorrectAnswers = Player.TotalCorrectAnswers++;
            SaveNumberCorrectAnswers(Player.TotalCorrectAnswers);
        }

        // get the player correct answers value
        public int GetNumberCorrectAnswers()
        {
            return Player.TotalCorrectAnswers;
        }

        // save the player correct answers value in playerprefs
        private void SaveNumberCorrectAnswers(int number)
        {
            Player.TotalCorrectAnswers = number;
            PlayerPrefs.SetInt(DataHelper.PlayerDataKey.CORRECT_ANSWERS_NUMBER, Player.TotalCorrectAnswers);
            PlayerPrefs.Save();
        }

        #endregion number correct answers

        #region total questions answered

        // set the player number questions answered value
        public void SetTotalQuestionsAnswered(int questionsAnswered)
        {
            if (questionsAnswered > Player.totalQuestionsAnswered)
            {
                Player.totalQuestionsAnswered = questionsAnswered;
                SaveTotalQuestionsAnswered(questionsAnswered);
            }
        }

        public void AddToTotalQuestionsAnswered()
        {
            Player.totalQuestionsAnswered = Player.totalQuestionsAnswered++;
            SaveTotalQuestionsAnswered(Player.totalQuestionsAnswered);
        }

        // get the player number questions answered value
        public int GetTotalQuestionsAnswered()
        {
            return Player.totalQuestionsAnswered;
        }

        // save the player number questions answered value in playerprefs
        private void SaveTotalQuestionsAnswered(int total)
        {
            Player.totalQuestionsAnswered = total;
            PlayerPrefs.SetInt(DataHelper.PlayerDataKey.TOTAL_ANSWERS_NUMBER, Player.totalQuestionsAnswered);
            PlayerPrefs.Save();
        }

        #endregion total questions answered

        #region question data

        // set the player question data
        public void SetQuestionData(string questionData)
        {
            if (questionData != _questionData)
            {
                _questionData = questionData;
                SaveQuestionData();
            }
        }

        // get the player question data
        public string GetQuestionData()
        {
            return _questionData;
        }

        // save the player question data in playerprefs
        public void SaveQuestionData()
        {
            PlayerPrefs.SetString(DataHelper.PlayerDataKey.QUESTION_DATA, _questionData);
            PlayerPrefs.Save();
        }

		//get saved question data
		public string GetSavedQuestionData()
		{
			return PlayerPrefs.GetString (DataHelper.PlayerDataKey.QUESTION_DATA, _questionData); //retrieve saved questionData
		}

        // set the worldwide highscore data
        public void SetHighscoreData(string highScoreData)
        {
            if (highScoreData != HighScoreJSON)
            {
                HighScoreJSON = highScoreData;
            }
        }

        // get worldwide highscore data
        public string GetHighScoreData()
        {
            return _highScoreData;
        }

        //set questandSub
        public void SetQuestandSubData(QuestAndSub[] questandSub)
        {
            _questandSub = questandSub;
        }

        //get questandSub
        public QuestAndSub[] GetQuestandSubData()
        {
            return _questandSub;
        }

        //get all saved games
        public SavedGameContainer GetSavedGames()
        {
            return SavedGames;
        }

        //add to saved games
        public void AddSavedGame(SavedGame game)
        {
            SavedGames.AllSavedRounds.Add(game);

            //resave games to persistent data, JSON  possibly just call the data Controller save method.
            if (DataController.Initialised)
                DataController.Instance.SaveSavedJSON(DataHelper.File.SAVE_LOCATION, SavedGames);
        }

        //set all saved games
        public void SetSavedGames(SavedGameContainer games)
        {
            SavedGames = games;
        }

        #endregion question data

        #region save

        public void Save(int id, string username, string email, string password, string dob, string questionsSubmitted,
                         int numQuestionsSubmitted, int numGamesPlayed, int highestScore, int numCorrectAnswers, int totalQuestionsAnswered)
        {
            SetId(id);
            SetUsername(username);
            SetEmail(email);
            SetPassword(password);
            SetDOB(dob);
            SetQuestionsSubmitted(questionsSubmitted);
            SetNumberQuestionsSubmitted(numQuestionsSubmitted);
            SetGamesPlayed(numGamesPlayed);
            // SetHighestScore(highestScore);
            // SetNumberCorrectAnswers(numCorrectAnswers);
            SetTotalQuestionsAnswered(totalQuestionsAnswered);
        }

        #endregion save

        #region load

        public void Load()
        {
            Player = new Player();

            // load player type
            if (PlayerPrefs.HasKey(DataHelper.PlayerDataKey.TYPE))
                PlayerType = PlayerPrefs.GetInt(DataHelper.PlayerDataKey.TYPE);

            // load player username
            if (PlayerPrefs.HasKey(DataHelper.PlayerDataKey.USERNAME))
                Player.username = PlayerPrefs.GetString(DataHelper.PlayerDataKey.USERNAME);

            // load player email
            if (PlayerPrefs.HasKey(DataHelper.PlayerDataKey.EMAIL))
                Player.email = PlayerPrefs.GetString(DataHelper.PlayerDataKey.EMAIL);

            // load player username
            if (PlayerPrefs.HasKey(DataHelper.PlayerDataKey.PASSWORD))
                Player.password = PlayerPrefs.GetString(DataHelper.PlayerDataKey.PASSWORD);

            // load player question data
            if (PlayerPrefs.HasKey(DataHelper.PlayerDataKey.QUESTION_DATA))
                _questionData = PlayerPrefs.GetString(DataHelper.PlayerDataKey.QUESTION_DATA);
        }

        public void Load(Player player)
        {
            SetId(player.ID);
            SetUsername(player.username);
            SetEmail(player.email);
            SetPassword(player.password);
            SetDOB(player.DOB);
            SetQuestionsSubmitted(player.questionsSubmitted);
            SetNumberQuestionsSubmitted(player.numQuestionsSubmitted);
            SetGamesPlayed(player.numGamesPlayed);
            SetHighestScore(player.HighestScore);
            SetTotalQuestionsAnswered(player.totalQuestionsAnswered);
            SetTotalPointsScored(player.totalPointsScore);
            SetTotalCorrectAnswers(player.TotalCorrectAnswers);
        }

        #endregion load

        #endregion PlayerController specific

        #endregion methods
    }
}