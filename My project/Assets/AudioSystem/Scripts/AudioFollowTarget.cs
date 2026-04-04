
using UnityEngine;

public class AudioFollowTarget : MonoBehaviour
{
    public Transform target;

    private void Update()
    {
        if (target != null)
        {
            transform.position = target.position;
        }
    }
}
