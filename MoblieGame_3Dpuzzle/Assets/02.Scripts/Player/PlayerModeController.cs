using UnityEngine;

public enum PlayerMode
{
    Normal,    // 기본 모드 (일반 이동)
    Turret,    // 터렛 모드 (터렛 조작)
    Restore    // 리스토어 모드 (거울 삭제)
}

public class PlayerModeController : MonoBehaviour
{
    public static PlayerModeController Instance { get; private set; }

    [SerializeField] private PlayerMode currentMode = PlayerMode.Normal;

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

    public PlayerMode GetCurrentMode()
    {
        return currentMode;
    }

    public void SetMode(PlayerMode mode)
    {
        currentMode = mode;
        Debug.Log("Player mode changed to: " + currentMode);
    }

    public bool IsMode(PlayerMode mode)
    {
        return currentMode == mode;
    }
}
