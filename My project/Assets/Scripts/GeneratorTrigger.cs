using UnityEngine;

public class GeneratorTrigger : MonoBehaviour
{
    public int generatorID = 0;
    private bool done = false;

    public void DidTheThing() {
        if (done) { return; }
        done = true;
        AudioManager.Instance.PlaySound("SFX_Generator", transform.position, null);
        PuzzleManager.PuzzleManagerIncrease?.Invoke(generatorID, transform.position);
    }
}
