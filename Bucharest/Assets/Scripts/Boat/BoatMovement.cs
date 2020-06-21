using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoatMovement : MonoBehaviour
{
    [SerializeField] private bool occupied;
    [SerializeField] private bool isGrounded;
    [SerializeField] private float speed;
    [SerializeField] private float rockSpeedX = 8, rockSpeedZ = 8;
    [SerializeField] private float rotationSensitivity;
    [SerializeField] private Vector3[] raycaststartingPoints;
    [SerializeField] private float floatForce;
    [SerializeField] private float gravitationalForce;
    [SerializeField] private float maxFloatHeight;
    [SerializeField] private Vector3 floatOffset;
    // [Range(0.01f, 0.04f)]
    //// [SerializeField] private float baseHeightAdjustmentSpeed;
    // [Range(0f, 1f)]
    // [SerializeField] private float fallSpeed;
    [SerializeField] private Vector3 normal;
    private RaycastHit groundPoint;
    private LayerMask mask = 2;
    private Rigidbody rb;
    //private float time;


    // Start is called before the first frame update
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
        // rb.centerOfMass = gameObject.transform.position;
        //time = 0;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        SetNormalAndPosition();

        if (Input.GetKey(KeyCode.W))
        {
            if (isGrounded)
            {
                if (rb.velocity.magnitude < speed)
                {
                    rb.AddForce(transform.forward * speed);
                }
                // * Time.deltaTime;
            }
            else
            {
                rb.velocity = Vector3.forward * speed;
            }

        }
        else
        {
            //
        }



    }

    void Update()
    {
        
        
    }

    void SetNormalAndPosition()
    {
        isGrounded = false;
        normal = Vector3.zero;
        for (int i = 0; i < raycaststartingPoints.Length; i++)
        {
            Debug.DrawRay(raycaststartingPoints[i] + transform.position, Vector3.up * -maxFloatHeight, Color.red, 0.1f);
        }

       
        
        for (int i = 0; i < raycaststartingPoints.Length; i++)
        {
            if (Physics.Raycast(raycaststartingPoints[i] + transform.position, Vector3.up * -1, out groundPoint, maxFloatHeight, ~mask, QueryTriggerInteraction.Ignore))
            {
                isGrounded = true;                      
            }
        }


        for (int i = 0; i < raycaststartingPoints.Length; i++)
        {
            if (Physics.Raycast(raycaststartingPoints[i] + transform.position, Vector3.up * -1, out groundPoint, Mathf.Infinity, ~mask, QueryTriggerInteraction.Ignore))
            {
                normal += groundPoint.normal;
            }
        }

        

        Physics.Raycast(raycaststartingPoints[4] + transform.position, Vector3.up * -1, out groundPoint, Mathf.Infinity, ~mask, QueryTriggerInteraction.Ignore);
        if (isGrounded)
        {
            //Debug.Log(groundPoint.distance);
            //transform.position = floor + floatOffset;
            Debug.Log(Vector3.up * floatForce * Mathf.Sqrt(floatOffset.y / (groundPoint.distance)) + Vector3.up * gravitationalForce);
            rb.AddForce(Vector3.up * floatForce * Mathf.Sqrt(floatOffset.y / (groundPoint.distance)) + Vector3.up * gravitationalForce);
            
        }
        else
        {
            rb.AddForce(Vector3.up * gravitationalForce + Vector3.up * floatForce * Mathf.Sqrt(floatOffset.y / (groundPoint.distance)));
        }
       // normal.x += Mathf.Sin(time += Time.deltaTime )/rockSpeedX;
       // normal.z += Mathf.Sin((time += Time.deltaTime)/2)/rockSpeedZ;
       transform.up = Vector3.Lerp(transform.up, normal, rotationSensitivity);
    }
}
