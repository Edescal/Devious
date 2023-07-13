using UnityEngine;

namespace Edescal.AI
{
    public class EyeInWall : MonoBehaviour
    {
        [SerializeField]
        private Transform player;
        [SerializeField]
        private Transform[] eyes;
        [SerializeField]
        private float smoothRotation = 2f;
        [SerializeField]
        private bool playerIsNear = false;

        private void Update()
        {
            if (player == null) return;
            if (playerIsNear)
            {
                foreach(var e in eyes)
                {
                    var direction = player.position - e.position;
                    var rotation = Quaternion.LookRotation(direction);
                    e.rotation = Quaternion.Slerp(e.rotation, rotation, smoothRotation * Time.deltaTime);
                }
            }
            else
            {
                foreach(var e in eyes)
                    e.localRotation = Quaternion.Slerp(e.localRotation, Quaternion.identity, smoothRotation * Time.deltaTime);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                playerIsNear = true;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                playerIsNear = false;
            }
        }
    }
}