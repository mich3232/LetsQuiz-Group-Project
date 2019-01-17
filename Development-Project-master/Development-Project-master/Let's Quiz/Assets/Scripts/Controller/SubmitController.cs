using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace _LetsQuiz
{
    public class SubmitController : MonoBehaviour
    {
        #region variables

        [Header("Components")]
        public InputField questionInput;
        public InputField correctInput;
        public InputField wrong1Input;
        public InputField wrong2Input;
        public InputField wrong3Input;
        public Dropdown categorySelection;

        [SerializeField] private Text _questionText;
        [SerializeField] private Text _correctAnswerText;
        [SerializeField] private Text _wrongAnswer1Text;
        [SerializeField] private Text _wrongAnswer2Text;
        [SerializeField] private Text _wrongAnswer3Text;

        [Header("Connection")]
        public float connectionTimer = 0;
        public const float connectionTimeLimit = 10000.0f;

        #endregion variables

        #region methods

        #region unity

        private void Awake()
        {
            if (PlayerController.Initialised)
                PlayerController.Instance.Load();

            List<string> categories = new List<string>();

            if (QuestionController.Initialised)
                categories = QuestionController.Instance.GetAllCategories();

            categorySelection.AddOptions(categories);
        }

        #endregion unity

        #region submit specific

        public void Submit()
        {
            FeedbackClick.Play();

            var question = questionInput.text;
            var correctAnswer = correctInput.text;
            var wrong1Answer = wrong1Input.text;
            var wrong2Answer = wrong2Input.text;
            var wrong3Answer = wrong3Input.text;
            var category = categorySelection.options[categorySelection.value].text;

            if (string.IsNullOrEmpty(question))
                FeedbackAlert.Show("Question cannot be empty.");

            if (string.IsNullOrEmpty(correctAnswer))
                FeedbackAlert.Show("Correct Answer cannot be empty.");

            if (string.IsNullOrEmpty(wrong1Answer))
                FeedbackAlert.Show("Wrong Answer 1 cannot be empty.");

            if (string.IsNullOrEmpty(wrong2Answer))
                FeedbackAlert.Show("Wrong Answer 2 cannot be empty.");

            if (string.IsNullOrEmpty(wrong3Answer))
                FeedbackAlert.Show("Wrong Answer 3 cannot be empty.");

            if (string.IsNullOrEmpty(category))
                FeedbackAlert.Show("Category cannot be empty.");

            if (!string.IsNullOrEmpty(question) && !string.IsNullOrEmpty(correctAnswer) && !string.IsNullOrEmpty(wrong1Answer) && !string.IsNullOrEmpty(wrong2Answer) && !string.IsNullOrEmpty(wrong3Answer) && !string.IsNullOrEmpty(category))
            {
                if (ValidSubmission(question, correctAnswer, wrong1Answer, wrong2Answer, wrong3Answer, category))
                {
                    FeedbackAlert.Show("Submission successful", 1.0f);

                    // clears input on sucessful submission
                    questionInput.text = string.Empty;
                    _questionText.text = string.Empty;

                    correctInput.text = string.Empty;
                    _correctAnswerText.text = string.Empty;

                    wrong1Input.text = string.Empty;
                    _wrongAnswer1Text.text = string.Empty;

                    wrong2Input.text = string.Empty;
                    _wrongAnswer2Text.text = string.Empty;

                    wrong3Input.text = string.Empty;
                    _wrongAnswer3Text.text = string.Empty;

                    categorySelection.value = 6;
                }
                else
                {
                    FeedbackAlert.Show("Submission unsuccesful", 1.0f);
                }
            }

            FeedbackAlert.Hide();
        }

        private bool ValidSubmission(string question, string correctAnswer, string wrong1Answer, string wrong2Answer, string wrong3Answer, string category)
        {
            WWWForm form = new WWWForm();

            form.AddField("questionText", question);
            form.AddField("correctAnswer", correctAnswer);
            form.AddField("wrong1", wrong1Answer);
            form.AddField("wrong2", wrong2Answer);
            form.AddField("wrong3", wrong3Answer);
            form.AddField("catagory", category);

            bool complete = false;

            WWW submitQuestion = new WWW(ServerHelper.Host + ServerHelper.SubmitUserQuestion, form);

            connectionTimer += Time.deltaTime;

            while (!submitQuestion.isDone)
            {
                if (connectionTimer > connectionTimeLimit)
                {
                    FeedbackAlert.Show("Server time out.");
                    Debug.LogErrorFormat("[{0}] ValidSubmission() : {1}", GetType().Name, submitQuestion.error);
                    Debug.Log(submitQuestion.text);
                    complete = false;
                }

                // extra check just to ensure a stream error doesn't come up
                if (connectionTimer > connectionTimeLimit || submitQuestion.error != null)
                {
                    FeedbackAlert.Show("Server time out.");
                    Debug.LogErrorFormat("[{0}] ValidSubmission() : {1}", GetType().Name, submitQuestion.error);
                    Debug.Log(submitQuestion.text);
                    complete = false;
                }
            }

            if (submitQuestion.error != null)
            {
                FeedbackAlert.Show("Connection error. Please try again.");
                Debug.LogErrorFormat("[{0}] ValidSubmission() : {1}", GetType().Name, submitQuestion.error);
                complete = false;
            }

            if (submitQuestion.isDone)
            {
                if (!string.IsNullOrEmpty(submitQuestion.text))
                {
                    Debug.LogFormat("[{0}] ValidSubmission() : Submission {1}", GetType().Name, submitQuestion.text);
                    complete = true;
                }
                else
                    complete = false;
            }

            return complete;
        }

        public void CategorySelected()
        {
            string category = categorySelection.options[categorySelection.value].text;
        }

        #endregion submit specific

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