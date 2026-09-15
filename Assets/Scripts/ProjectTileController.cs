using System;
using UnityEngine;

public class ProjectTileController : MonoBehaviour
{
    [SerializeField] private float _pushForce = 5f;
    [SerializeField] private float _lifetime = 5f;
    [SerializeField] private Rigidbody _rb;

    private PlayerController _player;
    private int _damage = 5;
    private float _timer = 0f;

    public void Initialized(int damage, Vector3 pushDirection, PlayerController player)
    {
        _timer = 0f;
        _player = player;
        _damage = damage;
        _rb.linearVelocity = Vector3.zero;
        _rb.angularVelocity = Vector3.zero;
        _rb.AddForce(pushDirection * _pushForce, ForceMode.Impulse);
    }

    public void Update()
    {
        _timer += Time.deltaTime;
        if (_timer >= _lifetime) OnBomb();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.TryGetComponent<PlayerController>(out PlayerController player))
        {
            player.TakeDamage(_damage);
            OnBomb();
        }
        else if (collision.gameObject.TryGetComponent<EnemyController>(out EnemyController enemy))
        {
            enemy.TakeDamage(_damage);
            OnBomb();
        }
        else OnBomb();
    }

    private void OnBomb()
    {
        _player.PlayHitEffect(this.transform.position);
        gameObject.SetActive(false);
        _timer = 0f;
    }
}
