using UnityEngine;

namespace _LetsQuiz
{
    public class Singleton<T> : MonoBehaviour where T : Singleton<T>
    {
        #region properties

        public static T Instance { get; private set; }

        public static bool Initialised { get { return Instance != null; } }

        #endregion properties

        #region methods

        #region unity

        protected virtual void OnEnable()
        {
            if (Instance != null)
                Debug.LogErrorFormat("[{0}] OnEnable() : Instance of {1} already initialised.", name, GetType().Name);
            else
                Instance = (T)this;
        }

        protected virtual void OnDisable()
        {
            if (Instance == this)
                Instance = null;
        }

        #endregion unity

        #endregion methods
    }
}