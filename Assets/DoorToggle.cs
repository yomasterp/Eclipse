using UnityEngine;
using System.Collections;

public class DoorToggle : MonoBehaviour
{
    public float openAngle = 90f;
    public float speed = 2f;
    bool isOpen = false;
    Quaternion closedRot, openRot;

    void Start()
    {
        closedRot = transform.rotation;
        openRot = closedRot * Quaternion.Euler(0, openAngle, 0);
    }

    public void Toggle()
    {
        StopAllCoroutines();
        StartCoroutine(Rotate(isOpen ? closedRot : openRot));
        isOpen = !isOpen;
    }

    IEnumerator Rotate(Quaternion target)
    {
        while (Quaternion.Angle(transform.rotation, target) > 0.1f)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, target, Time.deltaTime * speed);
            yield return null;
        }
    }
}
