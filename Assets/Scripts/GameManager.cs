using System;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine;
using Edescal;

public class GameManager : MonoBehaviour
{
    public static GameManager instance { get; private set; }
    public static event Action<SceneTransitionArgs> onSceneLoaded;

    [SerializeField]
    private float timeBetweenScenes = 1f;
    [field:SerializeField]
    public MusicHandler musicHandler { get; private set; }
    [field:SerializeField]
    public FadeScreen fadeScreen { get; private set; }

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void LoadScene(SceneTransitionArgs args)
    {
        if (fadeScreen == null) return;

        IEnumerator AsyncLoading()
        {
            WaitForSecondsRealtime wait;
            Time.timeScale = 0;

            fadeScreen.Show(out float fade);
            wait = new WaitForSecondsRealtime(fade);
            yield return wait;

            var asyncOp = SceneManager.LoadSceneAsync(args.sceneId);
            asyncOp.allowSceneActivation = false;
            while (asyncOp.progress < 0.9f)
            {
                yield return null;
            }

            if (musicHandler?.IsPlaying == true)
            {
                musicHandler.Stop();
                var waitMusicToStop = new WaitForSecondsRealtime(musicHandler.FadeDuration);
                yield return waitMusicToStop;
            }
            
            asyncOp.allowSceneActivation = true;
            yield return null;
            //Load new scene
            //Unload previous scene
            yield return null;

            Debug.Log("Scene loaded");
            onSceneLoaded?.Invoke(args);

            wait = new WaitForSecondsRealtime(timeBetweenScenes);
            yield return wait;

            fadeScreen.Hide(out fade);
            Debug.Log(fade);
            wait = new WaitForSecondsRealtime(fade);
            yield return wait;

            Time.timeScale = 1;
        }
        StartCoroutine(AsyncLoading());
    }

}
