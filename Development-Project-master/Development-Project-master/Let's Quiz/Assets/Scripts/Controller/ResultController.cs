using Facebook.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace _LetsQuiz
{
    public class ResultController : MonoBehaviour
    {
        #region variables

        [Header("Components")]
        public Image backgroundEffect;

        public Text score;
        public Text username;
        public Text rankText;
        public Text rank;
        public Text worldText;
        public GameObject finalResultsPanel;
        public GameObject shareButton;
        public Text WinnerText;

        private float _connectionTimer = 0.0f;
        private const float _connectionTimeLimit = 1000000.0f;

        private int _ranking;
        private FeedbackMusic _music;

        #endregion variables

        #region methods

        #region unity

        private void Awake()
        {
            _music = FindObjectOfType<FeedbackMusic>();
            _music.PlayBackgroundMusic();

            if (!FB.IsLoggedIn)
            {
                shareButton.SetActive(false);
            }
        }

        private void Start()
        {
            if (PlayerController.Instance.GetPlayerType() == PlayerStatus.LoggedIn)
            {
                score.enabled = true;
                username.enabled = true;
                rank.enabled = true;
                rankText.enabled = true;
                worldText.enabled = true;
            }
            else
            {
                score.enabled = false;
                username.enabled = true;
                rank.enabled = true;
                rankText.enabled = true;
                worldText.enabled = false;
            }
            if (DataController.Instance.TurnNumber == 6)
            {
                finalResultsPanel.SetActive(true);

                if (DataController.Instance.OngoingGameData.overAllScore == -1)
                    WinnerText.text = DataController.Instance.OngoingGameData.opponent + " won";

                if (DataController.Instance.OngoingGameData.overAllScore == 1)
                    WinnerText.text = DataController.Instance.OngoingGameData.player + " won";
            }

            StartCoroutine(FindRanking());
            submitRanking();
            Display();
        }

        private void Update()
        {
            // spins background image
            backgroundEffect.transform.Rotate(Vector3.forward * Time.deltaTime * 7.0f);
        }

        #endregion unity

        #region user feedback

        private void Display()
        {
            if (PlayerController.Instance.GetPlayerType() == PlayerStatus.LoggedIn)
            {
                score.text = PlayerController.Instance.UserScore.ToString();
                username.text = PlayerController.Instance.GetUsername();
            }
            else
            {
                username.text = PlayerController.Instance.GetUsername();
                rankText.text = "Your final score was";
                rank.text = PlayerController.Instance.UserScore.ToString();
            }
        }

        #endregion user feedback

        #region rank specific

        public IEnumerator FindRanking()
        {
            float _downloadTimer = 5.0f;

            WWW download = new WWW(ServerHelper.Host + ServerHelper.GetRanking);

            _downloadTimer += Time.deltaTime;

            while (!download.isDone)
            {
                if (_connectionTimer > _connectionTimeLimit)
                {
                    Debug.LogErrorFormat("[{0}] FindRanking() : Error {1}", GetType().Name, download.error);
                    yield return null;
                }
            }

            if (!download.isDone || download.error != null)
            {
                /* if we cannot connect to the server or there is some error in the data,
                 * check the prefs for previously saved questions */
                Debug.LogErrorFormat("[{0}] FindRanking() : Error {1}", GetType().Name, download.error);
                Debug.LogErrorFormat("[{0}] FindRanking() : Error : Failed to hit the server.");
                yield return null;
            }

            if (download.isDone)
            {
                // we got the string from the server, it is every question in JSON format
                Debug.LogFormat("[{0}] FindRanking() : {1}", GetType().Name, download.text);
                yield return download;
                calculateRanking(download.text);
            }
        }

        private void calculateRanking(string s)
        {
            var lines = new List<string>(s.Split(new string[] { "<br>" }, System.StringSplitOptions.RemoveEmptyEntries));
            var list = new List<int>();

            for (int i = 0; i < lines.Count; i++)
                list.Add(int.Parse(lines[i]));

            list.Sort();
            _ranking = 0;
            for (int i = list.Count - 1; i > 0; i--)
            {
                if (PlayerController.Instance.UserScore <= list[i])
                    _ranking = i - 1;
            }

            rank.text = (list.Count - _ranking) + " out of " + list.Count;
        }

        private void submitRanking()
        {
            WWWForm form = new WWWForm();

            form.AddField("username", PlayerController.Instance.GetUsername());
            form.AddField("score", PlayerController.Instance.UserScore);

            WWW submitRank = new WWW(ServerHelper.Host + ServerHelper.SetRanking, form);

            _connectionTimer += Time.deltaTime;

            while (!submitRank.isDone)
            {
                if (_connectionTimer > _connectionTimeLimit)
                {
                    FeedbackAlert.Show("Server time out.");
                    Debug.LogError("ResultController : ValidSubmission() : " + submitRank.error);
                    Debug.Log(submitRank.text);
                    return;
                }

                // extra check just to ensure a stream error doesn't come up
                if (_connectionTimer > _connectionTimeLimit || submitRank.error != null)
                {
                    FeedbackAlert.Show("Server time out.");
                    Debug.LogErrorFormat("[{0}] submitRanking() : Error {1} ", GetType().Name, submitRank.error);
                    Debug.Log(submitRank.text);
                    return;
                }
            }

            if (submitRank.error != null)
            {
                FeedbackAlert.Show("Connection error. Please try again.");
                Debug.LogErrorFormat("[{0}] submitRanking() : Error {1} ", GetType().Name, submitRank.error);
                return;
            }

            if (submitRank.isDone)
            {
                if (!string.IsNullOrEmpty(submitRank.text))
                {
                    Debug.LogFormat("[{0}] submitRanking() : {1}", GetType().Name, submitRank.text);
                    return;
                }
            }
        }

        #endregion rank specific

        #region user interaction

        public void BackToMenu()
        {
            FeedbackClick.Play();
			SceneManager.LoadScene(BuildIndex.Menu, LoadSceneMode.Single);
            DestroyImmediate(GameLobbyController.Instance.gameObject);
            DestroyImmediate(MenuController.Instance.gameObject);
            
        }

        #endregion user interaction

        #endregion methods
    }
}