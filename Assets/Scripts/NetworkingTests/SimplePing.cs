// https://discussions.unity.com/t/how-to-ping-specific-an-address/585354/4
// https://docs.unity3d.com/6000.0/Documentation/ScriptReference/Ping.html

using System.Collections;
using TMPro;
using UnityEngine;

namespace Mitchel.Networking
{
    public class SimplePing : MonoBehaviour
    {
        [SerializeField] private string ip;
        [SerializeField] private int attemptAmount = 4;
        [SerializeField] private float timeoutTime = 2f;
        [SerializeField] private TextMeshProUGUI screenText;
        private int iteration = 0;
        private float totalTime = 0;

        // Start is called before the first frame update
        void Start()
        {
            BeginPing();
        }

        private void BeginPing()
        {
            StartCoroutine(TimedPing());
        }

        private IEnumerator TimedPing()
        {
            float timeElapsed = 0;

            if (iteration < attemptAmount)
            {
                screenText.text += $"Attempting to send {attemptAmount} packets to {ip}...\n";

                Ping pingJob = new Ping(ip);
                while ((!pingJob.isDone || pingJob.time == -1) && timeElapsed < timeoutTime)
                {
                    Debug.Log(pingJob.time);
                    timeElapsed += Time.deltaTime;
                    totalTime += Time.deltaTime;
                    yield return null;
                }

                if (timeElapsed < timeoutTime)
                {
                    screenText.text += $"Request completed after {timeElapsed} seconds.\n";
                }
                else if (timeElapsed >= timeoutTime)
                {
                    screenText.text += "Request timed out.\n";
                }
                iteration++;
                BeginPing();
            }
            else
            {
                screenText.text += $"\nPacket job for {ip} completed after a total of {totalTime} seconds.\n";
                totalTime = 0;
            }
        }
    }
}