using System.Collections;
using UnityEngine;

namespace _LetsQuiz
{
    public class GetHighScores : MonoBehaviour
    {
        #region variables

        private float _downloadTimer = 5.0f;

        #endregion variables

        #region methods

        #region properties

        public DataController DataController
        {
            get
            {
                if (DataController.Initialised)
                    return DataController.Instance;
                else return null;
            }
        }
        public PlayerController PlayerController
        {
            get
            {
                if (PlayerController.Initialised)
                    return PlayerController.Instance;
                else return null;
            }
        }

        #endregion properties

        #region GetHighScores specific

        public IEnumerator PullAllHighScoresFromServer()
        {
            WWW download = new WWW(ServerHelper.Host + ServerHelper.HighScores);
            while (!download.isDone)
            {
                if (_downloadTimer < 0)
                {
                    Debug.LogError("[GetHighScores] PullAllHighScoresFromServer() : Server time out.");
                    DataController.ServerConnected = false;
                    break;
                }
                _downloadTimer -= Time.deltaTime;

                yield return null;
            }

            if (!download.isDone || download.error != null)
            {
                /* if we cannot connect to the server or there is some error in the data,
                 * check the prefs for previously saved questions */
                Debug.LogError(download.error);
                Debug.Log("[GetHighScores] PullAllHighScoresFromServer() : Failed to hit the server.");
                DataController.ServerConnected = false;
            }
            else
            {
                // we got the string from the server, it is every question in JSON format
                Debug.Log("[GetHighScores] PullAllHighScoresFromServer() : Data recieved");
                DataController.ServerConnected = true;
                DataController.AllHighScoreJSON = download.text;

                yield return download;

                PlayerController.SetHighscoreData(download.text);
                DataController.Init();
            }
        }

        #endregion GetHighScores specific

        #endregion methods
    }
}