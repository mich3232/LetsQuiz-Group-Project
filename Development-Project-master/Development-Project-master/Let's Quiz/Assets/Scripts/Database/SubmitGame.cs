using System.Collections;
using UnityEngine;

namespace _LetsQuiz
{
    public class SubmitGame : MonoBehaviour
    {
        private string _questionPool;
        private int _counter; //just used for testing, so the log can show what was submitted
        private bool ConnectionAvailable;

        private float _connectionTimer = 0.0f;
        private const float _connectionTimeLimit = 1000000.0f;

        public void SubmitGameToDB(string _questionPool)
        {
            checkForConnection();  //test for network connectivity
            //uploadExistingGames();                      //test for existing games to be uploaded
            this._questionPool = _questionPool;
            StartCoroutine(SubmitRoundData());
        }

        private IEnumerator SubmitRoundData()
        {
            string address = "";
            WWWForm form = new WWWForm();
            if (DataController.Instance.TurnNumber == 1)
            {
                form.AddField("gameNumberPost", DataController.Instance.GameNumber); //TODO need a better way to generate unique game numbers for the first game
                form.AddField("playerNamePost", PlayerController.Instance.GetUsername());
                form.AddField("askedQuestionsPost", QuestionController.Instance.GetAskedQuestions());
                form.AddField("QuestionsLeftInCatagoryPost", _questionPool);
                form.AddField("Round1CatagoryPost", DataController.Instance.Catagory.ToString());
                form.AddField("scorePost", PlayerController.Instance.UserScore);
                form.AddField("turnsCompletedPost", DataController.Instance.TurnNumber);

                Debug.Log(PlayerController.Instance.GetId().ToString() + " " + PlayerController.Instance.GetGamesPlayed().ToString() + " " + PlayerController.Instance.GetTotalQuestionsAnswered().ToString() + " " + PlayerController.Instance.GetNumberCorrectAnswers().ToString());
                form.AddField("userIDPost", PlayerController.Instance.GetId().ToString());
                form.AddField("totalGamesPlayedPost", PlayerController.Instance.GetGamesPlayed().ToString());
                form.AddField("totalQuestionsPost", PlayerController.Instance.GetTotalQuestionsAnswered().ToString());
                form.AddField("totalCorrectQuestionsPost", PlayerController.Instance.GetNumberCorrectAnswers().ToString());

                _counter = 1;

                address = ServerHelper.Host + ServerHelper.SubmitRound1Data;
            }

            if (DataController.Instance.TurnNumber == 2)
            {
                Debug.Log("Submitting round 2 data");
                form.AddField("gameNumberPost", DataController.Instance.OngoingGameData.gameNumber); //TODO need a better way to generate unique game numbers for the first game
                form.AddField("askedQuestionsPost", "nothing");
                form.AddField("opponentNamePost", PlayerController.Instance.GetUsername());
                form.AddField("scorePost", PlayerController.Instance.UserScore);
                form.AddField("gameRequiresOppoentPost", 0);
                form.AddField("turnsCompletedPost", DataController.Instance.TurnNumber);
                form.AddField("overAllScorePost", DataController.Instance.getOverAllScore());

                Debug.Log(PlayerController.Instance.GetId().ToString() + " " + PlayerController.Instance.GetGamesPlayed().ToString() + " " + PlayerController.Instance.GetTotalQuestionsAnswered().ToString() + " " + PlayerController.Instance.GetNumberCorrectAnswers().ToString());
                form.AddField("userIDPost", PlayerController.Instance.GetId().ToString());
                form.AddField("totalGamesPlayedPost", PlayerController.Instance.GetGamesPlayed().ToString());
                form.AddField("totalQuestionsPost", PlayerController.Instance.GetTotalQuestionsAnswered().ToString());
                form.AddField("totalCorrectQuestionsPost", PlayerController.Instance.GetNumberCorrectAnswers().ToString());

                form.AddField("notificationTo", "/topics/all");
                form.AddField("notificationTitle", "Let's Quiz");
                form.AddField("notificationBody", "It's Round 2");

                _counter = 2;

                address = ServerHelper.Host + ServerHelper.SubmitRound2Data;
            }

            if (DataController.Instance.TurnNumber == 3)
            {
                Debug.Log("Submitting round 3 data");
                form.AddField("gameNumberPost", DataController.Instance.OngoingGameData.gameNumber);
                form.AddField("askedQuestionsPost", QuestionController.Instance.GetAskedQuestions());
                form.AddField("Round2CatagoryPost", DataController.Instance.Catagory.ToString());
                form.AddField("QuestionsLeftInCatagoryPost", _questionPool);
                form.AddField("scorePost", PlayerController.Instance.UserScore);
                form.AddField("turnsCompletedPost", DataController.Instance.TurnNumber);

                Debug.Log(PlayerController.Instance.GetId().ToString() + " " + PlayerController.Instance.GetGamesPlayed().ToString() + " " + PlayerController.Instance.GetTotalQuestionsAnswered().ToString() + " " + PlayerController.Instance.GetNumberCorrectAnswers().ToString());
                form.AddField("userIDPost", PlayerController.Instance.GetId().ToString());
                form.AddField("totalGamesPlayedPost", PlayerController.Instance.GetGamesPlayed().ToString());
                form.AddField("totalQuestionsPost", PlayerController.Instance.GetTotalQuestionsAnswered().ToString());
                form.AddField("totalCorrectQuestionsPost", PlayerController.Instance.GetNumberCorrectAnswers().ToString());

                form.AddField("notificationTo", "/topics/all");
                form.AddField("notificationTitle", "Let's Quiz");
                form.AddField("notificationBody", "It's Round 3");

                _counter = 3;

                address = ServerHelper.Host + ServerHelper.SubmitRound3Data;
            }
            if (DataController.Instance.TurnNumber == 4)
            {
                Debug.Log("Submitting round 4 data");
                form.AddField("gameNumberPost", DataController.Instance.OngoingGameData.gameNumber);
                form.AddField("scorePost", PlayerController.Instance.UserScore);
                form.AddField("turnsCompletedPost", DataController.Instance.TurnNumber);
                form.AddField("overAllScorePost", DataController.Instance.getOverAllScore());

                form.AddField("notificationTo", "/topics/all");
                form.AddField("notificationTitle", "Let's Quiz");
                form.AddField("notificationBody", "It's Round 4");

                _counter = 4;

                address = ServerHelper.Host + ServerHelper.SubmitRound4Data;
            }
            if (DataController.Instance.TurnNumber == 5)
            {
                Debug.Log("Submitting round 5 data");
                form.AddField("gameNumberPost", DataController.Instance.OngoingGameData.gameNumber);
                form.AddField("askedQuestionsPost", QuestionController.Instance.GetAskedQuestions());
                form.AddField("Round3CatagoryPost", DataController.Instance.Catagory.ToString());
                form.AddField("QuestionsLeftInCatagoryPost", _questionPool);
                form.AddField("scorePost", PlayerController.Instance.UserScore);
                form.AddField("turnsCompletedPost", DataController.Instance.TurnNumber);

                form.AddField("notificationTo", "/topics/all");
                form.AddField("notificationTitle", "Let's Quiz");
                form.AddField("notificationBody", "It's Round 5");

                _counter = 5;

                address = ServerHelper.Host + ServerHelper.SubmitRound5Data;
            }

            if (DataController.Instance.TurnNumber == 6)
            {
                form.AddField("gameNumberPost", DataController.Instance.OngoingGameData.gameNumber);
                address = ServerHelper.Host + ServerHelper.SubmitRound6Data;
                DataController.Instance.OngoingGameData.opponentScore = +PlayerController.Instance.UserScore;

                form.AddField("notificationTo", "/topics/all");
                form.AddField("notificationTitle", "Let's Quiz");
                form.AddField("notificationBody", "It's Round 6");

                _counter = 6;
            }

            _connectionTimer += Time.deltaTime;

            WWW submitRequest = new WWW(address, form);

            while (!submitRequest.isDone)
            {
                if (_connectionTimer > _connectionTimeLimit)
                {
                    FeedbackAlert.Show("Server time out.");
                    Debug.LogError("SubmitGame : Submit() : " + submitRequest.error);
                    PlayerController.Instance.AddSavedGame(new SavedGame(submitRequest));
                    yield return null;
                }

                // extra check just to ensure a stream error doesn't come up
                if (_connectionTimer > _connectionTimeLimit || submitRequest.error != null)
                {
                    FeedbackAlert.Show("Server error.");
                    Debug.LogError("SubmitGame : Submit() : " + submitRequest.error);
                    PlayerController.Instance.AddSavedGame(new SavedGame(submitRequest));
                    yield return null;
                }
            }

            if (submitRequest.error != null)
            {
                FeedbackAlert.Show("Connection error. Please try again.");
                Debug.Log("SubmitGame : Submit() : " + submitRequest.error);
                PlayerController.Instance.AddSavedGame(new SavedGame(submitRequest));
                yield return null;
            }

            if (submitRequest.isDone)
            {
                Debug.Log("game data submitted, using data #" + _counter);
                yield return submitRequest;
                DestroyObject(gameObject);
            }
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
            //how you check if a connection is available
            if (ConnectionAvailable)
            {
                Debug.Log("Connection was succcessful");
            }
            else
            {
                Debug.Log("Connection was not succcessful");
            }
        }

