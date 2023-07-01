using System.Collections.Generic;
using UnityEngine;

public class SceneSpawn : MonoBehaviour
{
    [SerializeField]
    private Edescal.ThirdPersonController player;
    [SerializeField]
    private Edescal.ThirdPersonCamera camera_;
    [SerializeField]
    private List<Transform> spawnPoints;

    private void OnEnable()
    {
        Debug.Log("On enable");
        GameManager.onSceneLoaded += Set;
    }

    private void OnDisable()
    {
        Debug.Log("On disable");
        GameManager.onSceneLoaded -= Set;
    }

    private void Start()
    {
        Debug.Log("On start");
    }

    public void Set(SceneTransitionArgs args)
    {
        if (args.spawnId >= spawnPoints.Count || args.spawnId < 0)
        {
            Debug.LogError($"Spawn point index out of range: {args.spawnId} != [0 - {spawnPoints.Count}]");
        }

        var point = spawnPoints[args.spawnId];
        player.Position = point.position;
        player.transform.rotation = point.rotation;
        camera_.ResetPosition();
        Debug.Log("Set spawn point...");
    }
}
