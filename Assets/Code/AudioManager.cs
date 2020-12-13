using UnityEngine;
using System;

[System.Serializable]
public class Sound {
    public string name;

    public AudioClip clip;

    [Range(0f, 1f)]
    public float volume;

    [Range(0.1f, 3f)]
    public float pitch;

    public bool loop;

    [HideInInspector]
    public AudioSource source;
}

public class AudioManager : MonoBehaviour {

    [SerializeField]
    Sound[] sounds;

	// Use this for initialization
	void Awake () {
        foreach(Sound s in sounds) {
            s.source = gameObject.AddComponent<AudioSource>();

            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
            
        }
	}
	
    void Start() {
        play("MainTheme");
    }

    public void play(string name) {
        Sound s = Array.Find(sounds, sound => sound.name.Equals(name));

        if(s == null) {
            //Error: sound not found
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }else {
            s.source.Play();
        }
    }

    public void stop(string name) {
        Sound s = Array.Find(sounds, sound => sound.name.Equals(name));

        if(s == null) {
            //Error: sound not found
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        } else {
            s.source.Stop();
        }
    }

	// Update is called once per frame
	void Update () {
	
	}
}
