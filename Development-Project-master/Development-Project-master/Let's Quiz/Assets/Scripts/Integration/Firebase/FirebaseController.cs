using Firebase.Messaging;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace _LetsQuiz
{
    public class FirebaseController : Singleton<FirebaseController>
    {
        #region variables

        [Header("Components")]
        [SerializeField] private Text _tokenText;

        private float _connectionTimer = 0.0f;
        private const float _connectionTimeLimit = 10000.0f;

        #endregion variables

        #region properties

        public string Token { get; private set; }

        public string Header { get; private set; }

        public string Message { get; private set; }

        #endregion properties

        #region methods

        #region unity

        protected override void OnEnable()
        {
            base.OnEnable();
            DontDestroyOnLoad(gameObject);
        }

        private void Awake()
        {
            FirebaseMessaging.TokenReceived += OnTokenReceived;
            FirebaseMessaging.MessageReceived += OnMessageReceived;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            FirebaseMessaging.TokenReceived -= OnTokenReceived;
            FirebaseMessaging.MessageReceived -= OnMessageReceived;
        }

        #endregion unity

        #region events

        private void OnTokenReceived(object sender, TokenReceivedEventArgs e)
        {
            Debug.Log("[FirebaseController] OnTokenRecieved() Token : " + e.Token);

            FirebaseMessaging.SubscribeAsync(Token);
            FirebaseMessaging.SubscribeAsync("/topics/all");

            Token = e.Token;

            if (!string.IsNullOrEmpty(Token))
                _tokenText.text = Token;
        }

        private void OnMessageReceived(object sender, MessageReceivedEventArgs e)
        {
            Debug.Log("[FirebaseController] OnMessageReceived() Notification received from : " + e.Message.From + " with title : " + e.Message.Notification.Title + " with messgae : " + e.Message.Notification.Body);

            Header = e.Message.Notification.Title;
            Message = e.Message.Notification.Body;
        }

        #endregion events

        #region notification

        public void CreateNotification(string token, string header, string message)
        {
            Debug.LogFormat("[{0}] CreateNotification() : \nToken {1}\nHeading {2}\nMessage {3}", GetType().Name, token, header, message);

            if (!string.IsNullOrEmpty(token) && !string.IsNullOrEmpty(header) && !string.IsNullOrEmpty(message))
                StartCoroutine(SendNotification(token, header, message));
        }

        private IEnumerator SendNotification(string token, string header, string message)
        {
            Debug.LogFormat("[{0}] SendNotification() : \nToken {1}\nHeading {2}\nMessage {3}", GetType().Name, token, header, message);

            WWWForm form = new WWWForm();

            form.AddField("recipient", token);
            form.AddField("title", header);
            form.AddField("body", message);

            WWW notificationRequest = new WWW(ServerHelper.Host + ServerHelper.SendNotification, form);

            _connectionTimer += Time.deltaTime;

            while (!notificationRequest.isDone)
            {
                if (_connectionTimer > _connectionTimeLimit)
                {
                    Debug.LogError("[FirebaseController] SendNotification() : " + notificationRequest.error);
                    yield return null;
                }
                else if (notificationRequest.error != null)
                {
                    Debug.Log("[FirebaseController] SendNotification() : " + notificationRequest.error);
                    yield return null;
                }
                // extra check just to ensure a stream error doesn't come up
                else if (_connectionTimer > _connectionTimeLimit && notificationRequest.error != null)
                {
                    Debug.LogError("[FirebaseController] SendNotification() : " + notificationRequest.error);
                    yield return null;
                }
            }

            if (notificationRequest.isDone && notificationRequest.error != null)
            {
                Debug.Log("[FirebaseController] SendNotification() : " + notificationRequest.error);
                yield return null;
            }

            if (notificationRequest.isDone)
            {
                // check that the notification request returned something
                if (!string.IsNullOrEmpty(notificationRequest.text))
                {
                    Debug.Log("[FirebaseController] SendNotification() : " + notificationRequest.text);
                    yield return notificationRequest;
                }
            }
        }

        #endregion notification

        #region notification - debug

        public void CreateDebugNotification(string header, string message)
        {
            if (!string.IsNullOrEmpty(header) && !string.IsNullOrEmpty(message))
                StartCoroutine(SendDebugNotification(header, message));
        }

        private IEnumerator SendDebugNotification(string header, string message)
        {
            WWWForm form = new WWWForm();

            form.AddField("title", header);
            form.AddField("body", message);

            WWW notificationRequest = new WWW(ServerHelper.Host + ServerHelper.SendDebugNotification, form);

            _connectionTimer += Time.deltaTime;

            while (!notificationRequest.isDone)
            {
                if (_connectionTimer > _connectionTimeLimit)
                {
                    Debug.LogError("[FirebaseController] SendNotification() : " + notificationRequest.error);
                    yield return null;
                }
                else if (notificationRequest.error != null)
                {
                    Debug.Log("[FirebaseController] SendNotification() : " + notificationRequest.error);
                    yield return null;
                }
                // extra check just to ensure a stream error doesn't come up
                else if (_connectionTimer > _connectionTimeLimit && notificationRequest.error != null)
                {
                    Debug.LogError("[FirebaseController] SendNotification() : " + notificationRequest.error);
                    yield return null;
                }
            }

            if (notificationRequest.isDone && notificationRequest.error != null)
            {
                Debug.Log("[FirebaseController] SendNotification() : " + notificationRequest.error);
                yield return null;
            }

            if (notificationRequest.isDone)
            {
                // check that the notification request returned something
                if (!string.IsNullOrEmpty(notificationRequest.text))
                {
                    Debug.Log("[FirebaseController] SendNotification() : " + notificationRequest.text);
                    yield return notificationRequest;
                }
            }
        }

        #endregion notification - debug

        #endregion methods
    }
}