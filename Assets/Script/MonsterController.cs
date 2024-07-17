using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class MonsterController : MonoBehaviour
{
    public NavMeshAgent myAgent;
    public Transform targetTr;
    public float rayDistance = 5f; // Ray의 길이
    public LayerMask wallLayer; // 벽 레이어

    // Start is called before the first frame update
    void Start()
    {
        myAgent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, rayDistance, wallLayer))//레이어 생성
        {
            // 벽이 감지된 경우
            Debug.Log("벽 감지!");

            // 벽을 피하기 위한 새로운 방향 설정
            Vector3 avoidDirection = Vector3.Reflect(transform.forward, hit.normal);
            Vector3 newDestination = transform.position + avoidDirection * rayDistance;

            // 새로운 목적지를 설정
            myAgent.SetDestination(newDestination);
        }
        else
        {
            // 벽이 감지되지 않은 경우 원래 목적지로 이동
            myAgent.SetDestination(targetTr.position);
        }
    }
}

