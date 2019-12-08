using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterBehaviour : MonoBehaviour
{

    Transform target;
    Transform player;

    Rigidbody body;

    // Start is called before the first frame update
    void Start()
    {
        body = GetComponent<Rigidbody>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        InvokeRepeating("CalculateNearestTarget", 1, 2);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 toTarget = (target.position - transform.position).normalized;
        toTarget -= body.velocity;
        body.AddRelativeForce(toTarget);

    }
    void CalculateNearestTarget()
    {
        CreatureBehaviour[] creatures = FindObjectsOfType<CreatureBehaviour>();

        float nearestDist = 10000f;
        Transform nearestTarget = null;
        foreach(CreatureBehaviour creature in creatures)
        {
            float dist = Vector3.Distance(transform.position, creature.transform.position);
            if (dist < nearestDist)
            {
                nearestTarget = creature.transform;
                nearestDist = dist;
            }
        }
        float distToPlayer = Vector2.Distance(transform.position, player.position);
        if(distToPlayer < nearestDist)
        {
            nearestTarget = player;
        }

        target = nearestTarget;

    }
}
