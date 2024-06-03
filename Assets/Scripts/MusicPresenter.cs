public class MusicPresenter
{
    private readonly MusicModel model;
    private readonly MusicView view;

    public MusicPresenter(float timeSpawn, string notesFilePath, string audioClipName, MusicView view)
    {
        this.view = view;
        this.model = new MusicModel(timeSpawn, notesFilePath, audioClipName);
    }

    public void Start()
    {
        model.MusicTime = view.GetDspTime();
        view.PlayAudio(model.Clip);
    }

    public void Update()
    {
        model.SongPosition = view.GetDspTime() - model.MusicTime;

        if (model.NextIndex < model.Notes.Count && model.Notes[model.NextIndex].Item1 < (model.SongPosition + model.TimeSpawn))
        {
            NoteModel noteModel = new NoteModel(model.Notes[model.NextIndex].Item1, model.Notes[model.NextIndex].Item2);
            view.InstantiateNote(noteModel);
            model.NextIndex++;
        }
    }

    public MusicModel GetMusicModel()
    {
        return model;
    }
}