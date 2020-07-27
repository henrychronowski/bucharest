using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boat : MonoBehaviour
{
    [SerializeField] GameObject player;
    [SerializeField] int activationDistance;
    [SerializeField] Vector3 rayOffSet;
    [SerializeField] float raycastDistance;

    private BoatMovement boatMovementScript;
    private bool playerOnBoat;

    

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player");
        boatMovementScript = gameObject.GetComponent<BoatMovement>();
        
    }

    // Update is called once per frame
    void Update()
    {
        

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (!playerOnBoat)
            {
                if (Vector3.Distance(player.transform.position, transform.position) > activationDistance)
                {
                    RideBoat();
                }
            }
            else
            {
                ExitBoat();
            }
        }

       

        Debug.DrawRay(transform.TransformPoint(rayOffSet), Vector3.up * -raycastDistance, Color.red, 0.1f);


    }

    public void RideBoat()
    {
        player.SetActive(false);
        boatMovementScript.active = true;
        playerOnBoat = true;
    }

    public void ExitBoat()
    {
        player.SetActive(true);
        SetPlayerLoc();
        boatMovementScript.active = false;
        playerOnBoat = false;
    }


    private void SetPlayerLoc()
    {
        RaycastHit hit;
        LayerMask mask = 2;

        bool hitSomething = Physics.Raycast(transform.TransformPoint(rayOffSet), Vector3.up * -1, out hit, raycastDistance, ~mask, QueryTriggerInteraction.Ignore);


        if (hitSomething)
        {
            Debug.Log(hit.point);
            player.transform.position = hit.point + new Vector3(0, 2, 0);
        }
        else
        {
            Debug.Log("Failed");
        }
        
        

    }
}
