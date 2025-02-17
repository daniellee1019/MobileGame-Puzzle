using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCameraDriven : MonoBehaviour
{
    private Collider currentZone; // 현재 플레이어가 있는 Zone
    private CameraTriggerManager cameraTriggerManager;

    void Start()
    {
        // CameraTriggerManager 인스턴스를 찾아서 cameraTriggerManager 변수에 할당
        // 이를 통해, 이 스크립트는 카메라 존 정보에 접근할 수 있음
        cameraTriggerManager = FindObjectOfType<CameraTriggerManager>();
    }

    private void FixedUpdate()
    {
        foreach (var zone in cameraTriggerManager.cameraZones)
        {
            Collider zoneCollider = zone.zoneObject.GetComponent<Collider>();

            if (zoneCollider.bounds.Contains(this.transform.position))
            {
                // 플레이어가 새로운 존에 진입한 경우
                if (currentZone != zoneCollider)
                {
                    EnterZone(zoneCollider);
                    currentZone = zoneCollider;
                }
            }
            else if (currentZone == zoneCollider)
            {
                // 플레이어가 현재 존에서 벗어난 경우
                ExitZone(zoneCollider);
                currentZone = null;
            }
        }
    }

    private void EnterZone(Collider zone)
    {
        var cameraZone = cameraTriggerManager.GetCameraZoneFromCollider(zone);
        if (cameraZone != null)
        {
            cameraZone.camera.Priority = 11; // 카메라 우선순위 높임
            Debug.Log($"Entered Zone: {zone.name}");
        }
    }

    private void ExitZone(Collider zone)
    {
        var cameraZone = cameraTriggerManager.GetCameraZoneFromCollider(zone);
        if (cameraZone != null)
        {
            cameraZone.camera.Priority = 0; // 카메라 우선순위 초기화
            Debug.Log($"Exited Zone: {zone.name}");
        }
    }
}