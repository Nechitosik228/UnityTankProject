using System.Collections.Generic;
using System.Numerics;
using Unity.VisualScripting;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;
using Vector2 = UnityEngine.Vector2;
using Quaternion = UnityEngine.Quaternion;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private Rigidbody _rigidbody;

    [SerializeField] private float _driveSpeed = 2f;
    [SerializeField] private ParticleSystem _explosionParticle;
    private List<ParticleSystem> _explosionPull = new List<ParticleSystem>();
    private int _currentExplosion = 0;
    private int _pullSize = 5;
    [SerializeField] private ParticleSystem _shotParticle;
    [SerializeField] private float _TankRotationSpeed = 2f;
    [SerializeField] private float _TurretRotationSpeed = 2f;
    [SerializeField] private Transform _Turret;
    [SerializeField] private ProjectTileController projectTilePrefab;
    [SerializeField] private float _shiftDriveSpeed = 5f;
    [SerializeField] private int _health = 5;
    [SerializeField] private float _shotCD = 1.5f;
    [SerializeField] private int _damage = 5;
    [SerializeField] private Transform _shotPoint;
    [SerializeField] private float _shotRange = 40f;
    [SerializeField] private SoundManager _soundManager;
    [SerializeField] private GameObject _deadTank;
    [SerializeField] private Transform _camera;
    [SerializeField] private Canvas _endScreen;


    private float _shotTimer = 0f;
    private bool _isShotHeld = false;
    private bool _isTankRotating = false;
    private List<ProjectTileController> _projectTilePull = new List<ProjectTileController>();
    private int _ProjectTileIndex = 0;
    private int _projectTilePullSize = 15;
    public bool _isAlive => _health > 0;
    public int Health => _health;
    private bool _shiftPressed;
    private float _cordz;
    private float _cordx;
    private float _lookDeltaX;
    private Vector3 _moveVector;

    private void OnEnable()
    {
        MyInputManager.OnMovePressed += ReadMoveInput;
        MyInputManager.OnSpacePressed += PlayAnimation;
        MyInputManager.OnShiftPressed += ReadShiftInput;
        MyInputManager.OnAttackPressed += ShotWeapon;
        MyInputManager.OnLookPressed += ReadLookInput;

        for (int i = 0; i < _projectTilePullSize; i++)
        {
            var projectTile = Instantiate(projectTilePrefab, _shotPoint.position, Quaternion.identity);
            projectTile.gameObject.SetActive(false);
            _projectTilePull.Add(projectTile);
        }

        for (int i = 0; i < _pullSize; i++)
        {
            var explosion = Instantiate(_explosionParticle, this.transform.position, Quaternion.identity);
            _explosionPull.Add(explosion);
            _explosionPull[i].gameObject.SetActive(false);
        }

        _deadTank.SetActive(false);
        HideEndScrene();
    }

    private void OnDisable()
    {
        MyInputManager.OnSpacePressed -= PlayAnimation;
        MyInputManager.OnMovePressed -= ReadMoveInput;
        MyInputManager.OnShiftPressed -= ReadShiftInput;
        MyInputManager.OnAttackPressed -= ShotWeapon;
        MyInputManager.OnLookPressed -= ReadLookInput;
    }

    private void ReadMoveInput(Vector2 inputVector)
    {
        _cordz = inputVector.y;
        _cordx = inputVector.x;
    }

    private void ReadLookInput(Vector2 inputVector)
    {
        _lookDeltaX = inputVector.x;
    }

    private void RotateTurret()
    {
        _Turret.Rotate(Vector3.up * _lookDeltaX * _TurretRotationSpeed * Time.deltaTime);
    }

    private void Move()
    {
        if (_isTankRotating) return;
        if (_cordz <= 0) _shiftPressed = false;
        float currentSpeed = _shiftPressed ? _shiftDriveSpeed : _driveSpeed;
        _moveVector = transform.forward * _cordz;
        _moveVector *= currentSpeed * Time.fixedDeltaTime;
        _rigidbody.MovePosition(_moveVector + _rigidbody.position);
    }

    public void TakeDamage(int damage)
    {
        _health -= damage;
        Debug.Log(_health);
    }

    private void RotateTank()
    {
        if (_cordx == 0)
        {
            _isTankRotating = false;
            return;
        }
        _isTankRotating = true;
        transform.Rotate(Vector3.up, _cordx * _TankRotationSpeed * Time.deltaTime);
    }

    private void FixedUpdate()
    {
        RotateTank();
        Move();
    }

    private void Update()
    {
        _shotTimer += Time.deltaTime;
        if (_shotTimer >= _shotCD && _isShotHeld)
        {
            LaunchProjectTile();
            _shotTimer = 0;
        }
        DoAnim();
        RotateTurret();
        if (!_isAlive)
        {
            DeathProcess();
        }
    }

    public void ShowEndScrene()
    {
        _endScreen.gameObject.SetActive(true);
    }

    public void HideEndScrene()
    {
        _endScreen.gameObject.SetActive(false);
    }

    public void DeathProcess()
    {
        Debug.Log("Dead");
        _deadTank.transform.position = this.transform.position;
        _deadTank.transform.rotation = this.transform.rotation;
        _deadTank.SetActive(true);
        ShowEndScrene();
        _camera.SetParent(null);
        this.gameObject.SetActive(false);
        return;
    }

    private void PlayAnimation()
    {
        Debug.Log("asdkjhdakjhadkj");
    }

    private void ReadShiftInput(bool isPressed)
    {
        if (_cordz >= 0) _shiftPressed = isPressed;
    }

    private void DoAnim()
    {
        float currentSpeed = _shiftPressed ? _shiftDriveSpeed : _driveSpeed;
        if (animator != null)
        {
            // animator.SetFloat("Strafe", _cordx);
            animator.SetFloat("Right", _cordx);
            animator.SetFloat("Forward", _cordz);
        }
    }

    private void LaunchProjectTile()
    {
        _projectTilePull[_ProjectTileIndex].transform.position = _shotPoint.position;
        _projectTilePull[_ProjectTileIndex].gameObject.SetActive(true);
        _projectTilePull[_ProjectTileIndex].Initialized(_damage, _shotPoint.forward, this);
        _ProjectTileIndex = (_ProjectTileIndex + 1) % _projectTilePullSize;
        _soundManager.playShoot();
        PlayShotEffect();
    }

    public void PlayHitEffect(Vector3 position)
    {
        _explosionPull[_currentExplosion].gameObject.SetActive(false);
        _explosionPull[_currentExplosion].Stop();
        _explosionPull[_currentExplosion].transform.position = position;
        _explosionPull[_currentExplosion].gameObject.SetActive(true);
        _currentExplosion++;
        if (_currentExplosion >= _explosionPull.Count)
        {
            _currentExplosion = 0;
        }
    }

    public void PlayShotEffect()
    {
        _shotParticle.gameObject.SetActive(false);
        _shotParticle.gameObject.SetActive(true);
    }



    private void ShotWeapon(bool isPressed)
    {
        _isShotHeld = isPressed;
        //         if (!isPressed)
        //         {
        //             return;
        //         }
        // #if UNITY_EDITOR
        //         DrawRay();
        // #endif
        //         RaycastHit hit;
        //         if (Physics.Raycast(_shotPoint.position, _shotPoint.forward, out hit, _shotRange))
        //         {
        //             // Debug.Log(hit.collider.gameObject.name);
        //             if (hit.collider.gameObject.TryGetComponent<EnemyController>(out EnemyController enemy))
        //             {
        //                 if (!enemy._isAlive) return;
        //                 enemy.TakeDamage(_damage);
        //             }
        //         }
    }

#if UNITY_EDITOR
    private void DrawRay()
    {
        Debug.DrawRay(_shotPoint.position, _shotPoint.forward * _shotRange, Color.blue, 3f);
    }
#endif
}