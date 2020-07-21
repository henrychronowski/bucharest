/* Author: Henry Chronowski
 * Updated: 07/20/2020
 * Purpose: 3rd-person camera controller with collisions and mouse controls
 * */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
	[SerializeField] Transform target;
	[SerializeField] float ncp = 0.1f;
	[SerializeField] float minOffset = 1.0f;
	[SerializeField] float maxOffset = 4.0f;
	[SerializeField] float damping = 0.0f;
	[Tooltip("x = min, y = max")]
	[SerializeField] Vector2 xAngle = new Vector2(-75.0f, 0.0f);
	[SerializeField] Vector2 turnSpeed = new Vector2(4.0f, 4.0f);
	[SerializeField] float xMin = 1.0f;

	[Header("FOV")]
	[SerializeField] float maxView = 101.0f;
	[SerializeField] float minView = 60.0f;

	[Header("Whisker Casts")]
	[SerializeField] Vector3 interval = new Vector3(0.0f, 30.0f, 0.0f);
	[SerializeField] List<string> ignore = new List<string>();	//TODO: add a default "MainCamera" item
	
	private float rotX = 0.0f;
	private float offset;
	private float prevOffset;

	private void Awake()
	{
		prevOffset = maxOffset;

		//NB: This line should not be in this file... game manager?
		Cursor.lockState = CursorLockMode.Locked;
	}

	private void Update()
	{
#if UNITY_EDITOR
		Debug.DrawLine(target.position, transform.position, Color.blue);
		Debug.DrawLine(target.position, RotatePointAboutPoint(transform.position, target.position, interval), Color.red);
		Debug.DrawLine(target.position, RotatePointAboutPoint(transform.position, target.position, -1.0f * interval), Color.cyan);
		Debug.DrawLine(transform.position, transform.position - new Vector3(0.0f, xMin, 0.0f), Color.black);
#endif
	}

	private Vector3 RotatePointAboutPoint(Vector3 point, Vector3 pivot, Vector3 angles)
	{
		Vector3 dir = point - pivot;
		dir = Quaternion.Euler(angles) * dir;
		point = dir + pivot;
		return point;
	}

	private void LateUpdate()
	{
		RaycastHit centre, left, right, down;
		float y = Input.GetAxis("Mouse X") * turnSpeed.y;
		rotX += Input.GetAxis("Mouse Y") * turnSpeed.x;

		rotX = Mathf.Clamp(rotX, xAngle.x, xAngle.y);
		transform.eulerAngles = new Vector3(-rotX, transform.eulerAngles.y + y, transform.eulerAngles.z);
		target.Rotate(0, y, 0);

		//Will be used to provide a free-look
		//		Need to include some sort of storage of the original angle + lerp back to it after release
		//if(!Input.GetKey(KeyCode.LeftAlt))
		//{
		//	target.Rotate(0, y, 0);
		//}

		// Clamps offset to collide with objects, whisker casts attempt to be proactive with clipping
		offset = maxOffset;
		gameObject.GetComponent<Camera>().fieldOfView = Mathf.Lerp(minView, maxView, Mathf.Abs(transform.eulerAngles.x / xAngle.x));
		//offset = Mathf.Clamp(Mathf.Tan(((transform.eulerAngles.x) * Mathf.PI) / 180.0f) / Mathf.Tan(70.0f) * maxOffset, minOffset, maxOffset);
		//Debug.Log(Mathf.Tan(((transform.eulerAngles.x) * Mathf.PI) / 180.0f) / Mathf.Tan(70.0f));
		//offset = Mathf.Tan(Mathf.a)


		if (Physics.Linecast(target.position, transform.position, out centre))
		{
			if (!ignore.Contains(centre.transform.gameObject.tag))
			{
				offset = Mathf.Clamp(centre.distance - ncp, minOffset, maxOffset);
			}
		}
		else if (Physics.Linecast(target.position, RotatePointAboutPoint(transform.position, target.position, interval), out right))
		{
			if (!ignore.Contains(right.transform.gameObject.tag))
			{
				offset = Mathf.Clamp(right.distance - ncp, minOffset, maxOffset);
			}
		}
		else if (Physics.Linecast(target.position, RotatePointAboutPoint(transform.position, target.position, -1.0f * interval), out left))
		{
			if (!ignore.Contains(left.transform.gameObject.tag))
			{
				offset = Mathf.Clamp(left.distance - ncp, minOffset, maxOffset);
			}
		}
		else if (Physics.Linecast(transform.position, transform.position - new Vector3(0.0f, xMin, 0.0f), out down))
		{
			if (!ignore.Contains(down.transform.gameObject.tag))
			{
				offset = Mathf.Clamp((down.distance / xMin) * maxOffset, minOffset, maxOffset);
			}
		}

		// Lerps the above clamp from the previous to prevent epilepsy. This is not a good way to handle this but it hurts otherwise so... fix later
		offset = Mathf.Lerp(prevOffset, offset, Time.deltaTime * damping);

		transform.position = target.position - (transform.forward * offset);
		transform.LookAt(target);

		prevOffset = offset;
	}
}