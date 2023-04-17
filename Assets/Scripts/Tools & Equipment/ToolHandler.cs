using System.Collections;
using UnityEngine;

public class ToolHandler : MonoBehaviour
{
    public AnimatorManager animatorManager { get; private set; }
    public InputReader input;
    public Tool currentTool;
    public bool isUsing;

    private void OnEnable()
    {
        if (input == null)
        {
            input = FindObjectOfType<InputReader>();
        }

        if (input != null)
        {
            input.onUseItem += UseTool;
        }
    }

    private void OnDisable()
    {
        if (input != null)
        {
            input.onUseItem -= UseTool;
        }
    }

    private void Start()
    {
        animatorManager = GetComponent<AnimatorManager>();
        if (currentTool != null)
        {
            AsignTool(currentTool);
        }
    }

    public void AsignTool(Tool tool)
    {
        if (tool == null) return;

        IEnumerator OnAsign()
        {
            animatorManager.Play("Equip", 0.3f, 0, true);
            yield return new WaitForSeconds(0.3f);
            currentTool = tool;
            currentTool.Handler = this;
        }

        StartCoroutine(OnAsign());
    }

    public void UseTool()
    {
        if (currentTool == null || isUsing) return;

        IEnumerator OnUse()
        {
            isUsing = true;
            yield return new WaitForSeconds(currentTool.toolDuration - 0.3f);
            isUsing = false;
        }

        currentTool.Use();
        StartCoroutine(OnUse());
    }

    public void RemoveTool()
    {
        if (currentTool == null) return;

        IEnumerator OnRemove()
        {
            animatorManager.Play("Equip", 0.3f, 0, true);
            yield return new WaitForSeconds(0.3f);
            currentTool = null;
            animatorManager.OverrideController(null);
        }

        StartCoroutine(OnRemove());
    }
}
