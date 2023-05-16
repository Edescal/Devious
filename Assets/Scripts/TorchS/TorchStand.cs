using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TorchStand : MonoBehaviour, IDamageable
{
    [HideInInspector]
    public TorchMaster master;
    public bool onFire;
    public UnityEvent onFireEvent;

    public void LightStand()
    {
        if (onFire) return;
        onFire = true;
        master?.Check();
        onFireEvent?.Invoke();
    }

    public void ApplyDamage(int damage, object source)
    {
        LightStand();
    }
}
