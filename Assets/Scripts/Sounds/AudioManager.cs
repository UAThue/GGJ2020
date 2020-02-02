using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using System;

public class AudioManager : MonoBehaviour
{
    public Sounds[] sounds;

    // Start is called before the first frame update
    void Awake()
    {
        foreach (Sounds s in sounds)
        {
          s.source = gameObject.AddComponent<AudioSource>();
          s.source.clip = s.clip;

          s.source.volume = s.volume;
          s.source.pitch = s.pitch;
          s.source.loop = s.loop;
        }
    }

    void Start()
    {
        Play("Theme");

        // for use in other scripts:
        // FindObjectOfType<AudioManager>().Play("NameOfClipHere");
    }

    public void Play(string name)
    {
       Sounds s = Array.Find(sounds, sound => sound.Name == name);
       s.source.Play();

       if (s == null)
       {
           Debug.Log("Sound being referenced can't be found. Check if you spelled the name correctly");
       }
    }
}
