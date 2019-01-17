using UnityEngine;
using UnityEngine.UI;
using System.Collections;

//TODO: FIND WAY TO GIVE REAL TIME FEEDBACK ON ANSWER CORRECTNESS

public class AnswerButtonHelper : MonoBehaviour
{
    [Header("Component")]
    public Text answerText;

    private GameController gameController;
    private AnswerData answerData;
    private RectTransform _transform;

    void Start()
    {
        gameController = FindObjectOfType<GameController>();
        _transform = GetComponent<RectTransform>();
        _transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
    }

    public void SetUp(AnswerData data)
    {
        answerData = data;
        answerText.text = answerData.answerText;
    }

    public void HandleClick()
    {
        gameController.AnswerButtonClicked(answerData.isCorrect);
    }
}