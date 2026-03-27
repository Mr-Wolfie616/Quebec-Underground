using UnityEngine;

public class PlayAudioTrigger : MonoBehaviour
{
    public string audioID;
    public AudioDataSO so;

    public bool worldSound = false;
    public Transform soundSource;
    private Vector3? sourcePos;
    private void Awake()
    {
        if (audioID == null)
        {
            Debug.LogWarning("missing audio id, fallback to so");
            if (so == null)
            {
                Debug.LogWarning("missing SO, fallback unavailable");
                audioID = "MISSING AUDIO";
                return;
            }
            audioID = so.id;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("trigger enter");
        if (other.gameObject.TryGetComponent<FPCharacterController>(out FPCharacterController a))
        {
            Debug.Log($"PLAYER IN AUDIO TRIGGER, PLAYING {audioID}");

            sourcePos = worldSound ? soundSource.position : null;

            AudioManager.Instance.PlaySound(audioID, sourcePos, null);
        }
    }
}
