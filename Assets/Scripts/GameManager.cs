using System;
using System.IO;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine;
using Edescal;

public class GameManager : MonoBehaviour
{
    public static GameManager instance { get; private set; }
    public static bool inSceneTransition { get; private set; }
    public static event Action<ProgressInfo> onSceneLoaded;

    [field:SerializeField]
    public MusicHandler musicHandler { get; private set; }
    [field:SerializeField]
    public FadeScreen fadeScreen { get; private set; }

    public ProgressInfo ProgressInfo => progressInfo;

    [SerializeField]
    private float timeBetweenScenes = 1f;
    [SerializeField]
    private ProgressInfo progressInfo;


    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        inSceneTransition = false;
        DontDestroyOnLoad(gameObject);
        Localization.Init();
        //Screen.SetResolution(640, 480, true);
    }

    public void NewGame()
    {
        progressInfo = new ProgressInfo(0, 0, 0);
    }

    public void SaveProgress()
    {
        string json = JsonUtility.ToJson(progressInfo, true);
        File.WriteAllText(Application.persistentDataPath + "/SaveData.json", json);
    }

    public void LoadProgress()
    {
        string json = File.ReadAllText(Application.persistentDataPath + "/SaveData.json");
        ProgressInfo info = (ProgressInfo)JsonUtility.FromJson(json, typeof(ProgressInfo));
        Debug.Log($@"Progress loaded:
                    Phase: {info.phase}
                    Scene index: {info.scene}
                    Spawn point: {info.spawn}");
    }

    public void AddProgress(int n)
    {
        progressInfo.phase = n;
        Debug.Log($"El juego entró a una nueva fase: {n} | Nuevos eventos ocurrirán");
    }

    public void ChangeScene(SceneWarp warp)
    {
        if (warp == null || inSceneTransition || fadeScreen == null) return;

        inSceneTransition = true;

        int previousScene = progressInfo.scene;
        progressInfo.scene = warp.sceneIndex;
        progressInfo.spawn = warp.spawnPoint;

        IEnumerator Async()
        {
            Time.timeScale = 0;

            yield return fadeScreen.Show();

            var load = SceneManager.LoadSceneAsync(progressInfo.scene);
            load.allowSceneActivation = false;
            while(load.progress < 0.9f)
            {
                yield return null;
            }

            if (musicHandler?.IsPlaying == true)
            {
                musicHandler.Stop();
                var waitMusicToStop = new WaitForSecondsRealtime(musicHandler.FadeDuration);
                yield return waitMusicToStop;
            }

            load.allowSceneActivation = true;
            var wait = new WaitForSecondsRealtime(timeBetweenScenes);
            yield return wait;

            print($"Loaded scene {SceneManager.GetSceneByBuildIndex(progressInfo.scene).name}");
            onSceneLoaded?.Invoke(progressInfo);

            yield return fadeScreen.Hide();

            Time.timeScale = 1;
            inSceneTransition = false;
        }

        StartCoroutine(Async());
    }
}
