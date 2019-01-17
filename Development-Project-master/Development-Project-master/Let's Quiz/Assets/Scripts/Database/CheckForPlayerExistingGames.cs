using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace _LetsQuiz
{
    public class CheckForPlayerExistingGames : MonoBehaviour
    {
        #region variables

        [Header("Components")]
        public GameObject gameButton;

        public Transform buttonContainer;

        [Header("Settings")]
        public Color Red;

        public Color Green;

        private float _connectionTimer = 0.0f;
        private const float _connectionTimeLimit = 1000000.0f;

        private MenuController _menuController;
        private string _openGamesJSON;
        private bool _isInteractable = false;

        #endregion variables

        #region methods

        #region CheckForPlayerExistingGames specific

        public void GetPlayersOpenGames()
        {
            StartCoroutine(PlayersOpenGames());
        }

        private IEnumerator PlayersOpenGames()
        {
            Debug.Log("[CheckForPlayerExistingGames] PlayersOpenGames() : Checking players open games");

            if (!PlayerPrefs.HasKey((DataHelper.PlayerDataKey.GAMEKEY) + PlayerController.Instance.GetUsername()))
                Debug.Log("[CheckForPlayerExistingGames] PlayersOpenGames() :  Player has no open games in playerprefs");

            if (PlayerPrefs.HasKey((DataHelper.PlayerDataKey.GAMEKEY) + PlayerController.Instance.GetUsername()))
                Debug.Log("[CheckForPlayerExistingGames] PlayersOpenGames() : Player has open games: " + PlayerPrefs.GetString((DataHelper.PlayerDataKey.GAMEKEY) + PlayerController.Instance.GetUsername()));

            WWWForm form = new WWWForm();

            //TODO need a better way to generate unique game numbers for the first game
            form.AddField("gameNumbersPost", PlayerPrefs.GetString((DataHelper.PlayerDataKey.GAMEKEY) + PlayerController.Instance.GetUsername()));

            string address = ServerHelper.Host + ServerHelper.GetPlayersOpenGames;
            WWW submitRequest = new WWW(address, form);

            _connectionTimer += Time.deltaTime;
            while (!submitRequest.isDone)
            {
                if (_connectionTimer > _connectionTimeLimit)
                {
                    FeedbackAlert.Show("Server time out.");
                    Debug.LogError("[CheckForPlayerExistingGames] PlayersOpenGames() : Error: " + submitRequest.error);
                    yield return null;
                }

                // extra check just to ensure a stream error doesn't come up
                if (_connectionTimer > _connectionTimeLimit || submitRequest.error != null)
                {
                    FeedbackAlert.Show("Server error.");
                    Debug.LogError("[CheckForPlayerExistingGames] PlayersOpenGames() : Error: " + submitRequest.error);
                    yield return null;
                }
            }

            if (submitRequest.error != null)
            {
                FeedbackAlert.Show("Connection error. Please try again.");
                Debug.LogError("[CheckForPlayerExistingGames] PlayersOpenGames() : Error: " + submitRequest.error);
                yield return null;
            }

            if (submitRequest.isDone)
            {
                Debug.Log("[CheckForPlayerExistingGames] PlayersOpenGames() : Got open game data " + submitRequest.text);
                DisplayOpenGames(submitRequest.text);
                yield return submitRequest;
            }
        }

        public void DisplayOpenGames(string openGamesStartedByTheUser)
        {
            openGamesStartedByTheUser = "{\"dataForOpenGame\":" + openGamesStartedByTheUser + "}";

            Debug.Log("[CheckForPlayerExistingGames] DisplayOpenGames() : JSON of playersOngoingGames " + openGamesStartedByTheUser);

            OngoingGamesContainer gamesPlayerHasStarted = new OngoingGamesContainer();

            //serialize opengame data
            gamesPlayerHasStarted = JsonUtility.FromJson<OngoingGamesContainer>(openGamesStartedByTheUser);

            if (gamesPlayerHasStarted.dataForOpenGame.Length == 0)
                Debug.Log("[CheckForPlayerExistingGames] DisplayOpenGames() : User has no open games");

            for (int i = 0; i < gamesPlayerHasStarted.dataForOpenGame.Length; i++)
            {
                GameObject go = new GameObject();
                //bool isInteractable = isButtonInteractable (gamesPlayerHasStarted, i, playerOne, img);

                //Instantiate must be after you set all the variables on that object!
                go = Instantiate(gameButton) as GameObject;
                go.transform.SetParent(buttonContainer);

                OngoingGamesData gameData = gamesPlayerHasStarted.dataForOpenGame[i];
                go.GetComponentInChildren<Button>().onClick.AddListener(() => ContinueGameButtonPressed(gameData));

                var opponent = "";

                if (!string.IsNullOrEmpty(gameData.opponent))
                {
                    if (PlayerController.Instance.GetUsername() == gameData.opponent)
                        opponent = gameData.player;
                    else
                        opponent = gameData.opponent;
                }
                else
                    opponent = "No opponent found yet.";

                //this is what is written on each button
                go.GetComponentInChildren<Text>().text = "(" + gameData.gameNumber.ToString() + ") vs " + opponent;

                isInteractable(gameData, go);

                // the scale on my prefab is blowing out at runtime, this fixes that problem
                go.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
            }
        }

        private void ContinueGameButtonPressed(OngoingGamesData gameData)
        {
            Debug.Log("[CheckForPlayerExistingGames] ContinueGameButtonPressed() : " + gameData.gameNumber);

            DataController.Instance.OngoingGameData = gameData;
            DataController.Instance.TurnNumber = DataController.Instance.OngoingGameData.turnNumber;
            DataController.Instance.TurnNumber++;
            GameLobbyController.Instance.PresentPopUp();
        }

        private void isInteractable(OngoingGamesData gameData, GameObject go)
        {
            Button b;

            var colors = go.GetComponentInChildren<Button>().colors;

            if (PlayerController.Instance.GetUsername() == gameData.player)
            {
                if (gameData.turnNumber == 1 || gameData.turnNumber == 2 || gameData.turnNumber == 5)
                {
                    b = go.GetComponentInChildren<Button>();
                    b.GetComponent<Image>().color = Red;
                    go.GetComponentInChildren<Button>().interactable = false;
                }

                if (gameData.turnNumber == 3 || gameData.turnNumber == 4)
                {
                    b = go.GetComponentInChildren<Button>();
                    b.GetComponent<Image>().color = Green;
                    go.transform.SetAsFirstSibling();
                    go.GetComponentInChildren<Button>().interactable = true;
                }
            }

            if (PlayerController.Instance.GetUsername() == gameData.opponent)
            {
                if (gameData.turnNumber == 1)
                    Debug.LogError("[CheckForPlayerExistingGames] IsInteractable() : this should not happen");

                if (gameData.turnNumber == 3 || gameData.turnNumber == 4)
                {
                    b = go.GetComponentInChildren<Button>();
                    b.GetComponent<Image>().color = Red;
                    go.GetComponentInChildren<Button>().interactable = false;
                }

                if (gameData.turnNumber == 2 || gameData.turnNumber == 5)
                {
                    b = go.GetComponentInChildren<Button>();
                    b.GetComponent<Image>().color = Green;
                    go.GetComponentInChildren<Button>().interactable = true;
                }
            }
        }
    }

    #endregion CheckForPlayerExistingGames specific

    #endregion methods
}