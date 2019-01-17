using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace _LetsQuiz
{
    public class FeedbackAlert : MonoBehaviour
    {
        #region variables

        [Header("Component")]
        private static FeedbackAlert _instance;
        private static Text _message;

        #endregion variables

        #region methods

        // creates the alert instance
        private static void Create()
        {
            // create instance of alert prefab as gameobject
            _instance = Instantiate(Resources.Load<FeedbackAlert>("Feedback/Alert"));

            _message = _instance.GetComponentInChildren<Text>();

            // deactivate alert
            _instance.gameObject.SetActive(false);
        }

        // used to show the alert from external sources
        // time is optional
        public static void Show(string message, float time = 2.0f)
        {
            Create();

            // set the message text
            _message.text = message;

            // after everything has been set, show the alert
            _instance.gameObject.SetActive(true);

            // start coroutine to hide alert if time is greater than zero
            if (time > 0)
                _instance.GetComponent<FeedbackAlert>().StartCoroutine(Hide(time));
        }

        // used to hide the alert from external sources if time is set to zero
        public static void Hide()
        {
            // hide the alert
            _instance.gameObject.SetActive(false);

            if (!_instance.gameObject.activeInHierarchy)
                Destroy(_instance);
        }

        // used to hide the alert from internal source is time is set to greater than zero
        private static IEnumerator Hide(float time)
        {
            // hide the alert
            yield return new WaitForSeconds(time);

            if (!_instance.gameObject.activeInHierarchy)
                Destroy(_instance);
        }

        #endregion methods
    }
}