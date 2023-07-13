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
        GameManager.onSceneLoaded += Set;
    }

    private void OnDisable()
    {
        GameManager.onSceneLoaded -= Set;
    }
    public void Set(ProgressInfo args)
    {
        if (args.spawn >= spawnPoints.Count || args.spawn < 0)
        {
            Debug.LogError($"# {gameObject.scene.name} warp index out of range: {args.spawn} != [0 - {spawnPoints.Count}]");
            return;
        }

        var point = spawnPoints[args.spawn];
        if (player != null)
        {
            player.Position = point.position;
            player.transform.rotation = point.rotation;
        }
        camera_?.ResetPosition();
        Debug.Log("Set spawn point...");
    }
}
