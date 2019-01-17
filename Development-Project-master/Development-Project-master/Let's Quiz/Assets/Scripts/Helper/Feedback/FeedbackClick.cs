using UnityEngine;

namespace _LetsQuiz
{
    public class FeedbackClick : MonoBehaviour
    {
        #region variables

        [Header("Component")]
        private static GameObject _instance;
        private static AudioSource _source;

        #endregion

        #region methods

        #region unity

        private static void Create()
        {
            _instance = Instantiate(Resources.Load<GameObject>("Feedback/Click"));
        }

        #endregion

        #region audio specific

        public static void Play()
        { 
            Create();
            _source = _instance.GetComponent<AudioSource>();
            _source.Play();

            if (!_source.isPlaying)
                Destroy(_instance);
        }

        #endregion

        #endregion
    }
}