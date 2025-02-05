using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DamagePopup : MonoBehaviour
{
    public TMP_Text damageText;
    public float moveSpeed = 2f;
    public float fadeSpeed = 2f;
    public float lifetime = 1f;

    private float timer;
    private Color originalColor;

    private Vector3 moveDirection;   // 생성 시 결정된 이동 방향

    private void OnEnable()
    {
        // 재사용 시 timer 초기화
        timer = lifetime;
        if (damageText != null)
        {
            damageText.color = originalColor;
        }

        // 캔버스의 자식으로 생성되었을 경우, 로컬 좌표 기준으로 방향을 결정해도 무방합니다.
        // 90도 범위(즉, 위쪽 반구) 내에서 랜덤한 방향 선택:
        // Random.insideUnitCircle는 원의 내부의 무작위 점을 제공합니다.
        // (x, y)로 얻은 값을 사용하고, y축(위쪽)은 1로 고정한 후 정규화합니다.
        Vector2 randomOffset = Random.insideUnitCircle;
        moveDirection = new Vector3(randomOffset.x, 1f, randomOffset.y).normalized;
    }

    /// <summary>
    /// 데미지와 치명타 여부에 따라 텍스트 설정
    /// </summary>
    /// <param name="damage">표시할 데미지</param>
    /// <param name="isCrit">치명타 여부</param>
    public void Setup(float damage, bool isCrit)
    {
        if (damageText == null)
            damageText = GetComponent<TMP_Text>();

        // 데미지를 단위별로 포맷팅 (예: 1500 → 1.5K, 3200000 → 3.2M)
        damageText.text = FormatDamage(damage);

        // 치명타인 경우 색상을 변경 (예: 노란색), 아니면 흰색
        damageText.color = isCrit ? Color.red : Color.white;
        originalColor = damageText.color;
    }

    // 데미지를 단위 포맷팅하는 헬퍼 함수
    private string FormatDamage(float damage)
    {
        if (damage >= 1e9f)
            return (damage / 1e9f).ToString("F1") + "B";
        else if (damage >= 1e6f)
            return (damage / 1e6f).ToString("F1") + "M";
        else if (damage >= 1e3f)
            return (damage / 1e3f).ToString("F1") + "K";
        else
            return Mathf.RoundToInt(damage).ToString();
    }

    void Update()
    {
        // 위쪽으로 이동
        transform.position += Vector3.up * moveSpeed * Time.deltaTime;

        // 타이머 감소 후 페이드 아웃 시작
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            Color newColor = damageText.color;
            newColor.a -= fadeSpeed * Time.deltaTime;
            damageText.color = newColor;

            if (newColor.a <= 0)
            {
                // 완전히 투명해지면 오브젝트를 파괴하지 않고 풀에 반환
                PopupPoolManager.Instance.ReturnPopup(this);
            }
        }
    }
}
