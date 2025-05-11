using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    private Animator animator;

    [Header("Footstep Audio")]
    public AudioClip footstepClip;
    public float stepDistance = 1.0f;

    [Header("Detection & Movement")]
    public float detectionRange = 10f;
    public float wanderRadius = 5f;
    public float wanderInterval = 3f;
    public bool isDead = false;

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
    private bool isWalking = true;

    //footstep tracking
    private AudioSource audioSource;
    private Vector3 lastPosition;
    private float distanceAccumulated;

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

        // — initialize footstep audio system —
        // try to get an AudioSource; if you don’t have one, add it
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 1f;      // so it's 3D

        // set your starting position for the first distance check
        lastPosition = transform.position;
    }


    void Update()
    {
        if (agent == null || player == null) return;

        float dist = Vector3.Distance(transform.position, player.position);

        // 1) Attack
        if (dist <= attackRange)
        {
            if(agent.isOnNavMesh) agent.isStopped = true;
            animator?.SetBool("isWalking", false);  // if you have animator flags
            attackTimer += Time.deltaTime;
            if (attackTimer >= attackCooldown)
            {
                DoAttack();
                attackTimer = 0f;
            }
        }
        // 2) Chase
        else if (dist <= detectionRange)
        {
            if (agent.isOnNavMesh)  agent.isStopped = false;
            animator?.SetBool("isWalking", true);
            agent.SetDestination(player.position);
        }
        // 3) Wander
        else
        {
            if (agent.isOnNavMesh)  agent.isStopped = false;
            wanderTimer += Time.deltaTime;
            animator?.SetBool("isWalking", true);
            if (wanderTimer >= wanderInterval)
            {
                Vector3 newPos = RandomNavSphere(transform.position, wanderRadius, -1);
                agent.SetDestination(newPos);
                wanderTimer = 0f;
            }
        }

        HandleFootsteps();
    }

    private void DoAttack()
    {
        if (playerHealth != null)
        {
            Debug.Log($"{name} attacks {player.name} for {attackDamage} damage!");
            playerHealth.TakeDamage(attackDamage);
        }
    }

    private void HandleFootsteps()
    {
        // Only step when moving
        if (agent.velocity.magnitude > 0.1f)
        {
            // accumulate distance
            distanceAccumulated += Vector3.Distance(transform.position, lastPosition);

            if (distanceAccumulated >= stepDistance && detectionRange <= 10)
            {
                audioSource.PlayOneShot(footstepClip);
                distanceAccumulated = 0f;
            }
        }
        else
        {
            // not moving: reset accumulation so step doesn't immediately fire on move
            distanceAccumulated = 0f;
        }

        lastPosition = transform.position;
    }

    public void OnDeath()
    {
        isDead = true;
        if (agent.isOnNavMesh)  agent.isStopped = true;
        if (audioSource.isPlaying) audioSource.Stop();
    }

    public static Vector3 RandomNavSphere(Vector3 origin, float dist, int layermask)
    {
        Vector3 randDir = Random.insideUnitSphere * dist + origin;
        NavMesh.SamplePosition(randDir, out NavMeshHit navHit, dist, layermask);
        return navHit.position;
    }
}
