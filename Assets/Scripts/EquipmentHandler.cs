using System.Collections;
using UnityEngine;

public class EquipmentHandler : MonoBehaviour
{
    InputReader inputReader;
    bool performing;
    public Animator animator;
    IEquip currentEquipment;
    ThirdPersonController controller;

    public float equipT = 0.4f, unequipT = 0.3f;

    private void OnEnable()
    {
        if (inputReader == null)
        {
            inputReader = GetComponent<InputReader>();
        }

        inputReader.onUseItem += UseEquipment;
    }

    private void OnDisable()
    {
        if (inputReader == null) return;

        inputReader.onUseItem -= UseEquipment;
    }

    private void Start()
    {
        animator = GetComponent<Animator>();
        controller = GetComponent<ThirdPersonController>();
    }

    public void AsignEquipment(MonoBehaviour mono) => AsignEquipment(mono as IEquip);
    public void AsignEquipment(IEquip equip)
    {
        if (performing || equip == null) return;

        IEnumerator OnAsign()
        {
            controller.CanMove = false;
            performing = true;

            if (currentEquipment != null)
            {
                //Unequip
                currentEquipment.Cancel();
                animator.CrossFadeInFixedTime("Equip", unequipT);

                var unequip = new WaitForSeconds(unequipT);
                yield return unequip;
            }

            currentEquipment = equip;
            //Equip
            currentEquipment.Set();
            animator.CrossFadeInFixedTime("Equip", equipT);

            var time = new WaitForSeconds(equipT);
            yield return time;

            controller.CanMove = true;
            performing = false;
        }

        StartCoroutine(OnAsign());
    }

    public void UseEquipment()
    {
        if (performing || currentEquipment == null) return;
        performing = true;

        IEnumerator OnPerform()
        {
            performing = true;
            controller.CanMove = false;
            animator.applyRootMotion = true;

            currentEquipment.Use(this);
            var wait = new WaitForSeconds(currentEquipment.useDuration);
            yield return wait;

            animator.applyRootMotion = false;
            controller.CanMove = true;
            performing = false;
        }

        StartCoroutine(OnPerform());
    }

    public void RemoveEquipment()
    {
        if (performing || currentEquipment == null) return;

        IEnumerator OnRemove()
        {
            performing = true;
            controller.CanMove = false;

            currentEquipment.Cancel();
            currentEquipment = null;
            var wait = new WaitForSeconds(unequipT);
            yield return wait;

            controller.CanMove = true;
            performing = false;
        }

        StartCoroutine(OnRemove());
    }
}
