using UnityEngine;
using UnityEngine.EventSystems;

public class Enemy : MonoBehaviour
{
    private static readonly int IS_MOVE = Animator.StringToHash("IsMove");

    [field: SerializeField] public Transform Target; // Player 방향으로

    [SerializeField] private float moveSpeed = 1f; // enemy type 에 따라 값 상세 설정하도록

    private Animator Animator { get; set; }

    private void Awake()
    {
        Animator = GetComponentInChildren<Animator>();
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Animator.SetBool(IS_MOVE, true); // enemy 0 의 경우 가까이 가지 않으면 몰려오지 않음. 특정 범위 내에 들어오면 따라오기시작함
        Vector3 moveDirection = Target.position - transform.position;
        transform.Translate(new Vector3(moveDirection.x, moveDirection.y, 0) * (moveSpeed * Time.deltaTime));
    }
}
