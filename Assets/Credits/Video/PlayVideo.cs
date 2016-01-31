using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[RequireComponent (typeof(AudioSource))]

public class PlayVideo : MonoBehaviour {

  public MovieTexture movie;
  public AudioSource audio;

	void Start () {
    GetComponent<RawImage> ().texture = movie as MovieTexture;
    audio = GetComponent<AudioSource> ();
    audio.clip = movie.audioClip;
    movie.Play ();
    audio.Play ();
	}
	
	void Update () {
    if (!movie.isPlaying) {
      Start ();
      StartCoroutine(SomeCoroutine());
    }
	}

  private IEnumerator SomeCoroutine()
  {
    yield return new WaitForSeconds (0);
    Application.LoadLevel (0);
  }
}
