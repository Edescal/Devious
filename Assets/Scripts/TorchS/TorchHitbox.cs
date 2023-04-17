using UnityEngine;

public class TorchHitbox : MonoBehaviour
{
    private void OnTriggerStay(Collider other)
    {
        if (other.TryGetComponent<TorchStand>(out var stand))
        {
            stand.LightStand();
        }
    }
}