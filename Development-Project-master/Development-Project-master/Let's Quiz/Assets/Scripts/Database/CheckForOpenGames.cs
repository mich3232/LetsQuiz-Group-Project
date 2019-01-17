using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _LetsQuiz
{
    public class CheckForOpenGames : MonoBehaviour
    {
        #region variables

        private float _downloadTimer = 5.0f;
        private string _openGamesJSON;

        #endregion variables

        #region methods

        #region download specific

        public void CheckForGamesNeedingOpponents()
        {
            StartCoroutine(CheckForGamesNeedingOpponent());
        }

        private IEnumerator CheckForGamesNeedingOpponent()
        {
            WWW download = new WWW(ServerHelper.Host + ServerHelper.CheckForOpenGames);
            while (!download.isDone)
            {
                if (_downloadTimer < 0)
                {
                    Debug.LogError("Server time out.");
                    DataController.Instance.ServerConnected = false;
                    break;
                }
                _downloadTimer -= Time.deltaTime;
                yield return null;
            }

            if (!download.isDone || download.error != null)
            {
                /* TODO if we cannot connect to the server or there is some error in the data,
                 * check the prefs for previously saved questions */
                Debug.LogError(download.error);
                Debug.Log("Failed to hit the server.");
                DataController.Instance.ServerConnected = false;
            }
            else
            {
                // we got the string from the server, it is every question in JSON format
                Debug.Log("Vox transmition recieved");
                Debug.Log(download.text);

                DataController.Instance.ServerConnected = true;
                _openGamesJSON = download.text;

                handleData();
                yield return download;
            }
        }

        public void handleData()
        {
            OngoingGamesContainer og = new OngoingGamesContainer();
            _openGamesJSON = "{\"dataForOpenGame\":" + _openGamesJSON + "}";
            og = JsonUtility.FromJson<OngoingGamesContainer>(_openGamesJSON);

            int n = -1;
            for (int i = 0; i < og.dataForOpenGame.Length; i++)
            { //check the current user did not start the open game
                if (og.dataForOpenGame[i].player != PlayerController.Instance.GetUsername())
                    n = i;
            }
            if (n < 0)
            {
                Debug.Log("no open games - turn = 1");
                //continueExistingGame = false;
                DataController.Instance.TurnNumber = 1;
                int rand = Random.Range(1, 100000);
                Debug.Log("game number = " + rand);
                DataController.Instance.GameNumber = rand;
            }

            if (n > -1)
            {
                Debug.Log("there are open game(s) - turn = 2");
                DataController.Instance.TurnNumber = 2;
                Debug.Log("****asked questions = " + og.dataForOpenGame[n].askedQuestions);
                Debug.Log("****remaining questions = " + og.dataForOpenGame[n].questionsLeftInCat);

                if (PlayerPrefs.HasKey((DataHelper.PlayerDataKey.GAMEKEY) + PlayerController.Instance.GetUsername()))
                {
                    string games = PlayerPrefs.GetString((DataHelper.PlayerDataKey.GAMEKEY) + PlayerController.Instance.GetUsername());
                    games = games + "," + DataController.Instance.GameNumber.ToString();
                    PlayerPrefs.SetString(((DataHelper.PlayerDataKey.GAMEKEY) + PlayerController.Instance.GetUsername()), games);
                    Debug.Log("games in player prefs = " + PlayerPrefs.GetString((DataHelper.PlayerDataKey.GAMEKEY) + PlayerController.Instance.GetUsername()));
                }
                if (!PlayerPrefs.HasKey(PlayerController.Instance.GetUsername()))
                {
                    PlayerPrefs.SetString(((DataHelper.PlayerDataKey.GAMEKEY) + PlayerController.Instance.GetUsername()), DataController.Instance.GameNumber.ToString());
                    Debug.Log("games in player prefs = " + PlayerPrefs.GetString((DataHelper.PlayerDataKey.GAMEKEY) + PlayerController.Instance.GetUsername()));
                }

                DataController.Instance.OngoingGameData = og.dataForOpenGame[n];
                DataController.Instance.GameNumber = og.dataForOpenGame[n].gameNumber;
            }
            Debug.Log("turn number = " + DataController.Instance.TurnNumber);
            GameLobbyController.Instance.PresentPopUp();
        }

        #endregion download specific

        #endregion methods
    }
}