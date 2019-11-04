using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TapeSegmentCreator : MonoBehaviour
{


    [SerializeField] GameObject tapeSegmentPrefab;
    [SerializeField] float segmentSize = 1f;


    // Start is called before the first frame update
    void Start()
    {


    }


    public GameObject CreateNewSegment(GameObject lastSegment, Transform peelTransform)
    {
        if (lastSegment == null)
        {
            return Instantiate(tapeSegmentPrefab, peelTransform.position, peelTransform.rotation);
        }

        Vector3 pos = lastSegment.transform.position + lastSegment.transform.forward * segmentSize;
        GameObject newSegment = Instantiate(tapeSegmentPrefab, pos, Quaternion.LookRotation(transform.position - pos, peelTransform.up));
        newSegment.transform.localScale *= segmentSize;

        // RESET CONNECTIONS FOR LAST ITEM
        TapeSegmentController lastSegmentController = lastSegment.GetComponent<TapeSegmentController>();
        HingeJoint[] hinges = lastSegment.GetComponents<HingeJoint>();
        Vector3 hingePointA = lastSegmentController.hingeA;
        Vector3 hingePointB = lastSegmentController.hingeB;
        hinges[0].anchor = hingePointA;
        hinges[1].anchor = hingePointB;
        hinges[0].connectedBody = newSegment.GetComponent<Rigidbody>();
        hinges[1].connectedBody = newSegment.GetComponent<Rigidbody>();


        // SET UP NEW ITEM
        TapeSegmentController newSegmentController = newSegment.GetComponent<TapeSegmentController>();
        HingeJoint[]  newhinges = newSegment.GetComponents<HingeJoint>();
        Vector3 newhingePointA = newSegmentController.hingeA;
        Vector3 newhingePointB = newSegmentController.hingeB;
        newhinges[0].anchor = newhingePointA;
        newhinges[1].anchor = newhingePointB;
        newhinges[0].connectedBody = peelTransform.GetComponent<Rigidbody>();
        newhinges[1].connectedBody = peelTransform.GetComponent<Rigidbody>();

        return newSegment;

    }

}
