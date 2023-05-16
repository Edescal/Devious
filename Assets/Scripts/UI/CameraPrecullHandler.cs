using UnityEngine;

[DisallowMultipleComponent()]
public class CameraPrecullHandler : MonoBehaviour
{
    public static Camera.CameraCallback onPrecull;

    private Camera cam;

    private void Start()
    {
        cam = GetComponent<Camera>();
    }

    void OnPreCull()
    {
        if (cam != null)
        {
            print($"Camera callback from: {cam.name}");
            onPrecull?.Invoke(cam);
        }
    }

}