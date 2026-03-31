using UnityEngine;
using UnityEngine.Events;

public class EventInteractableButton : MonoBehaviour, IInteractable
{
    public UnityEvent OnButtonPressed;
    public UnityEvent OnInteracted;

    public void Press()
    {
        OnButtonPressed?.Invoke();
        AudioManager.Instance.PlaySound("sfx_KeypadPress", transform.position, null);
    }

    public void Interact()
    {
        OnInteracted?.Invoke();
    }
}
