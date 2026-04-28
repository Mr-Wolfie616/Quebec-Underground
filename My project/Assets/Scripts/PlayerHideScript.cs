using UnityEngine;

public class PlayerHideScript : MonoBehaviour
{
    public bool isHiding = false;
    public bool crouchHide = false;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("HideTrigger"))
        {
            Debug.Log("PLAYER HIDING!!!!!");
            crouchHide = true;
            isHiding = true;
        }

        if (other.CompareTag("SaferoomTrigger"))
        {
            Debug.Log("PLAYER HIDING!!!!!");
            crouchHide = false;
            isHiding = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("HideTrigger"))
        {
            Debug.Log("PLAYER NO LONGER HIDING!!!!!");
            crouchHide = false;
            isHiding = false;
        }

        if (other.CompareTag("SaferoomTrigger"))
        {
            Debug.Log("PLAYER NO LONGER HIDING!!!!!");
            crouchHide = false;
            isHiding = false;
        }
    }
}


