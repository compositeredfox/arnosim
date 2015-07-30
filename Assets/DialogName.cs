using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DialogName : MonoBehaviour {

	public Text label;
	public CanvasGroup canvasGroup;
	public Canvas canvas;

	public static DialogName instance;
	void Awake () {
		instance = this;
		Hide();
	}
	
	void Update () {
		Vector2 pos;
		RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, Input.mousePosition, canvas.worldCamera, out pos);
		transform.position = canvas.transform.TransformPoint(pos);
	}

	public void OnShow(string Label) {
		label.text = Label;
		canvasGroup.alpha = 1;
	}
	public void Hide() {
		canvasGroup.alpha = 0;
	}

}
