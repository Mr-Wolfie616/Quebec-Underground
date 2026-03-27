using System;
using UnityEngine;

public class Door : MonoBehaviour, IInteractable
{
    public void Interact()
    {
        Debug.Log ("Open Door");
    }
}
