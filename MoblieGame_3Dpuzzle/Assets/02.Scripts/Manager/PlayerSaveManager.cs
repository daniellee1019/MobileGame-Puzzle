using UnityEngine;
using System.IO;

public class PlayerSaveManager : MonoBehaviour
{
    public static PlayerSaveManager Instance;
    private string playerSaveFilePath; // persistentDataPath를 이용한 저장 경로

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
        // persistentDataPath 내에 playerSave.json 파일 경로 설정
        playerSaveFilePath = Path.Combine(Application.persistentDataPath, "playerCurrencyData.json");
    }

    /// <summary>
    /// 플레이어 데이터를 로드합니다.
    /// 우선 persistentDataPath에서 읽어오고, 없으면 Resources/Data 폴더의 초기 데이터를 사용합니다.
    /// </summary>
    public PlayerSaveData LoadPlayerData()
    {
        if (File.Exists(playerSaveFilePath))
        {
            string json = File.ReadAllText(playerSaveFilePath);
            PlayerSaveData data = JsonUtility.FromJson<PlayerSaveData>(json);
            Debug.Log("persistentDataPath에서 플레이어 데이터 로드 성공: " + playerSaveFilePath);
            return data;
        }
        else
        {
            TextAsset jsonText = Resources.Load<TextAsset>("Data/playerCurrencyData");
            if (jsonText != null)
            {
                PlayerSaveData data = JsonUtility.FromJson<PlayerSaveData>(jsonText.text);
                Debug.Log("Resources에서 플레이어 데이터 로드 성공");
                return data;
            }
            else
            {
                Debug.LogError("playerSave.json 파일을 찾을 수 없습니다. Resources/Data 폴더에 파일이 있는지 확인하세요.");
                return CreateNewPlayerSave();
            }
        }
    }

    /// <summary>
    /// 플레이어 데이터를 persistentDataPath에 저장합니다.
    /// </summary>
    public void SavePlayerData(PlayerSaveData data)
    {
        string json = JsonUtility.ToJson(data, true);
        Directory.CreateDirectory(Path.GetDirectoryName(playerSaveFilePath));
        File.WriteAllText(playerSaveFilePath, json);
        Debug.Log("플레이어 데이터 저장 완료: " + playerSaveFilePath);
    }

    /// <summary>
    /// 저장 파일이 없을 경우 새 플레이어 데이터를 생성합니다.
    /// </summary>
    public PlayerSaveData CreateNewPlayerSave()
    {
        PlayerSaveData data = new PlayerSaveData();
        data.playerCurrencyData = new PlayerCurrencyData();
        return data;
    }

    public void ResetPlayerData()
    {
        PlayerSaveData newData = CreateNewPlayerSave();
        SavePlayerData(newData);
        Debug.Log("플레이어 데이터가 초기화 되었습니다.");
    }
}
