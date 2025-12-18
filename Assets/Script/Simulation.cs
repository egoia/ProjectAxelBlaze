
using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Simulation : MonoBehaviour
{
    public GameObject player;
    public GameObject terrain;
    public float duration = 30f;
    public GameObject ballPrefab;
    public List<Goal> goals;
    GameObject ballObject;
    [HideInInspector] public int goalsScored = 0;
    [HideInInspector] public int ballTouched = 0;

    NeuralNetwork playerBrain;
    int neuralInputSize = 6;
    public int neuralHiddenSize = 20;    
    int neuralOutputSize = 2;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void OnEnable()
    {
        SpawnBall(); 
        foreach(var goal in goals)
        {
            goal.OnScore += Score;
        }

        playerBrain = new NeuralNetwork(neuralInputSize,neuralHiddenSize,neuralOutputSize);
        player.GetComponent<PlayerMovement>().brain = playerBrain;
        player.GetComponent<PlayerMovement>().neuralInput = GetNeuralInputs();
    }

    public void InitWithWeights(float[] weights)
    {
        playerBrain = new NeuralNetwork(weights, neuralInputSize,neuralHiddenSize,neuralOutputSize);
        player.GetComponent<PlayerMovement>().brain = playerBrain;
        player.GetComponent<PlayerMovement>().neuralInput = GetNeuralInputs();
    }

    void Update()
    {
        player.GetComponent<PlayerMovement>().neuralInput = GetNeuralInputs();
    }

    public float GetFitness() 
    {
        return goalsScored*1000f + ballTouched*50f; // + 0.5f*-Vector3.Distance(ballObject.transform.position, player.transform.position);
    }

    public float[] GetWeights()
    {
        return playerBrain.Flatten();
    }
    
    public int GetWeightCount() 
    {
        return (neuralInputSize + neuralOutputSize) * neuralHiddenSize;
    }

    public void EndSimulation()
    {
        player.GetComponent<PlayerMovement>().isPlaying = false;
    }

    public void Score()
    {
        goalsScored++;
        Destroy(ballObject);
        SpawnBall();
    }

    public void TouchBall()
    {
        ballTouched++;
    }

    float[] GetNeuralInputs()
    {//TODO input en 2d
        float[] inputs = new float[neuralInputSize];
        Vector3 inputBall = ballObject.transform.position - player.transform.position;
        inputs[0] = inputBall.x;
        inputs[1] = inputBall.y;
        inputs[2] = inputBall.z;
        Vector3 inputGoal = goals[0].transform.position - ballObject.transform.position;
        inputs[3] = inputGoal.x;
        inputs[4] = inputGoal.y;
        inputs[5] = inputGoal.z;
        

        return inputs;
    }

    void SpawnBall()
    {
        float spawnWidth = terrain.transform.localScale.x/4;
        float x = terrain.transform.position.x + Random.Range(-spawnWidth,spawnWidth);

        float spawnHeight = terrain.transform.localScale.z/4;
        float z = terrain.transform.position.z + Random.Range(-spawnHeight,spawnHeight);

        ballObject = Instantiate(ballPrefab, transform);
        ballObject.GetComponent<TouchCounter>().OnTouch += TouchBall;

        float y = ballObject.transform.position.y;

        ballObject.GetComponent<Rigidbody>().position = new Vector3(x,y,z);
    }

    
}
