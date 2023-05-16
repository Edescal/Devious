using UnityEngine;

[RequireComponent(typeof(Light)), RequireComponent(typeof(AudioSource))]
public class LightIntensity : MonoBehaviour
{
    [SerializeField] private float lightIntensity = 1;
    [SerializeField] private float startTime = 1f;
    [SerializeField] private AnimationCurve cycleCurve;
    [SerializeField] private Transform billboardQuad;
    [SerializeField] private bool maintainVerticalAlignment = true;
    [Header("Sound FX")]
    [SerializeField] private AudioClip fireSound;
    private Light _light;
    private AudioSource audioSource;

    private void Awake()
    {
        _light = GetComponent<Light>();
    }

    private void OnEnable()
    {
        if (fireSound != null)
        {
            if(audioSource == null)
            {
                audioSource = GetComponent<AudioSource>();
            }
            audioSource.clip = fireSound;
            audioSource.playOnAwake = true;
            audioSource.loop = true;
            audioSource.PlayDelayed(Random.Range(0, 2f));
        }

        if (billboardQuad == null) return;

        CameraPrecullHandler.onPrecull += Billboarding;

        _light.intensity = 0;
        var quadScale = billboardQuad.localScale;
        var quadPos = billboardQuad.localPosition;
        var startPos = billboardQuad.localPosition - (Vector3.up * 0.25f);
        billboardQuad.localScale = Vector3.zero;
        billboardQuad.localPosition = startPos;
        LeanTween.value(0, 1, startTime)
            .setEase(LeanTweenType.easeInSine)
            .setOnUpdate((float f) => {
                float t = f / 1;
                billboardQuad.localScale = Vector3.Lerp(Vector3.zero, quadScale, t);
                billboardQuad.localPosition = Vector3.Lerp(startPos, quadPos, t);
                _light.intensity = Mathf.Lerp(0, lightIntensity, t);

            })
            .setOnComplete(()=> {
                _light.intensity = lightIntensity;
                billboardQuad.localScale = quadScale;
            });
    }

    private void Billboarding(Camera cam)
    {
        if (billboardQuad != null)
        {
            if (maintainVerticalAlignment)
            {
                var dir = Vector3.ProjectOnPlane(cam.transform.forward, Vector3.up);
                billboardQuad.rotation = Quaternion.LookRotation(dir);
            }
            else billboardQuad.rotation = cam.transform.rotation;
        }
    }
}
