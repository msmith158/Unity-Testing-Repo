using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ScalingTestHudSystem : MonoBehaviour
{
    #region VARIABLES
    // Select which mode of HUD icon transforming with this enum
    [SerializeField] private enum elementTransformModeEnum { Absolute, Relative };
    [SerializeField] private elementTransformModeEnum elementTransformMode = elementTransformModeEnum.Relative;

    [Header("General HUD Settings")]
    [SerializeField] private hudStateEnum hudState;
    [SerializeField] private AnimationCurve hudSummonCurve;
    [SerializeField] private AnimationCurve hudDismissCurve;
    [SerializeField] private enum hudStateEnum { Shown, Hidden };

    [Header("Absolute Scale Settings")]
    [Tooltip("This will transform UI elements via localPosition using an absolute pixel value")] [SerializeField] private float hudChangeAbsAmount = 200f;

    [Header("Relative Scale Settings")]
    [Tooltip("This will transform UI elements via localPosition by multiplying the height of the canvas")] [SerializeField] private float hudChangeRelAmount = 0.2f;

    [Header("Object References")]
    [SerializeField] private Canvas canvas;
    [SerializeField] private RectTransform playerStatsRow;
    [SerializeField] private RectTransform inventoryRow;
    [SerializeField] private Image statsRowVignette;
    [SerializeField] private Image inventoryRowVignette;

    // Internal variables
    private Coroutine hudTransitionCoroutine;
    private Color statsVignetteOpaque;
    private Color statsVignetteTransparent;
    private Color inventoryVignetteOpaque;
    private Color inventoryVignetteTransparent;
    private Vector3 statsRowShownPos;
    private Vector3 statsRowHiddenPos;
    private Vector3 inventoryRowShownPos;
    private Vector3 inventoryRowHiddenPos;
    private float canvasHeight;
    #endregion

    #region SINGLETON INITIALISATION
    private static ScalingTestHudSystem instance;
    private void Awake()
    {
        if (instance != null)
            Destroy(instance);

        instance = this;
    }
    public static ScalingTestHudSystem Instance => instance;
    #endregion

    private void Start()
    {
        canvasHeight = canvas.GetComponent<RectTransform>().rect.height;

        statsVignetteOpaque = statsRowVignette.color;
        statsVignetteTransparent = statsVignetteOpaque;
        statsVignetteTransparent.a = 0;

        inventoryVignetteOpaque = inventoryRowVignette.color;
        inventoryVignetteTransparent = inventoryVignetteOpaque;
        inventoryVignetteTransparent.a = 0;

        statsRowShownPos = playerStatsRow.localPosition;
        inventoryRowShownPos = inventoryRow.localPosition;
        statsRowHiddenPos = playerStatsRow.localPosition;
        inventoryRowHiddenPos = inventoryRow.localPosition;

        switch (elementTransformMode)
        {
            case elementTransformModeEnum.Absolute:
                statsRowHiddenPos.y += hudChangeAbsAmount;
                inventoryRowHiddenPos.y -= hudChangeAbsAmount;
                break;
            case elementTransformModeEnum.Relative:
                statsRowHiddenPos.y += hudChangeRelAmount * canvasHeight;
                inventoryRowHiddenPos.y -= hudChangeRelAmount * canvasHeight;
                break;
        }

        switch (hudState)
        {
            case hudStateEnum.Hidden:
                playerStatsRow.localPosition = statsRowHiddenPos;
                inventoryRow.localPosition = inventoryRowHiddenPos;
                break;
            case hudStateEnum.Shown:
                playerStatsRow.localPosition = statsRowShownPos;
                inventoryRow.localPosition = inventoryRowShownPos;
                break;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            switch (hudState)
            {
                case hudStateEnum.Hidden:
                    if (hudTransitionCoroutine == null)
                    {
                        hudTransitionCoroutine = StartCoroutine(TransitionHudIn());
                        hudState = hudStateEnum.Shown;
                    }
                    break;
                case hudStateEnum.Shown:
                    if (hudTransitionCoroutine == null)
                    {
                        hudTransitionCoroutine = StartCoroutine(TransitionHudOut());
                        hudState = hudStateEnum.Hidden;
                    }
                    break;
            }
        }
    }

    private IEnumerator TransitionHudIn()
    {
        // Set the times to lerp between
        float timeElapsed = 0;
        float endTime = hudSummonCurve.keys[hudSummonCurve.length - 1].time;

        while (timeElapsed < endTime)
        {
            playerStatsRow.localPosition = Vector3.Lerp(statsRowHiddenPos, statsRowShownPos, hudSummonCurve.Evaluate(timeElapsed));
            inventoryRow.localPosition = Vector3.Lerp(inventoryRowHiddenPos, inventoryRowShownPos, hudSummonCurve.Evaluate(timeElapsed));

            statsRowVignette.color = Color.Lerp(statsVignetteTransparent, statsVignetteOpaque, timeElapsed / endTime);
            inventoryRowVignette.color = Color.Lerp(inventoryVignetteTransparent, inventoryVignetteOpaque, timeElapsed / endTime);

            timeElapsed += Time.deltaTime;
            yield return null;
        }

        playerStatsRow.localPosition = statsRowShownPos;
        inventoryRow.localPosition = inventoryRowShownPos;

        statsRowVignette.color = statsVignetteOpaque;
        inventoryRowVignette.color = inventoryVignetteOpaque;

        hudTransitionCoroutine = null;
    }

    private IEnumerator TransitionHudOut()
    {
        // Set the times to lerp between
        float timeElapsed = 0;
        float endTime = hudDismissCurve.keys[hudDismissCurve.length - 1].time;

        while (timeElapsed < endTime)
        {
            playerStatsRow.localPosition = Vector3.Lerp(statsRowShownPos, statsRowHiddenPos, hudDismissCurve.Evaluate(timeElapsed));
            inventoryRow.localPosition = Vector3.Lerp(inventoryRowShownPos, inventoryRowHiddenPos, hudDismissCurve.Evaluate(timeElapsed));

            statsRowVignette.color = Color.Lerp(statsVignetteOpaque, statsVignetteTransparent, timeElapsed / endTime);
            inventoryRowVignette.color = Color.Lerp(inventoryVignetteOpaque, inventoryVignetteTransparent, timeElapsed / endTime);

            timeElapsed += Time.deltaTime;
            yield return null;
        }

        playerStatsRow.localPosition = statsRowHiddenPos;
        inventoryRow.localPosition = inventoryRowHiddenPos;

        statsRowVignette.color = statsVignetteTransparent;
        inventoryRowVignette.color = inventoryVignetteTransparent;

        hudTransitionCoroutine = null;
    }
}