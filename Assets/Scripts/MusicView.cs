using UnityEngine;

public class MusicView : MonoBehaviour
{
    private MusicPresenter presenter;
    [SerializeField] private GameObject notePrefab;

    void Start()
    {
        string notesFilePath = Application.streamingAssetsPath + "/teresamarialol.txt"; // Path to your notes file
        string audioClipName = "yourAudioClip"; // Name of your audio clip in the Resources folder

        presenter = new MusicPresenter(3, notesFilePath, audioClipName, this);
        presenter.Start();
    }

    void Update()
    {
        presenter.Update();
    }

    public void PlayAudio(AudioClip clip)
    {
        AudioSource audioSource = GetComponent<AudioSource>();
        audioSource.clip = clip;
        audioSource.Play();
    }

    public float GetDspTime()
    {
        return (float)AudioSettings.dspTime;
    }

    public void InstantiateNote(NoteModel noteModel)
    { // Load your note prefab here
        GameObject note = Instantiate(notePrefab, new Vector3(11, 0, 0), Quaternion.identity);
        NoteView noteView = note.GetComponent<NoteView>();
        noteView.Initialize(new NotePresenter(noteModel, noteView, presenter.GetMusicModel()));
    }
}