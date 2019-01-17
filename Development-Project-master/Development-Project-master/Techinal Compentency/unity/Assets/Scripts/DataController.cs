using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DataController : MonoBehaviour
{
    [Header("Component")]
    [SerializeField]
    [Tooltip("Component used to display downloaded data.")]
    private Text _dataDisplay;
    [SerializeField]
    [Tooltip("Component used by user to input new data.")]
    private InputField _dataInput;
    [SerializeField]
    private ScrollRect _scrollView;

    [Header("Server")]
    [SerializeField]
    private string _hostUrl = "http://charnesnell.000webhostapp.com/letsquiz/";
    [SerializeField]
    private string _uploadFileName = "upload.php?data=";
    [SerializeField]
    private string _downloadFileName = "download.php";
    [SerializeField]
    private string _deleteFileName = "delete.php";

    [Header("Setting")]
    private float _uploadTimer;
    private float _uploadTimeLimit = 1000000.0f;
    private float _downloadTimer;
    private float _downloadTimeLimit = 30.0f;
    private float _deleteTimer;
    private float _deleteTimeLimit = 50000.0f;
    private bool _failed = false;

    private Alert _alert;

    private void Awake()
    {
        _scrollView.horizontal = false;
        _scrollView.vertical = false;
        _dataDisplay.alignment = TextAnchor.MiddleCenter;
        _alert = gameObject.GetComponent<Alert>();
        _alert.alert.SetActive(false);
    }

    public void Upload()
    {
        var data = _dataInput.text;

        if (string.IsNullOrEmpty(data))
        {
            _dataDisplay.text = "Error : Cannot upload nothing.";
            return;
        }
         
        var uploadUrl = _hostUrl + _uploadFileName + data;
        _uploadTimer = 0.0f;
        StartCoroutine(UploadData(uploadUrl));
        _scrollView.vertical = true;
        _dataInput.text = "";
    }

    private IEnumerator UploadData(string url)
    {
        WWW upload = new WWW(url);

        while (!upload.isDone)
        {
            _uploadTimer += Time.deltaTime;
            _alert.ShowAlert("Uploading...");
            if (_uploadTimer > _uploadTimeLimit)
            {
                _dataDisplay.text = "Error: Server time out.";
                _failed = true;
                break;
            }
            yield return null;
        }
        if (!upload.isDone || upload.error != null)
        {
            _dataDisplay.alignment = TextAnchor.MiddleCenter;
            _dataDisplay.text = "Error: " + upload.error;
        }
        else
        {
            _alert.HideAlert();
            if (!_failed)
            {
                _dataDisplay.text = upload.text;
                Download();
                yield return upload;
            }
        }
    }

    public void Download()
    {
        var downloadUrl = _hostUrl + _downloadFileName;
        _dataInput.text = "";
        _dataDisplay.text = "";
        _downloadTimer = 0.0f;
        StartCoroutine(DownloadData(downloadUrl));
        _scrollView.vertical = true;
    }

    private IEnumerator DownloadData(string url)
    {
        WWW download = new WWW(url);

        while (!download.isDone)
        {
            _downloadTimer += Time.deltaTime;
            _alert.ShowAlert("Downloading...");
            if (_downloadTimer > _downloadTimeLimit)
            {
                _dataDisplay.text = "Error: Server time out.";
                _failed = true;
                break;
            }
            yield return null;
        }
        if (!download.isDone || download.error != null)
        {
            _dataDisplay.alignment = TextAnchor.MiddleCenter;
            _dataDisplay.text = "Error: " + download.error;
        }
        else
        {
            _alert.HideAlert();
            if (!_failed)
            {
                _dataDisplay.alignment = TextAnchor.UpperLeft;
                _dataDisplay.text = download.text;
                yield return download;
            }
        }
    }

    public void Delete()
    {
        var deleteUrl = _hostUrl + _deleteFileName;
        _deleteTimer = 0.0f;
        StartCoroutine(DeleteData(deleteUrl));
    }

    private IEnumerator DeleteData(string url)
    {
        WWW delete = new WWW(url);

        while (!delete.isDone)
        {
            _deleteTimer += Time.deltaTime;
            _alert.ShowAlert("Deleting...");
            if (_deleteTimer > _deleteTimeLimit)
            {
                _dataDisplay.text = "Error: Server time out.";
                break;
            }
            yield return null;
        }
        if (!delete.isDone || delete.error != null)
        {
            _dataDisplay.alignment = TextAnchor.MiddleCenter;
            _dataDisplay.text = "Error: " + delete.error;
        }
        else
        {
            _alert.HideAlert();
            _dataDisplay.text = delete.text;
            yield return delete;
        }
    }
}