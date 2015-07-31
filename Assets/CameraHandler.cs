using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Camera))]
public class CameraHandler : MonoBehaviour {

	public Vector3 lookAtBounds = Vector3.one;
	public Transform lookAtIntro;
	public float angleIntro = -31; //z

	internal InteractiveObject targetObject;

	[Range(0,1)]
	public float intro = 1;

	Camera _camera;
	float fov = 0;

	public static CameraHandler instance;
	void Awake() {
		instance = this;
		_camera = GetComponent<Camera>();
		fov = _camera.fieldOfView;
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

		lookAtPos = Vector3.Lerp(lookAtPos, lookAtIntro.position, intro);
			 
		iTween.LookUpdate(gameObject, lookAtPos, 15);
		fov = iTween.FloatUpdate(fov, targetObject ? 24 : 60, 5);
		_camera.fieldOfView = Mathf.Lerp(fov, 3, intro);
	}
}
