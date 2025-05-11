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
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
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
        if (glintCount >= amount)
        {
            glintCount -= amount;
            onGlintCountChanged?.Invoke(glintCount);
            return true;
        }
        return false;
    }

}