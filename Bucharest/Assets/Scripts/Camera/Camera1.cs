using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Follows the player with damping, doesn't follow rotation

public class Camera1 : MonoBehaviour
{
	[SerializeField] Transform target;
	[SerializeField] Vector3 offset = Vector3.zero;
	[SerializeField] float damping = 0.0f;

	private void Start()
	{
		offset = transform.position - target.position;
	}

	private void LateUpdate()
	{
		Vector3 desiredPosition = target.position + offset;
		Vector3 position = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime * damping);
		transform.position = position;

		transform.LookAt(target);
	}
}
