using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonController : MonoBehaviour
{
    // private SpriteRenderer _spriteRenderer;
    // public Sprite Default;
    // public Sprite Pressed;

    public KeyCode KeyToPress;
    void Start()
    {
        //_spriteRenderer = GetComponent<SpriteRenderer>();
    }
    
    void Update()
    {
        if (Input.GetKeyDown(KeyToPress))
        {
            //_spriteRenderer.sprite = Pressed;
            GetComponent<AudioSource>().Play();
        }
        
        if (Input.GetKeyUp(KeyToPress))
        {
            //_spriteRenderer.sprite = Default;
        }
    }
    public KeyCode GetKeyToPress()
    {
        return KeyToPress;
    }
}
