using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class TorchMaster : MonoBehaviour
{
    public bool fired = false;
    public TorchStand[] stands;
    int currentLightStands = 0;
    public UnityEvent onAllTorchesLighted;
    public AudioClip lightSound, eventSound;
    public AudioSource audioSource;

    private void OnValidate()
    {
        var list = new List<TorchStand>(stands);
        for (int i = list.Count - 1; i >= 0; i--)
        {
            if (list[i] == null)
            {
                list.RemoveAt(i);
            }
        }
        stands = list.ToArray();
    }

    private void Start()
    {
        currentLightStands = 0;
        foreach (var torch in stands)
        {
            torch.master = this;
        }
    }

    public void Check()
    {
        if (fired) return;

        int c = 0;
        for (int i = 0; i < stands.Length; i++)
        {
            if (stands[i].onFire)
                c++;
        }

        if (c == stands.Length)
        {
            fired = true;
            IEnumerator OnMaster()
            {
                audioSource.PlayOneShot(eventSound);
                var delay = new WaitForSeconds(1f);
                yield return delay;
                onAllTorchesLighted?.Invoke();
            }
            StartCoroutine(OnMaster());
        }
        else if (c > currentLightStands)
        {
            audioSource.PlayOneShot(lightSound);
        }

        currentLightStands = c;
    }
}