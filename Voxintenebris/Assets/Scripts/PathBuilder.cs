using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathBuilder : MonoBehaviour
{

    public GameObject Tile;
    public float spawnDelay;
    public int TileThreshold = 4;
    public bool stopSpawning;

    private float spawnTimer = 0;
    private int number_tiles_spawned = 0;

    private GameObject floor;

    private System.Random rand = new System.Random ();  

    private List<Vector3> possibleDirections = new List<Vector3> {
        new Vector3(1,0,0),
        new Vector3(-1,0,0),
        new Vector3(0,0,1),
        new Vector3(0,0,-1)
    };

    private enum Directions {
        RIGHT = 0,
        LEFT = 1,
        FORWARD = 2,
        BACKWARD = 3,

    }

    // void Start() {
    //     floor = 
    // }

    void Update() {
        if (number_tiles_spawned < TileThreshold && spawnTimer > spawnDelay && !stopSpawning)
        {
           SpawnTile(new List<Vector3>(possibleDirections));
           spawnTimer = 0;
        }
        else {
            spawnTimer += Time.deltaTime;
        }
    }

    public void SpawnTile(List<Vector3> this_turn_possible_direction) {

        if(this_turn_possible_direction.Count == 0){
            Debug.Log("Something went wrong: this_turn_possible_direction is empty!");
            return;
        }       

        Vector3 random_move = this_turn_possible_direction[rand.Next(this_turn_possible_direction.Count)];
        // Vector3 newPosition = transform.position + (random_move * transform.localScale.x);

        // Ray out_of_bound_ray = new Ray(newPosition, Vector3.down);
        // RaycastHit hit_out_of_bound;

        // Debug.DrawLine(transform.position, transform.position + Vector3.left * 20, Color.red);

        if (!is_out_of_bound(transform.position, random_move)) {
            Debug.Log("Out of bound!");
            this_turn_possible_direction.Remove(random_move);
            SpawnTile(this_turn_possible_direction);
        }
        else {
            transform.position = transform.position + (random_move * transform.localScale.x);
            GameObject spawned_tile = Instantiate(Tile, transform.position, transform.rotation);
            number_tiles_spawned += 1;
        }

    }
 
    private bool is_out_of_bound(Vector3 current_position, Vector3 move) {
        Vector3 newPosition = current_position + (move * transform.localScale.x);

        Ray out_of_bound_ray = new Ray(newPosition, Vector3.down);
        RaycastHit hit_out_of_bound;

        return Physics.Raycast(out_of_bound_ray, out hit_out_of_bound, 1);
    }

}
