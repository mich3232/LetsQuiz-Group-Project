using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _LetsQuiz
{
    public class QuestionController : Singleton<QuestionController>
    {
        #region variables

        [Header("Components")]
        private GameLobbyController _gameLobbyController;

        private float _connectionTimer = 0.0f;
        private const float _connectionTimeLimit = 1000000.0f;

        private string _questionData;
        private GameData _allQuestions;
        private List<QuestionData> _askedQuestions;

        #endregion variables

        #region methods

        #region unity

        protected override void OnEnable()
        {
            base.OnEnable();
            DontDestroyOnLoad(gameObject);
        }

        #endregion unity

        #region load

        public void Load()
        {
            Debug.Log("[QuestionController] Load()");

            if (PlayerController.Initialised)
                _questionData = PlayerController.Instance.GetQuestionData();

            //re-attempt round upload
            //UploadExistingRounds();
        }

        #endregion load

        #region extract

        public GameData ExtractQuestionsFromJSON()
        {
            GameData allQ = JsonUtility.FromJson<GameData>(_questionData);
            return allQ;
        }

        public RoundData ExtractQuestionsFromJSON(string json) //TODO Pick up from here. not sure what we need to deserialize this into, probably Round data but there is no name so maybe a new object....
        {
            RoundData allQ = JsonUtility.FromJson<RoundData>(json);
            return allQ;
        }

        public QuestionData[] ExtractQuestionsFromJSON(int catagory)
        {
            GameData allQ = JsonUtility.FromJson<GameData>(_questionData);
            QuestionData[] allQuestionsInCatagory = allQ.allRoundData[catagory].questions;
            return allQuestionsInCatagory;
        }

        #endregion extract

        #region get

        public GameData GetAllQuestions()
        {
            return _allQuestions;
        }

        public QuestionData[] GetAllQuestionsAllCategories()
        {
            List<QuestionData> questionsList = new List<QuestionData>();

            GameData allQ = JsonUtility.FromJson<GameData>(_questionData);

            for (int i = 0; i < allQ.allRoundData.Length; i++)
            {
                for (int n = 0; n < allQ.allRoundData[i].questions.Length; n++)
                {
                    questionsList.Add(allQ.allRoundData[i].questions[n]);
                }
            }

            QuestionData[] allQuestionsAllCategories = questionsList.ToArray();

            return allQuestionsAllCategories;
        }

        public List<string> GetAllCategories()
        {
            List<string> catagoryList = new List<string>();

            GameData allQ = JsonUtility.FromJson<GameData>(_questionData);

            for (int i = 0; i < allQ.allRoundData.Length; i++)
            {
                catagoryList.Add(allQ.allRoundData[i].name);
                Debug.LogFormat("[{0}] GetAllCategories() : {1}", GetType().Name, allQ.allRoundData[i].name);
            }

            return catagoryList;
        }

        public string GetRandomCatagory()
        {
            List<string> catagoryList = GetAllCategories();

            if (DataController.Initialised)
            {
                catagoryList = RemoveCatagory(catagoryList, DataController.Instance.OngoingGameData.Round1Catagory);
                catagoryList = RemoveCatagory(catagoryList, DataController.Instance.OngoingGameData.Round2Catagory);
            }

            //gets random number between 0 and total number of questions
            int randomNumber = Random.Range(0, catagoryList.Count - 1);

            string catagory = catagoryList[randomNumber];
            DataController.Instance.Catagory = catagory;
            return catagory;
        }

        public QuestionData[] GetQuestionsInCatagory(string catagory)
        {
            QuestionData[] questionsInCatagory = null;
            GameData allQ = JsonUtility.FromJson<GameData>(_questionData);
            for (int i = 0; i < allQ.allRoundData.Length; i++)
            {
                if (allQ.allRoundData[i].name.Equals(catagory))
                {
                    questionsInCatagory = allQ.allRoundData[i].questions;
                    return questionsInCatagory;
                }
            }
            return questionsInCatagory;
        }

        public QuestionData[] GetQuestionsInCatagory(int selection)
        {
            QuestionData[] questionsFromCatagory;
            GameData allQ = JsonUtility.FromJson<GameData>(_questionData);
            questionsFromCatagory = allQ.allRoundData[selection].questions;
            return questionsFromCatagory;
        }

        public string GetAskedQuestions()
        {
            RoundData _RoundQuestions = new RoundData
            {
                name = "question list for opponent",
                questions = _askedQuestions.ToArray()
            };

            string askedQuestionsAsJSON = JsonUtility.ToJson(_RoundQuestions);
            Debug.LogFormat("[{1}] GetAskedQuestions() : Asked Questions : {1} ", GetType().Name, askedQuestionsAsJSON);

            //clear the list
            _askedQuestions = new List<QuestionData>();

            return askedQuestionsAsJSON;
        }

        public string GetRemainingQuestions(QuestionData[] questionPool)
        {
            RoundData _RemainingQuestions = new RoundData
            {
                name = "remaining questions in catagory, incase opponent answers all the 'askedQuestions' ",
                questions = questionPool
            };

            string remainingQuestionsAsJSON = JsonUtility.ToJson(_RemainingQuestions);
            Debug.LogFormat("[{0}] GetAskedQuestions() Remaining Questions : {1} ", GetType().Name, remainingQuestionsAsJSON);
            return remainingQuestionsAsJSON;
        }

        #endregion get

        #region remove

        public List<string> RemoveCatagory(List<string> categories, string remove)
        {
            for (int i = 0; i < categories.Count; i++)
            {
                if (categories[i].Equals(remove))
                    categories.RemoveAt(i);
            }
            return categories;
        }

        public QuestionData[] RemoveQuestion(QuestionData[] questionPool, int remove)
        {
            List<QuestionData> poolAsList = new List<QuestionData>();

            for (int i = 0; i < questionPool.Length; i++)
                poolAsList.Add(questionPool[i]);

            poolAsList.RemoveAt(remove);
            questionPool = poolAsList.ToArray();
            return questionPool;
        }

        #endregion remove

        #region add

        public void AddAskedQuestionToAskedQuestions(QuestionData currentQuestion)
        {
            try
            {
                _askedQuestions.Add(currentQuestion);
            }
            catch (Exception e)
            {
                Debug.Log("[QuestionController] AddAskedQuestionToAskedQuestions() Error : " + e.StackTrace);

                _askedQuestions = new List<QuestionData>
                {
                    currentQuestion
                };
            }
        }

        #endregion add

        #region offline redun

        //public void UploadExistingRounds()
        //{
        //    Debug.Log("[QuestionController] UploadExistingRounds(): Attempting to upload existing games");

        //    if (PlayerController.Initialised)
        //    {
        //        if (PlayerController.Instance.SavedGames.AllSavedRounds.Count > 0)
        //        {
        //            SavedGameContainer games = PlayerController.Instance.GetSavedGames();

        //            //iterate through length of saved games
        //            for (int i = 0; i < games.AllSavedRounds.Count; i++)
        //            {
        //                //for each game, attempt to upload.
        //                //if successful then remove from list
        //                _connectionTimer += Time.deltaTime;

        //                WWW submitRequest = games.AllSavedRounds[i]._submitRequest;

        //                if (submitRequest == null)
        //                    return;

        //                while (!submitRequest.isDone)
        //                {
        //                    if (_connectionTimer > _connectionTimeLimit)
        //                    {
        //                        FeedbackAlert.Show("Server time out.");
        //                        Debug.LogError("[QuestionController] UploadExistingRounds() Error: " + submitRequest.error);
        //                        break;
        //                    }

        //                    // extra check just to ensure a stream error doesn't come up
        //                    if (_connectionTimer > _connectionTimeLimit || submitRequest.error != null)
        //                    {
        //                        FeedbackAlert.Show("Server error.");
        //                        Debug.LogError("[QuestionController] UploadExistingRounds() Error: " + submitRequest.error);
        //                        break;
        //                    }
        //                }

        //                if (submitRequest.error != null)
        //                {
        //                    FeedbackAlert.Show("Connection error. Please try again.");
        //                    Debug.Log("[QuestionController] UploadExistingRounds() Error: " + submitRequest.error);
        //                    break;
        //                }

        //                if (submitRequest.isDone)
        //                {
        //                    Debug.Log("[QuestionController] UploadExistingRounds() Submission complete");
        //                    DestroyObject(gameObject);
        //                    //remove from current list
        //                    games.AllSavedRounds.RemoveAt(i);
        //                }
        //            }

        //            //resave games List to player prefs, any outstanding uploads will be saved back.
        //            PlayerController.Instance.SetSavedGames(games);
        //        }
        //        else
        //        {
        //            Debug.Log("[QuestionController] UploadExsistingRounds() : No outstanding games to upload");
        //        }
        //    }
        //}

        #endregion offline redun
    }

    #endregion methods
}