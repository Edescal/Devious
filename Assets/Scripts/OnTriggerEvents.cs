using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class OnTriggerEvents : MonoBehaviour
{
    [SerializeField]
    private UnityEvent onTriggerEnter;
    [SerializeField]
    private UnityEvent onTriggerExit;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            onTriggerEnter?.Invoke();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            onTriggerExit?.Invoke();
        }
    }
}
