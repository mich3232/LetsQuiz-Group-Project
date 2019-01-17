using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace _LetsQuiz
{
    public class GameController : MonoBehaviour
    {
        #region variables

        [Header("Timer")]
        public Slider timerBar;

        public Image timerImage;

        [Header("Color")]
        public Color timerMin;

        public Color timerMid;
        public Color timerMax;

        [Header("Score")]
        public Text scoreDisplay;

        [Header("Question")]
        public Text questionText;

        public GameObject questionDisplay;
        public QuestionData questionData;
        public QuestionData currentQuestion;
        public QuestionData currentQuestionData;

        private QuestionData[] _questionPool;
        private int _numberOfQuestionsAsked;

        [Header("Answers")]
        public SimpleObjectPool answerButtonObjectPool;

        public Transform answerButtonParent;

        [Header("Controllers")]
        private RoundData _currentRoundData;

        private SubmitScore _submitScore;

        [Header("Other")]
        private Upvote _upVote;

        private Downvote _downVote;
        private bool ConnectionAvailable;

        [SerializeField] private string _token;
        private float _timeRemaining = 20;

        //TODO the in game slider does not work until the timer is 20 or less
        private bool _isRoundActive;

        //private int _questionIndex;
        private FeedbackMusic _music;

        private bool _isCorrect = false;
        private AnswerButton _correctAnswerButton;
        private AnswerData _correctAnswerData;

        private List<GameObject> _answerButtonGameObjects = new List<GameObject>();
        private AnswerButton _userSelection;


		public List<QuestionData> _roundUpvotes = new List<QuestionData>();
		public List<QuestionData> _roundDownvotes = new List<QuestionData> ();

        #endregion variables

        #region properties

        public bool clicked { get; set; }

        #endregion properties

        #region methods

        #region unity

        private void Awake()
        {
            _music = FindObjectOfType<FeedbackMusic>();
            _music.PlayGameMusic();
        }

        private void Start()
        {
            _upVote = FindObjectOfType<Upvote>();
            _downVote = FindObjectOfType<Downvote>();
			_roundUpvotes.Clear ();
			_roundDownvotes.Clear ();

            PlayerController.Instance.AddToGamesPlayed();
            PlayerController.Instance.UserScore = 0;
			PlayerController.Instance.NumberOfCorrectQuestions = 0;
            QuestionController.Instance.Load();

            if (FirebaseController.Initialised)
                _token = FirebaseController.Instance.Token;

            _questionPool = GetQuestionPool();

            checkForConnection();
            if (ConnectionAvailable)
            {
                if (PlayerPrefs.HasKey((DataHelper.PlayerDataKey.GAMEKEY) + PlayerController.Instance.GetUsername()))
                {
                    string numbers = PlayerPrefs.GetString((DataHelper.PlayerDataKey.GAMEKEY) + PlayerController.Instance.GetUsername());
                    numbers = numbers + "," + DataController.Instance.GameNumber;
                    PlayerPrefs.SetString((DataHelper.PlayerDataKey.GAMEKEY) + PlayerController.Instance.GetUsername(), numbers);
                    Debug.Log("games in player prefs = " + PlayerPrefs.GetString((DataHelper.PlayerDataKey.GAMEKEY) + PlayerController.Instance.GetUsername()) + PlayerController.Instance.GetUsername());
                }
                else
                {
                    PlayerPrefs.SetString(((DataHelper.PlayerDataKey.GAMEKEY) + PlayerController.Instance.GetUsername()), DataController.Instance.GameNumber.ToString());
                    Debug.Log("games in player prefs = " + PlayerPrefs.GetString((DataHelper.PlayerDataKey.GAMEKEY) + PlayerController.Instance.GetUsername()));
                }
            }

            ShowQuestion();
        }

        private void Update()
        {
            _timeRemaining -= Time.deltaTime;

            UpdateTimeRemainingDisplay();

            if (_timeRemaining <= 0 && _timeRemaining > -100)
            {
                _timeRemaining = -101;
                Debug.Log("endround");
                EndRound();
            }
        }

        #endregion unity

        #region questionpool

        private QuestionData[] GetQuestionPool()
        {
            if (DataController.Instance.TurnNumber == 0)
            {
                //this should not actually happen as CheckForOpenGames() should have set the turn number to at least 1
                // treat this as a brand new game
                _questionPool = QuestionController.Instance.GetAllQuestionsAllCategories();
                return _questionPool;
            }

            if (DataController.Instance.TurnNumber == 1)
            {
                //there is no open games, the user is the 'player' they will be starting a new game which will get an opponent later
                Debug.Log("there are no open games, user set to player, starting brand new game");
                _questionPool = GameLobbyController.Instance.QuestionsPoolFromCatagory;
                return _questionPool;
            }
            if (DataController.Instance.TurnNumber == 2)
            {
                //there is an open game, the user will be the 'oppponent' they will be playing round 2 with a predetermined questionpool
                string roundDataJSON = DataController.Instance.OngoingGameData.askedQuestions;
                RoundData rd = JsonUtility.FromJson<RoundData>(roundDataJSON);
                _questionPool = rd.questions;
                return _questionPool;
            }

            if (DataController.Instance.TurnNumber == 3)
            {
                //Continuing a game. The opponent now gets to pick the catagory
                _questionPool = GameLobbyController.Instance.QuestionsPoolFromCatagory;

                return _questionPool;
            }

            if (DataController.Instance.TurnNumber == 4)
            {
                //there is an open game, the user will be the 'oppponent' they will be playing round 2 with a predetermined questionpool
                string roundDataJSON = DataController.Instance.OngoingGameData.askedQuestions;
                RoundData rd = JsonUtility.FromJson<RoundData>(roundDataJSON);
                _questionPool = rd.questions;
                return _questionPool;
            }
            if (DataController.Instance.TurnNumber == 5)
            {
                //there is no open games, the user is the 'player' they will be starting a new game which will get an opponent later
                _questionPool = GameLobbyController.Instance.QuestionsPoolFromCatagory;
                Destroy(GameLobbyController.Instance);
                return _questionPool;
            }

            if (DataController.Instance.TurnNumber == 6)
            {
                string roundDataJSON = DataController.Instance.OngoingGameData.askedQuestions;
                RoundData rd = JsonUtility.FromJson<RoundData>(roundDataJSON);
                _questionPool = rd.questions;
                return _questionPool;
            }
            else
            {
                Debug.Log("something went wrong");
                _questionPool = QuestionController.Instance.GetAllQuestionsAllCategories();
                return _questionPool;
            }
        }

        #endregion questionpool

        #region display question & answers

        public void ShowQuestion()
        {
            scoreDisplay.text = PlayerController.Instance.UserScore.ToString();
            clicked = false;
            RemoveAnswerButtons();
            PlayerController.Instance.AddToTotalQuestionsAnswered();

            currentQuestionData = null;

            //if all questions are asked
            if (_questionPool.Length <= 0)
            {
                if (DataController.Instance.TurnNumber == 0 | DataController.Instance.TurnNumber == 1)
                {
                    //the user is the player so if they have finished the question pool they have answered all the questions in the catagory.
                    Debug.Log("GameController : Show Questions(): Out of Questions");
                    Debug.Log("endround");
                    EndRound();
                }
                if (DataController.Instance.TurnNumber == 2)
                {
                    //the user is the oppenent so if they have finished the question pool they have answered all the questions asked of the player, go on to the remaining questions in the catagory

                    if (DataController.Instance.OngoingGameData.questionsLeftInCat.Length < 5)
                    {
                        Debug.Log("endround");
                        EndRound();
                    }

                    string roundDataJSON = DataController.Instance.OngoingGameData.questionsLeftInCat;
                    Debug.Log("out of asked questons, asking remaining questions: " + DataController.Instance.OngoingGameData.questionsLeftInCat);

                    DataController.Instance.OngoingGameData.questionsLeftInCat = "";
                    RoundData rd = JsonUtility.FromJson<RoundData>(roundDataJSON);
                    _questionPool = rd.questions;
                    ShowQuestion();
                }
            }
            else
            { //ask the question
                if (DataController.Instance.TurnNumber == 0 | DataController.Instance.TurnNumber == 1 | DataController.Instance.TurnNumber == 3 | DataController.Instance.TurnNumber == 5)
                {
                    //if user is player then quesitons should be asked in random order
                    int randomNumber = Random.Range(0, _questionPool.Length - 1); //gets random number between 0 and total number of questions
                    currentQuestionData = _questionPool[randomNumber];// Get the QuestionData for the current question
                    _questionPool = QuestionController.Instance.RemoveQuestion(_questionPool, randomNumber); //remove question from list
                    questionText.text = currentQuestionData.questionText;  // Update questionText with the correct text
                    QuestionController.Instance.AddAskedQuestionToAskedQuestions(currentQuestionData);//keep track of the questions we asked so we can repeat it for the oppoent player
                    _numberOfQuestionsAsked++;
                    ShowAnswers(currentQuestionData);
                }
                if (DataController.Instance.TurnNumber == 2 | DataController.Instance.TurnNumber == 4 | DataController.Instance.TurnNumber == 6)
                {
                    //if user is oppoent questions should be asked in the same order they were asked of player
                    currentQuestionData = _questionPool[0];// Get the QuestionData for the current question
                    _questionPool = QuestionController.Instance.RemoveQuestion(_questionPool, 0); //remove question from list
                    questionText.text = currentQuestionData.questionText;  // Update questionText with the correct text
                    _numberOfQuestionsAsked++;
                    ShowAnswers(currentQuestionData);
                }
            }
        }

        private void ShowAnswers(QuestionData currentQuestionData)
        {
            List<int> answerText = new List<int>();

            // For every AnswerData in the current QuestionData...
            for (int i = 0; i < currentQuestionData.answers.Length; i++)
            {
                int n = Random.Range(0, currentQuestionData.answers.Length);

                while (answerText.Contains(n))
                    n = Random.Range(0, currentQuestionData.answers.Length); //randomise where the answers are displayed

                answerText.Add(n);
                GameObject answerButtonGameObject = answerButtonObjectPool.GetObject(); // Spawn an AnswerButton from the object pool
                _answerButtonGameObjects.Add(answerButtonGameObject);

                answerButtonGameObject.transform.SetParent(answerButtonParent);
                answerButtonGameObject.transform.localScale = Vector3.one; //I was having an issue were the scale blew out, this fixed it...
                AnswerButton answerButton = answerButtonGameObject.GetComponent<AnswerButton>();
                answerButton.SetUp(currentQuestionData.answers[n]); // Pass the AnswerData to the AnswerButton to check if it correct

                if (answerButton.isCorrect(currentQuestionData.answers[n]))
                {
                    PlayerController.Instance.AddToNumberCorrectAnswers();
                    _correctAnswerButton = answerButton;
                    _isCorrect = true;
                    _correctAnswerData = currentQuestionData.answers[n];
                }
            }
        }

        #endregion display question & answers

        #region button set up

        public AnswerButton getCorrectAnswerButton() //getter used by the answerButton, I got an error when i tryed to declar with the variable at the top
        {
            return _correctAnswerButton;
        }

        private void RemoveAnswerButtons()  // Return all spawned AnswerButtons to the object pool
        {
            while (_answerButtonGameObjects.Count > 0)
            {
                answerButtonObjectPool.ReturnObject(_answerButtonGameObjects[0]);
                _answerButtonGameObjects.RemoveAt(0);
            }
        }

        #endregion button set up

        #region like & dislike buttons

        public void ReportQuestion()
        {

            //Debug.Log("****current question is: " + currentQuestionData.questionText);
            //_downVote.Dvote(currentQuestionData);
			if (!_roundDownvotes.Contains(currentQuestionData)) {
			_roundDownvotes.Add(currentQuestionData);
			}
        }

        public void LikeQuestion()
        {

            //Debug.Log("****current question is: " + currentQuestionData.questionText);
            //_upVote.Uvote(currentQuestionData);
			if (!_roundUpvotes.Contains (currentQuestionData)) {
				_roundUpvotes.Add (currentQuestionData);
			}
        }

        #endregion like & dislike buttons

        #region scoring specific

        public void Score(bool answer)
        {
            Debug.Log("GameController : Score(): score called bool = " + answer);

			if (answer) 
			{
				PlayerController.Instance.UserScore = PlayerController.Instance.UserScore + 10;
				PlayerController.Instance.NumberOfCorrectQuestions++; 
			}

            if (!answer)
                PlayerController.Instance.UserScore = PlayerController.Instance.UserScore - 5;

            ShowQuestion();
        }

        #endregion scoring specific

        #region timer specific

        private void UpdateTimeRemainingDisplay()
        {
            var timeRemaining = Mathf.Round(_timeRemaining);
            timerBar.value = timeRemaining;

            if (timerBar.value > 15)
                timerImage.color = timerMax;
            else if (timerBar.value < 16 && timerBar.value > 6)
                timerImage.color = timerMid;
            else if (timerBar.value <= 5)
                timerImage.color = timerMin;
        }

        #endregion timer specific

        #region navigation specific

        public void EndRound()
        {
            _music.Stop();

			SubmitVotes();
            SubmitToOngoingGamesDB();
            SubmitHighscoreData();


            Debug.Log("GameController : EndRound(): End of Round");
            Debug.Log(PlayerController.Instance.ScoreStatus);

            SceneManager.LoadScene(BuildIndex.Result, LoadSceneMode.Single);
            //SendPushNotification();
        }

        public void SubmitToOngoingGamesDB()
        {
            SubmitGame submitGame;
            submitGame = FindObjectOfType<SubmitGame>();
            submitGame.SubmitGameToDB(QuestionController.Instance.GetRemainingQuestions(_questionPool));
        }

        public void SubmitHighscoreData()
        {
            _submitScore = FindObjectOfType<SubmitScore>();
			_submitScore.SubmitScores(PlayerController.Instance.GetUsername(), PlayerController.Instance.NumberOfCorrectQuestions, PlayerController.Instance.UserScore);
            Debug.Log("Highscore Submitting. Player: " + PlayerController.Instance.GetUsername() + " Round Score: " + PlayerController.Instance.UserScore);
        }

		public void SubmitVotes() {
			for (int i = 0; i < _roundUpvotes.Count; i++) {
				_upVote.Uvote(_roundUpvotes[i]);
				Debug.Log("****current question is: " + _roundUpvotes[i].questionText);
			}

			for (int i = 0; i < _roundDownvotes.Count; i++) {
				_downVote.Dvote(_roundDownvotes[i]);
				Debug.Log("****current question is: " + _roundDownvotes[i].questionText);
			}


		}


        #endregion navigation specific

        #region internet specific

        public void checkForConnection()
        {
            //testing for network connectivity
            switch (Application.internetReachability)
            {
                case NetworkReachability.NotReachable:
                    ConnectionAvailable = false;
                    break;

                case NetworkReachability.ReachableViaCarrierDataNetwork:
                    ConnectionAvailable = true;
                    break;

                case NetworkReachability.ReachableViaLocalAreaNetwork:
                    ConnectionAvailable = true;
                    break;
            }
        }

        //private void SendPushNotification()
        //{
        //	if (DataController.Instance.TurnNumber == 2)
        //	{
        //		if (!string.IsNullOrEmpty(FirebaseController.Instance.Token))
        //			FirebaseController.Instance.CreateNotification(FirebaseController.Instance.Token, "You've got a new opponent!", "Take your turn now");
        //	}

        //	if (DataController.Instance.TurnNumber == 3)
        //	{
        //		if (!string.IsNullOrEmpty(FirebaseController.Instance.Token))
        //			FirebaseController.Instance.CreateNotification(FirebaseController.Instance.Token, "Are you ready?", "It's your turn!");
        //	}

        //	if (DataController.Instance.TurnNumber == 4)
        //	{
        //		if (!string.IsNullOrEmpty(FirebaseController.Instance.Token))
        //			FirebaseController.Instance.CreateNotification(FirebaseController.Instance.Token, "Ready?", "Last round");
        //	}
        //	if (DataController.Instance.TurnNumber == 5)
        //	{
        //		if (!string.IsNullOrEmpty(FirebaseController.Instance.Token))
        //			FirebaseController.Instance.CreateNotification(FirebaseController.Instance.Token, "Are you ready?", "It's your turn!");
        //	}

        //	if (DataController.Instance.TurnNumber == 6)
        //	{
        //		if (!string.IsNullOrEmpty(FirebaseController.Instance.Token))
        //			FirebaseController.Instance.CreateNotification(FirebaseController.Instance.Token, "Fingers crossed!", "You've got a new opponent!");
        //	}
        //}

        #endregion internet specific

        #endregion methods
    }
}