using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Game : MonoBehaviour {

	public GameEvent endingEvent_normal;
	public GameEvent endingEvent_future;
	public GameObject endingObject;
	public GameObject endButton;
	public CanvasGroup endingCanvasGroup;

	public CanvasGroup decorCg;
	public CanvasGroup objectsCg;

	List<InteractiveObject> objects = new List<InteractiveObject>();

	int _objectsInteracted = 0;

	bool _showedEnding = false;
	bool _showingEnding = false;

	public static Game instance;
	void Awake() {
		instance = this;
		_showedEnding = false;
	}

	// Use this for initialization
	void Start () {
		endingObject.SetActive(false);
		endButton.SetActive(false);
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

		if (_objectsInteracted >= 10 && !_showedEnding) {
			endButton.SetActive(true);
		}


	}

	public void TriggerEnding() {
		StartCoroutine(Ending ());
		endButton.SetActive(false);
	}

	IEnumerator Ending() {
		_showedEnding = true;
		_showingEnding = true;
		endingCanvasGroup.alpha = 0;
		endingObject.SetActive(true);

		float bgmvol = AudioListener.volume;
		float t=0,duration=4;
		while(t<duration) {
			t += Time.deltaTime;
			endingCanvasGroup.alpha = t/duration;
			AudioListener.volume = Mathf.Lerp(bgmvol, 0, t/duration);
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
		AudioListener.volume = 1;

		decorCg.alpha = objectsCg.alpha = 1;

	}
}
