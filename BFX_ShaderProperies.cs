using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

public class BFX_ShaderProperies : MonoBehaviour
{
    private bool isAutoDestroy;
    private float maxLifeTime;
    private DecalProjector decal;
    private Material decalMaterial;

    private void Awake()
    {
        if (BFX_GlobalSettings.inst == null)
        {
            Debug.LogError($"Attach a {nameof(BFX_GlobalSettings)} script to a gameobject within the scene!");
        }
        else
        {
            isAutoDestroy = BFX_GlobalSettings.inst.IsAutoDestroy();
            maxLifeTime = BFX_GlobalSettings.inst.GetAutoDestroyTimer();
        }

        decal = GetComponent<DecalProjector>();
        decal.material = new Material(decal.material);
        decalMaterial = decal.material;
        decalMaterial.SetFloat(BFX_MaterialProperties.LightIntensity, 1); // This appears to be useless
    }

    private void OnEnable()
    {
        decalMaterial.SetFloat(BFX_MaterialProperties.Cutout, 1);
    }

    private IEnumerator FadeOutSequence(float duration)
    {
        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            float normalizedTime = Mathf.InverseLerp(0, duration, timer);
            float evaluateCurve = BFX_GlobalSettings.inst.GetFadeOutTimeScaleNormalized(normalizedTime);
            decalMaterial.SetFloat(BFX_MaterialProperties.Cutout, evaluateCurve);
            decalMaterial.SetVector(BFX_MaterialProperties.DecalForwardDir, transform.up);
            yield return null;
        }

        ObjectPoolManager.ReturnObjectToPool(gameObject.transform.parent.gameObject);
    }

    public void BeginDecalFadeOutAutomatic()
    {
        if (isAutoDestroy)
        {
            StartCoroutine(FadeOutSequence(maxLifeTime));
        }
        else
        {
            decalMaterial.SetFloat(BFX_MaterialProperties.Cutout, 0);
        }
    }

    public void BeginDecalFadeOutDynamic(float duration)
    {
        StartCoroutine(FadeOutSequence(duration));
    }
}
