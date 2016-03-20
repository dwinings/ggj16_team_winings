using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Wisp.ElementalDefense {
  public class SFXManager : MonoBehaviour {

    public static SFXManager instance = null;

    [System.Serializable]
    public class NamedAudioClip {
      public string name;
      public AudioClip clip;
    }

    public NamedAudioClip[] audioClips;

    private Dictionary<string, AudioClip> audioDict;

    void Awake() {
      if (instance == null) {
        instance = this;
        this.transform.SetParent(GameManager.instance.AudioHolder);
      }
    }

    void Start() {
      //    _instance = this;
      audioDict = new Dictionary<string, AudioClip>();
      foreach (NamedAudioClip c in audioClips) {
        audioDict.Add(c.name, c.clip);
      }
    }

    AudioSource PlayClipAt(AudioClip clip, Vector3 pos) {
      GameObject tempGO = new GameObject("TempAudio"); // create the temp object
      tempGO.transform.position = pos; // set its position
      tempGO.transform.SetParent(GameManager.instance.AudioHolder);
      AudioSource aSource = tempGO.AddComponent<AudioSource>(); // add an audio source
      aSource.clip = clip; // define the clip
      aSource.Play(); // start the sound
      Destroy(tempGO, clip.length); // destroy object after clip duration
      return aSource; // return the AudioSource reference
    }

    public void PlaySoundAt(string name, Vector3 location) {
      PlayClipAt(audioDict[name], location);
    }

    public void PlaySound(string name) {
      PlayClipAt(audioDict[name], Camera.main.transform.position);
    }
  }
}