// Presenters/NotePresenter.cs
using UnityEngine;

public class NotePresenter
{
    private readonly NoteModel model;
    private readonly NoteView view;
    private readonly MusicModel musicModel;

    public NotePresenter(NoteModel model, NoteView view, MusicModel musicModel)
    {
        this.model = model;
        this.view = view;
        this.musicModel = musicModel;
    }

    public void Update()
    {
        Vector2 buttonPos = view.GetButtonPosition();
        Vector2 startPos = view.StartPosition;

        Vector2 position = Vector2.Lerp(
            buttonPos,
            new Vector2(startPos.x, buttonPos.y),
            (-(musicModel.SongPosition - model.BeatTime) / (musicModel.TimeSpawn))
        );

        view.SetPosition(position);

        if (view.IsKeyPressed())
        {
            OnKeyPress();
        }
    }

    private void OnKeyPress()
    {
        float timeDifference = Mathf.Abs(musicModel.SongPosition - model.BeatTime);
        string score = CalculateScore(timeDifference);
        Debug.Log(score);
        view.DestroyNote();
    }

    private string CalculateScore(float timeDifference)
    {
        if (timeDifference <= 1f / 60f) return "Perfect";
        if (timeDifference <= 2f / 60f) return "Great";
        if (timeDifference <= 4f / 60f) return "Good";
        if (timeDifference <= 6f / 60f) return "Okay";
        if (timeDifference <= 8f / 60f) return "Bad";
        return "Miss";
    }
}