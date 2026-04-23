using System.Collections;
using System.Text;
using TMPro;
using UnityEngine;

public class KeypadVisuals : MonoBehaviour
{
    public TextMeshProUGUI codeText;
    private KeypadPuzzleScript puzzleScript;

    private Coroutine messageRoutine;

    public bool showNumbers = true;

    public Color[] colours;
    private void Awake()
    {
        puzzleScript = GetComponent<KeypadPuzzleScript>();
    }

    public void UpdateCodeText()
    {
        SetColour(0);
        int[] current = puzzleScript.GetCurrentAttempt();
        int totalLength = puzzleScript.puzzleData.solution.Length;

        StringBuilder display = new StringBuilder();

        for (int i = 0; i < totalLength; i++)
        {
            if (i < current.Length)
                if (showNumbers)
                {
                    display.Append(current[i]);
                }
                else 
                {
                    display.Append("*");
                }
            else
                display.Append("-");

            if (i < totalLength - 1)
                display.Append(" ");
        }

        codeText.text = display.ToString();
    }

    public void StartTimedMessage(string message, int colour, float time)
    {
        if (messageRoutine != null)
        {
            StopCoroutine(messageRoutine);
        }

        messageRoutine = StartCoroutine(ShowMessageRoutine(message, time, colour));
    }

    public void OverrideText(string message, int colour)
    {
        SetColour(colour);
        codeText.text = message;
    }

    public void SetColour(int colour)
    {
        codeText.color = colours[colour];
    }

    private IEnumerator ShowMessageRoutine(string message, float time, int colour)
    {
        SetColour(colour);
        codeText.text = message;

        yield return new WaitForSeconds(time);
        
        UpdateCodeText();
        messageRoutine = null;
    }
}