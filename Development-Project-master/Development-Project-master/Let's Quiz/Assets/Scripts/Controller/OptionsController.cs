using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class OptionsController : MonoBehaviour
{
    [Header("Component")]
    public GameObject status;
    public Text statusMessageDisplay;

    [Header("Setting")]
    public int menuSceneIndex = 1;
    public float hideSeconds = 1.0f;

    private DataController _dataController;

    private void Start()
    {
        _dataController = FindObjectOfType<DataController>();
        status.SetActive(false);
    }

    public void ClearHighscore()
    {
        PlayerPrefs.DeleteKey(_dataController.scoreKey);

        ShowStatusMessage();
    }

    public void ButtonAction()
    {
        Debug.Log("OptionsController : ButtonAction()");
    }

    public void ReturnToMenu()
    {
        SceneManager.LoadScene(menuSceneIndex, LoadSceneMode.Single);
    }

    private void ShowStatusMessage()
    {
        status.SetActive(true);

        if (!PlayerPrefs.HasKey(_dataController.scoreKey))
            statusMessageDisplay.text = "Reset successful.";
        else
            statusMessageDisplay.text = "Reset unsuccessful.";
        
        StartCoroutine(HideStatusMessage());
    }

    private IEnumerator HideStatusMessage()
    {
        yield return new WaitForSeconds(hideSeconds);
        status.SetActive(false);
    }
}
