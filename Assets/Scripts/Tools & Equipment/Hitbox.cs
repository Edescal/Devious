using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Hitbox
{
    [SerializeField]
    private Transform transform;
    [SerializeField]
    private Collider ignoreCollider;
    [SerializeField]
    private Vector3 dimension;
    [SerializeField]
    private Vector3 offset;
    [SerializeField]
    private LayerMask layers;

    private Vector3 origin => transform.position + (transform.forward * offset.z) + (transform.up * offset.y);

    public void SetTransform(Transform transform) => this.transform = transform;

    public void setIgnoredCollider(Collider collider) => ignoreCollider = collider;

    public IDamageable[] Check()
    {
        var colliders = Physics.OverlapBox(origin, dimension / 2, transform.localRotation, layers);
        if (colliders.Length > 0)
        {
            HashSet<IDamageable> set = new HashSet<IDamageable>();
            for(int i = 0; i < colliders.Length; i++)
            {
                var collider = colliders[i];
                if (collider == ignoreCollider)
                {
                    continue;
                }

                if(!collider.TryGetComponent<IDamageable>(out var damageable))
                {
                    continue;
                }

                if (!set.Contains(damageable))
                {
                    set.Add(damageable);
                }
            }
            var array = new IDamageable[set.Count];
            set.CopyTo(array);
            return array;
        }

        return null;
    }

    public void Debug()
    {
        if (transform == null) return;

        Gizmos.color = Color.yellow;
        Gizmos.matrix = Matrix4x4.TRS(origin, transform.rotation, transform.lossyScale);
        Gizmos.DrawWireCube(Vector3.zero, dimension);
    }
}
