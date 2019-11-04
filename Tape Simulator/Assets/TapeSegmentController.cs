using UnityEngine;

public class TapeSegmentController : MonoBehaviour
{
    public Vector3 hingeA;
    public Vector3 hingeB;
    [SerializeField] float maxVel = 5f;

    Rigidbody body;
    // Start is called before the first frame update
    void Start()
    {
        Physics.IgnoreLayerCollision(gameObject.layer, LayerMask.NameToLayer("Player"));
        body = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(body.velocity.magnitude > maxVel)
        {
            body.velocity = body.velocity.normalized * maxVel;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        //GetComponent<Rigidbody>().freezeRotation = true;
        if (collision.gameObject.layer != gameObject.layer)
        {
            GetComponent<Rigidbody>().isKinematic = true;
            //transform.parent = collision.gameObject.transform;
        }

    }
}
