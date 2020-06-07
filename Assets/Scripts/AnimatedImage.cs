using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatedImage : MonoBehaviour
{

    public Sprite[] frames;
    public int framesPerSecond = 10;
    // Start is called before the first frame update
    SpriteRenderer sr;
    void Start()
    {
        sr = gameObject.GetComponent<SpriteRenderer>();
    }



    void Update()
    {
        int index = (int)(Time.time * framesPerSecond) % frames.Length;
        sr.sprite = frames[index];
       // renderer.material.mainTexture = frames[index];
    }
}
