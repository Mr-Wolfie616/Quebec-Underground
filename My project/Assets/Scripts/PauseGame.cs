using NUnit.Framework;
using UnityEngine;

public class PauseGame : MonoBehaviour
{
    [SerializeField] GameObject pausemenu = null;

   public Timer timer;
   public GameObject manager;

   private InputReader input;
   private bool isPaused = false;

    void Awake()
    {
        timer = manager.GetComponent<Timer>();
        input = GetComponent<InputReader>();
    }

    void Update()
    {
        if(input.pausePressed)
        {
           Pause();  
        }
    }
    void Pause()
    {
        isPaused = !isPaused;

        pausemenu.SetActive(isPaused);
        Time.timeScale = isPaused ? 0:1;
        
        Cursor.lockState = isPaused ? CursorLockMode.None:CursorLockMode.Locked;            
        Cursor.visible = isPaused;
    }

    public void Resume()
    {
        if(!isPaused) return;

        Pause();
    }

    public void ExitGame()
    {
        Application.Quit();
        Debug.Log("Quit");
    }
}
