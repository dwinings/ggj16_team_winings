using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SFXManager : MonoBehaviour {

  public static SFXManager instance = null;
//  private static SFXManager _instance;
//  public static SFXManager Instance {
//    get {
//      return _instance;
//    }
//  }

  [System.Serializable]
  public class NamedAudioClip {
    public string name;
    public AudioClip clip;
  }

  public NamedAudioClip[] audioClips;

  private Dictionary<string, AudioClip> audioDict;

  void Awake () {
    if(instance == null)
      instance = this;
  }

  void Start() {
//    _instance = this;
    audioDict = new Dictionary<string, AudioClip> ();
    foreach (NamedAudioClip c in audioClips) {
      audioDict.Add(c.name, c.clip);
    }
  }

  public void PlaySoundAt(string name, Vector3 location) {
    AudioSource.PlayClipAtPoint (audioDict[name], location);
  }

  public void PlaySound(string name) {
    AudioSource.PlayClipAtPoint (audioDict[name], Camera.main.transform.position);
  }
}