using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class KeypadPuzzleScript : MonoBehaviour
{
    public PuzzleDataSO puzzleData;
    public float resetTime = 1f;
    private List<int> currentInput = new();
    private int currentAttempts;
    private bool interactable = true;
    private KeypadVisuals visualScript;

    public UnityEvent OnPuzzleCompleted;

    private void Start()
    {
        visualScript = GetComponent<KeypadVisuals>();
        currentInput.Clear();
        currentAttempts = 0;
    }

    public void AddDigit(int digit)
    {
        if (!interactable) { return; }

        currentInput.Add(digit);
        visualScript.SetColour(0);

        visualScript.UpdateCodeText();

        Debug.Log("Current input: " + string.Join("", currentInput) + " Puzzle ID: " + puzzleData.puzzleID.ToString());

        if (currentInput.Count >= puzzleData.solution.Length)
        {
            Submit();
        }
    }

    public void Submit()
    {
        currentAttempts++;

        for (int i = 0; i < puzzleData.solution.Length; i++)
        {
            if (currentInput[i] != puzzleData.solution[i])
            {
                Debug.Log("CODE INCORRECT");
                if (currentAttempts != puzzleData.maxAttempts)
                {
                    AudioManager.Instance.PlaySound("keypadError", transform.position, null);
                }
                string xString = string.Join(" ", new string('X', puzzleData.solution.Length).ToCharArray());
                visualScript.StartTimedMessage(xString, 2, 2f);
                currentInput.Clear();
                CheckAttempts();
                return;
            }
        }

        visualScript.OverrideText("OPEN",1);
        interactable = false;
        AudioManager.Instance.PlaySound("keypadWin", transform.position, null);
        OnPuzzleCompleted?.Invoke();
        //PuzzleManager.PuzzleManagerIncrease?.Invoke();
        Debug.Log("CODE CORRECT!!!!");
    }

    private void CheckAttempts()
    {
        if (currentAttempts == puzzleData.maxAttempts)
        {
            Debug.Log("Too many attempts");
            StartCoroutine(TooManyAttemptsRoutine());
        }
    }

    private IEnumerator TooManyAttemptsRoutine()
    {
        interactable = false;

        AudioManager.Instance.PlaySound("keybad", transform.position, null);

        float timer = resetTime;

        while (timer > 0f)
        {
            visualScript.OverrideText(Mathf.CeilToInt(timer).ToString(), 2);

            timer -= Time.deltaTime;
            yield return null;
        }

        currentAttempts = 0;
        currentInput.Clear();
        visualScript.SetColour(0);
        interactable = true;

        visualScript.UpdateCodeText();
    }

    public void ClearInput()
    {
        currentInput.Clear();
        visualScript.UpdateCodeText();
    }

    public int[] GetCurrentAttempt()
    {
        return currentInput.ToArray();
    }

    public int GetCurrentAttempts()
    {
        return currentAttempts;
    }
}
