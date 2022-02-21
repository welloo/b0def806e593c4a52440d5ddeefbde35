using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class CollisionDetector : MonoBehaviour, IColisingDetecter
{
    private Vector3 Origin, StartPoint = Vector3.zero;
    private float LengthOfRay, DistanceBetweenRays, DirectionFactor;
    private int iterator = 0;
    private string[] _targetTags = null;
    private IColisingDetecter.RayDirection _rayDirection;
    private bool _isActive = false;
    private GameObject _owner = null;
    private UnityEvent _onDetectedEvent = new UnityEvent();
    private Type _senderType = null;
    private Collider2D _baseCollider;

    public float margin = 0.015f;
    public int RaysCount = 10;
    public bool debug = false;

    public string[] TargetTags { get => _targetTags; }
    public IColisingDetecter.RayDirection rayDirection { get => _rayDirection; }
    public bool IsActive { get => _isActive; }
    public GameObject Owner { get => _owner; }
    public UnityEvent OnDetectedEvent { get => _onDetectedEvent; }
    public Type SenderType { get => _senderType; }
    public Collider2D baseCollider { get => _baseCollider; }
    public Action<GameObject> OnColisionAction { get; private set; }

    #region Unity Methods
    void Start()
    {
        _baseCollider = GetComponent<Collider2D>();
        LengthOfRay = _baseCollider.bounds.extents.y;
        DirectionFactor = Mathf.Sign(Vector3.up.y);
    }
    private void FixedUpdate()
    {
        DetectCollision();
    }
    #endregion

    #region Private Methods
    bool IsCollidingVertically()
    {
        Origin = StartPoint;
        DistanceBetweenRays = (_baseCollider.bounds.size.x - 2 * margin) / (RaysCount - 1);
        for (iterator = 0; iterator < RaysCount; iterator++)
        {
            Vector3 direction = rayDirection switch
            {
                IColisingDetecter.RayDirection.FORWARD => transform.TransformDirection(Vector2.up),
                IColisingDetecter.RayDirection.UP => Vector2.up,
                IColisingDetecter.RayDirection.DOWN => Vector2.down,
                _ => Vector3.zero
            };
            var hit2D = Physics2D.Raycast(Origin, direction * DirectionFactor, LengthOfRay);
            if (debug)
                Debug.DrawRay(Origin, direction * DirectionFactor, Color.yellow);
            if (hit2D.collider &&
                hit2D.collider != _baseCollider &&
                hit2D.transform.gameObject != _owner &&
                !string.IsNullOrEmpty(_targetTags.SingleOrDefault(tag => tag == hit2D.collider.tag))
                )
            {
                if (debug)
                    Debug.Log($"Hit from {transform.tag} to {hit2D.collider.tag}");
                OnColisionAction?.Invoke(hit2D.transform.gameObject);
                return true;
            }
            Origin += new Vector3(DistanceBetweenRays, 0, 0);
        }
        return false;
    }
    #endregion

    #region Public Methods
    public bool Activate(Type senderType, Action<GameObject> colisionAction, GameObject owner, string[] targetTags, IColisingDetecter.RayDirection direction)
    {
        if (senderType == null || !owner)
        {
            Debug.LogError("Incorrect input arguments");
            return false;
        }
        OnColisionAction = colisionAction;
        _senderType = senderType;
        _owner = owner;
        _targetTags = targetTags;
        _rayDirection = direction;

        return _isActive = true;
    }

    public void DetectCollision()
    {
        StartPoint = new Vector3(_baseCollider.bounds.min.x + margin, transform.position.y, transform.position.z);
        if (IsCollidingVertically())
        {
            _onDetectedEvent?.Invoke();
        }
    }
    #endregion
}
