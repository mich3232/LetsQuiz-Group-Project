using UnityEngine;
using UnityEngine.UI;

namespace _LetsQuiz
{
    public class LeaderboardEntry : MonoBehaviour
    {
        public Text usernameText;
        public Text scoreText;

        private void Start()
        {
            transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        }

        public void SetUp(string username, string score)
        {
            usernameText.text = username;
            scoreText.text = score;
            Debug.Log("[LeaderboardEntry] SetUp() : User: " + usernameText.text + " Score: " + scoreText.text);
        }
    }
}