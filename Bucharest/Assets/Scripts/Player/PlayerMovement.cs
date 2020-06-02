/*Author(s): Your-Name(s)
 * Updated: 06/2/2020
 * Purpose: A short statement describing the functionality of the script
 */


using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private int jumpForce;
    [SerializeField] private float sprintSpeed;
    [SerializeField] private float speed;
    [SerializeField] private float accelerationRate;
    Rigidbody rb;
    Vector3 moveDir;

    private void Awake()
    {
     
        rb = gameObject.GetComponent<Rigidbody>();
    }
   

    

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        moveDir = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

        if (moveDir != Vector3.zero)
            Move(Input.GetKey(KeyCode.LeftShift));

        if (Input.GetKey(KeyCode.Space))
        {
            Jump();
        }
    }
    
    void Move(bool sprinting)
    {
        Debug.Log(moveDir);

        if (sprinting && moveDir.x == 0)
        {
            if (Mathf.Abs(rb.velocity.x) < sprintSpeed)
            {
                rb.velocity += new Vector3(moveDir.x * accelerationRate, 0, 0);
            }
                

            if (Mathf.Abs(rb.velocity.z) < sprintSpeed)
            {
                rb.velocity += new Vector3(0, 0, moveDir.z * accelerationRate);
            }
               
        }
        else
        {
            if (Mathf.Abs(rb.velocity.x) < speed)
            {
                rb.velocity += new Vector3(moveDir.x * accelerationRate, 0, 0);
            }
               

            if (Mathf.Abs(rb.velocity.z) < speed)
            {
                rb.velocity += new Vector3(0, 0, moveDir.z * accelerationRate);
            }
        }
               
            
    }

    void Jump()
    {
        rb.AddForce(new Vector3(0, jumpForce, 0));
    }

}

