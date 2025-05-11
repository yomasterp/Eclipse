// GlintCollector.cs
using UnityEngine;
using System;

public class GlintCollector : MonoBehaviour
{
    public static GlintCollector Instance { get; private set; }
    public int glintCount { get; private set; }

    /// <summary>Fired whenever glintCount changes</summary>
    public event Action<int> onGlintCountChanged;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>Call this to add glints</summary>
    public void AddGlints(int amount)
    {
        glintCount += amount;
        Debug.Log($"Glints collected: {glintCount}");
        onGlintCountChanged?.Invoke(glintCount);
    }

    public bool SpendGlints(int amount)
    {
        Debug.Log($"[Collector] Spend Glints called with amount={amount}. Current={glintCount}");
        if (glintCount >= amount)
        {
            glintCount -= amount;
            Debug.Log($"[Collector] Spend successful. New glintCount={glintCount}");
            onGlintCountChanged?.Invoke(glintCount);
            return true;
        }
        Debug.LogWarning($"[Collector] Spend failed. Not enough glints ({glintCount} < {amount})");
        return false;
    }
}