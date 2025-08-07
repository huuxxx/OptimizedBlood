using UnityEngine;

public class BFX_GlobalSettings : MonoBehaviour
{
    public static BFX_GlobalSettings inst;

    [Header("Timer settings:")]
    [SerializeField, Tooltip("Should the blood auto destroy after the timer is reached?\n\n*** Don't enable this if you implement the management of blood destruction/pooling elsewhere ***")]
    private bool autoDestroy;

    [SerializeField, Range(3, 1000), Tooltip("How long until the blood auto destroys? The decal will visibly fade out towards the end of the timer.")]
    private float autoDestroyTimer;

    [SerializeField, Tooltip("Influences the appearance of blood over a scale of time. 0 = fully visible, 1 = invisible")]
    private AnimationCurve fadeOutCurve;

    [Header("Blood mesh settings:")]
    [SerializeField, Range(1f, 5f), Tooltip("The life time and speed of the mesh portion of blood")]
    private float bloodMeshLifeTime;

    [SerializeField, Tooltip("The speed of the blood mesh relative to current time in life time")]
    private AnimationCurve bloodMeshCurve;

    [Header("Blood decal size and scale settings:")]
    [SerializeField, Range(0.1f, 1f)]
    private float minRandomScaleX;

    [SerializeField, Range(1f, 2f)]
    private float maxRandomScaleX;

    [SerializeField, Range(0.1f, 1f)]
    private float minRandomScaleY;

    [SerializeField, Range(1f, 2f)]
    private float maxRandomScaleY;

    [SerializeField, Range(1, 10)]
    private int scaleMultiplier;

    [Header("Grounding settings:")]
    [SerializeField, Range(1, 20), Tooltip("How far to check for the nearest ground? Distances relative to the spawn position of the blood to the ground, that are greater than this value will result in the blood being culled.")]
    private float groundingCheckDistance;

    [SerializeField, Tooltip("Which Layer(s) should be used to check for grounding?")]
    private LayerMask groundLayers;

    [SerializeField, Tooltip("Adjusts how long the decal will take to project on the ground - relative to the distance to the ground")]
    private AnimationCurve timeByHeight = AnimationCurve.EaseInOut(0, 0, 1, 1);

    private void Awake()
    {
        if (inst == null)
        {
            inst = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);

        if (groundLayers.value == 0)
        {
            Debug.LogError($"{nameof(groundLayers)} for {nameof(BFX_GlobalSettings)} is empty! Assign which layer(s) should be used for grounding checks on {gameObject.name}!");
        }

        timeByHeight ??= AnimationCurve.EaseInOut(0, 0, 1, 1);
        fadeOutCurve ??= AnimationCurve.EaseInOut(0, 0, 1, 1);
        bloodMeshCurve ??= AnimationCurve.Linear(0, 0, 1, 1);
    }

    public bool IsAutoDestroy()
    {
        return autoDestroy;
    }

    public float GetAutoDestroyTimer()
    {
        return autoDestroyTimer;
    }

    public LayerMask GetGroundLayers()
    {
        return groundLayers;
    }

    public float GetGroundingCheckDistance()
    {
        return groundingCheckDistance;
    }

    public float GetBloodMeshLifeTime()
    {
        return bloodMeshLifeTime;
    }

    public float GetBloodMeshCurveNormalized(float normalizedTimeScale)
    {
        if (normalizedTimeScale > 1 || normalizedTimeScale < 0)
        {
            Debug.LogWarning($"Normalize (scale of 0-1) the {nameof(normalizedTimeScale)} value before passing it to {nameof(GetBloodMeshCurveNormalized)}");
            Mathf.Clamp01(normalizedTimeScale);
        }

        return bloodMeshCurve.Evaluate(normalizedTimeScale);
    }

    public float GetFadeOutTimeScaleNormalized(float normalizedTimeScale)
    {
        if (normalizedTimeScale > 1 || normalizedTimeScale < 0)
        {
            Debug.LogWarning($"Normalize (scale of 0-1) the {nameof(normalizedTimeScale)} value before passing it to {nameof(GetFadeOutTimeScaleNormalized)}");
            Mathf.Clamp01(normalizedTimeScale);
        }

        return fadeOutCurve.Evaluate(normalizedTimeScale);
    }

    public float GetTimeDelayToProjectDecalNormalized(float normalizedHeight)
    {
        if (normalizedHeight > 1 || normalizedHeight < 0)
        {
            Debug.LogWarning($"Normalize (scale of 0-1) the {nameof(normalizedHeight)} value before passing it to {nameof(GetTimeDelayToProjectDecalNormalized)}");
            Mathf.Clamp01(normalizedHeight);
        }

        return timeByHeight.Evaluate(normalizedHeight);
    }

    // The size of the decal is determined by the X and Y axis, but still needs a Z value
    public Vector3 GetRandomScaleWithMultiplier()
    {
        return new Vector3
            (Random.Range(minRandomScaleX, maxRandomScaleX) * scaleMultiplier,
            Random.Range(minRandomScaleY, maxRandomScaleY) * scaleMultiplier,
            1);
    }

    public void SetBloodScaleMultiplier(int multiplier)
    {
        scaleMultiplier = Mathf.Clamp(multiplier, 1, 10);
    }
}
