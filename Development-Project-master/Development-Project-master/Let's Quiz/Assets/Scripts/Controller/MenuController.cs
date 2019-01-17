using Facebook.Unity;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace _LetsQuiz
{
    public class MenuController : Singleton<MenuController>
    {
        #region variables

        [Header("Component")]
        public Button accountButton;
        public Button leaderboardButton;
        public Button submitQuestionButton;

        private Text _usernameText;
        private FeedbackMusic _music;
        private GetAllQuestions _questionDownload;

        #endregion variables

        #region methods

        #region unity

        protected override void OnEnable()
        {
            if (Initialised)
                return;

            base.OnEnable();
            DontDestroyOnLoad(gameObject);
        }

        private void Awake()
        {
            _usernameText = GameObject.FindGameObjectWithTag("Username_Text").GetComponent<Text>();

            var playerType = 0;

            if (PlayerController.Initialised)
                playerType = PlayerController.Instance.GetPlayerType();

            if (PlayerPrefs.HasKey(DataHelper.PlayerDataKey.USERNAME) && (playerType == PlayerStatus.LoggedIn || playerType == PlayerStatus.Guest))
                _usernameText.text = PlayerController.Instance.GetUsername();

            if (playerType == PlayerStatus.Guest)
            {
                accountButton.gameObject.SetActive(false);
                leaderboardButton.gameObject.SetActive(false);
                submitQuestionButton.gameObject.SetActive(false);

            }
        }

        // TODO what is this for, I have seen a few private start() methods, when are they called, do they behave like a public Start()?
        // NOTE : does the same as public start() - just better programming pratise
        private void Start()
        {
            _music = FindObjectOfType<FeedbackMusic>();
            _questionDownload = FindObjectOfType<GetAllQuestions>();
            Destroy(_questionDownload);
        }

        #endregion unity

        #region game specific

        public void StartGame()
        {
            FeedbackClick.Play();
            SceneManager.LoadScene(BuildIndex.Game, LoadSceneMode.Single);
        }

        public void GoToGameLobby()
        {
            FeedbackClick.Play();
            SceneManager.LoadScene(BuildIndex.GameLobby, LoadSceneMode.Single);
        }

        #endregion game specific

        #region navigation specific

        public void OpenAccount()
        {
            FeedbackClick.Play();
            SceneManager.LoadScene(BuildIndex.Account, LoadSceneMode.Single);
        }

        public void OpenLeaderboard()
        {
            FeedbackClick.Play();
            SceneManager.LoadScene(BuildIndex.Leaderboard, LoadSceneMode.Single);
        }

        public void OpenSubmitQuestion()
        {
            FeedbackClick.Play();
            SceneManager.LoadScene(BuildIndex.SubmitQuestion, LoadSceneMode.Single);
        }

        public void OpenSetting()
        {
            FeedbackClick.Play();
            SceneManager.LoadScene(BuildIndex.Settings, LoadSceneMode.Single);
        }

        public void Logout()
		{
			FeedbackClick.Play();
			if (FB.IsLoggedIn) {
				FB.LogOut ();
				FeedbackTwoButtonModal.Show("Are you sure?", "Are you sure you want to log out?", "Log out", "Cancel", OpenLogin, FeedbackTwoButtonModal.Hide);
			}else{
				FeedbackClick.Play();
				FeedbackTwoButtonModal.Show("Are you sure?", "Are you sure you want to log out?", "Log out", "Cancel", OpenLogin, FeedbackTwoButtonModal.Hide);
			}
		}

        private void OpenLogin()
        {
            FeedbackClick.Play();
            PlayerController.Instance.SetPlayerType(PlayerStatus.LoggedOut);
            SceneManager.LoadScene(BuildIndex.Login, LoadSceneMode.Single);
        }

        public void Quit()
        {
            FeedbackClick.Play();
            FeedbackTwoButtonModal.Show("Are you sure?", "Are you sure you want to quit?", "Yes", "No", Application.Quit, FeedbackTwoButtonModal.Hide);
        }

        #endregion navigation specific

        #endregion methods
    }
}