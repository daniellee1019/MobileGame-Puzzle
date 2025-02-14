using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using DG.Tweening;

public class StageManager : MonoBehaviour
{
    // enum Stage 제거 ? 이제 Day 데이터로 관리합니다.
    // public Stage currentStage; // 제거
    public static StageManager Instance;

    [Header("Boss Settings")]
    public EnemyAI[] stageEnemiesPrefabs; // 각 Day에 맞는 적 프리팹 배열 (Day별로 하나씩)

    private TurretController turret;
    private List<GameObject> currentEnemies = new List<GameObject>(); // 현재 Day의 적들을 관리하는 리스트

    // 저장된 게임 진행 데이터를 보관 (GameSaveData에는 currentDay, dayRecords, currencyData가 포함됨)
    private GameSaveData saveData;

    [Header("RewardData")]
    // 보스 보상 기준 데이터 (gamesave.json의 currencyData; 예: Day1 보상)
    public CurrencyData bossRewardBase;

    [Header("Day Transition UI")]
    public CanvasGroup dayTransitionPanel; // UI 패널
    public TextMeshProUGUI dayText; // "Day 1" → "D-1" 표시
    
    [Header("Currency Text")]
    // 재화를 표시할 TextMeshPro UI 컴포넌트 (인스펙터에서 할당)
    public TMP_Text currencyText;
    // 스프라이트 애셋에 지정한 이름 (Inspector에서 변경 가능)
    public string goldSpriteName = "GoldIcon";
    public string woodSpriteName = "WoodIcon";
    public string stoneSpriteName = "StoneIcon";


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        // 저장된 게임 데이터를 로드합니다.
        saveData = SaveManager.Instance.LoadGame();

        FindTurret();
        UpdateDisplay();

        Debug.Log("Start: Initializing Day: " + saveData.currentDay);

