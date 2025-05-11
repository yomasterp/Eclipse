using UnityEngine;
using UnityEngine.AI;

public class EnemyAI_Teleport : MonoBehaviour
{
    private Animator animator;
    private NavMeshAgent agent;
    public Transform player;               // assign in Inspector
    private PlayerHealth playerHealth;         // cached reference

    [Header("Detection & Movement")]
    public float detectionRange = 10f;
    public float wanderRadius = 5f;
    public float wanderInterval = 3f;

    [Header("Teleport Settings")]
    [Tooltip("How far behind the player to blink when chase begins")]
    public float teleportOffsetDistance = 2f;

    [Header("Attack Settings")]
    public float attackRange = 2f;     // how close before we start attacking
    public float initialAttackDelay = 2f;     // wait this long before first hit
    public float attackDamage = 50f;
    public float attackCooldown = 1f;

    // runtime state
    private float wanderTimer;
    private float attackTimer;
    private bool inMeleeRange;
    private float rangeEnterTime;
    private bool hasTeleported;

    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        if (animator == null) Debug.LogError($"{name} – no Animator found!");

        agent = GetComponent<NavMeshAgent>();
        wanderTimer = wanderInterval;
        attackTimer = 0f;
        inMeleeRange = false;
        hasTeleported = false;

        if (player == null)
            Debug.LogError($"{name} – Player transform not assigned!");
        else
        {
            playerHealth = player.GetComponent<PlayerHealth>();
            if (playerHealth == null)
                Debug.LogError($"{name} – No PlayerHealth on {player.name}!");
        }
    }

    void Update()
    {
        if (agent == null || player == null) return;

        float dist = Vector3.Distance(transform.position, player.position);

        // 1) Attack state
        if (dist <= attackRange)
        {
            // reset teleport flag for next chase
            hasTeleported = false;

            if (!inMeleeRange)
            {
                inMeleeRange = true;
                rangeEnterTime = Time.time;
                attackTimer = 0f;
            }

            if (agent.isOnNavMesh)
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

            // teleport only once per chase-entry
            if (!hasTeleported)
            {
                TeleportBehindPlayer();
                hasTeleported = true;
            }

            if (agent.isOnNavMesh)
            {
                agent.isStopped = false;
                animator.SetBool("isWalking", true);
                animator.SetBool("isAttacking", false);
                agent.SetDestination(player.position);
            }

            wanderTimer = wanderInterval;
        }
        // 3) Wander state
        else
        {
            hasTeleported = false;
            inMeleeRange = false;

            if (agent.isOnNavMesh)
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
    }

    private void TeleportBehindPlayer()
    {
        // Desired spot
        Vector3 behind = player.position - player.forward * teleportOffsetDistance;

        // Try sampling the NavMesh at that spot
        if (NavMesh.SamplePosition(behind, out NavMeshHit hit, 1f, NavMesh.AllAreas))
        {
            agent.Warp(hit.position);
        }
        else
        {
            // fallback to raw teleport + warp
            agent.Warp(behind);
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
