using UnityEngine;

public class Footsteps : MonoBehaviour
{
    [System.Serializable]
    public struct FootstepInfo
    {
        [SerializeField]
        public string tag;
        [SerializeField]
        public AudioClip[] clips;
    }

    [SerializeField]
    private FootstepInfo[] footstepInfo;
    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void Trigger(AnimationEvent evt)
    {
        if (audioSource == null) return;
        if(evt.animatorClipInfo.weight > 0.5f)
        {
            var origin = transform.position + (Vector3.up * 0.5f);
            if (Physics.Raycast(origin, Vector3.down, out var hit, 0.75f))
            {
                foreach (var f in footstepInfo)
                {
                    //Encontrar cuál tiene el mismo tag que el objeto pisado
                    if (hit.collider.CompareTag(f.tag))
                    {
                        AudioClip clip = f.clips[Random.Range(0, f.clips.Length)];
                        if (clip != null)
                        {
                            audioSource.PlayOneShot(clip);
                        }
                    }
                }
            }
        }
    }
}