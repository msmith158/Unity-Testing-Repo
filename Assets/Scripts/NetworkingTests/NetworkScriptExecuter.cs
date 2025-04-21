using System.Collections;
using System.Net;
using Mitchel.Networking;
using UnityEngine;

public class NetworkScriptExecuter : MonoBehaviour
{
    [SerializeField] private GameObject systemObject;
    [SerializeField] private string textAssetURL;
    [SerializeField] private string imageURL;
    private SimpleContentDownload contentDownloader;
    private SimpleTextDownloader textDownloader;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        contentDownloader = systemObject.GetComponent<SimpleContentDownload>();
        textDownloader = systemObject.GetComponent<SimpleTextDownloader>();
        
        StartCoroutine(ExecuteScripts());
    }

    private IEnumerator ExecuteScripts()
    {
        contentDownloader.DownloadImage(imageURL);
        yield return new WaitUntil(() => contentDownloader.Success);
        textDownloader.DownloadTextAsset(textAssetURL);
    }
}
