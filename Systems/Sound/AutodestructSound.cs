using UnityEngine;
using System.Collections;

namespace LevelUpper.Sound {

	public class AutodestructSound : MonoBehaviour {
		void Awake() { if (transform.parent == null) { DontDestroyOnLoad(gameObject); } }
		void Start() { if (transform.parent == null) { DontDestroyOnLoad(gameObject); } }
		void LateUpdate() {
			if (GetComponent<AudioSource>() != null) {
				if (!GetComponent<AudioSource>().isPlaying) { Destroy(gameObject); }
			} else { Destroy(gameObject); }
		}
	}

}
