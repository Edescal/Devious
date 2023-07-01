using UnityEngine;

[CreateAssetMenu(fileName = "Music loop", menuName = "New music loop data")]
public class MusicData:ScriptableObject
{
    [field:SerializeField]
    public AudioClip musicClip { get; private set; }
    [field: SerializeField]
    public double loopStart { get; private set; }
    [field: SerializeField]
    public double loopEnd { get; private set; }

    public AudioClip GetLoop()
    {
        double loopLength = loopEnd - loopStart;
        int samples = (int)(musicClip.frequency * loopLength);

        AudioClip subclip = AudioClip.Create($"{musicClip.name}-loop", samples, musicClip.channels, musicClip.frequency, false);
        float[] sampleData = new float[subclip.samples * subclip.channels];
        int sampleStartPosition = (int)(musicClip.frequency * loopStart);

        musicClip.GetData(sampleData, sampleStartPosition);
        subclip.SetData(sampleData, 0);

        return subclip;
    }

    public AudioClip GetIntro()
    {
        int samples = (int)(musicClip.frequency * loopStart);
        AudioClip subclip = AudioClip.Create($"{musicClip.name}-intro", samples, musicClip.channels, musicClip.frequency, false);
        float[] sampleData = new float[subclip.samples * subclip.channels];
        musicClip.GetData(sampleData, 0);
        subclip.SetData(sampleData, 0);
        return subclip;
    }
}
