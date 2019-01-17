using UnityEngine;
using UnityEngine.UI;

namespace _LetsQuiz
{
    public class NotificationHelper : MonoBehaviour
    {
        [Header("Notification")]
        public InputField notifcationTitleInput;

        public InputField notifcationBodyInput;

        public void SendNotification()
        {
            var token = FirebaseController.Instance.Token;

            var header = notifcationTitleInput.text;
            var message = notifcationBodyInput.text;

            if (string.IsNullOrEmpty(header))
                header = "Notification Header";

            if (string.IsNullOrEmpty(message))
                header = "Notification Message";

            Debug.LogFormat("[{0}] SendNotification() \nToken {1}\nHeading {2} \nMessage {3}", GetType().Name, token, header, message);

            FirebaseController.Instance.CreateNotification(token, header, message);
        }

        public void SendDebugNotification()
        {
            var token = FirebaseController.Instance.Token;

            var header = notifcationTitleInput.text;
            var message = notifcationBodyInput.text;

            if (string.IsNullOrEmpty(header))
                header = "Notification Header";

            if (string.IsNullOrEmpty(message))
                header = "Notification Message";

            Debug.LogFormat("[{0}] SendDebugNotification() \nToken {1}\nHeading {2} \nMessage {3}", GetType().Name, token, header, message);

            FirebaseController.Instance.CreateDebugNotification(header, message);
        }
    }
}