using System;
using UnityEngine;

public interface IDestroyable
{
    Func<bool>[] DestroyParams { get; }
    GameObject Owner { get; }
    Action DestroyAction { get; }

    void DestroyByParams();
}
