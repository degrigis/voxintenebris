using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathBuilder : MonoBehaviour
{

    public GameObject spawneeTile;
    public bool stopSpawning;
    public float spawnTime;
    public float spawnDelay;
    // Start is called before the first frame update
    void Start()
    {
       InvokeRepeating("SpawnTile", spawnTime, spawnDelay);
    }

    public void SpawnTile() {
        Instantiate(spawneeTile, transform.position, transform.rotation);
        if (stopSpawning){
            // Stop path
        }
    }

}
