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

	public bool visible { get { return parent.activeSelf; } }

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

	CanvasGroup ButtonsGroup { get { return buttonsContainer.gameObject.GetComponent<CanvasGroup>(); } }

	public void Show(GameEvent CurrentEvent, InteractiveObject ParentObject=null) {
		/*while (buttonsContainer.childCount > 0) {
			Destroy(buttonsContainer.GetChild(0).gameObject);
		}*/
		_currentEvent = CurrentEvent;
		foreach(DialogButton b in buttons) { b.gameObject.SetActive(false); }

		if (_currentEvent.actions.Length == 0)  {
			SetButton(buttons[0], "Continue my quest for the truth.");
		} else {
			for(int i=0;i<Mathf.Min(_currentEvent.actions.Length,buttons.Length);i++) {
				SetButton(buttons[i], _currentEvent.actions[i].label);
			}
		}

		ButtonsGroup.alpha = 0;
		ButtonsGroup.interactable = false;
		ButtonsGroup.blocksRaycasts = false;

		if (ParentObject != null) {
			_currentObject = ParentObject;
		}
		if (_currentObject != null) {
			objectName.text = _currentObject.name;
		} else {
			objectName.text = "";
		}
		CameraHandler.instance.targetObject = _currentObject;

		parent.SetActive(true);

		dialogText.text = "";
		var reveal = dialogText.GetComponent<RevealText>();
		reveal.Reveal2(_currentEvent.description, onComplete: () => StartCoroutine(RevealButtons()));
	}

	IEnumerator RevealButtons() {
		yield return null;//new WaitForSeconds(0.5f);
		ButtonsGroup.alpha = 1.0f;
		ButtonsGroup.interactable = true;
		ButtonsGroup.blocksRaycasts = true;
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
		if (!_currentEvent.PerformAction(index)) {
			Game.instance.OnFinishedInteraction();
			_currentObject = null;
		}
	}

}
