using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private BoxCollider2D mapBounds;
    [SerializeField] private float followSpeed = 8f;
    [SerializeField] private Vector3 offset = new Vector3(0f, 0f, -10f);

    private Camera cam;

    private void Awake()
    {
        cam = GetComponent<Camera>();
    }

    private void LateUpdate()
    {
        if (target == null) return;

        Vector3 targetPosition = target.position + offset;
        targetPosition.z = offset.z;

        if (mapBounds != null)
            targetPosition = ClampToMapBounds(targetPosition);

        transform.position = Vector3.Lerp(
            transform.position,
            targetPosition,
            followSpeed * Time.deltaTime
        );
    }

    public void SetMapBounds(BoxCollider2D newMapBounds)
    {
        mapBounds = newMapBounds;
    }

    private Vector3 ClampToMapBounds(Vector3 position)
    {
        Bounds bounds = mapBounds.bounds;

        float halfHeight = cam.orthographicSize;
        float halfWidth = halfHeight * cam.aspect;

        float minX = bounds.min.x + halfWidth;
        float maxX = bounds.max.x - halfWidth;
        float minY = bounds.min.y + halfHeight;
        float maxY = bounds.max.y - halfHeight;

        position.x = minX > maxX ? bounds.center.x : Mathf.Clamp(position.x, minX, maxX);
        position.y = minY > maxY ? bounds.center.y : Mathf.Clamp(position.y, minY, maxY);

        return position;
    }

    public void SnapToTarget()
    {
        if (target == null) return;

        Vector3 targetPosition = target.position + offset;
        targetPosition.z = offset.z;

        if (mapBounds != null)
            targetPosition = ClampToMapBounds(targetPosition);

        transform.position = targetPosition;
    }
}
