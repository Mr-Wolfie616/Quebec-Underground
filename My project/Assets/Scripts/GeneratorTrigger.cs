using UnityEngine;

public class GeneratorTrigger : MonoBehaviour
{
    public int generatorID = 0;

    public void DidTheThing() {
        PuzzleManager.PuzzleManagerIncrease?.Invoke(generatorID);
    }
}
