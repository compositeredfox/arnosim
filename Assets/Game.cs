using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Game : MonoBehaviour {

	public GameEvent endingEvent_normal;
	public GameEvent endingEvent_future;
	public GameObject endingObject;
	public CanvasGroup endingCanvasGroup;

	public AudioSource bgm;

	public CanvasGroup decorCg;
	public CanvasGroup objectsCg;

	List<InteractiveObject> objects = new List<InteractiveObject>();

	int _objectsInteracted = 0;

	bool _showedEnding = false;
	bool _showingEnding = false;

	public static Game instance;
	void Awake() {
		instance = this;
	}

	// Use this for initialization
	void Start () {
		endingObject.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKey(KeyCode.Backspace) && Input.GetKeyDown(KeyCode.E)) {
			StartCoroutine(Ending ());
		}
	}

	public void RegisterObject(InteractiveObject o) {
		objects.Add(o);
	}

	public void OnFirstInteraction(InteractiveObject o) {
		_objectsInteracted++;
	}

	public void OnFinishedInteraction() {

		if (_objectsInteracted == objects.Count - 8 && !_showedEnding) {
			StartCoroutine(Ending ());
		}


	}

	IEnumerator Ending() {
		_showedEnding = true;
		_showingEnding = true;
		endingCanvasGroup.alpha = 0;
		endingObject.SetActive(true);

		float bgmvol = bgm.volume;
		float t=0,duration=4;
		while(t<duration) {
			t += Time.deltaTime;
			endingCanvasGroup.alpha = t/duration;
			bgm.volume = Mathf.Lerp(bgmvol, 0, t/duration);
			yield return 0;
		}

		yield return new WaitForSeconds(0.7f);

		endingEvent_normal.OnTriggered();

		while(Dialog.instance.visible) {
			yield return 0;
		}

		// fade out bg
		t=0;
		duration=4;
		while(t<duration) {
			t += Time.deltaTime;
			endingCanvasGroup.alpha = 1-(t/duration);
			yield return 0;
		}

		yield return new WaitForSeconds(0.3f);

		t=0;
		duration=4;
		while(t<duration) {
			t += Time.deltaTime;
			decorCg.alpha = objectsCg.alpha = 1-(t/duration);
			yield return 0;
		}

		yield return new WaitForSeconds(0.5f);

		endingEvent_future.OnTriggered();

		while(Dialog.instance.visible) {
			yield return 0;
		}

		endingObject.SetActive(false);
		bgm.volume = 1;

		decorCg.alpha = objectsCg.alpha = 1;

	}
}
