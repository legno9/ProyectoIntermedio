using UnityEngine;
using DG.Tweening;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float startSpeed = 10f;
    [SerializeField] private float lifetime = 5f;
    [SerializeField] private float timeToDieAfterCollision = 0f;
    [SerializeField] private float damage = 5f;
    [SerializeField] private GameObject explosionPrefab;

    private HitCollider hitCollider;

    public void Init(float startSpeed, float lifetime, float damage)
    {
        this.startSpeed = startSpeed;
        this.lifetime = lifetime;
        this.damage = damage;
    }

    private void Awake()
    {
        hitCollider = GetComponent<HitCollider>();
    }

    private void OnEnable()
    {
        hitCollider?.OnHit.AddListener(PerformDestruction);
    }

    private void Start()
    {
        GetComponent<Rigidbody>().linearVelocity = transform.forward * startSpeed;
        DOVirtual.DelayedCall(lifetime,() =>Destroy(gameObject));
    }


    private void OnDisable()
    {
        hitCollider?.OnHit.RemoveListener(PerformDestruction);
    }

    Tween deathTween = null;
    private void OnCollisionEnter(Collision collision)
    {
        if (deathTween == null)
        {
            deathTween = DOVirtual.DelayedCall(
                timeToDieAfterCollision,
                () =>
                {
                    Instantiate(explosionPrefab, transform.position, Quaternion.identity).GetComponent<IHitter>().SetDamage(damage);
                    Destroy(gameObject);
                }
            );
        }
    }

    public void PerformDestruction()
    {
        if (deathTween == null)
        {
            deathTween = DOVirtual.DelayedCall(
                timeToDieAfterCollision,
                () =>
                {
                    Instantiate(explosionPrefab, transform.position, Quaternion.identity);
                    Destroy(gameObject);
                }
            );
        }
    }
}
