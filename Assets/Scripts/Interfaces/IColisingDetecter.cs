using System;
using UnityEngine;
using UnityEngine.Events;

public interface IColisingDetecter
{
    enum RayDirection
    {
        FORWARD, UP, DOWN
    }

    public string[] TargetTags { get; }
    public RayDirection rayDirection { get; }
    bool IsActive { get; }
    GameObject Owner { get; }
    UnityEvent OnDetectedEvent { get; }
    Type SenderType { get; }
    Collider2D baseCollider { get; }
    Action<GameObject> OnColisionAction { get; }
    bool Activate(Type senderType, Action<GameObject> colisionAction, GameObject owner, string[] targetTags, IColisingDetecter.RayDirection direction);
    void DetectCollision();
}
