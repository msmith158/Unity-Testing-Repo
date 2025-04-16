// https://discussions.unity.com/t/how-to-ping-specific-an-address/585354/4
// https://docs.unity3d.com/6000.0/Documentation/ScriptReference/Ping.html

using System.Collections;
using TMPro;
using UnityEngine;

public class SimplePing : MonoBehaviour
{
    [SerializeField] private string ip;
    [SerializeField] private int attemptAmount = 4;
    [SerializeField] private float timeoutTime = 2f;
    [SerializeField] private TextMeshProUGUI screenText;
    private int iteration = 0;
    
    // Start is called before the first frame update
    void Start()
    {
        BeginPing();
    }

    private void BeginPing()
    {
        screenText.text += $"Attempting to ping {ip}...";
        StartCoroutine(TimedPing());
    }

    private IEnumerator TimedPing()
    {
        float timeElapsed = 0;
        
        while (iteration <= attemptAmount)
        {
            Ping pingJob = new Ping(ip);
            while (!pingJob.isDone || timeElapsed < timeoutTime)
            {
                timeElapsed += Time.deltaTime;
                yield return null;
            }
        }
    }
}
