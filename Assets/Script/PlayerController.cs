using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private static PlayerController instance;
    public static PlayerController Instance { get { return instance; } }

    private bool hasKey = false; // 열쇠 보유 여부

    void Awake()
    {
        instance = this;
    }

    public bool HasKey
    {
        get { return hasKey; }
        set { hasKey = value; }
    }
    public float moveSpeed = 5f; // 플레이어 이동 속도
    public float lookSpeed = 2f; // 카메라 회전 속도
    public float jumpForce = 5f; // 점프 힘
    public Camera playerCamera; // 플레이어 카메라

    private Rigidbody rb;
    private bool isGrounded;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        MovePlayer();
        RotateCamera();
    }

    void MovePlayer()
    {
        // 이동 입력
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        // 이동 방향 계산
        Vector3 move = transform.right * moveHorizontal + transform.forward * moveVertical;
        Vector3 moveVelocity = move * moveSpeed;
        moveVelocity.y = rb.velocity.y; // 수직 속도 유지

        // Rigidbody를 사용하여 이동 처리
        rb.velocity = moveVelocity;

        // 점프 입력
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    void RotateCamera()
    {
        // 마우스 입력을 사용하여 카메라를 회전시킵니다.
        float mouseX = Input.GetAxis("Mouse X") * lookSpeed;
        float mouseY = -Input.GetAxis("Mouse Y") * lookSpeed;

        // 플레이어를 회전시킵니다.
        transform.Rotate(0, mouseX, 0);

        // 카메라를 상하로 회전시킵니다.
        playerCamera.transform.Rotate(mouseY, 0, 0);
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("RoomTrigger"))  // 방 트리거에 진입했을 때
        {
            NotifyGuardsPlayerEnteredRoom();
        }
    }

    void NotifyGuardsPlayerEnteredRoom()
    {
        // GuardPatrol 클래스의 인스턴스가 존재하는지 확인 후, 플레이어가 방에 들어왔음을 알립니다.
        if (GuardPatrol.Instance != null)
        {
            GuardPatrol.Instance.OnPlayerEnteredRoom(true); // 플레이어가 방에 들어갔음을 알림
        }
    }
}
