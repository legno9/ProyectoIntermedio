using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EntityHealth))]
public class HealthChangeFlash : MonoBehaviour
{
    [SerializeField] private Color damageFlashColor = Color.red;
    [SerializeField] private Color healFlashColor = Color.green;
    [SerializeField] private float flashDuration = 0.25f;

    private List<Material[]> originalMaterials = new();
    private List<Color[]> originalColors = new();
    private SkinnedMeshRenderer[] meshRenderers;
    private List<Tween> flashTweens = new();

    private EntityHealth entityLife;

    private void Awake()
    {
        entityLife = GetComponent<EntityHealth>();

        meshRenderers = GetComponentsInChildren<SkinnedMeshRenderer>();
        foreach (SkinnedMeshRenderer renderer in meshRenderers)
        {
            originalMaterials.Add(renderer.materials);
            List<Color> colors = new List<Color>();
            for (int i = 0; i < renderer.materials.Length; i++)
            {
                if (renderer.materials[i].HasProperty("_BaseColor"))
                {
                    colors.Add(renderer.materials[i].GetColor("_BaseColor"));
                }
                else
                {
                    colors.Add(Color.black); // Dummy color for materials without _Color property
                }
            }
            originalColors.Add(colors.ToArray());
        }
    }

    private void OnEnable()
    {
        entityLife.OnHealthChanged.AddListener(PerformFlash);
    }

    private void OnDisable()
    {
        entityLife.OnHealthChanged.RemoveListener(PerformFlash);
    }

    public void TriggerFlash(Color flashColor)
    {
        foreach (Tween tween in flashTweens)
        {
            tween.Kill();
        }
        flashTweens.Clear();

        ChangeMaterialsToColor(flashColor);

        foreach (SkinnedMeshRenderer renderer in meshRenderers)
        {
            foreach (Material material in renderer.materials)
            {
                if (material.HasProperty("_BaseColor"))
                {
                    // Tween the color property back to the original color
                    flashTweens.Add(material.DOColor(
                        Color.white,
                        "_BaseColor",
                        flashDuration
                    ));
                }
            }
        }
    }

    private void ChangeMaterialsToColor(Color color)
    {
        foreach (SkinnedMeshRenderer renderer in meshRenderers)
        {
            var propertyBlock = new MaterialPropertyBlock();
            foreach (Material material in renderer.materials)
            {
                material.SetColor("_BaseColor", color);
            }
        }
    }

    private void PerformFlash(float newHealth, float damage)
    {
        if (Mathf.Sign(damage) == 1)
        {
            TriggerFlash(damageFlashColor);
        }
        else
        {
            TriggerFlash(healFlashColor);
        }
    }
}
