using UnityEngine;

[CreateAssetMenu(fileName = "EnemyStats", menuName = "ScriptableObjects/EnemyStats", order = 1)]
public class EnemyStats : ScriptableObject
{
    public float maxHealth;
    public float armor;
    public float speed;
    public float damage; // 적이 플레이어나 포탑에 가하는 피해량
}
