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
        Directions random_move_direction = (Directions)rand.Next(this_turn_possible_direction.Count);
        Vector3 random_move = this_turn_possible_direction[(int)random_move_direction];

        if (!is_within_bound(transform.position, random_move) || is_colliding(transform.position, random_move_direction) ) {
            this_turn_possible_direction.Remove(random_move);
            SpawnTile(this_turn_possible_direction);
        }
        else {
            transform.position = transform.position + (random_move * transform.localScale.x);
            GameObject spawned_tile = Instantiate(Tile, transform.position, transform.rotation);
            number_tiles_spawned += 1;
        }

    }
 
    private bool is_within_bound(Vector3 current_position, Vector3 move) {
        Vector3 newPosition = current_position + (move * transform.localScale.x);

        Ray out_of_bound_ray = new Ray(newPosition, Vector3.down);
        RaycastHit hit_out_of_bound;

        bool is_within_bound = Physics.Raycast(out_of_bound_ray, out hit_out_of_bound, 1);

        if (!is_within_bound) {
            Debug.Log("Out of bound!");
        }

        return is_within_bound;
    }

    // private bool is_within_bound(Vector3 current_position, Vector3 move) {
    //     Vector3 newPosition = current_position + (move * transform.localScale.x);

    //     Ray out_of_bound_ray = new Ray(newPosition, Vector3.down);
    //     RaycastHit hit_out_of_bound;

    //     return Physics.Raycast(out_of_bound_ray, out hit_out_of_bound, 1);
    // }

    private bool is_colliding(Vector3 position, Directions direction) {
        RaycastHit collision_hit;
        Ray collision_ray;

        switch (direction)
        {
            case Directions.RIGHT:
                collision_ray = new Ray(position, Vector3.right);
                break;
            case Directions.LEFT:
                collision_ray = new Ray(position, Vector3.left);
                break;
            case Directions.FORWARD:
                collision_ray = new Ray(position, Vector3.forward);
                break;
            case Directions.BACKWARD:
                collision_ray = new Ray(position, Vector3.back);
                break;
            default:
                Debug.Log("Something went wrong. Weird direction detected!");
                return true;
        }

        bool is_collision_detected = Physics.Raycast(collision_ray, out collision_hit, 2);

        if (is_collision_detected){
            Debug.Log("Collision detected!");
        }

        return is_collision_detected;

    }

}
