using UnityEngine;

namespace _LetsQuiz
{
    public static class PlayerJsonHelper
    {
        #region methods

        public static Player LoadPlayerFromServer(string playerString)
        {
            return !string.IsNullOrEmpty(playerString) ? JsonUtility.FromJson<Player>(playerString) : null;
        }

        #endregion
    }
}

