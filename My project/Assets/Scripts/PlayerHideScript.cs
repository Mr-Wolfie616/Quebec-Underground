using UnityEngine;

public class PlayerHideScript : MonoBehaviour
{
    public bool isHiding = false;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("HideTrigger"))
        {
            Debug.Log("PLAYER HIDING!!!!!");
            isHiding = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("HideTrigger"))
        {
            Debug.Log("PLAYER NO LONGER HIDING!!!!!");
            isHiding = false;
        }
    }
}


