using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    private Transform mainCamera;
    [SerializeField] private Transform quad;
    [SerializeField] private bool maintainVerticalAlignment = true;

    void Start()
    {
        mainCamera = Camera.main.transform;
    }

    void Update()
    {
        if (mainCamera == null) return;

        if (quad != null)
        {
            if (maintainVerticalAlignment)
            {
                var dir = Vector3.ProjectOnPlane(mainCamera.transform.forward, Vector3.up);
                quad.rotation = Quaternion.LookRotation(dir);
            }
            else quad.rotation = mainCamera.transform.rotation;
        }
    }
}
