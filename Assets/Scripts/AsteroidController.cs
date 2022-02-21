using UnityEngine;

public class AsteroidController : FlyableObject, IDamagebleObject
{
    private IDestroyable _destroyer;
    [SerializeField] private int _health = 0;
    public int Health { get => _health; }
    public string[] CollisionTags;
    public int onColisionDamage = 1;

    #region Unity Methods
    public override void Awake()
    {
        base.Awake();
        InitDestroyer();
        InitCollisionsDetecter();
    }

    public void Update()
    {
        _destroyer?.DestroyByParams();
    }

    private void OnApplicationQuit()
    {
        Destroy(gameObject);
    }
    #endregion

    #region Public Methods
    public override void Initialize()
    {
        base.Initialize();
    }

    public void SetHealth(int count)
    {
        _health = count <= 0 ? 0 : count;
    }

    public void TakeDamage(int damage)
    {
        var tempValue = _health - damage;
        _health = tempValue <= 0 ? 0 : tempValue;
    }
    #endregion

    private void InitDestroyer()
    {
        _destroyer = new DestroyableObject(
            gameObject,
            new System.Func<bool>[]
            {
                () => transform.position.y <= _destroyDistanceY,
                () => Health <= 0
            },
            () => Destroy(gameObject));
    }

    private void InitCollisionsDetecter()
    {
        if (colisingDetecter != null)
        {
            var colisionAction = new System.Action<GameObject>(obj =>
            {
                if (obj.TryGetComponent<IDamagebleObject>(out var component))
                {
                    component.TakeDamage(onColisionDamage);
                }
            });
            colisingDetecter.Activate(typeof(AsteroidController), colisionAction, this.gameObject, CollisionTags, IColisingDetecter.RayDirection.DOWN);
            colisingDetecter.OnDetectedEvent.AddListener(() => Destroy(gameObject));
        }
    }
}
