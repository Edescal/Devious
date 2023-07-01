using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraFOV : MonoBehaviour
{
    public CinemachineVirtualCamera virtualCamera;
    public float targetFOV = 60;
    public float duration = 2f;
    public bool ignoreTimeScale;
    public LeanTweenType curve;

    void Start()
    {
        if (virtualCamera == null) return;

        LeanTween.value(virtualCamera.m_Lens.FieldOfView, targetFOV, duration)
            .setEase(curve)
            .setOnUpdate((float f) =>
            {
                virtualCamera.m_Lens.FieldOfView = f;
            })
            .setOnComplete(() => virtualCamera.m_Lens.FieldOfView = targetFOV)
            .setIgnoreTimeScale(ignoreTimeScale);
    }

}
