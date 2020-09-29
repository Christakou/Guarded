using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Guard : MonoBehaviour
{

    public static event System.Action OnGuardHasSpottedPlayer;
    public static event System.Action OnPlayerEnterGuardFOV;
    public static event System.Action OnPlayerLeftGuardFOV;
    public Transform pathHolder;
    public float speed = 5.0f;
    public float waitTime = 2.0f;
    public int rotationSpeed = 2;
    public Light spotlight;
    public float viewDistance;
    public float viewAngle;
    public bool canSeePlayer = false;
    public bool sawPlayer = false;

    public GameObject player;
    public float playerDetectionTime = 10f;
    public float playerVisibleTimer = 1f;
    Color originalSpotlightColour;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        viewAngle = spotlight.spotAngle;
        originalSpotlightColour = spotlight.color;
        Vector3[] waypoints = new Vector3[pathHolder.childCount];
        for (int i = 0; i < waypoints.Length; i++)
        {
            waypoints[i] = pathHolder.GetChild(i).position;
            waypoints[i] = new Vector3(waypoints[i].x, transform.position.y, waypoints[i].z);
        }
        StartCoroutine(Move(waypoints));

    }


    private void OnDrawGizmos()
    {
        Vector3 startPosition = pathHolder.GetChild(0).position; 
        Vector3 previousPosition = startPosition;
        foreach (Transform waypoint in pathHolder)
        {
            Gizmos.DrawLine(previousPosition, waypoint.position);
            Gizmos.DrawSphere(waypoint.position, 1.0f);
            previousPosition = waypoint.position;
        }
        Gizmos.DrawLine(previousPosition, startPosition);
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position+Quaternion.Euler(0, -viewAngle/2, 0) * transform.forward *viewDistance);
        Gizmos.DrawLine(transform.position, transform.position + Quaternion.Euler(0, viewAngle / 2, 0) * transform.forward * viewDistance);
    }
    // Start is called before the first frame update




    bool CanSeePlayer()
    {
        Vector3 direction = (player.transform.position - transform.position).normalized;
        if ((player.transform.position - transform.position).magnitude < viewDistance)
        {
            if (Vector3.Angle(transform.forward, direction) < viewAngle / 2)
            {
                Ray ray = new Ray(transform.position, direction);
                RaycastHit hitInfo;
                if (Physics.Raycast(ray, out hitInfo, viewDistance))
                {
                    if (hitInfo.transform.gameObject.tag == "Player")
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    // Update is called once per frame
    void Update()
    {
        if (CanSeePlayer())
        {
            if (sawPlayer == false)
            {
                OnPlayerEnterGuardFOV();
                sawPlayer = true;
            }
            playerVisibleTimer += Time.deltaTime;
        }
        else
        {
            if (sawPlayer == true)
            {
                OnPlayerLeftGuardFOV();
                sawPlayer = false;
            }
            playerVisibleTimer -= Time.deltaTime;
        }
        playerVisibleTimer = Mathf.Clamp(playerVisibleTimer, 0, playerDetectionTime);
        spotlight.color = Color.Lerp(originalSpotlightColour, Color.red, playerVisibleTimer/playerDetectionTime);

        if (playerVisibleTimer >= playerDetectionTime)
        {
            if (OnGuardHasSpottedPlayer != null)
            {
                OnGuardHasSpottedPlayer();
            }

        }
    }


    IEnumerator Move(Vector3[] targetPositions)
    {
        while (true)
        {
            foreach (Vector3 targetPosition in targetPositions)
            {
                Vector3 direction = (targetPosition - transform.position).normalized;
                Quaternion lookRotation = Quaternion.LookRotation(direction);
                while (transform.rotation != lookRotation)
                {
                    transform.rotation = Quaternion.RotateTowards(transform.rotation, lookRotation, rotationSpeed * Time.deltaTime);
                    yield return null;
                }

                while (transform.position !=  targetPosition)
                {
                
                 yield return transform.position = Vector3.MoveTowards(transform.position,targetPosition,speed*Time.deltaTime);
        
                }
                
                
                yield return new WaitForSeconds(waitTime);
            }
        }
    }
}
