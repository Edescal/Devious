using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class FloorButton : MonoBehaviour
{
    public bool activated;
    public string id;
    public UnityEvent<string, bool> onActivate;
    public UnityEvent<string, bool> onDeactivate;
    public int count = 0;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            count++;
            CheckStatus();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            count--;
            CheckStatus();
        }
    }

    private void CheckStatus()
    {
        if (count <= 0 && activated)
        {
            activated = false;
            onDeactivate?.Invoke(id, activated);
            return;
        }
        else if(count > 0 && !activated)
        {
            activated = true;
            onActivate?.Invoke(id,activated);
        }
    }
}
