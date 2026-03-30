using TMPro;
using UnityEngine;

public class KeypadButton : MonoBehaviour, IInteractable
{
    public KeypadPuzzleScript parentKeypad;
    public int digit;
    public TextMeshProUGUI digitText;

    private void Awake()
    {
        if (digitText != null)
        {
            digitText.text = digit.ToString();
        }
    }

    public void Interact()
    {
        return;
    }

    public void Press()
    {
        //Debug.Log("press");
        parentKeypad.AddDigit(digit);
        AudioManager.Instance.PlaySound("sfx_KeypadPress", transform.position, null);
    }
}
