using UnityEngine;
using UnityEngine.SceneManagement;

namespace _LetsQuiz
{
    public class AndroidBackHelper : MonoBehaviour
    {
        #region variables

        private GameController _gameController;
        private bool _showingModal = false;

        #endregion variables

        #region methods

        public void Update()
        {
            if (_showingModal)
                return;

            if (SceneManager.GetActiveScene().buildIndex == BuildIndex.Splash || SceneManager.GetActiveScene().buildIndex == BuildIndex.Login ||
                SceneManager.GetActiveScene().buildIndex == BuildIndex.Menu)
            {
                if (Input.GetKey(KeyCode.Escape))
                {
                    if (!_showingModal)
                    {
                        FeedbackTwoButtonModal.Show("Are you sure?", "Are you sure you want to quit?", "Yes", "No", Application.Quit, Hide);
                        _showingModal = true;
                    }
                }
            }
            else if (SceneManager.GetActiveScene().buildIndex == BuildIndex.Account || SceneManager.GetActiveScene().buildIndex == BuildIndex.Leaderboard ||
                     SceneManager.GetActiveScene().buildIndex == BuildIndex.SubmitQuestion || SceneManager.GetActiveScene().buildIndex == BuildIndex.Settings ||
                     SceneManager.GetActiveScene().buildIndex == BuildIndex.GameLobby || SceneManager.GetActiveScene().buildIndex == BuildIndex.Result)
            {
                if (Input.GetKey(KeyCode.Escape))
                    SceneManager.LoadScene(BuildIndex.Menu, LoadSceneMode.Single);
            }
            else if (SceneManager.GetActiveScene().buildIndex == BuildIndex.Game)
            {
                if (Input.GetKey(KeyCode.Escape))
                {
                    _gameController = FindObjectOfType<GameController>();

                    if (!_showingModal)
                    {
                        FeedbackTwoButtonModal.Show("Are you sure?", "Are you sure you want to forfeit this game?", "Yes", "No", ForfeitGame, Hide);
                        _showingModal = true;
                    }
                }
            }
        }

        private void ForfeitGame()
        {
            _showingModal = false;
            _gameController.EndRound();
        }

        private void Hide()
        {
            _showingModal = false;
            FeedbackTwoButtonModal.Hide();
        }

        #endregion methods
    }
}