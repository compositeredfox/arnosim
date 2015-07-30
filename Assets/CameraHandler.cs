﻿using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Camera))]
public class CameraHandler : MonoBehaviour {

	public Vector3 lookAtBounds = Vector3.one;

	internal InteractiveObject targetObject;

	Camera _camera;

	public static CameraHandler instance;
	void Awake() {
		instance = this;
		_camera = GetComponent<Camera>();
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

		Vector3 lookAtPos = Vector3.zero;
		if (targetObject != null) {
			lookAtPos = targetObject.transform.position;
		} else {
			Vector3 viewportPoint = _camera.ScreenToViewportPoint(Input.mousePosition);
			viewportPoint -= Vector3.one * 0.5f;
			viewportPoint.Scale(Vector3.one * 2);
			lookAtPos = new Vector3(viewportPoint.x * 90, 0, viewportPoint.y * -250);
		}
		lookAtPos.x = Mathf.Clamp(lookAtPos.x, -lookAtBounds.x,lookAtBounds.x);
		lookAtPos.y = Mathf.Clamp(lookAtPos.y, -lookAtBounds.y,lookAtBounds.y);
		lookAtPos.z = Mathf.Clamp(lookAtPos.z, -lookAtBounds.z,lookAtBounds.z);
		iTween.LookUpdate(gameObject, lookAtPos, 15);
		_camera.fieldOfView = iTween.FloatUpdate(_camera.fieldOfView, targetObject ? 24 : 60, 5);
	}
}
