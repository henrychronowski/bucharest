using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Freely ish moving camera

public class Camera4 : MonoBehaviour
{
	[SerializeField] Transform target;
	[SerializeField] float offset = 0.0f;
	[SerializeField] float damping = 0.0f;
	[SerializeField] float minTurnAngle = -90.0f;
	[SerializeField] float maxTurnAngle = 0.0f;
	[SerializeField] float turnSpeed = 4.0f;

	private float rotX = 0.0f;

	private void LateUpdate()
	{
		float y = Input.GetAxis("Mouse X") * turnSpeed;
		rotX += Input.GetAxis("Mouse Y") * turnSpeed;

		rotX = Mathf.Clamp(rotX, minTurnAngle, maxTurnAngle);

		transform.eulerAngles = new Vector3(-rotX, transform.eulerAngles.y + y, 0.0f);
		transform.position = target.position - (transform.forward * offset);

		transform.LookAt(target);
	}
}
