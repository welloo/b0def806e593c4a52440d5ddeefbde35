using UnityEngine;

[RequireComponent(typeof(IColisingDetecter))]
public class FlyableObject : MonoBehaviour
{
    private float _rotationOffset, _offsetY = 0;

    public float maxRotationOffset, _speed, _maxOffsetY, _destroyDistanceY = 0;
    private bool _isInit = false;

    public IColisingDetecter colisingDetecter { get; private set; }
    public CircleCollider2D Collider { get; private set; }

    #region Unity Methods
    public virtual void Awake()
    {
        Initialize();
    }

    public virtual void FixedUpdate()
    {
        ChangeObjectPosition(_offsetY, _speed);
    }
    #endregion

    #region Public Methods
    public virtual void Initialize()
    {
        if (!_isInit)
        {
            _rotationOffset = Random.Range(0.0f, maxRotationOffset);
            _rotationOffset *= _rotationOffset > _rotationOffset / 2 ? -1 : 1;
            _offsetY = Random.Range(1.0f, _maxOffsetY) * -1;

            Collider = GetComponent<CircleCollider2D>();
            if (TryGetComponent<IColisingDetecter>(out var detecter))
            {
                colisingDetecter = detecter;
            }
            else
            {
                Debug.LogError("ColisingDetecter not found!");
            }
            _isInit = true;
        }
    }

    public virtual void ChangeObjectPosition(float maxDistanceY, float speed)
    {
        var newPosition = transform.position + new Vector3(0, maxDistanceY, 0) * speed * Time.fixedDeltaTime;
        transform.SetPositionAndRotation(newPosition, transform.rotation * Quaternion.Euler(0, 0, _rotationOffset));
    }
    #endregion
}
