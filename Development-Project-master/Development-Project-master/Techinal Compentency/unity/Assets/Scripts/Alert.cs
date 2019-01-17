using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Alert : MonoBehaviour
{
    public GameObject alert;

    public void ShowAlert(string text)
    {
        Text message = alert.GetComponentInChildren<Text>();
        message.text = text;
        alert.SetActive(true);
    }

    public void HideAlert()
    {
        alert.SetActive(false);
    }
}
