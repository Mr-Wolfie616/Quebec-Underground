using System.Collections;
using TMPro;
using UnityEngine;

public class KeypadButton : MonoBehaviour, IInteractable
{
    public KeypadPuzzleScript parentKeypad;
    public int digit;
    public TextMeshProUGUI digitText;

    public float pressDistance = 0.008f;
    public float pressInTime = 0.08f;
    public float releaseTime = 0.14f;

    private Vector3 startLocalPos;
    private Vector3 endLocalPos;
    private Coroutine pressRoutine;

    private void Awake()
    {
        startLocalPos = transform.localPosition;
        endLocalPos = startLocalPos + Vector3.forward * pressDistance;

        if (digitText != null)
        {
            digitText.text = digit.ToString();
        }
    }

    private IEnumerator PressAnimation()
    {
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime / pressInTime;
            transform.localPosition = Vector3.Lerp(startLocalPos, endLocalPos, t);
            yield return null;
        }

        t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime / releaseTime;
            transform.localPosition = Vector3.Lerp(endLocalPos, startLocalPos, t);
            yield return null;
        }

        transform.localPosition = startLocalPos;
        pressRoutine = null;
    }

    private void AnimatePress()
    {
        if (pressRoutine != null) StopCoroutine(pressRoutine);

        pressRoutine = StartCoroutine(PressAnimation());
    }

    public void Interact()
    {
        AnimatePress();
        parentKeypad.AddDigit(digit);
        AudioManager.Instance.PlaySound("sfx_KeypadPress", transform.position, null);
    }

    public void Press()
    {
        AnimatePress();
        parentKeypad.AddDigit(digit);
        AudioManager.Instance.PlaySound("sfx_KeypadPress", transform.position, null);
    }
}
