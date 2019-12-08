using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatureHitDetector : MonoBehaviour
{
    CreatureBehaviour behaviour;
    // Start is called before the first frame update
    void Start()
    {
        behaviour = GetComponentInParent<CreatureBehaviour>();
    }
    public void Hit()
    {
        Debug.Log("HIT");
        behaviour.Die();
    }
}
