using System.Collections.Generic;
using System.Numerics;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using Vector3 = UnityEngine.Vector3;
using Vector2 = UnityEngine.Vector2;
using Quaternion = UnityEngine.Quaternion;

public class EnemyController : MonoBehaviour
{
    private int health;
    private float _searchRange = 20f;
    private float _searchTimer = 0f;
    private float _searchCD = 2f;
    private float _attackTimer = 0f;
    private float _attackCD = 3f;
    private int _damage = 1;
    private PlayerController _target;
    private float speed;
    private List<ProjectTileController> _projectTilePull = new List<ProjectTileController>();
    private int _ProjectTileIndex = 0;
    private int _projectTilePullSize = 15;

    public bool _isAlive => health > 0;
    public EnemyState State { get; private set; } = EnemyState.Idle;

    [SerializeField] private NavMeshAgent _agent;
    [SerializeField] private LayerMask _searchLayer;
    [SerializeField] private Transform _tank;
    [SerializeField] private Transform _deadTank;
    [SerializeField] private ProjectTileController projectTilePrefab;
    [SerializeField] private Transform _shotPoint;
    [SerializeField] private float _wanderRadius = 100f;
    [SerializeField] private float _wanderWaitTime;

    private float _wanderTimer;

    private void OnEnable()
    {
        for (int i = 0; i < _projectTilePullSize; i++)
        {
            var projectTile = Instantiate(projectTilePrefab, _shotPoint.position, Quaternion.identity);
            projectTile.gameObject.SetActive(false);
            _projectTilePull.Add(projectTile);
        }
    }

    public void Initialize(EnemyStats enemyStats)
    {
        health = enemyStats.Health;
        speed = enemyStats.Speed;
        _agent.speed = speed;
    }

    private void Update()
    {
        _searchTimer += Time.deltaTime;
        if (_searchTimer >= _searchCD)
        {
            ScanForPlayer();
            _searchTimer = 0f;
        }
        switch (State)
        {
            case EnemyState.Idle:
                Wander();
                break;
            case EnemyState.Chase:
                GoTo();
                break;
            case EnemyState.Attack:
                Attack();
                break;
            default:
                return;
        }
    }

    private void GoTo()
    {
        if (_target == null) return;
        _agent.SetDestination(_target.transform.position);
        State = EnemyState.Attack;
    }

    private void Attack()
    {
        _attackTimer += Time.deltaTime;
        if (_attackTimer <= _attackCD) return;
        _attackTimer = 0;
        if (_target == null) return;
        LaunchProjectTile();
    }

    private void ScanForPlayer()
    {
        // _agent.ResetPath();
        Collider[] colliders = Physics.OverlapSphere(transform.position, _searchRange, _searchLayer);
        if (colliders.Length == 0)
        {
            _target = null;
            State = EnemyState.Idle;
            return;
        }

        if (colliders[0].gameObject.TryGetComponent<PlayerController>(out PlayerController player) && player._isAlive)
        {
            _target = player;
            State = EnemyState.Chase;
            _agent.ResetPath();
        }
        else
        {
            _target = null;
            State = EnemyState.Idle;
        }
    }

    private void LaunchProjectTile()
    {
        _projectTilePull[_ProjectTileIndex].transform.position = _shotPoint.position;
        _projectTilePull[_ProjectTileIndex].gameObject.SetActive(true);
        _projectTilePull[_ProjectTileIndex].Initialized(_damage, _shotPoint.forward, _target);
        _ProjectTileIndex = (_ProjectTileIndex + 1) % _projectTilePullSize;
    }

    public void TakeDamage(int damage)
    {
        if (!_isAlive) return;
        health -= damage;
        if (!_isAlive) DeathProcess();
    }

    private void DeathProcess()
    {
        _tank.gameObject.SetActive(false);
        _deadTank.gameObject.SetActive(true);
        _agent.ResetPath();
        enabled = false;
    }

    // private void Move()
    // {
    //     transform.Translate(Vector3.forward * speed * Time.deltaTime);
    // }

    private void Wander()
    {
        _wanderTimer += Time.deltaTime;

        if (_wanderTimer < _wanderWaitTime)
            return;

        _wanderTimer = 0f;

        Vector3 randomDirection = Random.insideUnitSphere * _wanderRadius;
        randomDirection += transform.position;

        if (NavMesh.SamplePosition(
            randomDirection,
            out NavMeshHit hit,
            _wanderRadius,
            NavMesh.AllAreas))
        {
            _agent.SetDestination(hit.position);
        }
    }
}

public enum EnemyState
{
    None = 0,
    Idle = 1,
    Chase = 2,
    Attack = 3,
}
