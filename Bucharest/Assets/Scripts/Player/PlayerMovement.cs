/*Author(s): Chris Foster, Henry Chronowski
 * Updated: 06/15/2020
 * Purpose: Player movement including jumping
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{

	//TODO: add default parameters to make unity stop complaining and make the code more portable -HC
    [SerializeField] private int jumpForce;
    [SerializeField] private float sprintSpeed;
    [SerializeField] private float speed;
    [SerializeField] private float accelerationRate;
    [SerializeField] private Vector3 raycastStartingLoc;
    [SerializeField] private float raycastDistance;
    Rigidbody rb;
    Vector3 moveDir;

    private void Awake()
    {
        rb = gameObject.GetComponent<Rigidbody>();
    }

    void Update()
    {
        moveDir = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

		if (moveDir != Vector3.zero)
		{
			Move(Input.GetKey(KeyCode.LeftShift));
		}

        Vector3 loc = transform.position + raycastStartingLoc;

        Debug.DrawLine(loc, loc + Vector3.down * raycastDistance, Color.red, 0.1f);
        if (Input.GetKey(KeyCode.Space))
        {
            Jump();
        }
    }
    
    void Move(bool sprinting)
    {
        if (sprinting && moveDir.x == 0)
        {
            if (Mathf.Abs(rb.velocity.magnitude) < sprintSpeed)
            {
				rb.velocity += transform.right * moveDir.x * accelerationRate;
				rb.velocity += transform.forward * moveDir.z * accelerationRate;
            }
               
        }
        else
        {
            if (Mathf.Abs(rb.velocity.magnitude) < speed)
            {
				rb.velocity += transform.right * moveDir.x * accelerationRate;
				rb.velocity += transform.forward * moveDir.z * accelerationRate;
			}
		}
    }

    void Jump()
    {
        RaycastHit hit;

        Vector3 loc = transform.position + raycastStartingLoc;
        LayerMask mask = 2;

        Debug.DrawRay(loc, Vector3.down * raycastDistance, Color.red);
        if (Physics.Raycast(loc, Vector3.down, out hit, raycastDistance, ~mask))
        {
            rb.AddForce(new Vector3(0, jumpForce, 0));
            
        }       
    }

}