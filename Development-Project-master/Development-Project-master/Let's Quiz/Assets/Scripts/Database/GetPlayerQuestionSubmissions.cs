using System.Collections;
using UnityEngine;

namespace _LetsQuiz
{
    public class GetPlayerQuestionSubmissions : MonoBehaviour
    {
        #region variables

        private float _downloadTimer = 5.0f;

        #endregion variables

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

        #region methods

        #region unity

        private void Start()
        {
            // StartCoroutine(PullQuesionSubmitters ());
            // Debug.Log("[GetPlayerQuestionSubmissions] Start() : Load Player Submission");
        }

        #endregion unity

        #region download specific

        public IEnumerator PullQuestionSubmitters()
        {
            WWW download = new WWW(ServerHelper.Host + ServerHelper.GetQuestionSubmissionStuff);

            while (!download.isDone)
            {
                if (_downloadTimer < 0)
                {
                    Debug.LogError("[GetPlayerQuestionSubmissions] PullQuestionsFromServer() : Server time out.");
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
                Debug.Log("[GetPlayerQuestionSubmissions] PullQuestionsFromServer() : Failed to hit the server.");
                DataController.ServerConnected = false;
            }
            else
            {
                // we got the string from the server, it is every question in JSON format
                Debug.Log("[GetPlayerQuestionSubmissions] PullQuestionsFromServer() : Questions: " + download.text);
                HandleData(download.text);
                yield return download;
            }
        }

        private void HandleData(string json)
        {
            QuestAndSubContainer qsc = new QuestAndSubContainer();
            json = "{\"dataForQuestAndSub\":" + json + "}"; //you have to do this because you cannot serialize directly into an array, you need an object that holds the array
            qsc = JsonUtility.FromJson<QuestAndSubContainer>(json);//now we have an object that holds our array of questAndSub objects
            QuestAndSub[] _questAndSub = qsc.dataForQuestAndSub; //fuck off the container and there is your array of stuff
            PlayerController.SetQuestandSubData(_questAndSub);
        }

        #endregion download specific

        #endregion methods
    }
}