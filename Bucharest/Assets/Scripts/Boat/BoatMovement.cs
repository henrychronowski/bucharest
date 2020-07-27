using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoatMovement : MonoBehaviour
{
    public bool active;

    [Header("Basic Movement Variables")]
    [SerializeField] private float speed;
    [SerializeField] private float acceleration;
    [SerializeField] private float drag;
    [SerializeField] private float rockSpeedX = 8, rockSpeedZ = 8;
    [SerializeField] private float maxSlideSpeed;


    
    [Header("Float/HoverVariables")]
    [SerializeField] private Vector3[] raycastStartingOffsets;
    [SerializeField] private float maxFloatHeight;
    [SerializeField] private float desiredHeight;
    [SerializeField] private Vector3 normal;
    [SerializeField] private PIDController PID;


    private RaycastHit groundPoint;
    private LayerMask mask = 2;
    private Rigidbody rb;
    private float time;
    private bool isGrounded;
    private bool occupied;
    public bool moving = false;
    private Vector3 moveDir;
    private float angle = 0;



    [Header("Debug")]
    [SerializeField] bool debugEnabled;

    private void OnValidate()
    {
        if (debugEnabled)
        {
            Vector3[] raycastStartingPoints = new Vector3[raycastStartingOffsets.Length];

            for (int i = 0; i < raycastStartingOffsets.Length; i++)
            {
                // raycastStartingPoints[i] = transform.TransformPoint(raycastStartingOffsets[i]);
                raycastStartingPoints[i] = raycastStartingOffsets[i] + transform.position;
                //raycastStartingPoints[i] = RotatePointAboutPoint(raycastStartingOffsets[i] + transform.position, transform.position, new Vector3(0,angle,0));


                Debug.DrawRay(raycastStartingPoints[i], Vector3.down * maxFloatHeight, Color.red, 0.1f);
            }
        }

        
    }

    // Start is called before the first frame update
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
        rb.drag = drag;
       
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        

        moveDir = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        if (active)
        {
            if (isGrounded)
            {
                if (moveDir != Vector3.zero)
                {
                    if (rb.velocity.magnitude < speed)
                    {
                        Move(moveDir);
                    }
                    moving = true;
                }
                else
                {
                    moving = false;
                }
            }
        }
        

        Hover();
    }
    private void Update()
    {
        SetNormalAndRotation();
    }

    private void Move(Vector3 moveDirection)
    {
        if ( moveDir.y == 0)
        {
            if(rb.velocity.y < speed)
            {
                rb.AddForce(transform.forward * moveDir.z * speed);
            }
        }
        float xVelocity = Mathf.Clamp(rb.velocity.x, -maxSlideSpeed, maxSlideSpeed);
        rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y, rb.velocity.z);

        

    }

    public Vector3 RotatePointAboutPoint(Vector3 point, Vector3 pivot, Vector3 angles)
    {
        Vector3 dir = point - pivot;
        dir = Quaternion.Euler(angles) * dir;
        point = dir + pivot;
        return point;
    }


    void SetNormalAndRotation()
    {
        Vector3[] raycastStartingPoints = new Vector3[raycastStartingOffsets.Length];

        isGrounded = false;
        normal = Vector3.zero;


        for (int i = 0; i < raycastStartingOffsets.Length; i++)
        {
           
            raycastStartingPoints[i] = raycastStartingOffsets[i] + transform.position;
          
            if (debugEnabled)
            {

                Debug.DrawRay(raycastStartingPoints[i], Vector3.down * maxFloatHeight, Color.red, 0.1f);

            }
        }
        

       
        
        for (int i = 0; i < raycastStartingPoints.Length; i++)
        {
            if (Physics.Raycast(raycastStartingPoints[i] , Vector3.down, out groundPoint, maxFloatHeight, ~mask, QueryTriggerInteraction.Ignore))
            {
                isGrounded = true;                      
            }
        }


        for (int i = 0; i < raycastStartingPoints.Length; i++)
        {
            if (Physics.Raycast(raycastStartingPoints[i], Vector3.down, out groundPoint, Mathf.Infinity, ~mask, QueryTriggerInteraction.Ignore))
            {
                normal += groundPoint.normal;
            }
        }

       //normal = normal / raycastStartingOffsets.Length;

        time += Time.deltaTime;

        if (!moving)
        {
           // normal.x += Mathf.Sin(time/2) / rockSpeedX;
           // normal.z += Mathf.Sin(time / 2) / rockSpeedZ;
        }

        Quaternion normalRot = Quaternion.LookRotation(Vector3.forward, normal);
        Quaternion resultRot = normalRot * Quaternion.AngleAxis(angle, Vector3.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, resultRot, Time.deltaTime);
        

        if (rb.velocity.y > 1 || rb.velocity.y < -1 )
        {
            if (moveDir.x != 0)
            {
                angle += moveDir.x;
                Debug.Log(angle);

            }
        }

    }

    public void Hover()
    {

        Vector3 pos = transform.TransformPoint(raycastStartingOffsets[raycastStartingOffsets.Length - 1]);
        Physics.Raycast(pos, Vector3.down, out groundPoint, Mathf.Infinity, ~mask, QueryTriggerInteraction.Ignore);
        if (isGrounded)
        {
            Debug.Log(normal);
            float force = PID.CalculateForce(desiredHeight, groundPoint.distance);
            rb.AddForce(force * normal.normalized);
        }
        else
        {
            rb.AddForce(9.81f * Vector3.down);
        }
    }
}


