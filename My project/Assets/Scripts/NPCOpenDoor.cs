using UnityEngine;

public class NPCOpenDoor : MonoBehaviour
{
    public bool NPCCanOpen = true;
    public Door door;

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<NPCStateManager>())
        {
            if (!NPCCanOpen) {return; }
            door.ForceOpen();
        }
    }
}
