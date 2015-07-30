using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Dialog : MonoBehaviour {

	public Text dialogText;
	public Text objectName;
	public RectTransform buttonsContainer;

	public GameObject parent;

	public DialogButton[] buttons;

	GameEvent _currentEvent;
	InteractiveObject _currentObject;
	CanvasGroup canvasGroup;

	public static Dialog instance;
	void Awake () {
		instance = this;
		canvasGroup = GetComponent<CanvasGroup>();
		canvasGroup.alpha = 0;
		Hide();
	}
	
	void Update () {
		canvasGroup.alpha = iTween.FloatUpdate(canvasGroup.alpha, parent.activeSelf ? 1 : 0, 4);
	}

	public void Show(GameEvent CurrentEvent, InteractiveObject ParentObject=null) {
		/*while (buttonsContainer.childCount > 0) {
			Destroy(buttonsContainer.GetChild(0).gameObject);
		}*/
		_currentEvent = CurrentEvent;
		foreach(DialogButton b in buttons) { b.gameObject.SetActive(false); }

		if (CurrentEvent.actions.Length == 0)  {
			SetButton(buttons[0], "Continue my quest for the truth.");
		} else {
			for(int i=0;i<Mathf.Min(_currentEvent.actions.Length,buttons.Length);i++) {
				SetButton(buttons[i], _currentEvent.actions[i].label);
			}
		}

		if (ParentObject != null) {
			_currentObject = ParentObject;
		}
		objectName.text = _currentObject.name;
		CameraHandler.instance.targetObject = _currentObject;

		dialogText.text = _currentEvent.description;

		parent.SetActive(true);

	}
	public void Hide() {
		//Debug.Log ("Hiding");
		parent.SetActive(false);
		CameraHandler.instance.targetObject = null;
		objectName.text = "";
	}

	void SetButton(DialogButton button, string Label) {
		if (string.IsNullOrEmpty(Label)) {
			button.gameObject.SetActive(false);
			return;
		}
		button.gameObject.SetActive(true);
		button.label.text = Label;

	}

	public void OnHitButton(int index) {
		Hide();
		_currentEvent.PerformAction(index);
	}

}
