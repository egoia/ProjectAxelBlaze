
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
    [HideInInspector] int goalsScored = 0;

    NeuralNetwork playerBrain;
    public int neuralInputSize = 3;
    public int neuralHiddenSize = 10;    
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

    void Update()
    {
        player.GetComponent<PlayerMovement>().neuralInput = GetNeuralInputs();
    }

    public float GetFitness() 
    {
        return -Vector3.Distance(ballObject.transform.position, player.transform.position);
    }

    public float[] GetWeights()
    {
        return playerBrain.Flatten();
    }
    
    public int GetWeightCount() 
    {
        return neuralInputSize*neuralHiddenSize*neuralOutputSize;
    }

    public void EndSimulation()
    {
        player.GetComponent<PlayerMovement>().isPlaying = false;
    }

    public void Score()
    {
        goalsScored++;
    }

    float[] GetNeuralInputs()
    {
        float[] inputs = new float[neuralInputSize];
        Vector3 input = ballObject.transform.position - player.transform.position;
        inputs[0] = input.x;
        inputs[1] = input.y;
        inputs[2] = input.z;
        return inputs;
    }

    void SpawnBall()
    {
        float spawnWidth = terrain.transform.localScale.x/4;
        float x = terrain.transform.position.x + Random.Range(-spawnWidth,spawnWidth);

        float spawnHeight = terrain.transform.localScale.z/4;
        float z = terrain.transform.position.z + Random.Range(-spawnHeight,spawnHeight);

        ballObject = Instantiate(ballPrefab, transform);
        float y = ballObject.transform.position.y;

        ballObject.GetComponent<Rigidbody>().position = new Vector3(x,y,z);
    }

    
}
