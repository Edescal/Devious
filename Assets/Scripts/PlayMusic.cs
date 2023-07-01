using UnityEngine;

public class PlayMusic : MonoBehaviour
{
    [SerializeField]
    private bool playOnStart;
    [SerializeField]
    private MusicData music;

    private void Start()
    {
        if (playOnStart)
        {
            Play();
        }
    }

    public void Play() => Play(music);
    public void Play(MusicData data)
    {
        if (music == null) return;

        var musicHandler = GameManager.instance.musicHandler;
        if (musicHandler != null)
        {
            musicHandler.Play(music);
        }
    }
}
