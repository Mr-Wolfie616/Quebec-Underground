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
                visualScript.StartMessage("X X X X", 2f);
                currentInput.Clear();
                CheckAttempts();
                return;
            }
        }

        visualScript.StartMessage("GOOD", 2f);
        interactable = false;
        //AudioManager.Instance.PlaySound(SFX_Keypad_Success);
        OnPuzzleCompleted?.Invoke();
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

        //AudioManager.Instance.PlaySound(SFX_Keypad_Alert);

        float timer = resetTime;

        while (timer > 0f)
        {
            visualScript.codeText.text = Mathf.CeilToInt(timer).ToString();

            timer -= Time.deltaTime;
            yield return null;
        }

        currentAttempts = 0;
        currentInput.Clear();
        interactable = true;

        visualScript.StartMessage("READY", 1f);
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
