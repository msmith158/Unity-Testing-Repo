//  This is a prototype for the dialogue system, intended to be used in the Cleanup Krue major project.
//  This specific model is designed to include all possible features, with the intention of being gutted to only include necessary features once implemented.
//  To use this dialogue system in scripting, you must ready the following variables (make sure you clear these variables first with "var.Clear()" or "var = null":
//
//  1. dialogueCharacterNames[]: Put all character names to appear in the dialogueCharacterNames string array, in sequence.
//  2. dialogueLines[]: Put all the dialogue lines in the dialogueLines string array, in sequence.
//  3. charDelayTimes[]: If you want specific timing for each dialogue line, set the values in the charDelayTimes float array, in sequence. Otherwise, make sure to set the
//     defaultCharDelayTime value in the Inspector.
//  4. dialogueLineSfx[]: If you want to set the sound effect for each dialogue line, set the values in the dialogueLineSfx AudioClip array, in sequence. Otherwise, make sure to set
//     the defaultDialogueSfx value in the Inspector.
//  5. If you want to set an effect for each dialogue line, set the values in the dialogueTextEffect enum array, in sequence. Otherwise, it will default to none.
//  6. PanelEffect is WIP.
//  7. TransitionEffect is WIP.

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Image = UnityEngine.UI.Image;
using Random = UnityEngine.Random;

namespace Mitchel.UISystems
{
    public class DialogueSystem : MonoBehaviour
    {
        [Header("Dialogue Values")]
        [SerializeField] private List<DialogueData> dialogueData;
        [SerializeField] private PanelTransition panelTransitionEffect;

        [Header("Dialogue Settings: General")] 
        [SerializeField][Tooltip("By default, the dialogue SFX is played when each character is printed. This sets the SFX onto an individualised timer that runs until the dialogue stops printing.\n\nThis can be handy for higher timing values that can cause the SFX to play too fast and produce an unwanted result.")] private bool isFixedDialogueSfxTiming;
        [SerializeField] private float fixedDialogueSfxTiming;
        [SerializeField] private AudioClip dialogueAdvanceSfx;
        [SerializeField] private bool pauseAtFullStop;
        [SerializeField] private float fullStopPauseTime;

        [Header("Dialogue Settings: Effects")]
        [SerializeField] private bool charBasedWave = false;
        [SerializeField] private float waveSpeed = 2f;
        [SerializeField] private float waveExponent = 10f;
        [SerializeField] private float waveIntensity = 0.01f;
        [SerializeField] private float shakeMagnitude;
        [SerializeField] private bool charBasedRipple = false;
        [SerializeField] private float rippleSpeed = 2f;
        [SerializeField] private float rippleExponent = 10f;
        [SerializeField] private float rippleIntensity = 0.01f;

        [Header("Panel Settings: Effects")]
        [SerializeField] private float panelFadeTime = 0.3f;
        [SerializeField] private float panelSlideTime = 0.5f;
        [SerializeField][Tooltip("This value is to ADD/SUBTRACT from the panel's current position, so it is not expecting an explicit position on the screen")] private Vector3 panelSlideInFromPos;
        [SerializeField][Tooltip("This value is to ADD/SUBTRACT from the panel's current position, so it is not expecting an explicit position on the screen")] private Vector3 panelSlideOutToPos;
        [SerializeField][Tooltip("Use this for a custom slide curve, e.g. to smooth the slide. Note that the total time is determined by the length of the AnimationCurve, so panelSlideTime and separateFadeAndSlideTime is ignored.")] private bool useSlideCurve = false;
        [SerializeField] private AnimationCurve slideCurve;
        [SerializeField][Tooltip("Should the panel slide out when it closes? If not, it will just utilise panelFadeTime and fade out.")] private bool slideOutOnExit = false;
        [SerializeField][Tooltip("By default, the fade and slide time is combined. This separates it to use both panelFadeTime and panelSlideTime.")] private bool separateFadeAndSlideTime = false;

        [Header("Misc Settings")]
        [SerializeField] private float globalDelayTime;

        [Header("Object References")]
        [SerializeField] private GameObject dialoguePanel;
        [SerializeField] private TextMeshProUGUI characterNameText;
        [SerializeField] private TextMeshProUGUI dialogueText;
        [SerializeField] private AudioSource dialogueSfxSource;
        public Sprite keyboardPromptButton;
        public Sprite controllerPromptButton;
        public Image promptButton;

        private bool isPrinting = false;
        private int iteration = 0;
        private bool effectRunning = false;
        private bool skipCheck = false;
        private bool dialogueDisengaged = false;
        private Vector3 lastMousePosition;
        private InputMode currentInputMode;
        
