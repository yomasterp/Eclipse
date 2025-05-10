using UnityEngine;

public class GateController : MonoBehaviour
{
    [Tooltip("How many kills required")]
    public int requiredKills = 3;
    [Tooltip("The blocking wall/cube")]
    public GameObject barrier;

    int _killCount;
    bool _opened;

    void OnEnable()
    {
        EnemyHealth.OnEnemyKilled += HandleKill;
    }

    void OnDisable()
    {
        EnemyHealth.OnEnemyKilled -= HandleKill;
    }

    void Start()
    {
        if (barrier != null) barrier.SetActive(true);

        if (UIManager.Instance != null)
            UIManager.Instance.UpdateKillCounter(0, requiredKills);
    }

    void HandleKill()
    {
        UIManager.Instance.UpdateKillCounter(_killCount, requiredKills);

        _killCount++;
        if (!_opened && _killCount >= requiredKills)
        {
            barrier.SetActive(false);
            _opened = true;
        }

        // update the on-screen kill count
        if (UIManager.Instance != null)
            UIManager.Instance.UpdateKillCounter(_killCount, requiredKills);

    }
}
