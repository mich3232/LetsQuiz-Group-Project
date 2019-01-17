using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace _LetsQuiz
{
    public class DataController : Singleton<DataController>
    {
        #region variables

        [Header("Components")]
        private GetAllQuestions _questionDownload;

        private GetHighScores _highscoreDownload;
        private GetPlayerQuestionSubmissions _questAndSub;

        [Header("Connection")]
        private const float _connectionTimeLimit = 1000000.0f;

        private float _connectionTimer = 0.0f;

        [Header("Validation Tests")]
        private string _username = "u";

        private string _password = "p";
        private int _status = -2;

        #endregion variables

        #region properties

        public Player Player { get; set; }
        public OngoingGamesData OngoingGameData { get; set; }
        public QuestionData[] TempQuestionPool { get; set; }
        public string Catagory { get; set; }
        public int TurnNumber { get; set; }
        public int GameNumber { get; set; }
        public bool ServerConnected { get; set; }
        public string AllQuestionJSON { get; set; }
        public string AllHighScoreJSON { get; set; }
        public bool ConnectionAvailable;

        #endregion properties

        #region methods

        #region unity

        protected override void OnEnable()
        {
            base.OnEnable();
            DontDestroyOnLoad(gameObject);
            TurnNumber = 0;
            Player = new Player();
        }

        private void Start()
        {
            Debug.LogFormat("[{0}] Start()", GetType().Name);

            if (PlayerController.Initialised)
                PlayerController.Instance.Load();

            if (HighscoreController.Initialised)
                HighscoreController.Instance.Load();

            _questionDownload = FindObjectOfType<GetAllQuestions>();
            _highscoreDownload = FindObjectOfType<GetHighScores>();
            _questAndSub = GetComponent<GetPlayerQuestionSubmissions>();

            //check for internet connection
            checkForConnection();

            if (ConnectionAvailable)
            {
                StartCoroutine(_questionDownload.PullAllQuestionsFromServer());

                StartCoroutine(_highscoreDownload.PullAllHighScoresFromServer());

                // add in for the Submitted Questions Highscore table.
                StartCoroutine(_questAndSub.PullQuestionSubmitters());

                if (PlayerController.Initialised)
                    PlayerController.Instance.SetSavedGames(LoadSavedJSON<SavedGameContainer>(DataHelper.File.SAVE_LOCATION));

                // retrive player username and password from PlayerPrefs if they have an id
                if (PlayerPrefs.HasKey(DataHelper.PlayerDataKey.ID))
                {
                    // TODO : is any of these ever used?
                    // NOTE : used to determine if player has already logged in or not for automatic login
                    _status = PlayerController.Instance.GetPlayerType();
                    _username = PlayerController.Instance.GetUsername().ToString();
                    _password = PlayerController.Instance.GetPassword().ToString();
                }
            }
            else
            {
                Debug.LogErrorFormat("[{0}] Start() : Error {1} ", GetType().Name, "No server connection found");
                _questionDownload.PullSavedQuestionsFromLocal();
            }

            if (QuestionController.Initialised)
                QuestionController.Instance.Load();

            if (HighscoreController.Initialised)
                HighscoreController.Instance.Load();

            List<string> categories = new List<string>();

            if (!File.Exists(DataHelper.File.SAVE_LOCATION))
                File.WriteAllText(DataHelper.File.SAVE_LOCATION, " { }");
        }

        #endregion unity

        #region server specific

        public void Init()
        {
            if (ServerConnected)
            {
                // check if username and password are stored in PlayerPrefs
                // if it is perform login, else load login scene
                if (_username != "u" && _password != "p" && (_status == PlayerStatus.LoggedIn || _status == PlayerStatus.Guest))
                    StartCoroutine(Login(_username, _password));
                else
                    SceneManager.LoadScene(BuildIndex.Login);
            }
            // prompt user if they wish to retry
            else
                DisplayRetryModal("Error connecting to the server.");
        }

        private IEnumerator Login(string username, string password)
        {
            WWWForm form = new WWWForm();

            form.AddField("usernamePost", username);
            form.AddField("passwordPost", password);

            WWW loginRequest = new WWW(ServerHelper.Host + ServerHelper.Login, form);

            _connectionTimer += Time.deltaTime;

            while (!loginRequest.isDone)
            {
                if (_connectionTimer > _connectionTimeLimit)
                {
                    FeedbackAlert.Show("Server time out.");
                    Debug.LogError("[DataController] Login() :  Server time out : " + loginRequest.error);
                    yield return null;
                }
                else if (loginRequest.error != null)
                {
                    FeedbackAlert.Show("Connection error. Please try again.");
                    Debug.LogError("[DataController] Login() : Server error " + loginRequest.error);
                    yield return null;
                }
                // extra check just to ensure a stream error doesn't come up
                else if (_connectionTimer > _connectionTimeLimit && loginRequest.error != null)
                {
                    FeedbackAlert.Show("Server error.");
                    Debug.LogError("[DataController] Login() : Server error : " + loginRequest.error);
                    yield return null;
                }
            }

            if (loginRequest.isDone && loginRequest.error != null)
            {
                FeedbackAlert.Show("Connection error. Please try again.");
                Debug.LogError("[DataController] Login() : Server error " + loginRequest.error);
                yield return null;
            }

            if (loginRequest.isDone)
            {
                // check that the login request returned something
                if (!string.IsNullOrEmpty(loginRequest.text))
                {
                    Player = JsonUtility.FromJson<Player>(loginRequest.text);

                    // if the retrieved login text doesn't have "ID" load login scene
                    if (!loginRequest.text.Contains("ID"))
                    {
                        SceneManager.LoadScene(BuildIndex.Login);
                        yield return null;
                    }
                    // otherwise save the player information to PlayerPrefs and load menu scene
                    else
                    {
                        Player = JsonUtility.FromJson<Player>(loginRequest.text);

                        if (Player != null)
                        {
                            PlayerController.Instance.Save(Player.ID, Player.username, Player.email, Player.password, Player.DOB, Player.questionsSubmitted,
                                                           Player.numQuestionsSubmitted, Player.numGamesPlayed, Player.totalPointsScore,
                                                           Player.TotalCorrectAnswers, Player.totalQuestionsAnswered);

                            FeedbackAlert.Show("Welcome back " + _username);
                        }

                        SceneManager.LoadScene(BuildIndex.Menu);
                        yield return loginRequest;
                    }
                }
            }
        }

        private void RetryPullData()
        {
            Debug.Log("[DataController] RetryPullData()");
            //FeedbackAlert.Show("Retrying connection...", 1.0f);  //alert breaking game
            checkForConnection();
            if (ConnectionAvailable)
            {
                StartCoroutine(_questionDownload.PullAllQuestionsFromServer());
            }
            else
            {
                Debug.Log("no server connected");
                DisplayRetryModal("Still no server connection");
            }
        }

        #endregion server specific

        #region feedback specific

        // positive action - retry question download
        // negative action - quit application
        private void DisplayRetryModal(string message)
        {
            FeedbackTwoButtonModal.Show("Error!", message + "\nDo you wish to retry?", "Yes", "Play offline", RetryPullData, offlineLoadState);
            //No option to play offline, changing "no" answer to load an offline state
        }

        public int getOverAllScore()
        {
            if (PlayerController.Instance.UserScore > OngoingGameData.playerScore)
            {
                OngoingGameData.overAllScore = -1; //opponent won the round

                if (!string.IsNullOrEmpty(FirebaseController.Instance.Token))
                    FirebaseController.Instance.CreateNotification(FirebaseController.Instance.Token, "Uh-Oh!", "You lost your game!");
            }

            if (PlayerController.Instance.UserScore < OngoingGameData.playerScore)
            {
                OngoingGameData.overAllScore = +1; //player won the round

                if (!string.IsNullOrEmpty(FirebaseController.Instance.Token))
                    FirebaseController.Instance.CreateNotification(FirebaseController.Instance.Token, "Woo-Hoo!", "You won your game!");
            }

            return OngoingGameData.overAllScore;
        }

        #endregion feedback specific

        #region offline redun

        // extract JSON and extract to array of SavedGame[]
        // save to PlayerController.
        public SavedGameContainer LoadSavedJSON<SavedGameContainer>(string location) where SavedGameContainer : new()
        {
            Debug.Log("[DataController] Load() : " + location);

            if (File.Exists(location))
            {
                var data = File.ReadAllText(location);
                return JsonUtility.FromJson<SavedGameContainer>(data);
            }
            else return new SavedGameContainer();
        }

        // SavetoJSON
        // saves current rounds into persistent data
        public void SaveSavedJSON<SavedGameContainer>(string location, SavedGameContainer games)
        {
            Debug.Log("[DataController] Save() : " + location);

            if (File.Exists(location))
            {
                var data = JsonUtility.ToJson(games, true);
                File.WriteAllText(location, data);
            }
            else
                File.WriteAllText(location, "{ }");
        }

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

        //create an offline state, similar to normal but with quest parameters
        private void offlineLoadState()
        {
            PlayerController.Instance.PlayerType = PlayerStatus.Guest;
            DataController.Instance.TurnNumber = 0;

            //loads the menu scene
            SceneManager.LoadScene(BuildIndex.Menu);
        }

        #endregion offline redun

        #endregion methods
    }
}