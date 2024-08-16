using UnityEngine;
using UnityEngine.UI;

public class EnemyAI : MonoBehaviour
{
    public EnemyStats enemyStats; // ���� �Ӽ����� ������ ScriptableObject
    public Transform target; // ��ž�� ��ǥ�� ����
    public Image healthBar; // ü�¹� UI

    private float currentHealth;

    void Start()
    {
        // ���� ObjectManager�� ���
        ObjectManager.Instance.RegisterEnemy(this);

        // �ͷ��� ObjectManager���� ������
        target = ObjectManager.Instance.turret.transform;

        InitializeEnemy(); // ���� �ʱ�ȭ
    }

    public void InitializeEnemy()
    {
        // ���� �ʱ� ü���� ����
        currentHealth = enemyStats.maxHealth;
        UpdateHealthBar(); // �ʱ� ü�¿� �°� ü�¹ٸ� ����
    }

    void Update()
    {
        // ��ž�� ���� õõ�� �̵�
        if (target != null)
        {
            Vector3 direction = (target.position - transform.position).normalized;
            transform.position += direction * enemyStats.speed * Time.deltaTime;
        }
    }

    public void TakeDamage(float damage)
    {
        // �������� ���¿� ���� �پ�� �� ü�¿��� ����
        float actualDamage = Mathf.Max(damage - enemyStats.armor, 0); // ���¸�ŭ �������� ����
        currentHealth -= actualDamage;
        currentHealth = Mathf.Clamp(currentHealth, 0, enemyStats.maxHealth); // ü���� 0 ���Ϸ� �������� �ʵ��� ����

        // ü�¹� ������Ʈ
        UpdateHealthBar();

        // ü���� 0�� �Ǹ� ���� ����
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void UpdateHealthBar()
    {
        // ü�¹��� fillAmount�� ���� ü�¿� ����ϰ� ����
        healthBar.fillAmount = currentHealth / enemyStats.maxHealth;
    }

    private void Die()
    {
        // �� ��� �� ó�� (��: �ı�, ���� ��� ��)
        Destroy(gameObject);
    }
}