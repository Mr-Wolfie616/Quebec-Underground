using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NPCDeathTrigger : MonoBehaviour
{
    private async Task OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<FPCharacterController>(out var controller))
        {
            SceneManager.LoadScene("YouDiedScene"); // really shit
        }
    }
}
