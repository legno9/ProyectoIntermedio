using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.AI;

public class BossController : MonoBehaviour
{
    [SerializeField] private List<BossAttack> attacks;
    [SerializeField] private Transform playerTransform;
    private Animator animator;
    private NavMeshAgent navMeshAgent;
    private bool attacking = false;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        foreach (BossAttack attack in attacks)
        {
            attack.ReduceCooldown(Time.deltaTime);
        }

        if (!attacking)
        {
            float distance = Vector3.Distance(transform.position, playerTransform.position);
            var usableAttacks = attacks.Where(a => a.IsReady && distance < a.MinDistance);

            if (usableAttacks.Any())
            {
                int index = UnityEngine.Random.Range(0, usableAttacks.Count() - 1);
                BossAttack attackToUse = usableAttacks.ElementAt(index);
                int comboLength = Mathf.FloorToInt((distance / attackToUse.MinDistance) * attackToUse.MaxComboLength) + 1;
                Debug.Log("Combo length: " + comboLength);

                animator.SetInteger("ComboLength", comboLength);
                animator.SetTrigger(attackToUse.TriggerName);

                attackToUse.ResetCooldown();
                attacking = true;
                navMeshAgent.isStopped = true;
            }
            else
            {
                navMeshAgent.SetDestination(playerTransform.position);
            }
        }
    }

    public void AttackStarted()
    {

    }

    public void AttackFinished()
    {
        attacking = false;
        navMeshAgent.isStopped = false;
    }
}

[Serializable]
public class BossAttack
{
    [field: SerializeField] public string TriggerName { get; set; }
    [field: SerializeField] public bool IsStrong { get; set; }
    [field: SerializeField] public int MaxComboLength { get; set; }
    [field: SerializeField] public float MinDistance { get; set; }
    [field: SerializeField] public float Cooldown { get; set; }

    private float currentCooldown = 0f;
    public bool IsReady { get => currentCooldown <= 0; }
    public void ResetCooldown() => currentCooldown = Cooldown;
    public void ReduceCooldown(float amount) => currentCooldown -= amount;
}
