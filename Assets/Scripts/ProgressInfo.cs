using System;

[Serializable]
public struct ProgressInfo
{
    public int phase;
    public int scene;
    public int spawn;

    public ProgressInfo(int phase, int scene, int spawn)
    {
        this.phase = phase;
        this.scene = scene;
        this.spawn = spawn;
    }
}
