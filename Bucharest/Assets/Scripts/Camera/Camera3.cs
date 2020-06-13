using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Follows the player at offset with horizontal rotation guided by mouse and no damping

public class Camera3 : MonoBehaviour
{
	[SerializeField] Transform target;
	[SerializeField] Vector3 offset = Vector3.zero;
	[SerializeField] float rotateSpeed = 1.0f;

	private void LateUpdate()
	{
		float horizontal = Input.GetAxis("Mouse X") * rotateSpeed;
		target.Rotate(0, horizontal, 0);

		float desiredAngle = target.eulerAngles.y;
		Quaternion rotation = Quaternion.Euler(0, desiredAngle, 0);
		transform.position = target.position - (rotation * offset);

		transform.LookAt(target);
	}
}
