/* Author: Henry Chronowski
 * Updated: 06/15/2020
 * Purpose: 3rd-person camera controller with collisions and mouse controls
 * */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
	[SerializeField] Transform target;
	[SerializeField] float inPlayer = 0.1f;
	[SerializeField] float minOffset = 1.0f;
	[SerializeField] float maxOffset = 4.0f;
	[SerializeField] float damping = 0.0f;
	[Tooltip("x = min, y = max")]
	[SerializeField] Vector2 xAngle = new Vector2(-75.0f, 0.0f);
	[SerializeField] Vector2 turnSpeed = new Vector2(4.0f, 4.0f);

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
		Debug.DrawLine(target.position, transform.position, Color.blue, 0.1f);
	}

	private void LateUpdate()
	{
		RaycastHit hit;
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

		// Clamps offset to collide with objects
		if (Physics.Linecast(target.position, transform.position, out hit))
		{
			offset = Mathf.Clamp(hit.distance, minOffset, maxOffset);

			//TODO: when going to clip into player, no lerp just immediate to inside player
		}
		else
		{
			offset = maxOffset;
		}

		// Lerps the above clamp from the previous to prevent epilepsy. This is not a good way to handle this but it hurts otherwise so... fix later
		offset = Mathf.Lerp(prevOffset, offset, Time.deltaTime * damping);

		transform.position = target.position - (transform.forward * offset);
		transform.LookAt(target);

		prevOffset = offset;
	}
}