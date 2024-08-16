using UnityEngine;
using System.Collections.Generic;

public class StageManager : MonoBehaviour
{
    public Stage currentStage; // ���� ���������� enum���� ����
    public EnemyAI[] stageEnemiesPrefabs; // �� ���������� �´� �� ������ �迭 (������������ �ϳ���)

    private TurretController turret;
    private List<GameObject> currentEnemies = new List<GameObject>(); // ���� ���������� ������ �����ϴ� ����Ʈ

    private void Start()
    {
        FindTurret();

        Debug.Log("Start: Initializing stage: " + currentStage); // �ʱ�ȭ �α� �߰�
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
        // �ν����Ϳ��� currentStage�� ����� ������ ȣ��
        //InitializeStage(currentStage);
    }
    private void FindTurret()
    {
        // ObjectManager�� ���� �÷��̾� ����
        turret = ObjectManager.Instance.turret;
    }

    public void InitializeStage(Stage stage)
    {
        Debug.Log("Initializing stage: " + stage); // ����� �α� �߰�

        // ���� ���������� ���� ����
        ClearCurrentEnemies();

        int stageIndex = (int)stage - 1; // Enum ������ �ε����� ��� ���� 1�� ��

        if (turret == null)
        {
            Debug.LogError("�ͷ��� �������� �ʾҽ��ϴ�.");
            return;
        }

        // ���� ���������� �´� �� �������� ����
        if (stageIndex >= 0 && stageIndex < stageEnemiesPrefabs.Length)
        {
            EnemyAI enemyPrefab = stageEnemiesPrefabs[stageIndex];
            if (enemyPrefab != null)
            {
                // ���� ��ġ�� �ͷ� �տ� ��ġ (Z ����)
                Vector3 spawnPosition = turret.transform.position + new Vector3(0, 5, 70);
                EnemyAI enemyInstance = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);

                // ���� ����Ʈ�� ObjectManager�� ���
                currentEnemies.Add(enemyInstance.gameObject);
                ObjectManager.Instance.RegisterEnemy(enemyInstance);

                Debug.Log("Spawned enemy for stage: " + stage); // ����� �α� �߰�
            }
            else
            {
                Debug.LogError("�� �������� null�Դϴ�. �������� �ε���: " + stageIndex);
            }
        }
        else
        {
            Debug.LogError("�������� �ε����� �� ������ �迭�� ������ ������ϴ�.");
        }
    }

    public void SetCurrentStage(Stage stage)
    {
        Debug.Log("Setting current stage to: " + stage); // ����� �α� �߰�
        currentStage = stage;
        InitializeStage(stage); // �ش� ���������� �ʱ�ȭ
    }

    // ���� ���������� �����ϴ� �Լ�
    public void NextStage()
    {
        Debug.Log("Proceeding to next stage from: " + currentStage); // ����� �α� �߰�
        SetCurrentStage((Stage)((int)currentStage + 1));
    }

    private void ClearCurrentEnemies()
    {
        Debug.Log("Clearing current enemies."); // ����� �α� �߰�

        // ���� ������������ ������ ��� ���� ����
        foreach (GameObject enemy in currentEnemies)
        {
            if (enemy != null)
            {
                Destroy(enemy);
                Debug.Log("Destroyed enemy: " + enemy.name); // ����� �α� �߰�
            }
        }
        currentEnemies.Clear(); // ����Ʈ �ʱ�ȭ
    }
}
