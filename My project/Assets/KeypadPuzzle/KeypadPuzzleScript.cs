using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class KeypadPuzzleScript : MonoBehaviour
{
    public PuzzleDataSO puzzleData;
    private List<int> currentInput = new();
    private int currentAttempts;
    private bool completed = false;
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
        if (completed) { return; }

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
                visualScript.StartMessage("INCORRECT");
                currentInput.Clear();
                return;
            }
        }

        visualScript.StartMessage("CORRECT");
        completed = true;
        OnPuzzleCompleted?.Invoke();
        Debug.Log("CODE CORRECT!!!!");
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
