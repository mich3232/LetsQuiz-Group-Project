using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace _LetsQuiz
{
    public class LeaderboardController : MonoBehaviour
    {
        #region variables

        [Header("Player High Scorers")]
        private HighScoresContainer _allHighScores;
        public SimpleObjectPool highScorerObjectPool;
        public Transform highScorerParent;
        private string _highScoreData;

        [Header("Question High Scorers")]
        private QuestAndSub[] _questandSub;
        public SimpleObjectPool questionHighscoreObjectPool;
        public Transform questionHighscoreParent;

        [Header("Total Correct Answers Scorers")]
        public SimpleObjectPool TotalCorrectObjectPool;
        public Transform TotalCorrectParent;

        //gameobject for overall highscore
        private List<GameObject> _highScorerGameObjects = new List<GameObject>();

        //gameobject for submitted questions
        private List<GameObject> _questionHighscoreObjects = new List<GameObject>();

        //gameobject for total questions correct by players
        private List<GameObject> _totalQuestionsCorrectObjects = new List<GameObject>();

        [Header("Panels")]
        public GameObject OverallScorePanel;
        public GameObject TotalCorrectPanel;
        public GameObject TopQuestionPanel;

        #endregion variables

        #region methods

        #region unity

        private void Start()
        {
            OverallScorePanel = GameObject.Find("OverallScorePanel");
            TotalCorrectPanel = GameObject.Find("TotalCorrectPanel");
            TopQuestionPanel = GameObject.Find("TopQuestionPanel");

            OverallScorePanel.gameObject.SetActive(true);
            TotalCorrectPanel.gameObject.SetActive(false);
            TopQuestionPanel.gameObject.SetActive(false);

            if (HighscoreController.Initialised)
                HighscoreController.Instance.Load();

            _allHighScores = HighscoreController.Instance.ExtractHighScores();

            ShowHighScorers(_allHighScores);

            //get the QuestandSub highscorers
            if (PlayerController.Initialised)
                _questandSub = PlayerController.Instance.GetQuestandSubData();

            ShowQuestionHighScorers(_questandSub);

            ShowTotalQuestionsCorrect(_allHighScores);
        }

        public void ToggleHighestScorePanel()
        {
            OverallScorePanel.SetActive(true);
            TotalCorrectPanel.SetActive(false);
            TopQuestionPanel.SetActive(false);
            Debug.Log("[LeaderboardController] ToggleHighestScorePanel(): Toggled Highest Score");
        }

        public void ToggleCorrectPanel()
        {
            OverallScorePanel.SetActive(false);
            TotalCorrectPanel.SetActive(true);
            TopQuestionPanel.SetActive(false);
            Debug.Log("[LeaderboardController] ToggleCorrectPanel(): Toggled Correct");
        }

        public void ToggleTopQuestionPanel()
        {
            OverallScorePanel.SetActive(false);
            TotalCorrectPanel.SetActive(false);
            TopQuestionPanel.SetActive(true);
            Debug.Log("[LeaderboardController] ToggleTopQuestionPanel(): Toggled Top Question");
        }

        #endregion unity

        #region high score specific

        //sorts highScorers and displays top 10 in highScorerParent using LeaderboardEntry prefabs.
        private void ShowHighScorers(HighScoresContainer allHighScorers)
        {
            //clear leaderboard to start
            RemoveHighScorers();

            //sort scores by totalScore.
            HighScoresObject[] sorted = allHighScorers.allHighScorers.OrderBy(c => c.getTotalScoreInt()).ToArray();

            //for some reason the sorted array is in reverse order, so the for loop runs from the last 10 items.
			if (sorted.Length > 10) {
				for (int i = sorted.Length - 1; i > sorted.Length - 11; i--) {
					GameObject highScorerGameObject = highScorerObjectPool.GetObject (); //create new GameObejct
					HighScoresObject currentHighScore = sorted [i]; 						//get current highscorer

					_highScorerGameObjects.Add (highScorerGameObject);
					highScorerGameObject.transform.SetParent (highScorerParent);
					LeaderboardEntry leaderBoardEntry = highScorerGameObject.GetComponent<LeaderboardEntry> ();

					leaderBoardEntry.SetUp (currentHighScore.userName, currentHighScore.totalScore); //pass in the data of current HighScorer
				}
			}
			else
				for (int i = sorted.Length - 1; i > 0; i--) {
					GameObject highScorerGameObject = highScorerObjectPool.GetObject (); //create new GameObejct
					HighScoresObject currentHighScore = sorted [i]; 						//get current highscorer

					_highScorerGameObjects.Add (highScorerGameObject);
					highScorerGameObject.transform.SetParent (highScorerParent);
					LeaderboardEntry leaderBoardEntry = highScorerGameObject.GetComponent<LeaderboardEntry> ();

					leaderBoardEntry.SetUp (currentHighScore.userName, currentHighScore.totalScore); //pass in the data of current HighScorer
				}
				
        }

        //removes all Player Highscore LeaderboardEntry Objects from the scene
        private void RemoveHighScorers()
        {
            while (_highScorerGameObjects.Count > 0)
            {
                highScorerObjectPool.ReturnObject(_highScorerGameObjects[0]);
                _highScorerGameObjects.RemoveAt(0);
            }
        }

        //show/sort  Player Questions by rating
        private void ShowQuestionHighScorers(QuestAndSub[] unsortedQuestions)
        {
            QuestAndSub[] sortedQuestionsByRating = unsortedQuestions.OrderBy(c => c.getRating()).ToArray();

            //for some reason the sorted array is in reverse order, so the for loop runs from the last 10 items.
			if (sortedQuestionsByRating.Length > 10) {
				for (int i = sortedQuestionsByRating.Length - 1; i > sortedQuestionsByRating.Length - 11; i--) {
					GameObject questionHighScoreObject = questionHighscoreObjectPool.GetObject (); //create new GameObejct
					QuestAndSub currentQuestionHighscore = sortedQuestionsByRating [i];                      //get current highscorer

					_questionHighscoreObjects.Add (questionHighScoreObject);
					questionHighScoreObject.transform.SetParent (questionHighscoreParent);
					LeaderboardEntry leaderBoardEntry = questionHighScoreObject.GetComponent<LeaderboardEntry> ();

					leaderBoardEntry.SetUp (currentQuestionHighscore.QuestionText, currentQuestionHighscore.Rating.ToString ()); //pass in the data of current HighScorer
				}
			}
			else
				for (int i = sortedQuestionsByRating.Length - 1; i > 0; i--) {
					GameObject questionHighScoreObject = questionHighscoreObjectPool.GetObject (); //create new GameObejct
					QuestAndSub currentQuestionHighscore = sortedQuestionsByRating [i];                      //get current highscorer

					_questionHighscoreObjects.Add (questionHighScoreObject);
					questionHighScoreObject.transform.SetParent (questionHighscoreParent);
					LeaderboardEntry leaderBoardEntry = questionHighScoreObject.GetComponent<LeaderboardEntry> ();

					leaderBoardEntry.SetUp (currentQuestionHighscore.QuestionText, currentQuestionHighscore.Rating.ToString ()); //pass in the data of current HighScorer
				}
				
        }

        //removes all Question Highscore Obejcts from the scene
        private void RemoveQuestionHighscores()
        {
            while (_questionHighscoreObjects.Count > 0)
            {
                questionHighscoreObjectPool.ReturnObject(_questionHighscoreObjects[0]);
                _questionHighscoreObjects.RemoveAt(0);
            }
        }

        //show most correct answers
        private void ShowTotalQuestionsCorrect(HighScoresContainer allHighScorers)
        {
            RemoveTotalQuestionsCorrect(); //clear leaderboard to start

            //sort scores by totalScore.
            HighScoresObject[] sorted = allHighScorers.allHighScorers.OrderBy(c => c.getTotalCorrect()).ToArray();

            //for some reason the sorted array is in reverse order, so the for loop runs from the last 10 items.
			if (sorted.Length > 10) {
				for (int i = sorted.Length - 1; i > sorted.Length - 11; i--) {
					GameObject totalCorrectgameObject = TotalCorrectObjectPool.GetObject (); //create new GameObejct
					HighScoresObject currentHighScore = sorted [i]; 						//get current highscorer

					_highScorerGameObjects.Add (totalCorrectgameObject);
					totalCorrectgameObject.transform.SetParent (TotalCorrectParent);
					LeaderboardEntry leaderBoardEntry = totalCorrectgameObject.GetComponent<LeaderboardEntry> ();

					leaderBoardEntry.SetUp (currentHighScore.userName, currentHighScore.questionsRight); //pass in the data of current HighScorer
				}
			}
			else 
				for (int i = sorted.Length - 1; i > 0; i--) {
					GameObject totalCorrectgameObject = TotalCorrectObjectPool.GetObject (); //create new GameObejct
					HighScoresObject currentHighScore = sorted [i]; 						//get current highscorer

					_highScorerGameObjects.Add (totalCorrectgameObject);
					totalCorrectgameObject.transform.SetParent (TotalCorrectParent);
					LeaderboardEntry leaderBoardEntry = totalCorrectgameObject.GetComponent<LeaderboardEntry> ();

					leaderBoardEntry.SetUp (currentHighScore.userName, currentHighScore.questionsRight); //pass in the data of current HighScorer
				}
        }

        //remove most correct answers objects
        private void RemoveTotalQuestionsCorrect()
        {
            while (_totalQuestionsCorrectObjects.Count > 0)
            {
                TotalCorrectObjectPool.ReturnObject(_totalQuestionsCorrectObjects[0]);
                _totalQuestionsCorrectObjects.RemoveAt(0);
            }
        }

        #endregion high score specific

        #region navigation specific

        public void BackToMenu()
        {
            FeedbackClick.Play();
            SceneManager.LoadScene(BuildIndex.Menu, LoadSceneMode.Single);
        }

        #endregion navigation specific

        #endregion methods
    }
}