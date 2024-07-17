using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuardManager : MonoBehaviour
{
    public static GuardManager Instance { get; private set; }
    public List<GuardPatrol> guards; // 모든 경비원 리스트

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    public void AlertGuards(Vector3 alarmPosition)
    {
        foreach (var guard in guards)
        {
            guard.OnAlarmTriggered(alarmPosition);
        }
    }
}
