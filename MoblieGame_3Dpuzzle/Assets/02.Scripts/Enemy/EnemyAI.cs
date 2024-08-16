using UnityEngine;
using UnityEngine.UI;

public class EnemyAI : MonoBehaviour
{
    public EnemyStats enemyStats; // 적의 속성들을 설정할 ScriptableObject
    public Transform target; // 포탑을 목표로 설정
    public Image healthBar; // 체력바 UI

    private float currentHealth;

    void Start()
    {
        // 적을 ObjectManager에 등록
        ObjectManager.Instance.RegisterEnemy(this);

        // 터렛을 ObjectManager에서 가져옴
        target = ObjectManager.Instance.turret.transform;

        InitializeEnemy(); // 적을 초기화
    }

    public void InitializeEnemy()
    {
        // 적의 초기 체력을 설정
        currentHealth = enemyStats.maxHealth;
        UpdateHealthBar(); // 초기 체력에 맞게 체력바를 설정
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

    public void TakeDamage(float damage)
    {
        // 데미지를 방어력에 의해 줄어든 후 체력에서 감소
        float actualDamage = Mathf.Max(damage - enemyStats.armor, 0); // 방어력만큼 데미지를 감소
        currentHealth -= actualDamage;
        currentHealth = Mathf.Clamp(currentHealth, 0, enemyStats.maxHealth); // 체력이 0 이하로 내려가지 않도록 설정

        // 체력바 업데이트
        UpdateHealthBar();

        // 체력이 0이 되면 적을 제거
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void UpdateHealthBar()
    {
        // 체력바의 fillAmount를 현재 체력에 비례하게 설정
        healthBar.fillAmount = currentHealth / enemyStats.maxHealth;
    }

    private void Die()
    {
        // 적 사망 시 처리 (예: 파괴, 사운드 재생 등)
        Destroy(gameObject);
    }
}