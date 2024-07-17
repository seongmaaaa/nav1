using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform player;
    public Vector3 offset;

    void Update()
    {
        // 카메라 위치 설정
        transform.position = player.position + offset;

        transform.LookAt(player);
    }
}

