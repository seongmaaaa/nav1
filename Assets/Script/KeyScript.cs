using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyScript : MonoBehaviour
{
    public float pickupRadius = 3f; // 열쇠를 획득할 수 있는 반경

    void Update()
    {
        // 플레이어와의 거리 계산
        if (Vector3.Distance(transform.position, PlayerController.Instance.transform.position) <= pickupRadius)
        {
            // 플레이어에게 열쇠를 획득
            PlayerController.Instance.HasKey = true;
            Destroy(gameObject); // 열쇠 오브젝트 제거
        }
    }
}
