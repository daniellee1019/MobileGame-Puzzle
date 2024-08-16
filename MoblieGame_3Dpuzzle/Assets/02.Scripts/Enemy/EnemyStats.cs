using UnityEngine;

[CreateAssetMenu(fileName = "EnemyStats", menuName = "ScriptableObjects/EnemyStats", order = 1)]
public class EnemyStats : ScriptableObject
{
    public float maxHealth;
    public float armor;
    public float speed;
    public float damage; // ���� �÷��̾ ��ž�� ���ϴ� ���ط�
}
