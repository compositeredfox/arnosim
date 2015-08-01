using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Intro : MonoBehaviour {

	public Gradient color_type;
	public Gradient color_shadow;
	public AnimationCurve shadowOffsetX;
	public AnimationCurve alpha;
	public AnimationCurve intro;
	public Text titletext;
	public Shadow titleshadow;
	public GameObject title;
	public CanvasGroup canvasGroup;
	public GameEvent firstevent;

	bool waiting = true;
	// Use this for initialization
	void Start () {

		//if (!Application.isEditor) {
			StartCoroutine(IntroSequence ());
		/*} else {
			title.SetActive(false);
			CameraHandler.instance.intro = 0;
		}*/

	}
	IEnumerator IntroSequence() {
		title.SetActive(true);
		UpdateText(0);
		CameraHandler.instance.intro = 1;

		float t=0,duration=2;
		while (t < duration) {
			t += Time.deltaTime;
			float r = Mathf.Clamp01(t / duration);

			UpdateText(r);
			yield return 0;
		}

		while(!Input.GetMouseButtonDown(0)) {
			yield return 0;
		}

		t=0;
		duration=12;
		while (t < duration) {
			t += Time.deltaTime;
			float r = Mathf.Clamp01(t / duration);
			canvasGroup.alpha = alpha.Evaluate(r);
			CameraHandler.instance.intro = intro.Evaluate(r);
			yield return 0;
		}

		yield return new WaitForSeconds(2);
		
		title.SetActive(false);

		firstevent.OnTriggered();
	}

	void Update() {
		if (waiting && Input.GetMouseButtonDown(0)) {

			waiting = false;
		}
	}

	void UpdateText(float Ratio) {
		titletext.color = color_type.Evaluate(Ratio);
		titleshadow.effectColor = color_shadow.Evaluate(Ratio);
		Vector2 v = titleshadow.effectDistance;
		v.x = shadowOffsetX.Evaluate(Ratio);
		titleshadow.effectDistance = v;

	}

}
