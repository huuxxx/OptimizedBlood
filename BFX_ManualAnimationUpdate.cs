using System.Collections;
using UnityEngine;

public class BFX_ManualAnimationUpdate : MonoBehaviour
{
    private float lifeTime;
    private Renderer renderer;
    private MaterialPropertyBlock mpb;

    private void Awake()
    {
        mpb = new MaterialPropertyBlock();
        renderer = GetComponent<Renderer>();

        if (BFX_GlobalSettings.inst == null)
        {
            Debug.LogError($"Attach a {nameof(BFX_GlobalSettings)} script to a gameobject within the scene!");
        }
        else
        {
            lifeTime = BFX_GlobalSettings.inst.GetBloodMeshLifeTime();
        }
    }

    private void OnEnable()
    {
        renderer.enabled = true;
        renderer.GetPropertyBlock(mpb);
        mpb.SetFloat(BFX_MaterialProperties.UseCustomTime, 1);
        mpb.SetFloat(BFX_MaterialProperties.TimeInFrames, 0);
        mpb.SetFloat(BFX_MaterialProperties.LightIntensity, 1);
        renderer.SetPropertyBlock(mpb);
        StartCoroutine(PlayAnimation());
    }

    private IEnumerator PlayAnimation()
    {
        float elapsed = 0f;

        while (elapsed < lifeTime)
        {
            float normalizedTime = Mathf.Clamp01(elapsed / lifeTime);
            float curvedTime = 1f - BFX_GlobalSettings.inst.GetBloodMeshCurveNormalized(normalizedTime);
            float timeInFrames = Mathf.Clamp01(curvedTime);

            mpb.SetFloat(BFX_MaterialProperties.TimeInFrames, timeInFrames);
            renderer.SetPropertyBlock(mpb);

            elapsed += Time.deltaTime;
            yield return null;
        }

        renderer.enabled = false;
    }
}
