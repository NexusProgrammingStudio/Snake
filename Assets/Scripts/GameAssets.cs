using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameAssets : MonoBehaviour
{

    public static GameAssets i;

    private void Awake() {
        i = this;
    }
    
    public Sprite snake1HeadSprite;
    public Sprite snake1BodySprite;
    public Sprite snake2HeadSprite;
    public Sprite snake2BodySprite;
    public Sprite foodSprite;
    public Sprite badfoodSprite;

    public SoundAudioClip[] soundAudioClipArray;

    [Serializable]
    public class SoundAudioClip {
        public SoundManager.Sound sound;
        public AudioClip audioClip;
    }
}
