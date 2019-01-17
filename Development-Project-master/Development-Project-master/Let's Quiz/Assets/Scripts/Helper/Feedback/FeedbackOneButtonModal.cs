using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace _LetsQuiz
{
    public class FeedbackOneButtonModal : MonoBehaviour
    {
        #region variables

        [Header("Component")]
        private static FeedbackOneButtonModal _instance;

        private static Text _heading;
        private static Text _message;
        private static Button _actionButton;
        private static Text _actionText;

        #endregion variables

        #region methods

        // creates the modal instance
        private static void Create()
        {
            // create instance of modal prefab as gameobject
            _instance = Instantiate(Resources.Load<FeedbackOneButtonModal>("Feedback/ModalOneButton"));

            // find all the required components
            _heading = GameObject.FindGameObjectWithTag("Modal_Heading").GetComponent<Text>();
            _message = GameObject.FindGameObjectWithTag("Modal_Message").GetComponent<Text>();
            _actionButton = GameObject.FindGameObjectWithTag("Modal_Action").GetComponentInChildren<Button>();
            _actionText = _actionButton.GetComponentInChildren<Text>();

            // deactivate modal
            _instance.gameObject.SetActive(false);
        }

        // used to show the modal from external sources
        // closeOnAction is optional - might wish to not close it on action
        public static void Show(string heading, string message, string action, UnityAction eventToAction, bool closeOnAction = true)
        {
            Create();

            // set the heading, message, and action text
            _heading.text = heading;
            _message.text = message;
            _actionText.text = action;

            // set the action of the modal
            _actionButton.onClick.AddListener(eventToAction);

            // set if modal will close on action
            if (closeOnAction)
                _actionButton.onClick.AddListener(Hide);

            // after everything has been set, show the modal
            _instance.gameObject.SetActive(true);
        }

        // used to hide the modal from external sources
        public static void Hide()
        {
            // hide the modal
            _instance.gameObject.SetActive(false);

            if (!_instance.gameObject.activeInHierarchy)
                Destroy(_instance);
        }

        #endregion methods
    }
}