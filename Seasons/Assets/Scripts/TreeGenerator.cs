using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeGenerator : MonoBehaviour
{

    [SerializeField] GameObject branchPrefab;
    [SerializeField] int maxBranches = 3;
    [SerializeField] int maxBranchSteps = 5;
    [SerializeField] float lengthMultiplier = 0.5f;
    [SerializeField] float previousBranchDirectionInfluence = 0.5f;
    [SerializeField] float previousBranchDirectionInfluencePersistence = 1.1f;
    [SerializeField] float chanceToBranch = 0.66f;
    [SerializeField] Vector2 scaleRandomRange;

    //[SerializeField] Gradient leafColours;
    
    ParticleSystem leaves;
    ParticleSystem.EmitParams leafParams;


    float scale;
    // Start is called before the first frame update
    void Start()
    {
        leaves = GetComponent<ParticleSystem>();
        scale = UnityEngine.Random.Range(scaleRandomRange.x, scaleRandomRange.y);
        CreateBranch(transform.position, Vector3.up, 1f, previousBranchDirectionInfluence, 0);


    }
    void CreateBranch(Vector3 position, Vector3 direction, float length, float influence, int n)
    {
        if(n > maxBranchSteps)
        {
            CreateLeaves(position);
            return;
        }

        n++;

        //Create new random rotation:
        Vector3 branchRotation = (length == 1) ? Vector3.up : UnityEngine.Random.onUnitSphere;

        //Influence random rotation by previous branch direction:
        branchRotation = (branchRotation + direction.normalized * influence).normalized * length*scale;   

        //Set new branch endpoint:
        Vector3 nextBranch = position + branchRotation;

        CreateBranchPair(position,  nextBranch);

        for(int i = 0; i < maxBranches; i++)
        {
            if (UnityEngine.Random.Range(0f, 1f) < chanceToBranch)
            {
                CreateBranch(nextBranch, branchRotation, length * lengthMultiplier, influence * previousBranchDirectionInfluencePersistence, n);
            }
        }
    }

    private void CreateLeaves(Vector3 position)
    {
        leafParams.position = position - transform.position;
        leafParams.startSize = UnityEngine.Random.Range(.2f, 1f);
        //leafParams.startColor = leafColours.Evaluate(UnityEngine.Random.Range(.5f, 1f));
        leaves.Emit(leafParams, 1);
    }

    void CreateBranchPair(Vector3 start, Vector3 end)
    {
        for (int i = 0; i < 2; i++)
        { 
            GameObject branch = Instantiate(branchPrefab, transform.position, Quaternion.identity, transform);
            branch.transform.Rotate(Vector3.up * 90 * i);
            LineRenderer branchRenderer = branch.GetComponent<LineRenderer>();
            branchRenderer.SetPosition(0, start);
            branchRenderer.SetPosition(1, end);
        }
    }

    void DestroyAllBranches()
    {
        for(int i = transform.childCount-1; i >=0; i--)
        {
            GameObject.DestroyImmediate(transform.GetChild(i).gameObject);
        }
    }



   
}
