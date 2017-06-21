using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Patrol : MonoBehaviour {

    public Transform[] points;
    public Transform[] runPoints;
    private int destPoint = 0;
    private UnityEngine.AI.NavMeshAgent agent;
    private bool spotted = false;
    private bool running = false;

    public GameObject player;
    public int viewAngle;
    public int viewRange;
    public int escapeRange;

    private movePlayer movePl;

    private Vector3 rayDirection;
    private Ray spotRay;
    private RaycastHit hit;
    private Ray ray;
    
    void Start()
    {
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        // Disabling auto-braking allows for continuous movement
        // between points (ie, the agent doesn't slow down as it
        // approaches a destination point).
        agent.autoBraking = true;

        GotoNextPoint();
    }

    void GotoNextPoint()
    {
        // Returns if no points have been set up
        if (points.Length == 0)
            return;

        // Set the agent to go to the currently selected destination.
        agent.destination = points[destPoint].position;

        // Choose the next point in the array as the destination,
        // cycling to the start if necessary.
        destPoint = (destPoint + 1) % points.Length;

        GameObject obj =  GameObject.FindGameObjectWithTag("Player");
        movePl = obj.GetComponent<movePlayer>();
    }

    void Update()
    {

        if ( !movePl.gameOver ) {
            rayDirection = player.transform.position - transform.position;

            Debug.DrawRay(transform.position, Vector3.forward * viewRange, Color.green);
            Debug.DrawRay(transform.position, Vector3.back * viewRange, Color.cyan);
            Debug.DrawRay(transform.position, Vector3.left * viewRange, Color.blue);
            Debug.DrawRay(transform.position, Vector3.right * viewRange, Color.grey);

            Debug.DrawRay(transform.position, rayDirection * viewRange, Color.red);

            if (movePl.invulnerabilityActive)
            {
                running = true;
            }
            //run
            if (movePl.invulnerabilityActive)
            {
                run();
            } else
            //patrol
            if (!spotted)
            {
                if (agent.remainingDistance < 0.5f)
                    GotoNextPoint();
                //if player is in sight, follow him
                if (spot())
                {
                    spotted = true;
                    agent.SetDestination(player.transform.position);
                }
            } else
            //chase
            {
                chase();
            }
        }else
        {
            agent.Stop();
        }
    }
   
    void run()
    {
        if( transform.position.x < player.transform.position.x )
        {
            if( transform.position.z < player.transform.position.z )
            {
                for( int i = 0; i < runPoints.Length ; i++ )
                {
                    if( runPoints[i].position.x < player.transform.position.x && runPoints[i].position.z < player.transform.position.z)
                    {
                        agent.SetDestination(runPoints[i].position);
                        return;
                    }
                }
            } else
            {
                for (int i = 0; i < runPoints.Length; i++)
                {
                    if (runPoints[i].position.x < player.transform.position.x && runPoints[i].position.z >= player.transform.position.z)
                    {
                        agent.SetDestination(runPoints[i].position);
                        return;
                    }
                }
            }
        } else
        {
            if( transform.position.z < player.transform.position.z )
            {
                for (int i = 0; i < runPoints.Length; i++)
                {
                    if (runPoints[i].position.x >= player.transform.position.x && runPoints[i].position.z < player.transform.position.z)
                    {
                        agent.SetDestination(runPoints[i].position);
                        return;
                    }
                }
            } else
            {
                for (int i = 0; i < runPoints.Length; i++)
                {
                    if (runPoints[i].position.x >= player.transform.position.x && runPoints[i].position.z >= player.transform.position.z)
                    {
                        agent.SetDestination(runPoints[i].position);
                        return;
                    }
                }
            }
        }
    }

    void chase()
    {
        Debug.DrawRay(transform.position, rayDirection * viewRange, Color.blue);
        agent.SetDestination(player.transform.position);

        if (agent.remainingDistance < 0.5f || agent.remainingDistance > escapeRange)
        {
            spotted = false;
        }
    }

    bool spot()
    {
        //Ray spotRay = new Ray(transform.position, transform.TransformDirection(Vector3.forward));
        spotRay = new Ray(transform.position, rayDirection * viewRange);

        if (Physics.Raycast(spotRay, out hit, viewRange) && Vector3.Angle(rayDirection, transform.forward) < viewAngle)
        {

            if (hit.collider.CompareTag("Player"))
            {
                Debug.Log("PEEK-A-BOO");
                // ghost can see the player!
                return true;
            }
        }
        return false;
    }
}
