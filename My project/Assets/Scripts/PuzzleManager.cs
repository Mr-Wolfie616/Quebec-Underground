using System;
using TMPro;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;

public class PuzzleManager : MonoBehaviour
{
    public int totalPuzzles = 3;
    public static int puzzlesCompleted = 0;
    public static Action<int> PuzzleManagerIncrease;
    public UnityEvent AllPuzzlesCompleted;
    public TextMeshProUGUI text;
    public List<int> completedPuzzles = new List<int>();

    private void Awake()
    {
        text.text = $"{puzzlesCompleted}/{totalPuzzles}";
    }
    private void OnEnable()
    {
        PuzzleManagerIncrease += HandlePuzzleCompleted;
    }

    private void OnDisable()
    {
        PuzzleManagerIncrease -= HandlePuzzleCompleted;
    }

    public void HandlePuzzleCompleted(int id)
    {
        if (completedPuzzles.Contains(id))  {
            Debug.Log("duplicate");
            return;
        }
        else{
            completedPuzzles.Add(id);
        }

        puzzlesCompleted++;
        Debug.Log($"Puzzle completed! Total: {puzzlesCompleted}/{totalPuzzles}");
        text.text = $"{puzzlesCompleted}/{totalPuzzles}";
        if (puzzlesCompleted >= totalPuzzles)
        {
            Debug.Log("All puzzles completed!");
            AllPuzzlesCompleted?.Invoke();
        }
    }

}
