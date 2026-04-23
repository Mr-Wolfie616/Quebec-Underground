using UnityEngine;
using UnityEngine.SceneManagement;

public class EsacpeTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Finish"))
        {
            SceneManager.LoadScene(3);
        }
    }
}
