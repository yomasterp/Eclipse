using UnityEngine;

public class DoorOpener : MonoBehaviour
{
    [Tooltip("Drag your Door_Hinge here")]
    public DoorToggle door;

    void OnTriggerStay(Collider other)
    {
        // make sure it’s the player and they press E
        if (other.CompareTag("Player") && Input.GetKeyDown(KeyCode.E))
        {
            door.Toggle();
        }
    }
}
