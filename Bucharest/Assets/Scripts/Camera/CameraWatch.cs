using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraWatch : MonoBehaviour
{
	[SerializeField] private Transform target;

	private void LateUpdate()
	{
		transform.LookAt(target);
	}
}
