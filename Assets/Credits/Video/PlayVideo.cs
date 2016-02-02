using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[RequireComponent (typeof(AudioSource))]

public class PlayVideo : MonoBehaviour {

  public MovieTexture movie;
  public AudioSource myAudio;

	void Start () {
    GetComponent<RawImage>().texture = movie as MovieTexture;
    myAudio = GetComponent<AudioSource> ();
    myAudio.clip = movie.audioClip;
    movie.Play ();
    myAudio.Play ();
	}
	
	void Update () {
    if (!movie.isPlaying) {
      Start();
      StartCoroutine(SomeCoroutine());
    }
	}

  private IEnumerator SomeCoroutine() {
    yield return new WaitForSeconds (0);
    SceneManager.LoadScene(0);
  }
}
