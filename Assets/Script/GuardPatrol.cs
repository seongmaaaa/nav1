using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
public class GuardPatrol : MonoBehaviour
{
    public static GuardPatrol Instance { get; private set; }
    public Transform[] patrolPoints;    // 순찰 지점 배열
    public float patrolSpeed = 4f;      // 순찰 속도
    public float chaseSpeed = 5f;       // 추격 속도
    public float rayDistance = 5f;      // Ray의 길이
    public LayerMask wallLayer;         // 벽 레이어
    public float detectionRadius = 10f; // 플레이어 감지 반경
    public float alarmDuration = 10f;   // 알람 지속 시간

    private NavMeshAgent agent;         // NavMeshAgent 컴포넌트
    private Vector3 initialPosition;    // 초기 위치
    private int currentPatrolIndex;     // 현재 순찰 지점 인덱스
    private bool returningToStart;      // 초기 위치로 돌아가는 중인지 여부
    private bool isChasing;             // 추격 중인지 여부
    private bool isAlarmed;             // 알람 상태인지 여부
    private Vector3 alarmPosition;      // 알람 위치
    private float alarmEndTime;         // 알람 종료 시간
    private bool isPlayerInRoom;        // 플레이어가 방 안에 있는지 여부
    private bool playerHasKey;          // 플레이어가 열쇠를 가지고 있는지 여부

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = patrolSpeed;
        initialPosition = transform.position;
        currentPatrolIndex = 0;
        returningToStart = false;
        Instance = this;

        if (patrolPoints.Length > 0)
        {
            agent.SetDestination(patrolPoints[currentPatrolIndex].position);
        }
    }

    void Update()
    {
        if (isAlarmed)
        {
            HandleAlarmState();
        }
        else if (isChasing)
        {
            agent.SetDestination(alarmPosition); // 플레이어 추격 중인 경우 알람 위치로 이동
        }
        else
        {
            PatrolRoutine();
            AvoidWalls();
        }

        CheckPlayerDetection();
    }

    void HandleAlarmState()
    {
        if (Time.time > alarmEndTime)
        {
            isAlarmed = false;
            returningToStart = true;
            agent.speed = patrolSpeed;
            agent.SetDestination(patrolPoints[currentPatrolIndex].position);
        }
        else
        {
            agent.SetDestination(alarmPosition);
        }
    }

    void PatrolRoutine()
    {
        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            if (returningToStart)
            {
                returningToStart = false;
                currentPatrolIndex = 0;
                agent.SetDestination(patrolPoints[currentPatrolIndex].position);
            }
            else if (currentPatrolIndex >= patrolPoints.Length - 1)
            {
                agent.SetDestination(initialPosition);
                returningToStart = true;
            }
            else
            {
                currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
                agent.SetDestination(patrolPoints[currentPatrolIndex].position);
            }
        }
    }

    void AvoidWalls()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, rayDistance, wallLayer))
        {
            Vector3 avoidDirection = Vector3.Reflect(transform.forward, hit.normal);
            Vector3 newDestination = transform.position + avoidDirection * rayDistance;
            agent.SetDestination(newDestination);
        }
    }

    void CheckPlayerDetection()
    {
        if (!isPlayerInRoom && Vector3.Distance(transform.position, PlayerController.Instance.transform.position) <= detectionRadius)
        {
            StartChasing();
        }
        else if (isPlayerInRoom)
        {
            StopChasing();
        }
    }

    void StartChasing()
    {
        if (!isChasing && playerHasKey)
        {
            isChasing = true;
            agent.speed = chaseSpeed;
            alarmPosition = PlayerController.Instance.transform.position;
            OnAlarmTriggered(alarmPosition);
        }
    }

    void StopChasing()
    {
        if (isChasing)
        {
            isChasing = false;
            agent.speed = patrolSpeed;
            agent.SetDestination(patrolPoints[currentPatrolIndex].position);
        }
    }

    public void OnAlarmTriggered(Vector3 position)
    {
        isAlarmed = true;
        alarmPosition = position;
        alarmEndTime = Time.time + alarmDuration;
        agent.speed = chaseSpeed;
        agent.SetDestination(alarmPosition);
    }

    public void OnPlayerEnteredRoom(bool entered)
    {
        isPlayerInRoom = entered;
        if (entered)
        {
            StopChasing(); // 방에 들어간 플레이어를 발견하면 추격을 멈추고 순찰 상태로 복귀
        }
    }

    public void OnPlayerExitedRoom()
    {
        isPlayerInRoom = false;
    }

    public bool PlayerHasKey
    {
        get { return playerHasKey; }
        set { playerHasKey = value; }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
        }
    }
}
