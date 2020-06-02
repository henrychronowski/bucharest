using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Freely ish moving camera

public class Camera4 : MonoBehaviour
{
	[SerializeField] Transform target;
	[SerializeField] float minOffset = 1.0f;
	[SerializeField] float maxOffset = 4.0f;
	[SerializeField] float damping = 0.0f;
	[SerializeField] float minTurnAngle = -90.0f;
	[SerializeField] float maxTurnAngle = 0.0f;
	[SerializeField] Vector2 turnSpeed = new Vector2(4.0f, 4.0f);

	private float rotX = 0.0f;
	private float offset;
	private float prevOffset;

	private void Awake()
	{
		prevOffset = maxOffset;

		//NB: This line should not be in this file
		Cursor.lockState = CursorLockMode.Locked;
	}

	private void LateUpdate()
	{
		RaycastHit hit;
		float y = Input.GetAxis("Mouse X") * turnSpeed.x;
		rotX += Input.GetAxis("Mouse Y") * turnSpeed.y;

		rotX = Mathf.Clamp(rotX, minTurnAngle, maxTurnAngle);
		transform.eulerAngles = new Vector3(-rotX, transform.eulerAngles.y + y, 0.0f);

		if(Physics.Linecast(target.position, transform.position, out hit))
		{
			offset = Mathf.Clamp(hit.distance, minOffset, maxOffset);
		}
		else
		{
			offset = maxOffset;
		}

		offset = Mathf.Lerp(prevOffset, offset, Time.deltaTime * damping);

		transform.position = target.position - (transform.forward * offset);
		transform.LookAt(target);

		prevOffset = offset;
	}
}