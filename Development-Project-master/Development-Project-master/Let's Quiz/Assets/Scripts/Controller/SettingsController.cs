using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace _LetsQuiz
{
    public class SettingsController : Singleton<SettingsController>
    {
        #region variables

        [Header("Components")]
        public AudioMixer MasterMixer;

        public PlayerSettings Settings;
        private Toggle _soundEffectSwitch;
        private Toggle _backgroundMusicSwitch;

        #endregion variables

        #region methods

        #region unity

        private void Awake()
        {
            Load();
        }

        #endregion unity

        #region navigation specific

        public void BackToMenu()
        {
            FeedbackClick.Play();
            SceneManager.LoadScene(BuildIndex.Menu, LoadSceneMode.Single);
        }

        #endregion navigation specific

        #region user interaction

        // set the sound effect mixer value based on toggle status
        public void ToggleSoundEffect(bool status)
        {
            FeedbackClick.Play();

            Settings.SoundEffectToggle = status ? 1 : 0;

            if (status)
                MasterMixer.SetFloat(DataHelper.AudioParameter.SOUND_EFFECT, DataHelper.AudioParameter.UNMUTED_VOLUME);
            else
                MasterMixer.SetFloat(DataHelper.AudioParameter.SOUND_EFFECT, DataHelper.AudioParameter.MUTED_VOLUME);

            MasterMixer.GetFloat(DataHelper.AudioParameter.SOUND_EFFECT, out Settings.SoundEffectVolume);

            PlayerPrefs.SetInt(DataHelper.PlayerSettingsKey.EFFECT_TOGGLE, Settings.SoundEffectToggle);
            PlayerPrefs.SetFloat(DataHelper.PlayerSettingsKey.EFFECT_VOLUME, Settings.SoundEffectVolume);

            Save();
        }

        // set the background music mixer value based on toggle status
        public void ToggleBackgroundMusic(bool status)
        {
            FeedbackClick.Play();

            Settings.BackgroundMusicToggle = status ? 1 : 0;

            if (status)
                MasterMixer.SetFloat(DataHelper.AudioParameter.BACKGROUND_MUSIC, DataHelper.AudioParameter.UNMUTED_VOLUME);
            else
                MasterMixer.SetFloat(DataHelper.AudioParameter.BACKGROUND_MUSIC, DataHelper.AudioParameter.MUTED_VOLUME);

            MasterMixer.GetFloat(DataHelper.AudioParameter.BACKGROUND_MUSIC, out Settings.BackgroundMusicVolume);

            PlayerPrefs.SetInt(DataHelper.PlayerSettingsKey.MUSIC_TOGGLE, Settings.BackgroundMusicToggle);
            PlayerPrefs.SetFloat(DataHelper.PlayerSettingsKey.MUSIC_VOLUME, Settings.BackgroundMusicVolume);

            Save();
        }

        #endregion user interaction

        #region load settings

        // save player settings to playerprefs
        public void Save()
        {
            PlayerPrefs.Save();
        }

        // load player settings from playerprefs
        public void Load()
        {
            Debug.LogFormat("[{0}] Load()", GetType().Name);

            if (PlayerPrefs.HasKey(DataHelper.PlayerDataKey.ID))
            {
                Settings.SoundEffectVolume = PlayerPrefs.GetFloat(DataHelper.PlayerSettingsKey.EFFECT_VOLUME);
                Settings.SoundEffectToggle = PlayerPrefs.GetInt(DataHelper.PlayerSettingsKey.EFFECT_TOGGLE);
                Settings.BackgroundMusicVolume = PlayerPrefs.GetFloat(DataHelper.PlayerSettingsKey.MUSIC_VOLUME);
                Settings.BackgroundMusicToggle = PlayerPrefs.GetInt(DataHelper.PlayerSettingsKey.MUSIC_TOGGLE);
            }
            else
            {
                Settings = new PlayerSettings();

                PlayerPrefs.SetInt(DataHelper.PlayerSettingsKey.MUSIC_TOGGLE, Settings.BackgroundMusicToggle);
                PlayerPrefs.SetFloat(DataHelper.PlayerSettingsKey.MUSIC_VOLUME, Settings.BackgroundMusicVolume);
                PlayerPrefs.SetInt(DataHelper.PlayerSettingsKey.MUSIC_TOGGLE, Settings.BackgroundMusicToggle);
                PlayerPrefs.SetFloat(DataHelper.PlayerSettingsKey.MUSIC_VOLUME, Settings.BackgroundMusicVolume);
            }

            if (SceneManager.GetActiveScene().buildIndex == BuildIndex.Settings)
            {
                _soundEffectSwitch = GameObject.Find("SoundEffectToggle").GetComponent<Toggle>();
                _soundEffectSwitch.isOn = Settings.SoundEffectToggle == 1 ? true : false;

                _backgroundMusicSwitch = GameObject.Find("BackgroundMusicToggle").GetComponent<Toggle>();
                _backgroundMusicSwitch.isOn = Settings.BackgroundMusicToggle == 1 ? true : false;
            }

            MasterMixer.SetFloat(DataHelper.AudioParameter.SOUND_EFFECT, Settings.SoundEffectVolume);
            MasterMixer.SetFloat(DataHelper.AudioParameter.BACKGROUND_MUSIC, Settings.BackgroundMusicVolume);
        }

        public void Reset()
        {
            PlayerPrefs.DeleteAll();
        }

        #endregion load settings

        #endregion methods
    }
}