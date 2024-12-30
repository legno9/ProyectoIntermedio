using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EntityHealth))]
public class EntityBleeding : MonoBehaviour
{
    private List<SkinnedMeshRenderer> renderers = new();
    private List<MaterialPropertyBlock> propertyBlocks = new();

    [SerializeField] private GameObject bloodVFX;
    [SerializeField] private float bleedingSpread = 30f;
    [SerializeField][Range(0f,1f)] private float bloodSpawnDistance = 0.5f;
    [SerializeField] private float bloodPuddleHeight = 0.75f;
    [SerializeField] private float bloodPuddleRadius = 1f;

    private EntityHealth entityLife;
    private HurtCollider hurtCollider;
    private Collider bleedCollider;

    private void Awake()
    {
        bloodPuddleHeight *= transform.localScale.y;
        bloodPuddleRadius *= transform.localScale.y;

        foreach (SkinnedMeshRenderer renderer in GetComponentsInChildren<SkinnedMeshRenderer>())
        {
            if (renderer.material.HasProperty("_BloodStrength"))
            {
                renderers.Add(renderer);
                propertyBlocks.Add(new MaterialPropertyBlock());
            }
        }

        entityLife = GetComponent<EntityHealth>();
        hurtCollider = GetComponentInChildren<HurtCollider>();
        bleedCollider = hurtCollider.GetComponent<Collider>();
    }

    private void OnEnable()
    {
        //entityLife.OnHealthChanged.AddListener(UpdateBloodMaterials);
        //entityLife.OnDeath.AddListener(SpawnDeathBloodPuddle);

        hurtCollider.OnHitWithCollision.AddListener(SpawnBloodOnCollision);
        hurtCollider.OnHitWithTrigger.AddListener(SpawnBloodOnTrigger);
    }

    private void OnDisable()
    {
        //entityLife.OnHealthChanged.RemoveListener(UpdateBloodMaterials);
        //entityLife.OnDeath.RemoveListener(SpawnDeathBloodPuddle);

        hurtCollider.OnHitWithCollision.RemoveListener(SpawnBloodOnCollision);
        hurtCollider.OnHitWithTrigger.RemoveListener(SpawnBloodOnTrigger);
    }

    private void UpdateBloodMaterials(float currentHealth, float damage)
    {
        for (int i = 0; i < renderers.Count; i++)
        {
            SkinnedMeshRenderer renderer = renderers[i];
            MaterialPropertyBlock propertyBlock = propertyBlocks[i];

            float bloodStrength = 1 - entityLife.GetCurrentHealthPercentage();
            renderer.GetPropertyBlock(propertyBlock);
            propertyBlock.SetFloat("_BloodStrength", bloodStrength);
            renderer.SetPropertyBlock(propertyBlock);
        }
    }

    private void SpawnBloodOnCollision(float damage, Collision collision)
    {
        foreach (ContactPoint point in collision.contacts)
        {
            if (point.otherCollider == bleedCollider)
            {
                Vector3 spawnPos = point.point - (Vector3.ProjectOnPlane(point.point - transform.position, Vector3.up) * bloodSpawnDistance);
                SpawnBlood(damage, spawnPos, point.normal);
                return;
            }
        }
    }

    private void SpawnBloodOnTrigger(float damage, Vector3 triggerPos, Vector3 normal)
    {
        Vector3 spawnPos = triggerPos - (Vector3.ProjectOnPlane(triggerPos - transform.position, Vector3.up) * bloodSpawnDistance);
        SpawnBlood(damage, spawnPos, normal);
    }

    private void SpawnDeathBloodPuddle()
    {
        for (int i = 0; i < 20; i++)
        {
            Vector3 spawnPos = GetRandomPointInRadius(transform.position + Vector3.up * bloodPuddleHeight, bloodPuddleRadius);
            SpawnBlood(1, spawnPos, Vector3.down);
        }
    }

    private Vector3 GetRandomPointInRadius(Vector3 center, float radius)
    {
        // Generate a random angle in radians
        float angle = Random.Range(0f, Mathf.PI * 2);

        // Generate a random distance within the radius
        float distance = Random.Range(0f, radius);

        // Calculate the random point
        float x = center.x + Mathf.Cos(angle) * distance;
        float z = center.z + Mathf.Sin(angle) * distance;

        // Return the new point with the same Y coordinate
        return new Vector3(x, center.y, z);
    }

    private void SpawnBlood(float amount, Vector3 spawnPos, Vector3 spawnDir)
    {
        for (int i = 0; i < amount; i++)
        {
            float spread = Random.Range(-bleedingSpread / 2, bleedingSpread / 2);
            //Vector3 spawnDirOnPlaneNormalized = Vector3.ProjectOnPlane(spawnDir, Vector3.up).normalized;
            Vector3 spawnDirOnPlaneNormalized = spawnDir;
            Quaternion spawnRotation = Quaternion.LookRotation(spawnDirOnPlaneNormalized) * Quaternion.Euler(0, spread, 0);
            Instantiate(bloodVFX, spawnPos, spawnRotation).GetComponent<VFXResizer>().ChangeSize(transform.localScale.y * 0.25f);
        }
    }
}
