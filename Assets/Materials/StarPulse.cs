using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class StarPulse : MonoBehaviour
{
    [Header("Emission Pulse")]
    [SerializeField] private float minEmissionMultiplier = 0.75f;
    [SerializeField] private float maxEmissionMultiplier = 1.25f;
    [SerializeField] private float pulseSpeed = 2f;

    [Header("Scale Pulse")]
    [SerializeField] private bool pulseScale = true;
    [SerializeField] private float scalePulseAmount = 0.05f;

    private Renderer starRenderer;
    private Material mat;
    private Color originalEmission;
    private Vector3 originalScale;

    private void Awake()
    {
        starRenderer = GetComponent<Renderer>();
        mat = starRenderer.material;

        originalScale = transform.localScale;

        mat.EnableKeyword("_EMISSION");

        if (mat.HasProperty("_EmissionColor"))
        {
            originalEmission = mat.GetColor("_EmissionColor");
        }
        else
        {
            Debug.LogError("StarPulse: у материала нет свойства _EmissionColor.", this);
            enabled = false;
        }
    }

    private void Update()
    {
        float pulse = Mathf.Sin(Time.time * pulseSpeed) * 0.5f + 0.5f;

        float emissionMultiplier = Mathf.Lerp(
            minEmissionMultiplier,
            maxEmissionMultiplier,
            pulse
        );

        mat.SetColor("_EmissionColor", originalEmission * emissionMultiplier);

        if (pulseScale)
        {
            float scaleMultiplier = 1f + Mathf.Lerp(
                -scalePulseAmount,
                scalePulseAmount,
                pulse
            );

            transform.localScale = originalScale * scaleMultiplier;
        }
    }
}