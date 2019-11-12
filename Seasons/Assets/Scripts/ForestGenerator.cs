using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForestGenerator : MonoBehaviour
{
    [SerializeField] GameObject treeGeneratorPrefab;
    [SerializeField] int numberOfTrees = 10;
    [SerializeField] float radius = 100f;

    List<GameObject> trees = new List<GameObject>();

    [SerializeField] bool triggerFall = false;

    // Start is called before the first frame update
    void Start()
    {
        for(int i = 0; i < numberOfTrees; i++)
        {
            Vector2 pos2D = UnityEngine.Random.insideUnitCircle * radius;
            Vector3 pos3D = new Vector3(pos2D.x, 0, pos2D.y);

            Ray ray = new Ray(transform.position + pos3D, Vector3.down);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 100f))
            {
                GameObject tree = Instantiate(treeGeneratorPrefab, hit.point, Quaternion.identity, transform);
                trees.Add(tree);
                Debug.Log("Tree");
            }

            
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (triggerFall)
        {
            triggerFall = false;

            StartFall();
        }
    }

    public List<GameObject> GetTrees()
    {
        return trees;
    }

    private void StartFall()
    {
        for (int i = 0; i < trees.Count; i++){
            ParticleSystemRenderer rend = trees[i].GetComponent<ParticleSystemRenderer>();

            rend.material.SetColor("_Colour1", Color.yellow*0.75f);
            rend.material.SetColor("_Colour2", Color.red);
        }
    }
}
