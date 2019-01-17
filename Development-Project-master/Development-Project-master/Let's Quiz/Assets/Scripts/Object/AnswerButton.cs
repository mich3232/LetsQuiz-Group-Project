using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace _LetsQuiz
{
    public class AnswerButton : MonoBehaviour
    {
        #region variables

        [Header("Component")]
        public Text answerText;
        public Button answerButton;
        public Image buttonImage;

        [Header("Colour")]
        public Color original;
        public Color red;
        public Color green;

        private AnswerData answerData;
        private GameController _gameController;

        #endregion

        #region methods

        #region unity

        void Start()
        {
            _gameController = FindObjectOfType<GameController>();
            buttonImage = GetComponent<Image>();
        }

        #endregion

        #region setup

        public void SetUp(AnswerData data)
        {
            answerData = data;
            answerText.text = answerData.answerText;
        }

        #endregion

        #region validation

        public bool isCorrect(AnswerData data)
        {
            bool isCorrect = data.isCorrect;
            return isCorrect;
        }

        public void HandleAnswerClick()
        {
            if (!_gameController.clicked) //prevents players from selecting multiple answers
            {
                _gameController.clicked = true;

                if (!answerData.isCorrect)
                    changeToRed();
                else
                    changeToGreen();
            }		
        }

        #endregion

        #region colour feedback

        public void changeToGreen()
        {
            StartCoroutine("Green");
        }

        IEnumerator Green()
        {
            buttonImage.color = green; //set wrong push to red
            yield return new WaitForSeconds(1); //give people a chance to see 
            buttonImage.color = original; //change color back
            _gameController.Score(answerData.isCorrect);
        }

        public void changeToRed()
        {
            StartCoroutine("Red");
        }

        IEnumerator Red()
        {
            buttonImage.color = red; //set wrong push to red
            AnswerButton rightButton = _gameController.getCorrectAnswerButton();  //get the correct answer
            rightButton.buttonImage.color = green; //change it to green
            yield return new WaitForSeconds(1); //give people a chance to see 
            buttonImage.color = original; //change wrong answer color back 
            rightButton.buttonImage.color = original; //change correct answer color back
            _gameController.Score(answerData.isCorrect);
        }

        #endregion

        #endregion
    }
}