using System;
using System.Linq;
using UnityEngine;

public class DestroyableObject : IDestroyable
{
    private Func<bool>[] _params;
    private GameObject _owner;
    private Action _destroyAction;
    public Func<bool>[] DestroyParams { get => _params; }

    public GameObject Owner { get => _owner; }
    public Action DestroyAction { get => _destroyAction; }

    public DestroyableObject(GameObject owner, Func<bool>[] @params, Action destroyAction)
    {
        _owner = owner;
        _params = @params;
        _destroyAction = destroyAction;
    }

    public void DestroyByParams()
    {
        if (_params.Any(item => item.Invoke() == true))
        {
            _destroyAction?.Invoke();
        }
    }
}
