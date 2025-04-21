// https://docs.unity3d.com/6000.0/Documentation/Manual/web-request-retrieving-texture.html

using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace Mitchel.Networking
{
    public class SimpleContentDownload : MonoBehaviour
    {
        [SerializeField] private string imageURL;
        [SerializeField] private Image targetImage;
        [SerializeField] private TextMeshProUGUI screenText;
        [HideInInspector] public bool Success = false;
        private float totalTime = 0;
        private Coroutine downloadCoroutine;
        private Coroutine timerCoroutine;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        /*void Start()
        {
            screenText.text = "/// NETWORK IMAGE DOWNLOAD DEMONSTRATION ///\n";
            DownloadImage(imageURL);
        }*/

        public void DownloadImage(string url)
        {
            if (downloadCoroutine == null)
            {
                downloadCoroutine = StartCoroutine(StartDownloadRequest(url));
            }
            else screenText.text += "ERROR: Image download is already in progress. Cannot start a new request.\n";
        }

        private IEnumerator StartDownloadRequest(string url)
        {
            timerCoroutine = StartCoroutine(StartTimer());
            screenText.text += $"Beginning download at {imageURL}...\n";
            // Create a texture WebRequest and send it off
            UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);
            request.SendWebRequest();

            int lastLoggedProgress = 0;
            while (!request.isDone)
            {
                // These will be used to track the download progress and report back
                float progress = request.downloadProgress;
                int percent = Mathf.FloorToInt(progress * 100f); // Turns a 0.01 float to a 1 integer

                // Only log if we hit a new 10% milestone
                if (percent >= lastLoggedProgress + 10)
                {
                    lastLoggedProgress = percent - (percent % 10); // Snap to nearest lower multiple of 10
                    screenText.text += $"{lastLoggedProgress}% downloaded...\n";
                }

                yield return null;
            }

            // Final check in case it finishes before reaching 100% in the loop
            if (lastLoggedProgress < 100)
                screenText.text += $"Downloaded 100% of image.\n";

            // Check if the request is finished, and if so, assign it to the target
            if (request.result != UnityWebRequest.Result.Success)
            {
                screenText.text += $"Image download failed. Details: {request.error}\n";
            }
            else
            {
                screenText.text += "Image download complete. Assigning image to target...\n";
                Texture2D texture = DownloadHandlerTexture.GetContent(request);
                Sprite sprite = ConvertTextureToSprite(texture);
                targetImage.sprite = sprite;

                StopCoroutine(timerCoroutine);
                screenText.text += $"Image download task completed in {totalTime} seconds.\n";
                Success = true;
            }

            downloadCoroutine = null;
        }

        #region ====================  UTILITY FUNCTIONS  ====================
        private IEnumerator StartTimer()
        {
            while (true)
            {
                totalTime += Time.deltaTime;
                yield return null;
            }
        }

        private Sprite ConvertTextureToSprite(Texture2D texture)
        {
            return Sprite.Create(
                texture,
                new Rect(0, 0, texture.width, texture.height),
                new Vector2(0.5f, 0.5f) // pivot
            );
        }
        #endregion
    }
}