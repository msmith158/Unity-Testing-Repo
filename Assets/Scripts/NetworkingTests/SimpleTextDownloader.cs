using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public class SimpleTextDownloader : MonoBehaviour
{
    [SerializeField] private string textAssetURL;
    [SerializeField] private TextMeshProUGUI screenText;
    [HideInInspector] public bool Success = false;
    private Coroutine downloadCoroutine;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    /*void Start()
    {
        DownloadTextAsset(textAssetURL);
    }*/

    public void DownloadTextAsset(string url)
    {
        if (downloadCoroutine == null)
        {
            downloadCoroutine = StartCoroutine(StartDownloadRequest(url));
        }
        else screenText.text += "ERROR: Image download is already in progress. Cannot start a new request.\n";
    }

    private IEnumerator StartDownloadRequest(string url)
    {
        screenText.text += $"Beginning download at {textAssetURL}...\n";
        
        // Create a request for the .txt file and send it
        UnityWebRequest request = UnityWebRequest.Get(textAssetURL);
        yield return request.SendWebRequest();
        
        // Check if the request is finished, and if so, assign it to the target
        if (request.result != UnityWebRequest.Result.Success)
        {
            screenText.text += $"TextAsset download failed. Details: {request.error}\n";
        }
        // Print the resulting text to the screen
        else
        {
            screenText.text += "TextAsset download complete. Assigning string to target...\n";
            screenText.text = request.downloadHandler.text;
            Success = true;
        }
        
        yield return null;
    }
}