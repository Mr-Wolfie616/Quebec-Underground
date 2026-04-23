using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public Transform Player;
    public Transform Credits;
    public Transform CameraTransform;

    void Awake()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    public void PlayGame()
    {
        SceneManager.LoadScene(1);
    }
    
    public void ExitGame()
    {
        Application.Quit();
        Debug.Log("Quit");
    }

    public void Retry()
    {
        SceneManager.LoadScene(0);
    }

    public void Credit()
    {
        CameraTransform.SetParent(Credits);
        CameraTransform.localPosition = Vector3.zero;
        CameraTransform.localRotation = Quaternion.identity;
    }

     public void Back()
    {
        CameraTransform.SetParent(Player);
        CameraTransform.localPosition = Vector3.zero;
        CameraTransform.localRotation = Quaternion.identity;
    }
}
