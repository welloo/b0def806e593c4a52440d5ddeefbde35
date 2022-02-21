using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using static Extensions;

[RequireComponent(typeof(IColisingDetecter))]
public class PlayerController : Singleton<PlayerController>, IDamagebleObject, IBonuseOwner
{
    private float yVelocity = 0.0f;
    private int _health = 3;
    private bool _isInvulable = false;
    private bool _isControlLocked = true;

    public GameObject bulletSpawner = null;
    public Vector3 basePosition = Vector3.zero;
    public float movingSmoothTime = 0.3f;
    public float movingSpeed = 2f;
    public float maxDistanceToDestination = 10f;

    public float bulletSpeed = 6;
    public float shieldDelay = 5;
    public string[] targetTags;
    public int Health { get => _health; }
    public bool IsInvulable { get => _isInvulable; }
    public bool IsControlLocked { get => _isControlLocked; }
    public UnityEvent OnDieEvent;

    #region Unity Methods
    private void Start()
    {
        Initialize();
    }

    private void Update()
    {
        if (!IsControlLocked)
        {
            LookAtMouse();
            UpdateMovingLogic();
            if (_health <= 0)
                OnDieEvent?.Invoke();
        }
    }
    #endregion

    #region Public Methods
    public void ResetPlayerState()
    {
        transform.position = basePosition;
        transform.LookAt2D(Vector2.up);
        _health = 3;
    }

    public void SetHealth(int count)
    {
        _health = count < 0 ? 0 : count;
    }

    public void TakeDamage(int damage)
    {
        if (_isInvulable || _isControlLocked) return;
        var tempValue = _health - damage;
        _health = tempValue < 0 ? 0 : tempValue;
    }

    public void SetControlLockedState(bool state) => _isControlLocked = state;

    public void ActivateBonuseByType(IBonuseState.BonuseType bonuseType)
    {
        switch (bonuseType)
        {
            case IBonuseState.BonuseType.SHIELD:
                StartCoroutine(ActivateShieldBonuse(shieldDelay));
                break;
            case IBonuseState.BonuseType.SMARTBULLETS:
                ActivateSmartBulletsBonus();
                break;
        }
    }
    #endregion

    #region Private Methods
    private void Initialize()
    {
        ResetPlayerState();
    }

    private void ActivateSmartBulletsBonus()
    {
        var instance = GameManager.Instance;
        var targets = instance.SpawnedObjects.Where(item => item != null).ToList();
        targets.ForEach(target =>
        {
            var bulletPrefab = instance.BulletPrefab;
            var instanseBullet = Instantiate(
                bulletPrefab,
                bulletSpawner.transform.position,
                target.transform.rotation
                );
            instanseBullet.transform.SetParent(instance.transform);
            if (instanseBullet.TryGetComponent<Bullet>(out var bulletComponent))
            {
                bulletComponent.SetBulletDamage(99999); //huge dps =)
                bulletComponent.Activate(gameObject, targetTags, bulletSpeed, Bullet.BulletState.SMART, target.transform);
            }
        });
    }

    private void LookAtMouse()
    {
        var direction = Input.mousePosition - Camera.main.WorldToScreenPoint(transform.position);
        var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    private void Shoot()
    {
        var instance = GameManager.Instance;
        var bulletPrefab = instance.BulletPrefab;
        var instanseBullet = Instantiate(
            bulletPrefab,
            bulletSpawner.transform.position,
            transform.rotation
            );
        instanseBullet.transform.SetParent(instance.transform);
        if (instanseBullet.TryGetComponent<Bullet>(out var bulletComponent))
        {
            bulletComponent.Activate(gameObject, targetTags, bulletSpeed, Bullet.BulletState.BASE);
        }
    }

    private void UpdateMovingLogic()
    {
        if (Input.GetKey(KeyCode.A))
        {
            MovingToDestination(-maxDistanceToDestination);
        }

        if (Input.GetKey(KeyCode.D))
        {
            MovingToDestination(maxDistanceToDestination);
        }

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            Shoot();
        }

        void MovingToDestination(float maxDistance)
        {
            var newPositionX = Mathf.SmoothDamp(transform.position.x, maxDistance, ref yVelocity, movingSmoothTime, movingSpeed);
            transform.position = new Vector3(newPositionX, transform.position.y, transform.position.z);
        }
    }
    #endregion

    #region Courutines
    private IEnumerator ActivateShieldBonuse(float delay)
    {
        if (_isInvulable) yield return null;
        _isInvulable = true;
        yield return new WaitForSeconds(delay);
        _isInvulable = false;
    }
    #endregion
}
