using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tool : MonoBehaviour
{
    private ToolHandler handler;
    public AnimatorOverrideController animatorController;
    private AnimatorManager animatorManager;
    public float toolDuration = 1.5f;


    public ToolHandler Handler
    {
        get => handler;
        set
        {
            handler = value;
            animatorManager = handler.animatorManager;
            animatorManager.OverrideController(animatorController);
        }
    }

    public void Use()
    {
        print("Tool used");
        animatorManager.Play("Use", 0.3f, 0, true);
    }
}
