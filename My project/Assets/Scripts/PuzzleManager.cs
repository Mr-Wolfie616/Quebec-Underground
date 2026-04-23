using System;
using UnityEngine;
using UnityEngine.Events;

public class PuzzleManager : MonoBehaviour
{
    public int totalPuzzles = 3;
    public static int puzzlesCompleted = 0;
    public static Action PuzzleManagerIncrease;
    public UnityEvent AllPuzzlesCompleted;
    private void OnEnable()
    {
        PuzzleManagerIncrease += HandlePuzzleCompleted;
    }

    private void OnDisable()
    {
        PuzzleManagerIncrease -= HandlePuzzleCompleted;
    }

    public void HandlePuzzleCompleted()
    {
        puzzlesCompleted++;
        Debug.Log($"Puzzle completed! Total: {puzzlesCompleted}/{totalPuzzles}");
        if (puzzlesCompleted >= totalPuzzles)
        {
            Debug.Log("All puzzles completed!");
            AllPuzzlesCompleted?.Invoke();
        }
    }

}
