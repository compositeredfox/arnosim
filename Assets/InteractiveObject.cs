using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class InteractiveObject : MonoBehaviour {

	public enum ObjectState {
		Interactable,
		Dirty
	}

	public string name = "";
	public ObjectState state = ObjectState.Interactable;
	public GameObject graphic_dirty;
	public GameObject graphic_done;

	public GameEvent triggerEvent;
	bool _interactable = true;
	bool _interacted = false;

	void Start() {
		SetState(state);
		Game.instance.RegisterObject(this);
	}

	public void SetState(ObjectState State) {
		state = State;
		//_interactable = State != ObjectState.Done;
		graphic_dirty.SetActive(State == ObjectState.Dirty);
		graphic_done.SetActive(triggerEvent._triggered);
	}

	public void OnSelect() {
		if (!_interactable)
			return;
		//Debug.Log ("click");
		if (triggerEvent)
			triggerEvent.OnTriggered(this);
		if (!_interacted) {
			_interacted = true;
			Game.instance.OnFirstInteraction(this);
		}
		graphic_done.SetActive(triggerEvent._triggered);
	}

	public void OnPointerEnter() {
		//if (!_interactable)
		//	return;
		DialogName.instance.OnShow(name);

		//Debug.Log ("enter");
	}
	public void OnPointerExit() {
		DialogName.instance.Hide();
	}
}
