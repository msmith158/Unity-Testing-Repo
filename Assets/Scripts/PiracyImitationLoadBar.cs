using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PiracyImitationLoadBar : MonoBehaviour
{
    [SerializeField] private Scrollbar m_scrollbar;
    [SerializeField] private Image m_scrollbarHandle;
    [SerializeField] private Button m_downloadButton;
    [SerializeField] private AnimationCurve m_loadSpeed;

    private void Start()
    {
        m_scrollbarHandle.enabled = false;
    }
    
    // Update is called once per frame
    public void BeginLoading(float loadTime)
    {
        StartCoroutine(DoLoading(loadTime));
    }

    private IEnumerator DoLoading(float loadingTime)
    {
        m_scrollbarHandle.enabled = true;
        m_downloadButton.interactable = false;
        float timeElapsed = 0;

        while (timeElapsed < loadingTime)
        {
            m_scrollbar.size = Mathf.Lerp(0, 1, timeElapsed / loadingTime);
            timeElapsed += Time.deltaTime;

            yield return null;
        }
        m_scrollbar.size = 1;
    }
}