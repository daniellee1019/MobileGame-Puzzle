using UnityEngine;
using Cinemachine;
using System.Linq; // LINQ 사용

/// <summary>
/// CameraTriggerManager는 여러 카메라 존을 관리하고,
/// 각 존에 진입하거나 떠날 때 카메라 전환을 처리하는 클래스이다.
/// LINQ를 사용하여 특정 Collider와 연결된 CameraZone을 효율적으로 찾을 수 있음.
/// LINQ를 사용하면 배열, 리스트, XML, 데이터베이스, 그 외 다양한 데이터 소스에 대해 통일된 방식으로 쿼리를 작성할 수 있다.
/// </summary>
public class CameraTriggerManager : MonoBehaviour
{
    // CameraZone 클래스를 정의. 이 클래스는 각 카메라 존의 설정을 저장.
    [System.Serializable]
    public class CameraZone
    {
        public CinemachineVirtualCamera camera; // 카메라 존에 사용될 Cinemachine 가상 카메라
        public Vector3 position; // 카메라 존의 위치
        public Vector3 rotation; // 카메라 존의 회전 (오일러 각도)
        public Vector3 boxsize; // 카메라 존의 크기
        public GameObject zoneObject; // 카메라 존을 나타내는 게임 오브젝트
    }

    [SerializeField] public CameraZone[] cameraZones; // 에디터에서 설정할 수 있는 카메라 존 배열
    private CinemachineVirtualCamera activeCamera = null; // 현재 활성화된 가상 카메라

    private void Start()
    {
        int zoneIndex = 0;
        foreach (var zone in cameraZones)
        {
            // 각 카메라 존에 대한 게임 오브젝트를 생성하고 설정함
            string zoneObjectName = zone.camera.name + "_Zone_" + zoneIndex;
            GameObject zoneObj = new GameObject(zoneObjectName);
            zoneObj.transform.position = zone.position;
            zoneObj.transform.rotation = Quaternion.Euler(zone.rotation); // 회전 설정
            zoneObj.transform.parent = this.transform;

            //카메라존 오브젝트에 태그 추가
            zoneObj.tag = "CameraZone";

            // BoxCollider 컴포넌트를 추가하고 크기 및 트리거 설정함
            BoxCollider collider = zoneObj.AddComponent<BoxCollider>();
            collider.size = zone.boxsize;
            collider.isTrigger = true;

            // ZoneIndex 컴포넌트를 추가하여 각 존을 식별할 수 있는 인덱스를 설정함
            CameraZoneIndex zIdx = zoneObj.AddComponent<CameraZoneIndex>();
            zIdx.index = zoneIndex;

            // 생성된 게임 오브젝트를 CameraZone의 zoneObject로 설정함
            zone.zoneObject = zoneObj;
            Debug.Log(zoneObj + "," + zoneIndex);

            // 각 카메라의 우선 순위를 초기화
            activeCamera = zone.camera;
            activeCamera.Priority = 0;
            zoneIndex++;
        }
    }

    /// <summary>
    /// 주어진 Collider와 연결된 CameraZone을 찾는다.
    /// LINQ의 FirstOrDefault를 사용하여 효율적으로 해당 조건을 만족하는 첫 번째 요소를 반환함.
    /// </summary>
    /// <param name="collider">찾고자 하는 Collider</param>
    /// <returns>Collider와 연결된 CameraZone, 없으면 null 반환</returns>
    public CameraZone GetCameraZoneFromCollider(Collider collider)
    {
        // LINQ를 사용하여 collider와 연결된 CameraZone을 찾고 반환
        return cameraZones.FirstOrDefault(zone => zone.zoneObject.GetComponent<Collider>() == collider);
    }

    // 유니티 에디터의 Gizmo를 사용하여 카메라 존의 위치와 크기를 시각화
    /// <summary>
    /// Matrix4x4.TRS 행렬을 사용해서 로테이션을 조절해야함.
    /// 3D 오브젝트의 공간 변환을 효과적으로 관리할 수 있는 매우 중요한 도구이다.
    /// 이 메서드를 통해 복잡한 3D 변환을 간단한 API 호출로 쉽게 처리할 수 있음.
    /// </summary>
    private void OnDrawGizmos()
    {
        foreach (var zone in cameraZones)
        {
            // Gizmo의 색상을 녹색으로 설정
            Gizmos.color = Color.green;

            // 위치, 회전, 크기를 반영한 매트릭스 설정
            Matrix4x4 oldGizmosMatrix = Gizmos.matrix; // 현재 Gizmos 매트릭스를 저장
            Gizmos.matrix = Matrix4x4.TRS(zone.position, Quaternion.Euler(zone.rotation), Vector3.one);

            // 회전을 반영한 와이어 프레임 큐브를 그림
            Gizmos.DrawWireCube(Vector3.zero, zone.boxsize);

            // 원래의 Gizmos 매트릭스로 복원
            Gizmos.matrix = oldGizmosMatrix;
        }
    }
}



