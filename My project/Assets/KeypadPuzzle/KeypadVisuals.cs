using System.Collections;
using System.Text;
using TMPro;
using UnityEngine;

public class KeypadVisuals : MonoBehaviour
{
    public TextMeshProUGUI codeText;
    private KeypadPuzzleScript puzzleScript;

    private Coroutine messageRoutine;

    private void Awake()
    {
        puzzleScript = GetComponent<KeypadPuzzleScript>();
    }

    public void UpdateCodeText()
    {
        int[] current = puzzleScript.GetCurrentAttempt();
        int totalLength = puzzleScript.puzzleData.solution.Length;

        StringBuilder display = new StringBuilder();

        for (int i = 0; i < totalLength; i++)
        {
            if (i < current.Length)
                display.Append(current[i]);
            else
                display.Append("-");

            if (i < totalLength - 1)
                display.Append(" ");
        }

        codeText.text = display.ToString();
    }

    public void StartMessage(string message, float time)
    {
        if (messageRoutine != null)
        {
            StopCoroutine(messageRoutine);
        }

        messageRoutine = StartCoroutine(ShowMessageRoutine(message, time));
    }

    private IEnumerator ShowMessageRoutine(string message, float time)
    {
        codeText.text = message;

        yield return new WaitForSeconds(time);

        UpdateCodeText();
        messageRoutine = null;
    }
}