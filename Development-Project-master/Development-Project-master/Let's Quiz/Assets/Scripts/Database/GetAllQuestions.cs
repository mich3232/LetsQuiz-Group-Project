using System.Collections;
using UnityEngine;

namespace _LetsQuiz
{
    public class GetAllQuestions : MonoBehaviour
    {
        #region variables

        private float _downloadTimer = 5.0f;

        private DataController _dataController;
        private PlayerController _playerController;

        #endregion variables

        #region methods

        #region unity

        private void Start()
        {
            _dataController = FindObjectOfType<DataController>();
            _playerController = FindObjectOfType<PlayerController>();
        }

        #endregion unity

        #region download specific

        public IEnumerator PullAllQuestionsFromServer()
        {
            WWW download = new WWW(ServerHelper.Host + ServerHelper.DownloadQuestion);
            while (!download.isDone)
            {
                if (_downloadTimer < 0)
                {
                    Debug.LogError("[GetAllQuestions] PullAllQuestionsFromServer() : Server time out.");
					Debug.Log ("Server time out: Loading Quesitons from local storage");
					_playerController.SetQuestionData (_playerController.GetSavedQuestionData ());
                    _dataController.ServerConnected = false;
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
                Debug.Log("[GetAllQuestions] PullAllQuestionsFromServer() : Failed to hit the server.");
				Debug.Log ("Server could not be hit: Loading Quesitons from local storage");
				_playerController.SetQuestionData (_playerController.GetSavedQuestionData ());
                _dataController.ServerConnected = false;
            }
            else
            {
                // we got the string from the server, it is every question in JSON format
                Debug.Log("[GetAllQuestions] PullAllQuestionsFromServer() : Data recieved " + download.text);

                _dataController.ServerConnected = true;
                _dataController.AllQuestionJSON = download.text;

                yield return download;

                _playerController.SetQuestionData(download.text);
				_playerController.SaveQuestionData ();   //saves the _questionData in PlayerPrefs to internal storage
				if(QuestionController.Initialised)
					QuestionController.Instance.Load();
                _dataController.Init();
            }
        }

		public void PullSavedQuestionsFromLocal()
		{
			Debug.Log ("No internet connection: Loading Quesitons from local storage");
			string questionJSON = _playerController.GetSavedQuestionData ();
			_playerController.SetQuestionData (questionJSON);
			_dataController.ServerConnected = false;
			_dataController.AllQuestionJSON = questionJSON;

			if(QuestionController.Initialised)
				QuestionController.Instance.Load();
			_dataController.Init();
			
		}

        #endregion download specific

        #endregion methods
    }
}