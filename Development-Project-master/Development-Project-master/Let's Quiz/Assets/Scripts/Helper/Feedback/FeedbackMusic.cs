using UnityEngine;

namespace _LetsQuiz
{
    public class FeedbackMusic : MonoBehaviour
    {
        #region variables

        [Header("Components")]
        public AudioSource source;
        public AudioClip backgroundMusicClip;
        public AudioClip gameMusicClip;

        #endregion variables

        #region methods

        #region unity

        private void Start()
        {
            DontDestroyOnLoad(gameObject);
        }

        #endregion unity

        #region audio specific

        public void PlayBackgroundMusic()
        {
            source.clip = backgroundMusicClip;
            source.Play();
        }

        public void PlayGameMusic()
        {
            source.clip = gameMusicClip;
            source.Play();
        }

        public void Stop()
        {
            source.Stop();
        }

        #endregion audio specific

        #endregion methods
    }
}