        // Day 전환 연출 후, 시간을 0으로 리셋한 후 보스 소환 대기
        StartCoroutine(StartDayTransition(saveData.currentDay));
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
        // 인스펙터에서 값이 변경되었을 때 호출 (필요 시 사용)
    }

    /// <summary>
    /// 새로운 Day 시작 전 연출 (Day Transition)
    /// </summary>
    private IEnumerator StartDayTransition(int day)
    {
        ClearCurrentEnemies();

        #region Day UI 설정
        // UI 요소의 상태를 초기화
        dayTransitionPanel.alpha = 1;
        dayTransitionPanel.gameObject.SetActive(true);

        // dayText를 활성화하고 기본 상태로 리셋
        dayText.gameObject.SetActive(true);
        dayText.transform.localScale = Vector3.one * 0.5f;
        // 원하는 시작 위치로 재설정 (예: 중앙에 위치)
        dayText.transform.localPosition = Vector3.zero;
        // 알파를 1로 초기화 (즉시 적용)
        dayText.DOFade(1, 0);

        // 전환 연출 시작
        dayText.text = "Day " + day;
        dayText.transform.DOScale(1.5f, 0.5f).SetEase(Ease.OutBack);
        yield return new WaitForSeconds(1f);

        // 텍스트 변경
        dayText.text = "D-" + day;
        yield return new WaitForSeconds(1f);

        // 화면 위로 이동 및 페이드아웃
        dayText.transform.DOMoveY(Screen.height + 100, 1f);
        dayText.DOFade(0, 1f);
        yield return new WaitForSeconds(1f);

        dayText.gameObject.SetActive(false);
        #endregion

        // 새 Day 시작 시 TimeManager의 시간을 초기화
        //TimeManager.Instance.ResetDayTime();

        // 여기서 보스 웨이브(적 소환)는 게임 내 시간이 12시(360초)에 도달한 후에 진행
        // 현재 Day가 시작된 후 TimeManager의 currentDayTime이 360초가 될 때까지 대기합니다.
        yield return new WaitUntil(() => TimeManager.Instance.currentDayTime >= 360f);

        // 스테이지 초기화 시작
        InitializeStage(day);
    }

    private void FindTurret()
    {
        // ObjectManager를 통해 플레이어 참조
        turret = ObjectManager.Instance.turret;
    }

    /// <summary>
    /// 해당 Day에 맞는 적(보스 등)을 소환하고 등록합니다.
    /// Day 번호를 배열 인덱스로 변환하여 적 프리팹을 선택합니다.
    /// </summary>
    public void InitializeStage(int day)
    {
        Debug.Log("Initializing Day: " + day);
        ClearCurrentEnemies();

        // 플레이어 재화 출력 (현재 Day 시작 시)
        PlayerSaveData playerData = PlayerSaveManager.Instance.LoadPlayerData();
        Debug.Log("Player currency at start of Day " + day +
                  ": Gold = " + playerData.playerCurrencyData.gold +
                  ", Wood = " + playerData.playerCurrencyData.wood +
                  ", Stone = " + playerData.playerCurrencyData.stone);

        int stageIndex = day - 1; // Day 1 -> index 0

        if (turret == null)
        {
            Debug.LogError("터렛이 설정되지 않았습니다.");
            return;
        }

        if (stageIndex >= 0 && stageIndex < stageEnemiesPrefabs.Length)
        {
            EnemyAI enemyPrefab = stageEnemiesPrefabs[stageIndex];
            if (enemyPrefab != null)
            {
                // 터렛 앞쪽 (예: Y+5, Z+70) 위치에 적을 소환
                Vector3 spawnPosition = turret.transform.position + new Vector3(0, 5, 70);
                // Y축 180도 회전하여 터렛을 향하도록 생성
                EnemyAI enemyInstance = Instantiate(enemyPrefab, spawnPosition, Quaternion.Euler(0, 180, 0));
                currentEnemies.Add(enemyInstance.gameObject);
                ObjectManager.Instance.RegisterEnemy(enemyInstance);
                Debug.Log("Spawned enemy for Day: " + day);
            }
            else
            {
                Debug.LogError("적 프리팹이 null입니다. Day 인덱스: " + stageIndex);
            }
        }
        else
        {
            Debug.LogError("Day 인덱스가 적 프리팹 배열의 범위를 벗어났습니다.");
        }
    }

    /// <summary>
    /// 현재 Day를 설정하고 해당 Day에 맞게 초기화합니다.
    /// </summary>
    /// <param name="day">새로운 Day 번호</param>
    public void SetCurrentDay(int day)
    {
        Debug.Log("Setting current day to: " + day);
        saveData.currentDay = day;
        // 새 Day로 전환할 때 TimeManager를 리셋합니다.
        TimeManager.Instance.ResetDayTime();

        StartCoroutine(StartDayTransition(day));
    }

    /// <summary>
    /// 다음 Day로 진행합니다.
    /// OnBossCleared()에서 이미 currentDay가 증가되었으므로, NextStage()는 추가 증가는 하지 않습니다.
    /// </summary>
    public void NextStage()
    {
        Debug.Log("Proceeding to next Day: " + saveData.currentDay);
        // 이미 currentDay가 증가된 상태이므로 그대로 초기화
        SetCurrentDay(saveData.currentDay);
    }

    /// <summary>
    /// 현재 Day에서 생성된 모든 적들을 제거합니다.
    /// </summary>
    private void ClearCurrentEnemies()
    {
        Debug.Log("Clearing current enemies.");
        foreach (GameObject enemy in currentEnemies)
        {
            if (enemy != null)
            {
                Destroy(enemy);
                Debug.Log("Destroyed enemy: " + enemy.name);
            }
        }
        currentEnemies.Clear();
    }

    /// <summary>
    /// 보스(현재 Day의 적)를 클리어했을 때 호출하는 함수입니다.
    /// 보스 클리어 여부를 기록하고, 재화 보상 및 다음 Day 진행을 처리합니다.
    /// </summary>
    public void OnBossCleared()
    {
        Debug.Log("Boss cleared for Day: " + saveData.currentDay);

        // 현재 Day의 보스 클리어 기록 업데이트
        DayRecordData currentDayRecord = saveData.dayRecords.Find(record => record.day == saveData.currentDay);
        if (currentDayRecord != null)
        {
            currentDayRecord.bossCleared = true;
        }

        // 보상 증가율: 매 Day마다 10% 증가 → multiplier = 1.1^(currentDay - 1)
        float rewardMultiplier = Mathf.Pow(1.1f, saveData.currentDay - 1);
        int goldReward = Mathf.RoundToInt(saveData.currencyData.gold * rewardMultiplier);
        int woodReward = Mathf.RoundToInt(saveData.currencyData.wood * rewardMultiplier);
        int stoneReward = Mathf.RoundToInt(saveData.currencyData.stone * rewardMultiplier);

        Debug.Log("Rewards for Day " + saveData.currentDay + ": Gold = " + goldReward +
                  ", Wood = " + woodReward + ", Stone = " + stoneReward);

        // 플레이어 재화에 보상 추가
        PlayerSaveData playerData = PlayerSaveManager.Instance.LoadPlayerData();
        playerData.playerCurrencyData.AddCurrency(goldReward, woodReward, stoneReward);
        PlayerSaveManager.Instance.SavePlayerData(playerData);
        UpdateDisplay();

        // 보스 처치 시 TimeManager에 알림
        TimeManager.Instance.OnBossDefeated();

        // 새 Day 시작을 위해 TimeManager의 시간을 리셋합니다.
        TimeManager.Instance.ResetDayTime();

        // 다음 Day 진행: currentDay 증가 및 새 DayRecord 생성
        saveData.currentDay++;
        DayRecordData newRecord = new DayRecordData();
        newRecord.day = saveData.currentDay;
        newRecord.bossCleared = false;
        saveData.dayRecords.Add(newRecord);

        SaveManager.Instance.SaveGame(saveData);

        // 다음 Day(스테이지) 초기화
        NextStage();
    }
    public void OnResetButtonClicked()
    {
        // 방법 1: 파일 삭제
        //SaveManager.Instance.ResetGameData_DeleteFiles();
        //PlayerSaveManager.Instance.ResetPlayerData_DeleteFiles();

        // 또는 방법 2: 기본 데이터로 덮어쓰기
        SaveManager.Instance.ResetGameData();
        PlayerSaveManager.Instance.ResetPlayerData();
    }

    /// <summary>
    /// PlayerSaveData에서 재화 정보를 불러와 텍스트에 업데이트합니다.
    /// </summary>
    public void UpdateDisplay()
    {
        // PlayerSaveManager를 통해 플레이어 데이터를 로드합니다.
        PlayerSaveData playerData = PlayerSaveManager.Instance.LoadPlayerData();
        int gold = playerData.playerCurrencyData.gold;
        int wood = playerData.playerCurrencyData.wood;
        int stone = playerData.playerCurrencyData.stone;

        // 스프라이트 태그를 사용하여 아이콘과 숫자를 표시합니다.
        currencyText.text = $"<sprite name=\"{goldSpriteName}\"> {gold}\n<sprite name=\"{woodSpriteName}\"> {wood}\n<sprite name=\"{stoneSpriteName}\"> {stone}";
    }

    /// <summary>
    /// 타임 시스템에 의해 보스가 제때 처치되지 않았을 때 호출됩니다.
    /// 모든 진행 데이터를 초기화하고 Day 1로 돌아갑니다.
    /// </summary>
    public void ResetToDay1()
    {
        Debug.Log("Resetting to Day 1 due to boss failure.");
        // GameSaveData 리셋: currentDay를 1로, dayRecords 초기화
        saveData.currentDay = 1;
        saveData.dayRecords.Clear();
        DayRecordData firstDay = new DayRecordData();
        firstDay.day = 1;
        firstDay.bossCleared = false;
        saveData.dayRecords.Add(firstDay);
        SaveManager.Instance.SaveGame(saveData);

        // 새 Day 시작 전에 TimeManager의 시간 리셋
        TimeManager.Instance.ResetDayTime();
        ClearCurrentEnemies();

        // Stage 재설정
        SetCurrentDay(1);
    }
}
