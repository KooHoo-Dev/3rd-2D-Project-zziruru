using UnityEngine;

public class Player : MonoBehaviour
{
    [field: SerializeField] private Transform MousePoint { get; set; }

    private void Update()
    {
        Vector3 targetPosition = MousePoint.position;
        Vector3 direction = (targetPosition - transform.position).normalized;
        transform.up = direction;

    // TODO: mousepoint 따라서 8방향 회전하도록 sprite 설정
    }

    // mousePoint 클릭 좌표를 받아와서 Bullet 생성(생성할때 좌표넘겨주면 그 좌표로 이동하는 bullet 생성)
}
