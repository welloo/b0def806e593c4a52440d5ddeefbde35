using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class AIBehaviour : FlyableObject, IDamagebleObject
{
    public enum AIState
    {
        SHOOTING, RELOADING
    }
    private IDestroyable _destroyer;
    private int _health;
    [SerializeField] private bool _bonusSpawnerState;

    public string[] CollisionTags;
    public GameObject bulletSpawner;
    public float reloadDelay = 1.5f;
    public float bulletSpeed = 3f;
    public int onColisionDamage = 1;
    public int Health { get => _health; }
    public AIState State { get; private set; }
    public UnityEvent OnDestroyEvent;

    #region Unity Methods
    private void Update()
    {
        _destroyer?.DestroyByParams();
        transform.LookAt2D(PlayerController.Instance.transform);
        switch (State)
        {
            case AIState.SHOOTING:
                Shoot();
                break;
            case AIState.RELOADING:
                WaitingByDelay(reloadDelay);
                break;
        }
    }

    private void OnDestroy()
    {
        OnDestroyEvent?.Invoke();
    }
    #endregion

    #region Public Methods
    public void SetBonuseSpawnerState(bool state) => _bonusSpawnerState = state;

    public void SetHealth(int count)
    {
        _health = count <= 0 ? 0 : count;
    }

    public void TakeDamage(int damage)
    {
        var tempValue = _health - damage;
        _health = tempValue <= 0 ? 0 : tempValue;
    }

    public void DestroyByParams(params Func<bool>[] paramaters)
    {
        if (paramaters.Any(item => item.Invoke()))
        {
            Destroy(gameObject);
        }
    }

    public override void Initialize()
    {
        base.Initialize();
        SetHealth(3);
        State = AIState.SHOOTING;
        InitDestroyer();
        InitColisionsDetecter();
        InitBonusSpawner();
    }
    #endregion

    #region Private Methods
    private void InitColisionsDetecter()
    {
        if (colisingDetecter != null)
        {
            var colisionAction = new Action<GameObject>(obj =>
            {
                if (obj.TryGetComponent<IDamagebleObject>(out var component))
                {
                    component.TakeDamage(onColisionDamage);
                }
            });
            colisingDetecter.Activate(typeof(AIBehaviour), colisionAction, this.gameObject, CollisionTags, IColisingDetecter.RayDirection.FORWARD);
            colisingDetecter.OnDetectedEvent.AddListener(() => Destroy(gameObject));
        }
    }

    private void InitBonusSpawner()
    {
        if (_bonusSpawnerState)
        {
            OnDestroyEvent.AddListener(() =>
            {
                if (PlayerController.Instance.IsControlLocked) return;
                var gameInstance = GameManager.Instance;
                gameInstance.UnResgisterElement(gameObject);
                var bonusePrefab = gameInstance.BonusPrefab;
                var bonuseInstance = Instantiate(bonusePrefab, transform.position, new Quaternion(0, 0, 0, 0));
                bonuseInstance.transform.SetParent(gameInstance.transform);
                var bonusesCount = Enum.GetNames(typeof(IBonuseState.BonuseType)).Count();
                var randomBonuseType = ((IBonuseState.BonuseType)UnityEngine.Random.Range(0, bonusesCount));
                bonuseInstance.GetComponent<GameBonuse>().Activate(randomBonuseType);
            });
        }
    }

    private void InitDestroyer()
    {
        _destroyer = new DestroyableObject(
            gameObject,
            new Func<bool>[]
            {
                () => transform.position.y <= _destroyDistanceY,
                () => Health <= 0
            },
            () => Destroy(gameObject));
    }

    private void Shoot()
    {
        if (transform.position.y > 4.5f || transform.position.y < -4.5f)
            return;
        var gameInstance = GameManager.Instance;
        var bulletPrefab = gameInstance.BulletPrefab;
        if (bulletPrefab)
        {
            var instance = Instantiate(bulletPrefab, bulletSpawner.transform.position, transform.rotation);
            instance.transform.SetParent(gameInstance.transform);
            instance.GetComponent<Bullet>().Activate(gameObject, CollisionTags, bulletSpeed);
        }

        State = AIState.RELOADING;
        StartCoroutine(WaitingByDelay(reloadDelay));
    }
    #endregion

    #region Coroutines
    private IEnumerator WaitingByDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        State = AIState.SHOOTING;
    }
    #endregion

}
