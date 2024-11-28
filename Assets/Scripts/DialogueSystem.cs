using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueSystem : MonoBehaviour
{
    [Header("Dialogue Values")]
    [SerializeField][Tooltip("This is for listing the name of the characters that are talking per dialogue line.\n\nMake sure this is in sequence with the order of the dialogue lines.")] private string[] dialogueCharacterNames;
    [SerializeField][Tooltip("This is for each line of dialogue in this one dialogue event.")] private string[] dialogueLines;
    [SerializeField][Tooltip("This is for the timing of each dialogue line. Example: for fast, set 0.02f; for slow, set 0.2f.\n\nMake sure this is in sequence with the order of the dialogue lines. If list is empty, system will use default timing.")] private float[] charDelayTimes;
    [SerializeField][Tooltip("This is for what will play when each character of a dialogue line is being printed out.\n\nMake sure this is in sequence with the order of the dialogue lines.")] private AudioClip[] dialogueLineSfx;

    [Header("Dialogue Settings")]
    [SerializeField][Range(0.01f, 0.5f)] private float defaultCharDelayTime;
    [SerializeField][Tooltip("By default, the dialogue SFX is played when each character is printed. This sets the SFX onto an individualised timer that runs until the dialogue stops printing.\n\nThis can be handy for higher timing values that can cause the SFX to play too fast and produce an unwanted result.")] private bool isFixedDialogueSfxTiming;
    [SerializeField] private float fixedDialogueSfxTiming;

    [Header("Object References")]
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TextMeshProUGUI characterNameText;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private Image promptButton;
    [SerializeField] private AudioSource dialogueSfxSource;

    private bool isPrinting = false;
    private int iteration = 0;

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
                if (iteration < dialogueLines.Length)
                {
                    StartCoroutine(PrintDialogue(dialogueCharacterNames[iteration], dialogueLines[iteration]));
                }
                else
                {
                    dialoguePanel.SetActive(false);
                }
            }
        }
    }

    private IEnumerator PrintDialogue(string characterNameLine, string dialogueLine)
    {
        // Set all the starting values and wipe the strings.
        isPrinting = true;
        dialogueText.text = "";
        characterNameText.text = "";
        promptButton.enabled = false;

        // This checks to see if there is a time set at the current iteration. If so, set the time as that. If not, set the time as the default value.
        float f = 0;
        if (charDelayTimes.Length != iteration) f = charDelayTimes[iteration];
        else f = defaultCharDelayTime;

        // Set up the dialogue clip and sort the SFX timing based on what isFixedDialogueSfxTiming set to.
        dialogueSfxSource.clip = dialogueLineSfx[iteration];
        if (isFixedDialogueSfxTiming) StartCoroutine(PlaySfxFixed());

        // Print out the values to the dialogue box.
        characterNameText.text = dialogueCharacterNames[iteration];
        foreach (char c in dialogueLine)
        {
            dialogueText.text += c;
            if (!isFixedDialogueSfxTiming) dialogueSfxSource.Play();
            yield return new WaitForSeconds(f);
        }

        // Change those starting values at the end and iterate.
        promptButton.enabled = true;
        isPrinting = false;
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
}
