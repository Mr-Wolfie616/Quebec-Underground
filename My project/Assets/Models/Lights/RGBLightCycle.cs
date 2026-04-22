using UnityEngine;

[RequireComponent(typeof(Light))]
public class RGBLightCycle : MonoBehaviour
{
    public float speed = 0.25f;
    private Light pointLight;

    void Start()
    {
        pointLight = GetComponent<Light>();
    }

    void Update()
    {
        float hue = Mathf.PingPong(Time.time * speed, 1f);
        Color color = Color.HSVToRGB(hue, 1f, 1f);
        pointLight.color = color;
    }
}