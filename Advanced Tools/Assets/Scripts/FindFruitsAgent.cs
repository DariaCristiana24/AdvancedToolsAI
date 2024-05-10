using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

public class FindFruitsAgent : Agent
{
    [SerializeField]
    bool addBomb;

    [SerializeField]
    float speed = 5;
    [SerializeField]
    Renderer floor;
    [SerializeField]
    Transform fruitPrefab;
    [SerializeField]
    Transform bombPrefab;



    Transform fruit;
    Transform bomb;

    [SerializeField]
    Transform[] possiblePos;


    private void Start()
    {
        fruit = Instantiate(fruitPrefab, new Vector3(0, 1, 0), Quaternion.identity);


        if (addBomb)
        {
            bomb = Instantiate(bombPrefab, new Vector3(0, 1, 0), Quaternion.identity);
        }


        MoveItems();
    }
    public override void OnEpisodeBegin()
    {
        transform.localPosition = Vector3.zero; 
        MoveItems();
    }
    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.localPosition);  
        sensor.AddObservation(fruit.localPosition);

        if (addBomb)
        {
            sensor.AddObservation(bomb.localPosition);
        }

    }
    public override void OnActionReceived(ActionBuffers actions)
    {
      //  float moveX = actions.ContinuousActions[0];
       // float moveZ = actions.ContinuousActions[1];

        //transform.position += new Vector3(moveX, 0, moveZ)* Time.deltaTime* speed;
       
        int dir =actions.DiscreteActions[0];
        Debug.Log(dir);
        Quaternion quaternion = Quaternion.identity;
        quaternion.eulerAngles = new Vector3(0, 90 * dir, 0);
        transform.rotation = quaternion;
        transform.position += transform.forward * Time.deltaTime * speed;

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Fruit")
        {
            SetReward(1f);
            floor.material.color = Color.green;
            EndEpisode();
        }
        if (other.tag == "Bomb")
        {
            SetReward(-1f);
            floor.material.color = Color.red;
            EndEpisode();
        }
        if (other.tag == "Obstacle")
        {
            SetReward(-1f);
           // floor.material.color = Color.white;
            floor.material.color = Color.red;
            EndEpisode();
        }

    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> act = actionsOut.ContinuousActions;
        act[0] = Input.GetAxisRaw("Horizontal") * speed;
        act[1] = Input.GetAxisRaw("Vertical") * speed;
    }

    private void MoveItems()
    {
        fruit.position = possiblePos[Random.Range(0, possiblePos.Count())].position;

        

        if (bomb != null)
        {
            bomb.position = possiblePos[Random.Range(0, possiblePos.Count())].position;
            if (fruit.position == bomb.position)
            {
                MoveItems();
            }
        }

    }

}
