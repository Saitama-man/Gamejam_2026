using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class StarPulse : MonoBehaviour
{
    [Header("Emission Color")]
    [SerializeField]
    private Color emissionColor = new Color(
        26f / 255f,
        133f / 255f,
        229f / 255f
    );

    [Header("HDR Intensity")]
    [SerializeField] private float minHDRIntensity = 7.8f;
    [SerializeField] private float maxHDRIntensity = 9.2f;
    [SerializeField] private float pulseSpeed = 1.5f;

    [Header("Scale Pulse")]
    [SerializeField] private bool pulseScale = true;
    [SerializeField] private float minScale = 0.24f;
    [SerializeField] private float maxScale = 0.27f;

    private Renderer starRenderer;
    private Material mat;

    private void Awake()
    {
        starRenderer = GetComponent<Renderer>();
        mat = starRenderer.material;

        mat.EnableKeyword("_EMISSION");

        if (mat.HasProperty("_BaseColor"))
            mat.SetColor("_BaseColor", Color.white);
    }

    private void Update()
    {
        float pulse = Mathf.Sin(Time.time * pulseSpeed) * 0.5f + 0.5f;

        float hdrIntensity = Mathf.Lerp(minHDRIntensity, maxHDRIntensity, pulse);

        // Важно: имитируем Unity HDR Color Intensity
        Color finalEmission = emissionColor * Mathf.Pow(2f, hdrIntensity);

        mat.SetColor("_EmissionColor", finalEmission);

        if (pulseScale)
        {
            float scale = Mathf.Lerp(minScale, maxScale, pulse);
            transform.localScale = Vector3.one * scale;
        }
    }
}