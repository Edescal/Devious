using UnityEngine;

public class SceneWarp : MonoBehaviour
{
    public int sceneIndex;
    public int spawnPoint;

    public void Load()
    {
        if (GameManager.instance == null)
        {
            Debug.Log("No Game Manager found");
            return;
        }

        GameManager.instance.ChangeScene(this);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Load();
        }
    }
}
