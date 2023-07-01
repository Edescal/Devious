using UnityEngine;

public class SceneWarp : MonoBehaviour
{
    public SceneTransitionArgs transitionArgs;

    public void Load()
    {
        if (GameManager.instance == null)
        {
            Debug.Log("No Game Manager found");
            return;
        }

        GameManager.instance.LoadScene(transitionArgs);
    }
}
