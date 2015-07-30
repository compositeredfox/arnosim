using UnityEngine;
using System.Collections;

public class RandomSeek : MonoBehaviour {

	// Use this for initialization
	void Start () {
		GetComponent<AudioSource>().time = Random.Range(0, GetComponent<AudioSource>().clip.length);
	}

}
