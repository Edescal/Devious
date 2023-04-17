using UnityEngine;

[RequireComponent(typeof(Light)), RequireComponent(typeof(AudioSource))]
public class LightIntensity : MonoBehaviour
{
    [SerializeField] private Vector2 lightIntensityRange = new Vector2(2.5f, 3);
    [SerializeField] private float startTime = 1f;
    [SerializeField] private float cycleTime = 4f;
    [SerializeField] private AnimationCurve cycleCurve;
    [SerializeField] private Transform billboardQuad;
    [SerializeField] private bool maintainVerticalAlignment = true;
    [Header("Sound FX")]
    [SerializeField] private AudioClip fireSound;
    private Transform mainCamera;
    private Light _light;
    private AudioSource audioSource;

    private void Awake()
    {
        mainCamera = Camera.main.transform;
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
            audioSource.Play();
        }

        if (billboardQuad == null) return;

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
                _light.intensity = Mathf.Lerp(0, lightIntensityRange.x, t);

                if (maintainVerticalAlignment)
                {
                    var dir = mainCamera.forward;
                    dir.y = 0;
                    billboardQuad.rotation = Quaternion.LookRotation(dir);
                }
                else billboardQuad.rotation = mainCamera.rotation;

            })
            .setOnComplete(()=> {
                _light.intensity = lightIntensityRange.x;
                billboardQuad.localScale = quadScale;
                LightIntensityCurve();
            });
    }

    [ContextMenu("Reiniciar")]
    private void LightIntensityCurve()
    {
        LeanTween.cancel(gameObject);
        LeanTween.value(gameObject, lightIntensityRange.x, lightIntensityRange.y, cycleTime)
            .setEase(cycleCurve)
            .setOnUpdate((float f) =>
            {
                _light.intensity = f;
                if (billboardQuad != null)
                {
                    if (maintainVerticalAlignment)
                    {
                        var dir = mainCamera.forward;
                        dir.y = 0;
                        billboardQuad.rotation = Quaternion.LookRotation(dir);
                    }
                    else billboardQuad.rotation = mainCamera.rotation;
                }
            })
            .setRepeat(-1);            
    }
}
