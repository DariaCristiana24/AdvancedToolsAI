using System.Collections;
using System.Collections.Generic;
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
    [SerializeField]
    Transform topLeft;  
    [SerializeField]
    Transform bottomRight;
    List<Vector3> assetGrid = new List<Vector3>();

    Transform fruit;
    Transform bomb;


    private void Start()
    {
        fruit = Instantiate(fruitPrefab, new Vector3(0, 1, 0), Quaternion.identity); 

        if (addBomb)
        {
            bomb = Instantiate(bombPrefab, new Vector3(0, 1, 0), Quaternion.identity);
        }

        for (float i = topLeft.position.x; i < bottomRight.position.x+1; i+=1.5f)
        {
            for(float j =topLeft.position.z ; j > bottomRight.position.z; j-=1.5f)
            {
                assetGrid.Add(new Vector3(i, topLeft.position.y, j));
            }
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
        float moveX = actions.ContinuousActions[0];
        float moveZ = actions.ContinuousActions[1];

        transform.position += new Vector3(moveX, 0, moveZ)* Time.deltaTime* speed;

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Fruit")
        {
            SetReward(10f);
            MoveItems();
            floor.material.color = Color.green;
            EndEpisode();
        }
        if (other.tag == "Bomb")
        {
            SetReward(-10f);
            MoveItems(); 
            floor.material.color = Color.red;
            EndEpisode();
        }
        if (other.tag == "Obstacle")
        {
            SetReward(-10f);
            floor.material.color = Color.white;
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
        fruit.position = assetGrid[Random.Range(0,assetGrid.Count)];
        if (bomb != null)
        {
            bomb.position = assetGrid[Random.Range(0, assetGrid.Count)];
            if (fruit.position == bomb.position || fruit.position == bomb.position)
            {
                MoveItems();
            }
        }

    }

}
