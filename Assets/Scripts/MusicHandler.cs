using System.Collections;
using UnityEngine;

public class MusicHandler : MonoBehaviour
{
    public bool IsPlaying => (sources[1]?.isPlaying == true || sources[0]?.isPlaying == true);
    public float FadeDuration => fadeDuration;

    [SerializeField]
    private AudioSource[] sources = new AudioSource[2];
    [SerializeField]
    private bool playOnStart = false;
    [SerializeField]
    private MusicData currentMusic;
    [SerializeField]
    private float fadeDuration = 1f;
    private double scheduledTime;
    private bool isFading;

    public void Play(MusicData data)
    {
        if (isFading) return;

        if (currentMusic != null)
        {
            IEnumerator StopThenPlay()
            {
                Stop();
                var wait = new WaitForSeconds(fadeDuration);
                yield return wait;
                Play(data);
            }
            StartCoroutine(StopThenPlay());
            return;
        }

        currentMusic = data;

        sources[0].Stop();
        sources[1].Stop();

        SetAndPlay();

        isFading = true;
        LeanTween.value(0, 1, fadeDuration)
            .setEase(LeanTweenType.easeInOutSine)
            .setIgnoreTimeScale(true)
            .setOnUpdate((float f) =>
            {
                sources[0].volume = f;
                sources[1].volume = f;
            })
            .setOnComplete(() => isFading = false);
    }

    public void Stop()
    {
        if (currentMusic == null || isFading) return;

        isFading = true;
        LeanTween.value(1, 0, fadeDuration)
            .setEase(LeanTweenType.easeInOutSine)
            .setIgnoreTimeScale(true)
            .setOnUpdate((float f) =>
            {
                sources[0].volume = f;
                sources[1].volume = f;
            })
            .setOnComplete(() =>
            {
                isFading = false;
                sources[0].Stop();
                sources[1].Stop();
                currentMusic = null;
            });
    }

    private void SetAndPlay()
    {
        var intro = currentMusic.GetIntro();
        var loop = currentMusic.GetLoop();
        double introLength = (double)intro.samples / (double)intro.frequency;

        double startTime = AudioSettings.dspTime + 0.1;
        scheduledTime = startTime + introLength;

        sources[0].clip = intro;
        sources[0].PlayScheduled(startTime);
        sources[0].SetScheduledEndTime(scheduledTime);

        print($"Printing current music loop data:\nIntro duration: \t{introLength}\nStart: \t\t{startTime}\nScheduled: \t{scheduledTime}");

        sources[1].clip = loop;
        sources[1].PlayScheduled(scheduledTime);

    }

    private void Start()
    {
        if (sources[1] == null || sources[0] == null)
            return;
        else
        {
            sources[0].volume = 0;
            sources[0].loop = false;
            sources[1].volume = 0;
            sources[1].loop = true;
        }

        if (playOnStart && currentMusic != null)
        {
            Play(currentMusic);
        }
    }
}
