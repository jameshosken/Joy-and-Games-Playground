using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlipCam : MonoBehaviour
{
    [SerializeField] float duration;

    Quaternion currentRot;
    Quaternion endRot;

    Camera cam;


    [SerializeField] Color StartBG;
    [SerializeField] Color EndBG;

    bool flipped = false;
    Vector3 camOffset;

    int count = 1;
        // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
        currentRot = cam.transform.rotation;
        endRot = currentRot;
        camOffset = cam.transform.localPosition;
        StartBG = cam.backgroundColor;

        RenderSettings.fogColor = cam.backgroundColor;


    }

    // Update is called once per frame
    void FixedUpdate()
    {


    }

    IEnumerator FlipAnimation()
    {
        Color goalColour;
        if(cam.backgroundColor == StartBG)
        {
            goalColour = EndBG;
        }
        else
        {
            goalColour = StartBG;
        }

        for (float i = 0; i < duration; i+=Time.deltaTime)
        {
            float step = Mathf.SmoothStep(0f, 1f, i / duration);

            cam.transform.localRotation = Quaternion.Slerp(cam.transform.localRotation, endRot, step);
            cam.transform.localPosition = Vector3.Lerp(cam.transform.localPosition, camOffset, step);
            cam.backgroundColor = Color.Lerp(cam.backgroundColor, goalColour, step);
            RenderSettings.fogColor = cam.backgroundColor;
            yield return null;
        }
        count += 1;
        yield return null;
    }


    public void Flip()
    {
        flipped = true;
        
        endRot = Quaternion.Euler(Vector3.forward * (-180 * count ) );
        camOffset = new Vector3(cam.transform.localPosition.x, cam.transform.localPosition.y * -1, cam.transform.localPosition.z);

        StartCoroutine(FlipAnimation());

        
    }


}
