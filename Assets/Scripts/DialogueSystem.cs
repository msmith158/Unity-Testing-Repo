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

using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class DialogueSystem : MonoBehaviour
{
    [Header("Dialogue Values")]
    [SerializeField][Tooltip("This is for listing the name of the characters that are talking per dialogue line. Make sure this is in sequence with the order of the dialogue lines.\n\nTip: inserting a blank string will print nothing, which can be handy for displaying system messages through the dialogue system, e.g. in-game tips.")] private string[] dialogueCharacterNames;
    [SerializeField][Tooltip("This is for each line of dialogue in this one dialogue event.")] private List<string> dialogueLines = new List<string>();
    [SerializeField][Tooltip("This is for the timing of each dialogue line. Example: for fast, set 0.02f; for slow, set 0.2f.\n\nMake sure this is in sequence with the order of the dialogue lines. If list is empty, system will use default timing.")] private float[] charDelayTimes;
    [SerializeField][Tooltip("This is for what will play when each character of a dialogue line is being printed out.\n\nMake sure this is in sequence with the order of the dialogue lines.")] private AudioClip[] dialogueLineSfx;
    [SerializeField] private TextEffect[] dialogueTextEffect;
    [SerializeField] private PanelEffect[] dialogueBoxEffect;
    [SerializeField] private TransitionEffect transitionEffect;

    [Header("Dialogue Settings: General")]
    [SerializeField][Range(0.01f, 0.5f)] private float defaultCharDelayTime;
    [SerializeField][Tooltip("By default, the dialogue SFX is played when each character is printed. This sets the SFX onto an individualised timer that runs until the dialogue stops printing.\n\nThis can be handy for higher timing values that can cause the SFX to play too fast and produce an unwanted result.")] private bool isFixedDialogueSfxTiming;
    [SerializeField] private float fixedDialogueSfxTiming;
    [SerializeField] private AudioClip defaultDialogueSfx;
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

    [Header("Object References")]
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TextMeshProUGUI characterNameText;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private Image promptButton;
    [SerializeField] private AudioSource dialogueSfxSource;

    private bool isPrinting = false;
    private int iteration = 0;
    private bool effectRunning = false;
    private bool skipCheck = false;

    // Start is called before the first frame update
    void Start()
    {
        dialoguePanel.SetActive(true);

        if (!isPrinting)
        {
            StartCoroutine(PrintDialogue(dialogueCharacterNames[0], dialogueLines[0]));
        }
    }

    private void Update()
    {
        if (!isPrinting)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                if (iteration < dialogueLines.Count)
                {
                    StartCoroutine(PrintDialogue(dialogueCharacterNames[iteration], dialogueLines[iteration]));
                }
                else
                {
                    dialoguePanel.SetActive(false);
                }
            }
        }
        else if (isPrinting)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                skipCheck = true;
            }
        }
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
        float f = 0;
        if (charDelayTimes.Length > iteration) f = charDelayTimes[iteration];
        else f = defaultCharDelayTime;

        // Set up the dialogue clip and sort the SFX timing based on what isFixedDialogueSfxTiming set to.
        if (dialogueLineSfx.Length > iteration) dialogueSfxSource.clip = dialogueLineSfx[iteration];
        else dialogueSfxSource.clip = defaultDialogueSfx;
        if (isFixedDialogueSfxTiming) StartCoroutine(PlaySfxFixed());
        
        if (dialogueTextEffect.Length > iteration)
        {
            switch (dialogueTextEffect[iteration])
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
        }

        // Print out the values to the dialogue box.
        characterNameText.text = dialogueCharacterNames[iteration];
        int charIteration = 0;
        foreach (char c in dialogueLine)
        {
            dialogueText.text += c;
            if (!isFixedDialogueSfxTiming) dialogueSfxSource.Play();
            if (c == '.' && pauseAtFullStop && charIteration != dialogueLine.Length - 1) yield return new WaitForSeconds(fullStopPauseTime);
            yield return new WaitForSeconds(f);
            if (skipCheck)
            {
                dialogueText.text = dialogueLines[iteration];
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

    private enum TextEffect
    {
        None,
        Wavy,
        Ripple,
        Shaky
    }

    private enum PanelEffect
    {
        None,
        Shake
    }

    private enum TransitionEffect
    {
        None,
        FadeIn,
        FadeSlideIn
    }
}