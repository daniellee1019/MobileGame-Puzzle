using UnityEngine;
using System.IO;
using System.Collections.Generic;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance;
    private string saveFilePath; // persistentDataPath를 이용한 저장 경로

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
        // persistentDataPath 내에 gamesave.json 파일 경로 설정
        saveFilePath = Path.Combine(Application.persistentDataPath, "gamesave.json");
    }

    /// <summary>
    /// 게임 진행 데이터를 로드합니다.
    /// 먼저 persistentDataPath에 저장된 파일이 있는지 확인하고,
    /// 없으면 Resources/Data 폴더에서 초기 데이터를 읽어옵니다.
    /// </summary>
    public GameSaveData LoadGame()
    {
        // persistentDataPath에 저장된 파일이 있다면 우선 읽어옵니다.
        if (File.Exists(saveFilePath))
        {
            string json = File.ReadAllText(saveFilePath);
            GameSaveData data = JsonUtility.FromJson<GameSaveData>(json);
            Debug.Log("persistentDataPath에서 게임 데이터 로드 성공: " + saveFilePath);
            return data;
        }
        else
        {
            // 파일이 없으면 Resources 폴더에서 로드 시도 (파일명 확장자 없이 "gamesave")
            TextAsset jsonText = Resources.Load<TextAsset>("Data/gamesave");
            if (jsonText != null)
            {
                GameSaveData data = JsonUtility.FromJson<GameSaveData>(jsonText.text);
                Debug.Log("Resources에서 게임 데이터 로드 성공");
                return data;
            }
            else
            {
                Debug.LogError("gamesave.json 파일을 찾을 수 없습니다. Resources/Data 폴더에 파일이 있는지 확인하세요.");
                return CreateNewGameSave();
            }
        }
    }

    /// <summary>
    /// 게임 데이터를 persistentDataPath에 저장합니다.
    /// Resources 폴더는 수정할 수 없으므로, 저장은 persistentDataPath를 사용합니다.
    /// </summary>
    public void SaveGame(GameSaveData data)
    {
        string json = JsonUtility.ToJson(data, true);
        // 저장 전 대상 디렉토리가 없으면 생성
        Directory.CreateDirectory(Path.GetDirectoryName(saveFilePath));
        File.WriteAllText(saveFilePath, json);
        Debug.Log("게임 데이터 저장 완료: " + saveFilePath);
    }

    /// <summary>
    /// 저장 파일이 없을 경우 새 게임 데이터를 생성합니다.
    /// </summary>
    public GameSaveData CreateNewGameSave()
    {
        GameSaveData data = new GameSaveData();
        data.currentDay = 1;
        data.dayRecords = new List<DayRecordData>();
        DayRecordData firstDay = new DayRecordData();
        firstDay.day = 1;
        firstDay.bossCleared = false;
        data.dayRecords.Add(firstDay);
        data.currencyData = new CurrencyData();
        // 기본 보상 기준값 (예: Day 1 보상)
        data.currencyData.gold = 100;
        data.currencyData.wood = 50;
        data.currencyData.stone = 20;
        return data;
    }

    public void ResetGameData()
    {
        GameSaveData newData = CreateNewGameSave();
        SaveGame(newData);
        Debug.Log("게임 데이터가 초기화 되었습니다.");
    }
}
