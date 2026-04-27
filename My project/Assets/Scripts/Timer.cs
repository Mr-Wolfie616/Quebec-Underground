using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class Timer : MonoBehaviour
{
    public float duration = 0;
    public TextMeshProUGUI Yourtime;
    public bool gamePaused = false;

    // Update is called once per frame
    void Update()
    {
        if (!gamePaused)
        {
            duration += Time.deltaTime;
        }
    }

    public void FindDisplay()
    {
        Yourtime = GameObject.Find("Timer Text").GetComponent<TextMeshProUGUI>();
        DisplayTime(duration);
    }

    void DisplayTime(float timeToDisplay)
    {
        int Seconds = Mathf.FloorToInt(duration % 60);
        int Minutes = Mathf.FloorToInt(duration / 60);
        Yourtime.text = string.Format("Your Time: {0:00}:{1:00}", Minutes, Seconds);
    }


}
