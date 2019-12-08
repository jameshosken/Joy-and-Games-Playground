using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatureBehaviour : MonoBehaviour
{
    bool isAlive = true;
    public bool isFlocking = false;
    public CreatureHandler handler;

    [SerializeField] private float separationMultiplier;
    [SerializeField] private float alignmentMultiplier;
    [SerializeField] private float cohesionMultiplier;
    [SerializeField] private float maxSpeed;
    [SerializeField] private float maxForce;
    [SerializeField] private float desiredSeparation;
    [SerializeField] private float neighbourDistance;
    float seekStrength;
    private Rigidbody body;

    Renderer[] renderers;

    // Start is called before the first frame update
    private void Start()
    {
        body = GetComponent<Rigidbody>();
        renderers = GetComponentsInChildren<Renderer>();
    }

    public void ApplyFlockingSettings(
        float separationMultiplier,
        float alignmentMultiplier,
        float cohesionMultiplier,
        float maxSpeed,
        float maxForce,
        float desiredSeparation,
        float neighbourDistance,
        float seekStrength
        )
    {
        this.separationMultiplier = separationMultiplier;
        this.alignmentMultiplier = alignmentMultiplier;
        this.cohesionMultiplier = cohesionMultiplier;
        this.maxSpeed = maxSpeed;
        this.maxForce = maxForce;
        this.desiredSeparation = desiredSeparation;
        this.seekStrength = seekStrength;
    }

    private void FixedUpdate()
    {
        if (!isAlive)
        {
            return;
        }

        body.velocity = Limit(body.velocity, maxSpeed);

        Ray ray = new Ray(transform.position, transform.forward);

        RaycastHit hit;
        if(Physics.Raycast(ray, out hit, desiredSeparation/2))
        {
            AvoidTarget(hit.point);
        }

        body.AddRelativeForce(Vector3.forward * 0.01f);

        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(body.velocity), 0.02f);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + (transform.forward * desiredSeparation));
    }

    /// <summary>
    /// DEATH SEQUENCE
    /// </summary>
    public void Die()
    {
        StartCoroutine(DeathSequence());
        isAlive = false;
        body.freezeRotation = false;
        body.useGravity = true;
        
    }

    IEnumerator DeathSequence()
    {
        
        Color col = renderers[0].materials[0].GetColor("_EmissionColor");
        Color albedo = renderers[0].materials[0].GetColor("_BaseColor");

        float deathSpeed = 0.005f;
        for (float i = 1; i >= 0; i -= deathSpeed)
        {
            foreach (Renderer renderer in renderers)
            {
                Material mat = renderer.materials[0];
                mat.SetColor("_EmissionColor", col * i);
                mat.SetColor("_BaseColor", Color.Lerp(albedo, Color.clear, 1 - i));
            }
            yield return null;
        }

   
        yield return new WaitForSeconds(1);
        handler.RemoveCreatureFromList(GetComponent<CreatureBehaviour>());
        GameObject.Destroy(gameObject);

       

    }

    /// <summary>
    /// FLOCKING
    /// </summary>

    public void Flock(List<CreatureBehaviour> creatures)
    {
        Vector3 sep = Separation(creatures);   // Separation
        Vector3 ali = Alignment(creatures);      // Alignment
        Vector3 coh = Cohesion(creatures);   // Cohesion
                                         // Arbitrarily weight these forces
        sep *= (separationMultiplier);
        ali *= (alignmentMultiplier);
        coh *= (cohesionMultiplier);

        // Add the force vectors to acceleration
        body.AddForce(sep);
        body.AddForce(ali);
        body.AddForce(coh);
    }


    public void SeekTarget(Vector3 target)
    {
        if (Vector3.Distance(transform.position, target) > desiredSeparation)
        {
            Vector3 seek = Seek(target);
            seek *= seekStrength;
            body.AddForce(seek);
        }
        else
        {
            AvoidTarget(target);
        }

    }

    public void AvoidTarget(Vector3 target)
    {

        Vector3 avoid = Avoid(target);
        avoid *= 0.1f;
        body.AddForce(avoid);

    }

    private Vector3 Avoid(Vector3 target)
    {
        Vector3 desired = transform.position - target;

        desired.Normalize();
        desired *= maxSpeed;

        // Steering = Desired minus Velocity
        Vector3 steer = desired - body.velocity;

        steer = Limit(steer, maxForce);

        return steer;
    }

    private Vector3 Seek(Vector3 target)
    {
        Vector3 desired = target - transform.position;

        desired.Normalize();
        desired *= maxSpeed;

        // Steering = Desired minus Velocity
        Vector3 steer = desired - body.velocity;

        steer = Limit(steer, maxForce);

        return steer;
    }

    private Vector3 Separation(List<CreatureBehaviour> creatures)
    {

        Vector3 steer = new Vector3(0, 0, 0);
        int count = 0;
        // For every boid in the system, check if it's too close
        for (int i = 0; i < creatures.Count; i++)
        {
            if (creatures[i] == this)
            {
                continue;
            }
            CreatureBehaviour other = creatures[i];
            if (!other.isAlive)
            {
                continue;
            }
            float d = Vector3.Distance(transform.position, other.transform.position);
            // If the distance is greater than 0 and less than an arbitrary amount (0 when you are yourself)
            if ((d > 0) && (d < desiredSeparation))
            {
                // Calculate vector pointing away from neighbor
                Vector3 diff = transform.position - other.transform.position;
                diff.Normalize();
                diff /= d;        // Weight by distance
                steer += diff;
                count++;            // Keep track of how many
            }
        }

        if (count > 0)
        {
            steer /= (float)count;
        }

        // As long as the vector is greater than 0
        if (steer.magnitude > 0)
        {
            // Implement Reynolds: Steering = Desired - Velocity
            steer.Normalize();
            steer *= maxSpeed;
            steer -= body.velocity;
            steer = Limit(steer, maxForce);
        }
        return steer;
    }

    private Vector3 Limit(Vector3 vector, float maxMagnitude)
    {

        if (vector.magnitude > maxMagnitude)
        {
            vector = vector.normalized * maxMagnitude;
        }
        return vector;
    }

    private Vector3 Alignment(List<CreatureBehaviour> creatures)
    {
        Vector3 sum = Vector3.zero;
        int count = 0;

        for (int i = 0; i < creatures.Count; i++)
        {
            if (creatures[i] == this)
            {
                continue;
            }
            CreatureBehaviour other = creatures[i];
            if (!other.isAlive)
            {
                continue;
            }
            float d = Vector3.Distance(transform.position, other.transform.position);

            if ((d > 0) && (d < neighbourDistance))
            {
                sum += other.GetComponent<Rigidbody>().velocity;
                count++;
            }
        }


        if (count > 0)
        {
            sum /= (float)count;

            sum.Normalize();
            sum *= maxSpeed;
            Vector3 steer = sum - body.velocity;
            steer = Limit(steer, maxForce);
            return steer;
        }
        else
        {
            return Vector3.zero;
        }
    }

    // Cohesion
    private Vector3 Cohesion(List<CreatureBehaviour> creatures)
    {
        Vector3 sum = Vector3.zero;   // Start with empty vector to accumulate all positions
        int count = 0;
        for (int i = 0; i < creatures.Count; i++)
        {
            if (creatures[i] == this)
            {
                continue;
            }

            CreatureBehaviour other = creatures[i];
            if (!other.isAlive)
            {
                continue;
            }
            float d = Vector3.Distance(transform.position, other.transform.position);
            if ((d > 0) && (d < neighbourDistance))
            {
                sum += other.transform.position; // Add position
                count++;
            }
        }
        if (count > 0)
        {
            sum /= count;
            return Seek(sum);  // Steer towards the position
        }
        else
        {
            return Vector3.zero;
        }
    }

    /// <summary>
    /// TRIG EVENTS
    /// </summary>

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.name == "SphereOfInfluence")
        {
            isFlocking = true;
            //Add self to active creatures
            if (!handler)
            {
                handler = GetComponentInParent<CreatureHandler>();
            }
            handler.AddCreatureToList(GetComponent<CreatureBehaviour>());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.name == "SphereOfInfluence")
        {
            isFlocking = false;
            //Remove self from active creatures
            handler.RemoveCreatureFromList(GetComponent<CreatureBehaviour>());
        }
    }
}
