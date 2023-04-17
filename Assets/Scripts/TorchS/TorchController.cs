using System.Collections;
using UnityEngine;

namespace Edescal
{
    public class TorchController : MonoBehaviour, IEquip
    {
        [field:SerializeField]
        public float useDuration { get; private set; }
        public Animator animator;
        private bool isEquiped = false;
        public float equipTime = 2f;
        public GameObject box, hitbox;
        public float flareStart, flareEnd;

        public void Set()
        {
            if (isEquiped) return;
            isEquiped = true;

            LeanTween.value(animator.GetFloat("torch"), 1, equipTime)
                .setEase(LeanTweenType.easeInSine)
                .setOnUpdate((f) => { animator.SetFloat("torch", f); })
                .setOnComplete(() => { box.SetActive(true); } );
        }

        public void Use(EquipmentHandler handler)
        {
            handler.animator.CrossFadeInFixedTime("Light Torch", 0.4f);

            IEnumerator OnUse()
            {
                var wait = new WaitForSeconds(flareStart);
                yield return wait;
                hitbox.SetActive(true);
                var wait2 = new WaitForSeconds(flareEnd);
                yield return wait2;
                hitbox.SetActive(false);
                var wait3 = new WaitForSeconds(useDuration - flareStart - flareEnd);
                yield return wait3;
            }

            StartCoroutine(OnUse());
        }

        public void Cancel()
        {
            if (!isEquiped) return;
            isEquiped = false;

            LeanTween.value(animator.GetFloat("torch"), 0, equipTime)
                .setEase(LeanTweenType.easeInSine)
                .setOnUpdate((f) => { animator.SetFloat("torch", f); })
                .setOnComplete(() => { box.SetActive(false); });
        }
    }
}
