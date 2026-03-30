using UnityEngine;

[CreateAssetMenu(fileName = "PuzzleDataSO", menuName = "ScriptableObjects/PuzzleDataSO", order = 2)]
public class PuzzleDataSO : ScriptableObject
{
    public int puzzleID;
    public int[] solution;
    public int maxAttempts = 3;
}