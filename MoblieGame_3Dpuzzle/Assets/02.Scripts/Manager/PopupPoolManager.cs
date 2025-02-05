using System.Collections.Generic;
using UnityEngine;

public class PopupPoolManager : MonoBehaviour
{
    public static PopupPoolManager Instance;

    public DamagePopup damagePopupPrefab; // 인스펙터에 DamagePopup 프리팹 할당
    public int poolSize = 20;

    private Queue<DamagePopup> pool = new Queue<DamagePopup>();

    private void Awake()
    {
        // 싱글톤 패턴 설정
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // 초기 풀 생성
        for (int i = 0; i < poolSize; i++)
        {
            DamagePopup popup = Instantiate(damagePopupPrefab, transform);
            popup.gameObject.SetActive(false);
            pool.Enqueue(popup);
        }
    }

    /// <summary>
    /// 사용 가능한 DamagePopup 객체를 풀에서 가져옴
    /// </summary>
    public DamagePopup GetPopup()
    {
        if (pool.Count > 0)
        {
            DamagePopup popup = pool.Dequeue();
            popup.gameObject.SetActive(true);
            return popup;
        }
        else
        {
            // 풀에 객체가 부족하면 새로 생성 (또는 풀 사이즈를 늘릴 수 있음)
            DamagePopup popup = Instantiate(damagePopupPrefab, transform);
            return popup;
        }
    }

    /// <summary>
    /// 사용이 끝난 DamagePopup 객체를 풀로 반환
    /// </summary>
    public void ReturnPopup(DamagePopup popup)
    {
        popup.gameObject.SetActive(false);
        pool.Enqueue(popup);
    }
}
