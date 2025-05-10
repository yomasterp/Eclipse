using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    private Animator animator;

    [Header("Detection & Movement")]
    public float detectionRange = 10f;
    public float wanderRadius = 5f;
    public float wanderInterval = 3f;

    [Header("Attack Settings")]
    public float attackRange = 2f;      // how close before we start attacking
    public float attackDamage = 100f;     // damage per attack
    public float attackCooldown = 1f;      // seconds between attacks

    private NavMeshAgent agent;
    public Transform player;                // assign in Inspector
    private PlayerHealth playerHealth;      // cached reference
    private float wanderTimer;
    private float attackTimer;


    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        if (animator == null)
            Debug.LogError($"{name} - No Animator Found");

        agent = GetComponent<NavMeshAgent>();
        wanderTimer = wanderInterval;
        attackTimer = attackCooldown;

        if (player == null)
            Debug.LogError($"{name} – Player transform is not assigned!");

        // try to cache PlayerHealth
        if (player != null)
        {
            playerHealth = player.GetComponent<PlayerHealth>();
            if (playerHealth == null)
                Debug.LogError($"{name} – No PlayerHealth component found on {player.name}!");
        }
    }

    void Update()
    {
        if (agent == null || player == null) return;

        float dist = Vector3.Distance(transform.position, player.position);

        // 1) If within attackRange ? stop and attack
        if (dist <= attackRange)
        {
            agent.isStopped = true;
            animator.SetBool("isWalking", false);
            animator.SetBool("isAttacking", true);
            attackTimer += Time.deltaTime;

            if (attackTimer >= attackCooldown)
            {
                DoAttack();
                attackTimer = 0f;
            }
        }
        // 2) Else if within detectionRange ? chase
        else if (dist <= detectionRange)
        {
            agent.isStopped = false;
            animator.SetBool("isWalking", true);
            animator.SetBool("isAttacking", false);
            agent.SetDestination(player.position);
            wanderTimer = wanderInterval;  // reset wandering
        }
        // 3) Else ? wander
        else
        {
            agent.isStopped = false;
            animator.SetBool("isAttacking", false);
            wanderTimer += Time.deltaTime;
            if (wanderTimer >= wanderInterval)
            {
                Vector3 newPos = RandomNavSphere(transform.position, wanderRadius, -1);
                agent.SetDestination(newPos);
                wanderTimer = 0f;
            }

            animator.SetBool("isWalking", true);

        }
    }

    private void DoAttack()
    {
        if (playerHealth != null)
        {
            Debug.Log($"{name} attacks {player.name} for {attackDamage} damage!");
            playerHealth.TakeDamage(attackDamage);
        }
    }

    public static Vector3 RandomNavSphere(Vector3 origin, float dist, int layermask)
    {
        Vector3 randDir = Random.insideUnitSphere * dist + origin;
        NavMesh.SamplePosition(randDir, out NavMeshHit navHit, dist, layermask);
        return navHit.position;
    }
}
