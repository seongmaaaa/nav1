using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
public class GuardPatrol : MonoBehaviour
{
    public static GuardPatrol Instance { get; private set; }
    public Transform[] patrolPoints;    // ���� ���� �迭
    public float patrolSpeed = 4f;      // ���� �ӵ�
    public float chaseSpeed = 5f;       // �߰� �ӵ�
    public float rayDistance = 5f;      // Ray�� ����
    public LayerMask wallLayer;         // �� ���̾�
    public float detectionRadius = 10f; // �÷��̾� ���� �ݰ�
    public float alarmDuration = 10f;   // �˶� ���� �ð�

    private NavMeshAgent agent;         // NavMeshAgent ������Ʈ
    private Vector3 initialPosition;    // �ʱ� ��ġ
    private int currentPatrolIndex;     // ���� ���� ���� �ε���
    private bool returningToStart;      // �ʱ� ��ġ�� ���ư��� ������ ����
    private bool isChasing;             // �߰� ������ ����
    private bool isAlarmed;             // �˶� �������� ����
    private Vector3 alarmPosition;      // �˶� ��ġ
    private float alarmEndTime;         // �˶� ���� �ð�
    private bool isPlayerInRoom;        // �÷��̾ �� �ȿ� �ִ��� ����
    private bool playerHasKey;          // �÷��̾ ���踦 ������ �ִ��� ����

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
            agent.SetDestination(alarmPosition); // �÷��̾� �߰� ���� ��� �˶� ��ġ�� �̵�
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
            StopChasing(); // �濡 �� �÷��̾ �߰��ϸ� �߰��� ���߰� ���� ���·� ����
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
