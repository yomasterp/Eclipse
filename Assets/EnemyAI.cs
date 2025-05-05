using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public float detectionRange = 10f;
    public float wanderRadius = 5f;
    public float wanderInterval = 3f;

    private NavMeshAgent agent;
    public Transform player;   // expose in Inspector instead of FindWithTag
    private float timer;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        if (player == null)
            Debug.LogError($"{name} – Player transform is not assigned!");
        timer = wanderInterval;
    }

    void Update()
    {
        if (agent == null || player == null) return;
        float dist = Vector3.Distance(transform.position, player.position);
        if (dist <= detectionRange)
        {
            Debug.Log($"{name} – Chasing Player…");
            bool ok = agent.SetDestination(player.position);
            Debug.Log($"{name} – SetDestination returned {ok}, dest={agent.destination}");
        }
        else
        {
            Debug.Log($"{name} – Wandering");
            if (timer >= wanderInterval)
            {
                Vector3 newPos = RandomNavSphere(transform.position, wanderRadius, -1);
                agent.SetDestination(newPos);
                timer = 0;
            }
            timer += Time.deltaTime;
        }
    }

    public static Vector3 RandomNavSphere(Vector3 origin, float dist, int layermask)
    {
        Vector3 randDir = Random.insideUnitSphere * dist + origin;
        NavMesh.SamplePosition(randDir, out NavMeshHit navHit, dist, layermask);
        return navHit.position;
    }
}
