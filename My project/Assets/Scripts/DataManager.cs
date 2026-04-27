using UnityEngine;
using UnityEngine.SceneManagement;

public class DataManager : MonoBehaviour
{
   public static DataManager Instance;
   public Timer timer;

    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
        timer = GetComponent<Timer>();
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnsceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnsceneLoaded;
    }

    void OnsceneLoaded(Scene scene, LoadSceneMode mode)
    {
        switch(scene.buildIndex)
        {
            case 0:
                Debug.Log("Start Menu");
                timer.duration = 0;
                timer.gamePaused = true;
            break;

            case 1:
                Debug.Log("Play Scene");
                timer.gamePaused = false;
            break;

            case 2:
                Debug.Log("Game Over Scene");
                timer.gamePaused = true;
                timer.duration = 0;
            break;

            case 3:
                Debug.Log("Win Scene");
                timer.gamePaused = true;
                timer.FindDisplay();
            break;

        }
    }
}
