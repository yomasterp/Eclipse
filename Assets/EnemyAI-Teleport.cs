// EnemyAI_T.cs
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI_T : MonoBehaviour
{
    private Animator animator;

    [Header("Detection & Movement")]
    public float detectionRange = 10f;
    public float wanderRadius = 5f;
    public float wanderInterval = 3f;

    [Header("Teleport Settings")]
    [Tooltip("How far behind the player to teleport when chase starts")]
    public float teleportOffsetDistance = 2f;

    [Header("Attack Settings")]
    public float attackRange = 2f;         // how close before we start attacking
    public float initialAttackDelay = 2f;  // how long to wait on first attack
    public float attackDamage = 50f;       // damage per attack
    public float attackCooldown = 1f;      // seconds between subsequent attacks

    private NavMeshAgent agent;
    public Transform player;               // assign in Inspector
    private PlayerHealth playerHealth;     // cached reference

    // timers & state
    private float wanderTimer;
    private float attackTimer;
    private bool inMeleeRange = false;
    private float rangeEnterTime;

    // teleport‐only‐once flag
    private bool hasTeleported = false;

    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        if (animator == null) Debug.LogError($"{name} - No Animator Found");

        agent = GetComponent<NavMeshAgent>();
        wanderTimer = wanderInterval;
        attackTimer = 0f;

        if (player == null)
            Debug.LogError($"{name} – Player transform is not assigned!");
        else
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

        // 1) Attack state
        if (dist <= attackRange)
        {
            hasTeleported = false;  // allow teleport next time

            if (!inMeleeRange)
            {
                inMeleeRange = true;
                rangeEnterTime = Time.time;
                attackTimer = 0f;
            }

            agent.isStopped = true;
            animator.SetBool("isWalking", false);
            animator.SetBool("isAttacking", true);

            float timeInRange = Time.time - rangeEnterTime;
            if (timeInRange >= initialAttackDelay)
            {
                attackTimer += Time.deltaTime;
                if (attackTimer >= attackCooldown)
                {
                    DoAttack();
                    attackTimer = 0f;
                }
            }
        }
        // 2) Chase state
        else if (dist <= detectionRange)
        {
            inMeleeRange = false;

            if (!hasTeleported)
            {
                TeleportBehindPlayer();
                hasTeleported = true;
            }

            agent.isStopped = false;
            animator.SetBool("isWalking", true);
            animator.SetBool("isAttacking", false);
            agent.SetDestination(player.position);
            wanderTimer = wanderInterval;
        }
        // 3) Wander state
        else
        {
            hasTeleported = false;
            inMeleeRange = false;

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

    private void TeleportBehindPlayer()
    {
        Vector3 behindPos = player.position - player.forward * teleportOffsetDistance;
        NavMeshHit hit;
        if (NavMesh.SamplePosition(behindPos, out hit, 1f, NavMesh.AllAreas))
            agent.Warp(hit.position);
        else
            agent.Warp(behindPos);
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
