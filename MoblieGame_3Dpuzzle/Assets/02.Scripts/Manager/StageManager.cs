using UnityEngine;
using System.Collections.Generic;

public class StageManager : MonoBehaviour
{
    public Stage currentStage; // 현재 스테이지를 enum으로 관리
    public EnemyAI[] stageEnemiesPrefabs; // 각 스테이지에 맞는 적 프리팹 배열 (스테이지별로 하나씩)

    private TurretController turret;
    private List<GameObject> currentEnemies = new List<GameObject>(); // 현재 스테이지의 적들을 관리하는 리스트

    private void Start()
    {
        FindTurret();

        Debug.Log("Start: Initializing stage: " + currentStage); // 초기화 로그 추가
        InitializeStage(currentStage);
    }

    private void Update()
    {
        if (turret == null)
        {
            FindTurret();
        }
    }

    private void OnValidate()
    {
        // 인스펙터에서 currentStage가 변경될 때마다 호출
        //InitializeStage(currentStage);
    }
    private void FindTurret()
    {
        // ObjectManager를 통해 플레이어 참조
        turret = ObjectManager.Instance.turret;
    }

    public void InitializeStage(Stage stage)
    {
        Debug.Log("Initializing stage: " + stage); // 디버그 로그 추가

        // 기존 스테이지의 적들 제거
        ClearCurrentEnemies();

        int stageIndex = (int)stage - 1; // Enum 값에서 인덱스를 얻기 위해 1을 뺌

        if (turret == null)
        {
            Debug.LogError("터렛이 설정되지 않았습니다.");
            return;
        }

        // 현재 스테이지에 맞는 적 프리팹을 선택
        if (stageIndex >= 0 && stageIndex < stageEnemiesPrefabs.Length)
        {
            EnemyAI enemyPrefab = stageEnemiesPrefabs[stageIndex];
            if (enemyPrefab != null)
            {
                // 적의 위치를 터렛 앞에 배치 (Z 방향)
                Vector3 spawnPosition = turret.transform.position + new Vector3(0, 5, 70);
                EnemyAI enemyInstance = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);

                // 적을 리스트와 ObjectManager에 등록
                currentEnemies.Add(enemyInstance.gameObject);
                ObjectManager.Instance.RegisterEnemy(enemyInstance);

                Debug.Log("Spawned enemy for stage: " + stage); // 디버그 로그 추가
            }
            else
            {
                Debug.LogError("적 프리팹이 null입니다. 스테이지 인덱스: " + stageIndex);
            }
        }
        else
        {
            Debug.LogError("스테이지 인덱스가 적 프리팹 배열의 범위를 벗어났습니다.");
        }
    }

    public void SetCurrentStage(Stage stage)
    {
        Debug.Log("Setting current stage to: " + stage); // 디버그 로그 추가
        currentStage = stage;
        InitializeStage(stage); // 해당 스테이지로 초기화
    }

    // 다음 스테이지로 진행하는 함수
    public void NextStage()
    {
        Debug.Log("Proceeding to next stage from: " + currentStage); // 디버그 로그 추가
        SetCurrentStage((Stage)((int)currentStage + 1));
    }

    private void ClearCurrentEnemies()
    {
        Debug.Log("Clearing current enemies."); // 디버그 로그 추가

        // 현재 스테이지에서 생성된 모든 적을 제거
        foreach (GameObject enemy in currentEnemies)
        {
            if (enemy != null)
            {
                Destroy(enemy);
                Debug.Log("Destroyed enemy: " + enemy.name); // 디버그 로그 추가
            }
        }
        currentEnemies.Clear(); // 리스트 초기화
    }
}
