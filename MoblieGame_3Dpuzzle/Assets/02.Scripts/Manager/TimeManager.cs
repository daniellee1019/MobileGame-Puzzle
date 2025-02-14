using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public static TimeManager Instance;

    // 전체 하루 길이 (24시간 → 24 * 30초 = 720초)
    public float dayDuration = 720f;
    // 현재 하루 경과 시간 (초)
    public float currentDayTime = 0f;

    // 보스 웨이브 구간: 12:00 (360초) ~ 17:00 (510초)
    public float bossWaveStart = 360f;
    public float bossWaveEnd = 510f;

    // 현재 Day의 보스가 처치되었는지 여부
    public bool bossDefeated = false;

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
        }
    }

    private void Update()
    {
        currentDayTime += Time.deltaTime;

        // 하루가 끝나면 타이머를 초기화하고 보스 플래그 리셋
        if (currentDayTime >= dayDuration)
        {
            ResetDayTime();
        }

        // 보스 웨이브 종료 시간에 도달했는데 보스가 아직 처치되지 않았다면
        if (currentDayTime >= bossWaveEnd && !bossDefeated)
        {
            Debug.Log("보스가 17:00까지 처치되지 않았습니다. Day 1으로 리셋합니다.");
            StageManager.Instance.ResetToDay1();
            ResetDayTime();
        }
    }

    /// <summary>
    /// 현재 Day의 시간을 0으로 초기화하고 보스 플래그를 리셋합니다.
    /// </summary>
    public void ResetDayTime()
    {
        currentDayTime = 0f;
        bossDefeated = false;
    }

    /// <summary>
    /// 현재 진행 중인 시간을 "HH:MM" 형태로 반환합니다.
    /// (1시간 = 30초)
    /// </summary>
    public string GetFormattedTime()
    {
        float gameHours = currentDayTime / 30f;
        int hours = Mathf.FloorToInt(gameHours);
        int minutes = Mathf.FloorToInt((gameHours - hours) * 60f);
        return string.Format("{0:00}:{1:00}", hours, minutes);
    }

    /// <summary>
    /// 보스 처치 시 StageManager에서 호출합니다.
    /// </summary>
    public void OnBossDefeated()
    {
        bossDefeated = true;
    }
}
