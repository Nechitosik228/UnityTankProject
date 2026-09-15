using UnityEngine;

[CreateAssetMenu(fileName = "New Enemy Stats", menuName ="Enemy Stats")]
public class EnemyStats : ScriptableObject
{
    [SerializeField] private int health;
    [SerializeField] private float speed;

    public int Health => health;
    public float Speed => speed;
}
