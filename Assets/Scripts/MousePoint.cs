using UnityEngine;
using UnityEngine.InputSystem;

public class MousePoint : MonoBehaviour
{
    private void Update()
    {
        Vector2 mousePosition = Mouse.current.position.ReadValue();
        Vector3 worldMousePoint = Camera.main.ScreenToWorldPoint(mousePosition);
        worldMousePoint.z = transform.position.z;
        transform.position = worldMousePoint;
    }
}
