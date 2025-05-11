using UnityEngine;
using UnityEngine.UI;

public class ShopUI : MonoBehaviour
{
    public Button upgradeMovementButton;
    public Button upgradeDashButton;

    public int movementCost = 1;
    public int dashCost = 2;

    public float speedIncrement = 2f;
    public float dashIncrement = 3f;

    public int movementCostStep = 1;
    public int dashCostStep = 2;

    public PlayerMovement player;
    public GameObject shopPanel;

    private void Start()
    {
        if (upgradeMovementButton == null) Debug.LogError("UpgradeMovementButton not set!");
        if (upgradeDashButton == null) Debug.LogError("UpgradeDashButton not set!");
        if (player == null) Debug.LogError("Player reference not set!");
        if (GlintCollector.Instance == null) Debug.LogError("No GlintCollector in scene!");

        upgradeMovementButton.onClick.AddListener(() =>
        {
            Debug.Log("Clicked Upgrade Movement — glints before: " + GlintCollector.Instance.glintCount);
            Debug.Log($"Attempting upgrade: glints = {GlintCollector.Instance.glintCount}, cost = {movementCost}");
            if (GlintCollector.Instance.SpendGlints(movementCost))
            {
                player.speed += speedIncrement;
                movementCost += movementCostStep;              // bump cost up
                Debug.Log($"Speed upgraded to {player.speed} — next costs {movementCost} glints");
                Debug.Log($"  Success! New speed = {player.speed}, remaining glints = {GlintCollector.Instance.glintCount}");
            }
            else
            {
                Debug.LogWarning("Not enough glints to upgrade speed!");
            }
            
        });

        upgradeDashButton.onClick.AddListener(() =>
        {
            if (GlintCollector.Instance.SpendGlints(dashCost))
            {
                player.dashSpeed += dashIncrement;
                dashCost += dashCostStep;                  // bump cost up
                Debug.Log($"Dash upgraded to {player.dashSpeed} — next costs {dashCost} glints");
            }
        });
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            bool open = !shopPanel.activeSelf;
            shopPanel.SetActive(open);

            // pause/unpause
            Time.timeScale = open ? 0f : 1f;
            Cursor.lockState = open ? CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible = open;

            // disable camera look when shop open
            player.canLook = !open;
        }
    }


}
