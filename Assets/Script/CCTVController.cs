using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CCTVController : MonoBehaviour
{
    public float rotationSpeed = 30f; // 회전 속도
    public float detectionAngle = 30f; // 인식 각도
    public float detectionDistance = 6f; // 인식 거리
    public LayerMask playerLayer; // 플레이어 레이어
    public LayerMask obstacleLayer; // 장애물 레이어
    public GameObject alarmLight; // 알람 표시 (광원 또는 경고 등)

    private bool alarmTriggered = false; // 알람 상태

    void Update()
    {
        // CCTV 자동 회전
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);

        // 플레이어 탐지
        DetectPlayer();
    }

    void DetectPlayer()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, detectionDistance, playerLayer);
        foreach (var hitCollider in hitColliders)
        {
            Vector3 directionToPlayer = (hitCollider.transform.position - transform.position).normalized;
            float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);

            if (angleToPlayer < detectionAngle / 2)
            {
                // 장애물 체크
                if (!Physics.Raycast(transform.position, directionToPlayer, detectionDistance, obstacleLayer))
                {
                    // 플레이어 발각
                    TriggerAlarm();
                    break;
                }
            }
        }
    }

    void TriggerAlarm()
    {
        if (!alarmTriggered)
        {
            alarmTriggered = true;
            alarmLight.SetActive(true); // 알람 표시 활성화
            Debug.Log("플레이어 발각! 알람이 울립니다.");

            
            GuardManager.Instance.AlertGuards(transform.position);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionDistance);
        Gizmos.color = Color.blue;
        Vector3 frontRayPoint = transform.position + (transform.forward * detectionDistance);
        Gizmos.DrawLine(transform.position, frontRayPoint);
        Gizmos.DrawRay(transform.position, Quaternion.Euler(0, detectionAngle / 2, 0) * transform.forward * detectionDistance);
        Gizmos.DrawRay(transform.position, Quaternion.Euler(0, -detectionAngle / 2, 0) * transform.forward * detectionDistance);
    }
}