        //upload any exisitng games stored in the player files
        //public void uploadExistingGames()
        //{
        //    Debug.Log("Attempting to upload Existing Games");

        //    if (PlayerController.Instance.SavedGames.AllSavedRounds.Count > 0)
        //    {
        //        SavedGameContainer games = PlayerController.Instance.GetSavedGames();

        //        //iterate through length of saved games

        //        for (int i = 0; i < games.AllSavedRounds.Count; i++)
        //        {
        //            //for each game, attempt to upload.
        //            //if successful then remove from list
        //            _connectionTimer += Time.deltaTime;

        //            WWW submitRequest = games.AllSavedRounds[i]._submitRequest;

        //            if (submitRequest == null)
        //                return;

        //            while (!submitRequest.isDone)
        //            {
        //                if (_connectionTimer > _connectionTimeLimit)
        //                {
        //                    FeedbackAlert.Show("Server time out.");
        //                    Debug.LogError("SubmitScore : Submit() : " + submitRequest.error);
        //                    break;
        //                }

        //                // extra check just to ensure a stream error doesn't come up
        //                if (_connectionTimer > _connectionTimeLimit || submitRequest.error != null)
        //                {
        //                    FeedbackAlert.Show("Server error.");
        //                    Debug.LogError("SubmitScore : Submit() : " + submitRequest.error);
        //                    break;
        //                }
        //            }

        //            if (submitRequest.error != null)
        //            {
        //                FeedbackAlert.Show("Connection error. Please try again.");
        //                Debug.Log("SubmitScore : Submit() : " + submitRequest.error);
        //                break;
        //            }

        //            if (submitRequest.isDone)
        //            {
        //                Debug.Log("game data submitted, using data #" + _counter);
        //                DestroyObject(gameObject);
        //                games.AllSavedRounds.RemoveAt(i);  //remove from current list
        //            }
        //        }

        //        //resave games List to player prefs, any outstanding uploads will be saved back.
        //        PlayerController.Instance.SetSavedGames(games);
        //    }
        //    else
        //    {
        //        Debug.Log("No outstanding games to upload");
        //    }
        //}
    }
}