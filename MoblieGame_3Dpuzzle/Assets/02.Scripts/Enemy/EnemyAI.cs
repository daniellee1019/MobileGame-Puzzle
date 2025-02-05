using UnityEngine;
using UnityEngine.UI;

public class EnemyAI : MonoBehaviour
{
    public EnemyStats enemyStats; // 적의 속성들을 설정할 ScriptableObject
    public Transform target; // 포탑을 목표로 설정
    public Image healthBar; // 체력바 UI

    [Header("데미지 팝업 관련")]
    public float popupHeightOffset = 2f;  // 적 머리 위 어느 정도 오프셋

    private float currentHealth;

    void Start()
    {
        // 적을 ObjectManager에 등록
        ObjectManager.Instance.RegisterEnemy(this);

        // 터렛을 ObjectManager에서 가져옴
        target = ObjectManager.Instance.turret.transform;

        InitializeEnemy(); // 적을 초기화
    }

    /// <summary>
    /// 적 초기화: 최대 체력 설정 및 체력바 업데이트
    /// </summary>
    public void InitializeEnemy()
    {
        currentHealth = enemyStats.maxHealth;
        UpdateHealthBar();
    }

    void Update()
    {
        // 포탑을 향해 천천히 이동
        if (target != null)
        {
            Vector3 direction = (target.position - transform.position).normalized;
            transform.position += direction * enemyStats.speed * Time.deltaTime;
        }
    }

    /// <summary>
    /// 데미지와 방어구 관통력, 치명타 여부를 적용하여 실제 데미지를 계산하고 적용하는 함수
    /// </summary>
    /// <param name="damage">입력 데미지</param>
    /// <param name="armorPenetration">방어구 관통력</param>
    /// <param name="isCrit">치명타 여부</param>
    public void TakeDamage(float damage, float armorPenetration, bool isCrit)
    {
        // 적의 방어력에서 관통력을 차감한 후 최소 0 이상의 값 계산
        float effectiveArmor = Mathf.Max(enemyStats.armor - armorPenetration, 0);
        // 데미지 감소 공식 (예시): 방어력이 높을수록 데미지 감소 효과 적용
        float finalDamage = damage * (100f / (100f + effectiveArmor));

        currentHealth -= finalDamage;
        currentHealth = Mathf.Clamp(currentHealth, 0, enemyStats.maxHealth);

        UpdateHealthBar();

        // 데미지 팝업 표시
        ShowDamagePopup(finalDamage, isCrit);

        Debug.Log($"[EnemyAI] 받은 데미지: {finalDamage}, 남은 체력: {currentHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    /// <summary>
    /// 체력바를 현재 체력에 맞게 업데이트하는 함수
    /// </summary>
    private void UpdateHealthBar()
    {
        if (healthBar != null)
        {
            healthBar.fillAmount = currentHealth / enemyStats.maxHealth;
        }
    }

    /// <summary>
    /// 데미지 팝업을 생성하여 적 머리 위에 띄우는 함수
    /// </summary>
    /// <param name="damage">표시할 데미지</param>
    /// <param name="isCrit">치명타 여부</param>
    private void ShowDamagePopup(float damage, bool isCrit)
    {
        if (PopupPoolManager.Instance != null)
        {
            // 적 머리 위 기본 위치에 소폭의 랜덤 오프셋 추가 (X, Y 축)
            Vector3 popupPosition = transform.position
                + new Vector3(Random.Range(-0.3f, 0.3f), popupHeightOffset + Random.Range(0f, 0.5f), 0);
            DamagePopup popup = PopupPoolManager.Instance.GetPopup();
            popup.transform.position = popupPosition;
            popup.Setup(damage, isCrit);
        }
    }

    /// <summary>
    /// 적이 사망할 때 처리하는 함수
    /// </summary>
    private void Die()
    {
        // 적 사망 시 추가 효과(사운드, 애니메이션 등)를 넣을 수 있음
        Destroy(gameObject);
    }
}
