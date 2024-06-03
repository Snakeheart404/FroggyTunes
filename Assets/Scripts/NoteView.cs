using UnityEngine;

public class NoteView : MonoBehaviour
{
    private NotePresenter presenter;
    private Vector2 startPos;

    public void Initialize(NotePresenter presenter)
    {
        this.presenter = presenter;
        startPos = transform.position;
    }

    void Start()
    {
        presenter = new NotePresenter();
    }

    void Update()
    {
        presenter.Update();
    }

    public void SetPosition(Vector2 position)
    {
        transform.position = position;
    }

    public void DestroyNote()
    {
        Destroy(gameObject);
    }

    public Vector2 GetButtonPosition()
    {
        return GameObject.Find("Button").transform.position;
    }

    public Vector2 StartPosition => startPos;

    public bool IsKeyPressed()
    {
        return Input.GetKeyDown(KeyCode.Space); 
    }
}