        private enum InputMode { Controller, Keyboard }
        public enum PanelEffect { None, Shake }
        private enum PanelTransition { None, Fade, FadeAndSlide, FadeAndZoom }
        public enum TextEffect { None, Wavy, Ripple, Shaky }
        private enum TextTransition { None, FadeIn, FadeAndSlideIn }

        [System.Serializable]
        public class DialogueData
        {
            [Tooltip("This is for listing the name of the characters that are talking per dialogue line. Make sure this is in sequence with the order of the dialogue lines.\n\nTip: inserting a blank string will print nothing, which can be handy for displaying system messages through the dialogue system, e.g. in-game tips.")] public string characterName;
            [TextArea]
            [Tooltip("This is for each line of dialogue in this one dialogue event.")] public string dialogueLine;
            [Space(10)]
            [Tooltip("This is for the timing of each dialogue line. Example: for fast, set 0.02f; for slow, set 0.2f.\n\nMake sure this is in sequence with the order of the dialogue lines. If list is empty, system will use default timing.")] public float charDelayTime;
            [Tooltip("This is for what will play when each character of a dialogue line is being printed out.\n\nMake sure this is in sequence with the order of the dialogue lines.")] public AudioClip dialogueLineSfx;
            public TextEffect dialogueTextEffect;
            public PanelEffect dialogueBoxEffect;
        }
        // Start is called before the first frame update
        void Start()
        {
            StartCoroutine(GlobalDelay());
        }

        private void Update()
        {
            if (!isPrinting)
            {
                if (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Joystick1Button0))
                {
                    dialogueSfxSource.PlayOneShot(dialogueAdvanceSfx);
                    if (iteration < dialogueData.Count)
                    {
                        StartCoroutine(PrintDialogue(dialogueData[iteration].characterName, dialogueData[iteration].dialogueLine));
                    }
                    else
                    {
                        if (!dialogueDisengaged) StartCoroutine(BeginPanelTransition(panelTransitionEffect, false));
                    }
                }
            }
            else if (isPrinting)
            {
                if (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Joystick1Button0))
                {
                    skipCheck = true;
                }
            }
            
