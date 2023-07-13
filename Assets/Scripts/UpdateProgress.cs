using System;
using UnityEngine;
using UnityEngine.Events;

public class UpdateProgress : MonoBehaviour
{
    [Serializable]
    private struct PhaseActions
    {
        public int phase;
        public UnityEvent onPhase;
    }
    [SerializeField]
    private PhaseActions[] phaseActions;

    private void Start()
    {
        Evaluate();
    }

    private void Evaluate()
    {
        foreach(var phase in  phaseActions)
        {
            if (GameManager.instance?.ProgressInfo.phase == phase.phase)
            {
                phase.onPhase?.Invoke();
            }
        }
    }

    public void NextPhase(int n)
    {
        if (GameManager.instance?.ProgressInfo.phase == (n - 1))
        {
            Debug.Log($"Nueva fase: {n} | Cosas nuevas sucederán");
            GameManager.instance.AddProgress(n);
            Evaluate();
        }
    }
}
