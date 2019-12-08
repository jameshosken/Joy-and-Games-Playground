using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatureHandler : MonoBehaviour
{

    [SerializeField] GameObject[] creatureTemplates;
    [SerializeField] GameObject playerSphereOfInfluence;
    [SerializeField] Transform player;

    [Space()]
    [Header("General Settings")]

    [SerializeField] int maxCreatures = 10;

    [Space()]
    [Header("Flocking Settings")]
    [SerializeField] private float separationMultiplier;
    [SerializeField] private float alignmentMultiplier;
    [SerializeField] private float cohesionMultiplier;
    [SerializeField] private float maxSpeed;
    [SerializeField] private float maxForce;
    [SerializeField] private float desiredSeparation;
    [SerializeField] private float neighbourDistance;
    [SerializeField] float seekStrength = 1f;

    List<CreatureBehaviour> creatures = new List<CreatureBehaviour>();

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < maxCreatures; i++)
        {
            Vector3 randomVec = UnityEngine.Random.insideUnitSphere * 300f;

            GenerateCreature(randomVec);
        }
    }

    void GenerateCreature(Vector3 position)
    {
        int choice = UnityEngine.Random.Range(0, creatureTemplates.Length);

        GameObject crt = Instantiate(creatureTemplates[choice], position, UnityEngine.Random.rotation);
        crt.transform.parent = transform;
        CreatureBehaviour behaviour = crt.GetComponent<CreatureBehaviour>();
        behaviour.handler = GetComponent<CreatureHandler>();

        behaviour.ApplyFlockingSettings(
            separationMultiplier,
            alignmentMultiplier,
            cohesionMultiplier,
            maxSpeed,
            maxForce,
            desiredSeparation,
            neighbourDistance,
            seekStrength);
    }

    void FixedUpdate()
    {

        for (int i = 0; i < creatures.Count; i++)
        {
            if(creatures[i] == null)
            {
                creatures.RemoveAt(i);
                continue;
            }
            creatures[i].SeekTarget(player.position);
            creatures[i].Flock(creatures);
        }
    }


    // For testing purposes, make sure values update when editor changes:

    private void OnValidate()
    {

        for (int i = 0; i < creatures.Count; i++)
        {
            creatures[i].ApplyFlockingSettings(
               separationMultiplier,
               alignmentMultiplier,
               cohesionMultiplier,
               maxSpeed,
               maxForce,
               desiredSeparation,
               neighbourDistance,
               seekStrength);
        }
    }

    /// <summary>
    /// ADD/REMOVE Creatures
    /// </summary>
    
    public void AddCreatureToList(CreatureBehaviour creature)
    {
        Debug.Log("Adding creature to list!");
        creatures.Add(creature);
    }

    public void RemoveCreatureFromList(CreatureBehaviour creature)
    {
        Debug.Log("Removing creature from list!");

        creatures.Remove(creature);
    }

}
