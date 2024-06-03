public class NoteModel
{
    private float beatTime;
    private int channel;


    public float BeatTime
    {
        get => beatTime;
        set => beatTime = value;
    }

    public int Channel
    {
        get => channel;
        set => channel = value;
    }

    public NoteModel(float beatTime, int channel)
    {
        this.beatTime = beatTime;
        this.channel = channel;
    }
}