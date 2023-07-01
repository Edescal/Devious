using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class Tool : MonoBehaviour
{
    private ToolHandler handler;
    public AnimatorOverrideController animatorController;
    private AnimatorManager animatorManager;
    public float toolDuration = 1.5f;
    public Hitbox hitbox;
    public float startHit;
    public float endHit;

    public ToolHandler Handler
    {
        get => handler;
        set
        {
            handler = value;
            animatorManager = handler.animatorManager;
            animatorManager.OverrideController(animatorController);
            hitbox.SetTransform(handler.transform);
            hitbox.setIgnoredCollider(handler.GetComponent<Collider>());
        }
    }

    public bool inAction;
    public bool debug;

    public void Use()
    {
        animatorManager.Play("Use", 0.3f, 0, true);

        IEnumerator OnUse()
        {
            yield return new WaitForSeconds(startHit);

            inAction = true;

            var set = new HashSet<IDamageable>();
            float counter = endHit - startHit;
            while (counter > 0)
            {
                counter -= Time.deltaTime;
                var damageables = hitbox.Check();
                if (damageables?.Length > 0)
                {
                    foreach (var damageable in damageables)
                    {
                        if (!set.Contains(damageable))
                        {
                            damageable.ApplyDamage(3, this);
                            set.Add(damageable);
                        }
                    }
                }
                yield return null;
            }

            inAction = false;

            yield return new WaitForSeconds(toolDuration - startHit - endHit);
        }

        StopAllCoroutines();
        StartCoroutine(OnUse());
    }

    private void OnDrawGizmos()
    {
        if (!debug || !inAction) return;

        hitbox.Debug();
    }
}
