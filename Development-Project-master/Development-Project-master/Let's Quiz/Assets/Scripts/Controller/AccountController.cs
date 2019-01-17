using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace _LetsQuiz
{
    public class AccountController : MonoBehaviour
    {
        #region variables

        [Header("Components")]
        public Text username;
        public Text email;

        #endregion variables

        #region methods

        #region unity

        private void Awake()
        {
            if (!username)
                Debug.Log("Username Input Field is null");

            if (!email)
                Debug.Log("Email Input Field is null");
        }

        private void Start()
        {
            username.text = PlayerController.Instance.GetUsername();
            email.text = PlayerController.Instance.GetEmail();
        }

        #endregion unity

        #region navigation specific

        public void BackToMenu()
        {
            FeedbackClick.Play();
            SceneManager.LoadScene(BuildIndex.Menu, LoadSceneMode.Single);
        }

        #endregion navigation specific

        #endregion methods
    }
}