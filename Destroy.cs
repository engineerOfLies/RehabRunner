using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destroy : MonoBehaviour
{
    public BuildingCreator bc;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider col)
    {
        Debug.Log("Touching");
        if (col.gameObject.name == "Clouds" || col.gameObject.name == "Clouds(Clone)") 
        {
            bc.SpawnClouds();
            Destroy(col.gameObject);
            
        }
        else if (col.gameObject.tag == "Building")
        {
            Destroy(col.gameObject);
            bc.numBuildings--;
        }

        
    }
}
