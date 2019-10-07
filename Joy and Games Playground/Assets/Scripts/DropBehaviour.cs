using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropBehaviour : MonoBehaviour
{
    [SerializeField] float aliveTime = 10;
    [SerializeField] float fadeSpeed = 0.01f;

    bool dying = false;

    float originalScale;
    // Start is called before the first frame update
    void Start()
    {
        originalScale = transform.localScale.x;


        GetComponent<Rigidbody>().drag = Random.Range(0f, 0.25f);
    }

    // Update is called once per frame
    void Update()
    {
        if(aliveTime > 0)
        {

            aliveTime -= Time.deltaTime;
        }

        if (aliveTime <= 0 && dying == false)
        {
            dying = true;
            StartCoroutine("Die");
        }
    }

    IEnumerator Die()
    {
        for (float i = 1; i > 0; i -= fadeSpeed)
        {
            transform.localScale = Vector3.one *  i * originalScale;
            yield return null;

        }
        GameObject.Destroy(gameObject);
        yield return null;
        
    }
}
