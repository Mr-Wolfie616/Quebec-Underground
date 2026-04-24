using UnityEngine;

[RequireComponent(typeof(Light))]
public class LightFlicker : MonoBehaviour
{
    public float minIntensity = 0.1f;
    public float maxIntensity = 1f;

    public float flickerSpeed = 0.1f; // lower = faster flicker

    private Light lightSource;
    private float timer;

    void Start()
    {
        lightSource = GetComponent<Light>();
    }

    void Update()
    {
        timer -= Time.deltaTime;

        if (timer <= 0f)
        {
            lightSource.intensity = Random.Range(minIntensity, maxIntensity);
            timer = flickerSpeed;
        }
    }
}