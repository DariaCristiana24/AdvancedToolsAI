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
    bool moreBombs;
    [SerializeField]
    bool obstacles;
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

    Transform bombSecond = null;

    private void Start()
    {
        fruit = Instantiate(fruitPrefab, new Vector3(0, 1, 0), Quaternion.identity); 
        bomb = Instantiate(bombPrefab, new Vector3(0, 1, 0), Quaternion.identity);
        if (moreBombs)
        {
            bombSecond = Instantiate(bombPrefab, new Vector3(0, 1, 0), Quaternion.identity);
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
        oldDist = Vector3.Distance(transform.position, fruit.position); 
        MoveItems();
    }
    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.localPosition);  
        sensor.AddObservation(fruit.localPosition);
        sensor.AddObservation(bomb.localPosition); 
        sensor.AddObservation(oldDist);

        if (moreBombs)
        {
            sensor.AddObservation(bombSecond.localPosition);
        }

    }
    public override void OnActionReceived(ActionBuffers actions)
    {
        float moveX = actions.ContinuousActions[0];
        float moveZ = actions.ContinuousActions[1];

        transform.position += new Vector3(moveX, 0, moveZ)* Time.deltaTime* speed;

    }

    float oldDist;
    private void Update()
    {
        if(Vector3.Distance(transform.position, fruit.position) < oldDist)
        {
            SetReward(0.1f);
            
        }
        else
        {
            SetReward(-0.1f);
        }
        oldDist = Vector3.Distance(transform.position, fruit.position);
        /*
        
        if(GetCumulativeReward() > 2)
        {
            floor.material.color = Color.green;
            EndEpisode();
        }
        if (GetCumulativeReward() < -2)
        {
            floor.material.color = Color.red;
            EndEpisode();
        }*/

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
        if (other.tag == "Reward")
        {
            SetReward(2f);
            MoveItems();
            floor.material.color = Color.yellow;
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
        bomb.position = assetGrid[Random.Range(0, assetGrid.Count)];
        if (bombSecond != null)
        {
            bombSecond.position = assetGrid[Random.Range(0, assetGrid.Count)];
        }
        if(fruit.position == bomb.position || fruit.position == bombSecond.position || bombSecond.position == bomb.position)
        {
            MoveItems();
        }
    }

}
