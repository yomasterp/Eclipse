using UnityEngine;
using UnityEngine.UI;

public class ShopUI : MonoBehaviour
{
    public Button upgradeMovementButton;
    public Button upgradeDashButton;

    public int movementCost = 1;
    public int dashCost = 2;

    public PlayerMovement player;
    public GameObject shopPanel;

    private void Start()
    {
        upgradeMovementButton.onClick.AddListener(() =>
        {
            if (GlintCollector.Instance.SpendGlints(movementCost))
            {
                player.speed += 2f;
                Debug.Log("Speed upgraded!");
            }
            else
            {
                Debug.Log("Not enough glints to upgrade speed.");
            }
        });

        upgradeDashButton.onClick.AddListener(() =>
        {
            if (GlintCollector.Instance.SpendGlints(dashCost))
            {
                player.dashSpeed += 5f;
                Debug.Log("Dash upgraded!");
            }
            else
            {
                Debug.Log("Not enough glints to upgrade dash.");
            }
        });
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            Debug.Log("Toggling shop panel...");
            shopPanel.SetActive(!shopPanel.activeSelf);
        }
    }


}
