using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class CarnivourWorldSim : MonoBehaviour
{
    float hunger;
    float thirst;
    int maxPackSize;
    int huntingPackSize;
    int strengthLvl;
    float timeBetweenTracks;
    Vector3 velocity = Vector3.zero;
    public float speed;
    public Transform targetPosition;
    private Seeker seeker;
    private CharacterController controller;
    public Path path;
    public float nextWaypointDistance = 3;
    private int currentWaypoint = 0;
    public bool reachedEndOfPath;
    public float maxSteer;
    public float steeringSharpness;


    Vector3 savedTarget;



    public void Start () 
    {
        hunger = 100;
        thirst = 100;
        seeker = GetComponent<Seeker>();
        controller = GetComponent<CharacterController>();

        // Start a new path to the targetPosition, call the the OnPathComplete function
        // when the path has been calculated (which may take a few frames depending on the complexity)
        
    }

    public void OnPathComplete (Path p) {
        Debug.Log("A path was calculated. Did it fail with an error? " + p.error);

        if (!p.error) {
            path = p;
            // Reset the waypoint counter so that we start to move towards the first point in the path
            currentWaypoint = 0;
        }
    }

    public void Update () {

        if(targetPosition.position != savedTarget){
            path = null;
            savedTarget = targetPosition.position;
            seeker.StartPath(transform.position, savedTarget, OnPathComplete);
        }

        if (path == null) {
            return;
        }  

        reachedEndOfPath = false;
        float distanceToWaypoint;
        while (true) {
            distanceToWaypoint = Vector3.Distance(transform.position, path.vectorPath[currentWaypoint]);
            if (distanceToWaypoint < nextWaypointDistance) {
                if (currentWaypoint + 1 < path.vectorPath.Count) {
                    currentWaypoint++;
                } else {
                    reachedEndOfPath = true;
                    break;
                }
            } 
            else 
            {
                break;
            }
        }

        var speedFactor = reachedEndOfPath ? Mathf.Sqrt(distanceToWaypoint/nextWaypointDistance) : 1f;

        Vector3 dir = (path.vectorPath[currentWaypoint] - transform.position).normalized;
        Vector3 velocity = dir * speed * speedFactor;
        controller.SimpleMove(velocity);

    }

    Vector3 Seek(Vector3 Target)
    {
        Vector3 desiredVelocity = (Target - transform.position).normalized;
        Vector3 steering = Vector3.ClampMagnitude(desiredVelocity - velocity, maxSteer) * Time.deltaTime * steeringSharpness;
        Vector3 _velocity = (velocity + steering).normalized;
        return _velocity;
    }

}
