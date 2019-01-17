using UnityEngine;

namespace _LetsQuiz
{
    public class DataHelper
    {
        public class AudioParameter
        {
            public const int MUTED_VOLUME = -80;
            public const int UNMUTED_VOLUME = -10;
            public const string SOUND_EFFECT = "SoundEffectVolume";
            public const string BACKGROUND_MUSIC = "BackgroundMusicVolume";
        }

        public class File
        {
            public static string SAVE_LOCATION = Application.persistentDataPath + "/save.json";
        }

        public class PlayerDataKey
        {
			public const string TYPE = "PlayerType";
            public const string ID = "PlayerId";
            public const string USERNAME = "PlayerUsername";
			public const string GAMEKEY = "GameKey";
            public const string EMAIL = "PlayerEmail";
            public const string PASSWORD = "PlayerPassword";
            public const string DOB = "PlayerDOB";
            public const string QUESTIONS_SUBMITTED = "PlayerQuestionsSubmitted";
            public const string QUESTIONS_SUBMITTED_NUMBER = "PlayerNumberQuestionsSubmitted";
            public const string GAMES_PLAYED_NUMBER = "PlayerNumberGamesPlayed";
            public const string HIGHEST_SCORE = "PlayerHighestScore";
            public const string CORRECT_ANSWERS_NUMBER = "PlayerNumberCorrectAnswers";
            public const string TOTAL_ANSWERS_NUMBER = "PlayerTotalQuestionsAnswered";
            public const string QUESTION_DATA = "PlayerQuestionData";
            public const string ONGOING_GAMES = "OngoingGamesKey";
        }

        public class PlayerSettingsKey
        {
            public const string EFFECT_VOLUME = "SoundEffectVolume";
            public const string EFFECT_TOGGLE = "SoundEffectToggle";
            public const string MUSIC_VOLUME = "BackgroundMusicVolume";
            public const string MUSIC_TOGGLE = "BackgroundMusicToggle";
            public const string NOTIFICATION_TOGGLE = "NotificationToggle";
        }
    }
}