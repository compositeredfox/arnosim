using UnityEngine;
using System.Collections;

public class MoveToCursor : MonoBehaviour {

	public Canvas canvas;

	// Update is called once per frame
	void Update () {
		Vector2 pos;
		RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, Input.mousePosition, canvas.worldCamera, out pos);
		transform.position = canvas.transform.TransformPoint(pos);
	}
}
