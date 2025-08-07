using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

public class BFX_DecalSettings : MonoBehaviour
{
    private Transform parent;
    private float groundingCheckDistance;
    private Transform transform;
    private BFX_ShaderProperies shaderProperies;
    private DecalProjector decal;
    private LayerMask groundingLayers;
    private const float DecalHeightOffset = 0.05f;

    private void Awake()
    {
        parent = gameObject.transform.parent;
        decal = GetComponent<DecalProjector>();
        transform = gameObject.transform;
        transform.localScale = Vector3.one;
        shaderProperies = GetComponent<BFX_ShaderProperies>();

        if (BFX_GlobalSettings.inst == null)
        {
            Debug.LogError($"Attach a {nameof(BFX_GlobalSettings)} script to a gameobject within the scene!");
        }
        else
        {
            groundingCheckDistance = BFX_GlobalSettings.inst.GetGroundingCheckDistance();
            groundingLayers = BFX_GlobalSettings.inst.GetGroundLayers();
        }
    }

    private void OnEnable()
    {
        decal.enabled = false;

        if (!Physics.Raycast(parent.position, Vector3.down, out RaycastHit groundHitPoint, groundingCheckDistance, groundingLayers))
        {
            return;
        }

        float heightDifference = parent.position.y - groundHitPoint.point.y;
        float normalizedHeight = Mathf.InverseLerp(0, groundingCheckDistance, heightDifference);
        float timeDelay = BFX_GlobalSettings.inst.GetTimeDelayToProjectDecalNormalized(normalizedHeight);
        StartCoroutine(DisplayDecal(timeDelay, groundHitPoint));
    }

    private IEnumerator DisplayDecal(float delay, RaycastHit groundHitPoint)
    {
        yield return new WaitForSeconds(delay);
        decal.enabled = true;
        Vector3 randomSize = BFX_GlobalSettings.inst.GetRandomScaleWithMultiplier();
        decal.size = randomSize;
        transform.localScale = randomSize;
        transform.position = new Vector3(transform.position.x, groundHitPoint.point.y + DecalHeightOffset, transform.position.z);
        shaderProperies.BeginDecalFadeOutAutomatic();
    }
}
