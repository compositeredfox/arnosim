using UnityEngine;
using System.Collections;

public class GameEvent : MonoBehaviour {

	public bool triggerOnlyOnce = false;

	[Multiline]
	public string description = "";
	public GameEventAction[] actions;

	internal bool _triggered = false;

	public void OnTriggered(InteractiveObject ByObject = null) {
		if (triggerOnlyOnce && _triggered)
			return;
		Debug.Log ("showing " + this);
		Dialog.instance.Show(this, ByObject);
		_triggered = true;
	}

	public void PerformAction(GameEventAction Action) {
		if (Action.actionType == GameEventAction.ActionType.TriggerEvent) {
			Action.triggerEvent.OnTriggered();
		} else if (Action.actionType == GameEventAction.ActionType.SetObjectState) {
			InteractiveObject o = Action.targetObject;
			if (o == null) {
				o = transform.parent.parent.GetComponentInChildren<InteractiveObject>();
			}
			if (o == null) {
				return;
			}
			o.SetState(Action.targetState);
		}
	}
	public void PerformAction(int Index) {
		if (actions.Length > 0 && Index < actions.Length)
			PerformAction(actions[Index]);
	
	}
}

[System.Serializable]
public class GameEventAction {
	public enum ActionType {
		TriggerEvent,
		SetObjectState
	}
	[Multiline]
	public string label = "";
	public ActionType actionType = ActionType.TriggerEvent;
	public GameEvent triggerEvent;
	public InteractiveObject targetObject = null;
	public InteractiveObject.ObjectState targetState = InteractiveObject.ObjectState.Interactable;

}