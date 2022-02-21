using UnityEngine;

public class Bullet : MonoBehaviour
{
    public enum BulletState
    {
        BASE, SMART
    }

    private System.Action<BulletState> movingByState;
    private Transform _targetTransform;

    public float bulletSpeed;
    public BulletState bulletState;
    public int bulletDamage = 1;
    public string[] TargetTags;

    public IColisingDetecter colisingDetecter { get; private set; }
    public GameObject owner;

    #region Unity Methods
    private void Update()
    {
        StateLogic();
    }

    private void FixedUpdate()
    {
        movingByState?.Invoke(bulletState);
    }
    #endregion

    #region Public Methods
    public void Activate(GameObject owner,
        string[] targetTags,
        float bulletSpeed,
        BulletState bulletState = BulletState.BASE,
        Transform target = null)
    {
        this.owner = owner;
        _targetTransform = target;
        TargetTags = targetTags;
        this.bulletSpeed = bulletSpeed;
        this.bulletState = bulletState;
        movingByState += MovingByBulletState;
        if (TryGetComponent<IColisingDetecter>(out var detecter))
        {
            colisingDetecter = detecter;
            var colisionAction = new System.Action<GameObject>(obj =>
            {
                if (obj.TryGetComponent<IDamagebleObject>(out var component))
                {
                    component.TakeDamage(bulletDamage);
                }
            });
            colisingDetecter.Activate(typeof(Bullet), colisionAction, owner, TargetTags, IColisingDetecter.RayDirection.FORWARD);
            colisingDetecter.OnDetectedEvent.AddListener(() => Destroy(gameObject));
        }
        else
        {
            Debug.LogError("ColisingDetecter not found");
        }
        gameObject.SetActive(true);
    }

    public void SetBulletDamage(int damage) => bulletDamage = damage;
    #endregion

    #region Private Methods
    private void StateLogic()
    {
        if (Vector2.Distance(transform.position, PlayerController.Instance.transform.position) > 25 ||
            PlayerController.Instance.IsControlLocked)
        {
            Destroy(gameObject);
        }
    }

    private void MovingByBulletState(BulletState state)
    {
        if (state == BulletState.SMART && _targetTransform)
        {
            transform.LookAt2D(_targetTransform);
        }
        transform.position += transform.up * bulletSpeed * Time.deltaTime;
    }
    #endregion
}
