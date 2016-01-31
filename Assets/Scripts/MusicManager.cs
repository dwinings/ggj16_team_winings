using UnityEngine;
using System.Collections;

public class MusicManager : MonoBehaviour {

  public static MusicManager instance = null;
  public AudioClip [] clips;
  public AudioClip jokeAudio;
	public int clipIndex = 0;
	public bool selectRandom = true;
	public bool playMusic = true;

//	private int x;

	// Use this for initialization

  void Awake () {
    if(instance == null)
      instance = this;
  }

	void Start () {
		if (playMusic) {
			if (selectRandom) 
        clipIndex = (int)Random.Range(0, clips.Length);
      var audio = GetComponent<AudioSource> ();
			audio.clip = clips[clipIndex];
      audio.loop = true;
			audio.Play ();
		}
	}

  public void StartJoke () {
    Stop ();
    if (playMusic) {
      var audio = GetComponent<AudioSource> ();
      audio.clip = jokeAudio;
      audio.loop = true;
      audio.Play ();
    }
  }

  public void Stop () {
    var audio = GetComponent<AudioSource> ();
    audio.Stop ();
  }

}