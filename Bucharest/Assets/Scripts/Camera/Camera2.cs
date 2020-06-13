using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Follows behind the player with damping on rotation and distance

public class Camera2 : MonoBehaviour
{
	[SerializeField] Transform target;
	[SerializeField] Vector3 offset = Vector3.zero;
	[SerializeField] float damping = 0.0f;

	private void Start()
	{
		offset = target.position - transform.position;
	}

	private void LateUpdate()
	{
		float angle = Mathf.LerpAngle(transform.eulerAngles.y, target.eulerAngles.y, Time.deltaTime * damping);

		Quaternion rotation = Quaternion.Euler(0.0f, angle, 0.0f);
		transform.position = target.transform.position - (rotation * offset);

		transform.LookAt(target);
	}
}
