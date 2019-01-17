using UnityEngine;

namespace _LetsQuiz
{
    public class HighscoreController : Singleton<HighscoreController>
    {
        #region properties

        public string HighScoreData { get; private set; }

        #endregion properties

        #region methods

        public void Load()
        {
            DontDestroyOnLoad(gameObject);

            Debug.LogFormat("[{0}] Load()", GetType().Name);

            if (PlayerController.Initialised)
                HighScoreData = PlayerController.Instance.HighScoreJSON;

            Debug.LogFormat("[{0}] Load() : AllHighScore Data {1}", GetType().Name, HighScoreData);
        }

        public HighScoresContainer ExtractHighScores()
        {
            HighScoresContainer AllHighScorers = JsonUtility.FromJson<HighScoresContainer>(HighScoreData);
			Debug.Log (HighScoreData);

            return AllHighScorers;
        }

        #endregion methods
    }
}