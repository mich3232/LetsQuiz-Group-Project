using System.Collections;
using UnityEngine;

namespace _LetsQuiz
{
    public class Downvote : MonoBehaviour
    {
        #region variables

        private float _connectionTimer = 0.0f;
        private const float _connectionTimeLimit = 100000.0f;

        #endregion variables

        #region methods

        #region unity

        #endregion unity

        #region submit specific

        public void Dvote(QuestionData currentQuestion)
        {
            StartCoroutine(vote(currentQuestion));
        }

        private IEnumerator vote(QuestionData currentQuestion)
        {
            WWWForm form = new WWWForm();

            form.AddField("questionPost", currentQuestion.questionText);

            WWW submitRequest = new WWW(ServerHelper.Host + ServerHelper.Downvote, form);

            _connectionTimer += Time.deltaTime;

            while (!submitRequest.isDone)
            {
                if (_connectionTimer > _connectionTimeLimit)
                {
                    FeedbackAlert.Show("Server time out.");
                    Debug.LogError("SubmitScore : Submit() : " + submitRequest.error);
                    yield return null;
                }

                // extra check just to ensure a stream error doesn't come up
                if (_connectionTimer > _connectionTimeLimit || submitRequest.error != null)
                {
                    FeedbackAlert.Show("Server error.");
                    Debug.LogError("SubmitScore : Submit() : " + submitRequest.error);
                    yield return null;
                }
            }

            if (submitRequest.error != null)
            {
                FeedbackAlert.Show("Connection error. Please try again.");
                Debug.Log("SubmitScore : Submit() : " + submitRequest.error);
                yield return null;
            }

            if (submitRequest.isDone)
            {
                Debug.Log("Downvote submitted");
                yield return submitRequest;
                DestroyObject(gameObject);
            }
        }

        #endregion submit specific

        #endregion methods
    }
}