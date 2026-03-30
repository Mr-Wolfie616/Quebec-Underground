using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NPCDeathTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<FPCharacterController>(out var controller))
        {
            SceneManager.LoadScene("YouDiedScene"); // really shit
        }
    }
}