            if ((Input.anyKeyDown || Input.mousePosition != lastMousePosition))
            {
                ChangeGlyphs(0);
                lastMousePosition = Input.mousePosition;
            }
        }

        private IEnumerator GlobalDelay()
        {
            yield return new WaitForSeconds(globalDelayTime);
            StartCoroutine(BeginPanelTransition(panelTransitionEffect, true));
        }

        // ### PANEL TRANSITION FUNCTION ###
        private IEnumerator BeginPanelTransition(PanelTransition panelTransition, bool enable)
        {
            dialogueText.text = "";
            characterNameText.text = "";
            promptButton.enabled = false;
            float timeElapsed = 0;
            float duration;
            Image dialoguePanelImage = dialoguePanel.GetComponent<Image>();

            Vector4 opaque = dialoguePanelImage.color;
            Vector4 transparent = new Vector4(dialoguePanelImage.color.r, dialoguePanelImage.color.g, dialoguePanelImage.color.b, 0f);

            switch (panelTransition)
            {
                case PanelTransition.Fade:
                    switch (enable)
                    {
                        case true:
                            dialoguePanel.SetActive(true);
                            dialoguePanelImage.color = transparent;

                            while (timeElapsed < panelFadeTime)
                            {
                                dialoguePanelImage.color = Vector4.Lerp(transparent, opaque, timeElapsed / panelFadeTime);
                                timeElapsed += Time.deltaTime;
                                yield return null;
                            }

                            dialoguePanelImage.color = opaque;
                            if (!isPrinting) StartCoroutine(PrintDialogue(dialogueData[0].characterName, dialogueData[0].dialogueLine));
                            break;
                        case false:
                            dialoguePanelImage.color = opaque;

                            while (timeElapsed < panelFadeTime)
                            {
                                dialoguePanelImage.color = Vector4.Lerp(opaque, transparent, timeElapsed / panelFadeTime);
                                timeElapsed += Time.deltaTime;
                                yield return null;
                            }
                            dialoguePanelImage.color = transparent;
                            dialoguePanel.SetActive(false);
                            break;
                    }
                    break;
                case PanelTransition.FadeAndSlide:
                    if (panelFadeTime < panelSlideTime) duration = panelSlideTime;
                    else duration = panelFadeTime;
                    Vector3 newPanelPos = dialoguePanel.transform.position;

                    switch (enable)
                    {
                        case true:
                            dialoguePanel.SetActive(true);
                            dialoguePanelImage.color = transparent;
                            dialoguePanel.transform.position = newPanelPos + panelSlideInFromPos;

                            while (timeElapsed < duration)
                            {
                                if (separateFadeAndSlideTime)
                                {
                                    if ((timeElapsed / panelFadeTime) < 1) dialoguePanelImage.color = Vector4.Lerp(transparent, opaque, timeElapsed / panelFadeTime);
                                }
                                else if ((timeElapsed / panelSlideTime) < 1) dialoguePanelImage.color = Vector4.Lerp(transparent, opaque, timeElapsed / panelSlideTime);

                                switch (useSlideCurve)
                                {
                                    case true:
                                        if ((timeElapsed / slideCurve[slideCurve.length - 1].time) < 1) dialoguePanel.transform.position = Vector3.Lerp(newPanelPos + panelSlideInFromPos, newPanelPos, timeElapsed / slideCurve[slideCurve.length - 1].time);
                                        break;
                                    case false:
                                        if ((timeElapsed / panelSlideTime) < 1) dialoguePanel.transform.position = Vector3.Lerp(newPanelPos + panelSlideInFromPos, newPanelPos, timeElapsed / panelSlideTime);
                                        break;
                                }

                                timeElapsed += Time.deltaTime;
                                yield return null;
                            }

                            dialoguePanelImage.color = opaque;
                            dialoguePanel.transform.position = newPanelPos;
                            if (!isPrinting) StartCoroutine(PrintDialogue(dialogueData[0].characterName, dialogueData[0].dialogueLine));
                            break;
                        case false:
                            if (slideOutOnExit)
                            {
                                while (timeElapsed < duration)
                                {
                                    if (separateFadeAndSlideTime)
                                    {
                                        if ((timeElapsed / panelFadeTime) < 1 && separateFadeAndSlideTime) dialoguePanelImage.color = Vector4.Lerp(opaque, transparent, timeElapsed / panelFadeTime);
                                    }
                                    else if ((timeElapsed / panelSlideTime) < 1) dialoguePanelImage.color = Vector4.Lerp(opaque, transparent, timeElapsed / panelSlideTime);
                                    if ((timeElapsed / panelSlideTime) < 1) dialoguePanel.transform.position = Vector3.Lerp(newPanelPos, newPanelPos + panelSlideOutToPos, timeElapsed / panelSlideTime);

                                    timeElapsed += Time.deltaTime;
                                    yield return null;
                                }

                                dialoguePanelImage.color = transparent;
                                dialoguePanel.transform.position = newPanelPos + panelSlideOutToPos;
                                dialoguePanel.SetActive(false);
                            }
                            else StartCoroutine(BeginPanelTransition(PanelTransition.Fade, false));
                            break;
                    }
                    break;
                case PanelTransition.FadeAndZoom:
                    switch (enable)
                    {
                        case true:
                            dialoguePanel.SetActive(true);
                            dialoguePanelImage.color = transparent;
                            break;
                        case false:
                            break;
                    }
                    break;
            }
            yield return null;
        }

        private IEnumerator PrintDialogue(string characterNameLine, string dialogueLine)
        {
            // Set all the starting values and wipe the strings.
            effectRunning = false;
            isPrinting = true;
            dialogueText.text = "";
            characterNameText.text = "";
            promptButton.enabled = false;

            // This checks to see if there is a time set at the current iteration. If so, set the time as that. If not, set the time as the default value.
            float f = dialogueData[iteration].charDelayTime;

            // Set up the dialogue clip and sort the SFX timing based on what isFixedDialogueSfxTiming set to.
            dialogueSfxSource.clip = dialogueData[iteration].dialogueLineSfx;
            if (isFixedDialogueSfxTiming) StartCoroutine(PlaySfxFixed());

            switch (dialogueData[iteration].dialogueTextEffect)
            {
                case TextEffect.None:
                    break;
                case TextEffect.Wavy:
                    Application.targetFrameRate = 60;
                    StartCoroutine(TextAnimation(TextEffect.Wavy));
                    break;
                case TextEffect.Ripple:
                    StartCoroutine(TextAnimation(TextEffect.Ripple));
                    break;
                case TextEffect.Shaky:
                    StartCoroutine(TextAnimation(TextEffect.Shaky));
                    break;
            }

            // Print out the values to the dialogue box.
            characterNameText.text = dialogueData[iteration].characterName;
            int charIteration = 0;
            foreach (char c in dialogueLine)
            {
                dialogueText.text += c;
                if (!isFixedDialogueSfxTiming) dialogueSfxSource.Play();
                if (c == '.' && pauseAtFullStop && charIteration != dialogueLine.Length - 1) yield return new WaitForSeconds(fullStopPauseTime);
                yield return new WaitForSeconds(f);
                if (skipCheck)
                {
                    dialogueText.text = dialogueData[iteration].dialogueLine;
                    break;
                }
                charIteration++;
            }

            // Change those starting values at the end and iterate.
            promptButton.enabled = true;
            isPrinting = false;
            skipCheck = false;
            iteration++;
        }

        private IEnumerator PlaySfxFixed()
        {
            while (isPrinting)
            {
                dialogueSfxSource.Play();
                yield return new WaitForSeconds(fixedDialogueSfxTiming);
            }
        }

        // ### TEXT EFFECT FUNCTION ###
        // Thanks to Kemble Software's tutorial for creating this effect: https://youtu.be/FXMqUdP3XcE
        private IEnumerator TextAnimation(TextEffect effect)
        {
            yield return null;
            effectRunning = true;
            while (effectRunning)
            {
                dialogueText.ForceMeshUpdate();
                var textInfo = dialogueText.textInfo;

                for (int i = 0; i < textInfo.characterCount; ++i)
                {
                    var charInfo = textInfo.characterInfo[i];
                    if (!charInfo.isVisible) continue;
                    bool firstRun = true;
                    var verts = textInfo.meshInfo[charInfo.materialReferenceIndex].vertices;
                    Vector3 vertOffset = new Vector3();
                    for (int j = 0; j < 4; ++j)
                    {
                        switch (effect)
                        {
                            case TextEffect.Wavy:
                                switch (charBasedWave)
                                {
                                    case true:
                                        if (firstRun)
                                        {
                                            // Solution generated by ChatGPT: https://chatgpt.com/share/67487aec-17c8-800e-b0f3-b77910dec72b
                                            float offsetY = Mathf.Sin(Time.time * waveSpeed + charInfo.vertexIndex * waveIntensity) * waveExponent;
                                            vertOffset = new Vector3(0, offsetY, 0);
                                            firstRun = false;
                                        }
                                        verts[charInfo.vertexIndex + j] += vertOffset;
                                        break;
                                    case false:
                                        var orig2 = verts[charInfo.vertexIndex + j];
                                        verts[charInfo.vertexIndex + j] = orig2 + new Vector3(0, Mathf.Sin(Time.time * waveSpeed + orig2.x * waveIntensity) * waveExponent, 0);
                                        break;
                                }
                                break;
                            case TextEffect.Shaky:
                                if (firstRun)
                                {
                                    float x = Random.Range(-1f, 1f) * shakeMagnitude;
                                    float y = Random.Range(-1f, 1f) * shakeMagnitude;
                                    vertOffset = new Vector3(x, y, 0);
                                    firstRun = false;
                                }
                                verts[charInfo.vertexIndex + j] += vertOffset;
                                break;
                            case TextEffect.Ripple:
                                switch (charBasedRipple)
                                {
                                    case true:
                                        if (firstRun)
                                        {
                                            float offsetX = Mathf.Sin(Time.time * rippleSpeed + charInfo.vertexIndex * rippleIntensity) * rippleExponent;
                                            vertOffset = new Vector3(offsetX, 0, 0);
                                            firstRun = false;
                                        }
                                        verts[charInfo.vertexIndex + j] += vertOffset;
                                        break;
                                    case false:
                                        var orig = verts[charInfo.vertexIndex + j];
                                        verts[charInfo.vertexIndex + j] = orig + new Vector3(Mathf.Sin(Time.time * rippleSpeed + orig.x * rippleIntensity) * rippleExponent, 0, 0);
                                        break;
                                }
                                break;
                        }
                    }
                }
                for (int i = 0; i < textInfo.meshInfo.Length; ++i)
                {
                    var meshInfo = textInfo.meshInfo[i];
                    meshInfo.mesh.vertices = meshInfo.vertices;
                    dialogueText.UpdateGeometry(meshInfo.mesh, i);
                }

                yield return null;
            }
        }

        public void ChangeGlyphs(int glyphMode)
        {
            switch (glyphMode)
            {
                case 0:
                    if (promptButton.sprite != keyboardPromptButton) promptButton.sprite = keyboardPromptButton;
                    break;
                case 1:
                    if (promptButton.sprite != controllerPromptButton) promptButton.sprite = controllerPromptButton;
                    break;
            }
        }
    }
}