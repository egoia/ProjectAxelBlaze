using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class Simulation : MonoBehaviour
{
    public GameObject player;
    public GameObject terrain;
    GameObject _ballObject;
    public GameObject ballPrefab;
    
    //Fitness Parameters
    public List<Goal> goals;
    [HideInInspector] public int goalsScored;
    [HideInInspector] public int ballTouched;
    private List<float> _ballDistances;
    private float _travelledDistance;
    private Vector3 _playerLastPosition;
    public float registerDataTimer=1f;
    private float _timer;

    //Brain Parameters
    NeuralNetwork _playerBrain;
    readonly int _neuralInputSize = 4;
    public int neuralHiddenSize = 20;    
    readonly int _neuralOutputSize = 2;
    
    
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void OnEnable()
    {
        _playerLastPosition = player.transform.position;
        _ballDistances = new List<float>();
        SpawnBall(); 
        foreach(var goal in goals)
        {
            goal.OnScore += Score;
        }

        _playerBrain = new NeuralNetwork(_neuralInputSize,neuralHiddenSize,_neuralOutputSize);
        player.GetComponent<PlayerMovement>().brain = _playerBrain;
        player.GetComponent<PlayerMovement>().neuralInput = GetNeuralInputs();
    }

    void Update()
    {
        _timer += Time.deltaTime;
        if (_timer > registerDataTimer)
        {
            _timer = 0;
            _ballDistances.Add(Vector3.Distance(_ballObject.transform.position, player.transform.position));
            _travelledDistance +=(Vector3.Distance(_playerLastPosition, player.transform.position));
            _playerLastPosition = player.transform.position;
        }

        player.GetComponent<PlayerMovement>().neuralInput = GetNeuralInputs();
    }
    
    public void InitWithWeights(float[] weights)
    {
        _playerBrain = new NeuralNetwork(weights, _neuralInputSize,neuralHiddenSize,_neuralOutputSize);
        player.GetComponent<PlayerMovement>().brain = _playerBrain;
        player.GetComponent<PlayerMovement>().neuralInput = GetNeuralInputs();
    }


    public float GetFitness() 
    {
        float avgBallDistance = _ballDistances.Average();
        print(avgBallDistance);
        print(_travelledDistance);
        return goalsScored * 1000f + ballTouched * 50f - 10f * avgBallDistance + 0.1f*_travelledDistance; // + 0.5f*-Vector3.Distance(ballObject.transform.position, player.transform.position);
    }

    public float[] GetWeights()
    {
        return _playerBrain.Flatten();
    }
    
    public int GetWeightCount() 
    {
        return (_neuralInputSize + _neuralOutputSize) * neuralHiddenSize;
    }

    public void EndSimulation()
    {
        player.GetComponent<PlayerMovement>().isPlaying = false;
    }

    private void Score()
    {
        goalsScored++;
        Destroy(_ballObject);
        SpawnBall();
    }

    private void TouchBall()
    {
        ballTouched++;
    }

    private float[] GetNeuralInputs()
    {
        float[] inputs = new float[_neuralInputSize];
        Vector3 inputBall = _ballObject.transform.position - player.transform.position;
        inputs[0] = inputBall.x;
        inputs[1] = inputBall.y;
        Vector3 inputGoal = goals[0].transform.position - _ballObject.transform.position;
        inputs[2] = inputGoal.x;
        inputs[3] = inputGoal.y;
        

        return inputs;
    }

    private void SpawnBall()
    {
        float spawnWidth = terrain.transform.localScale.x/4;
        float x = terrain.transform.position.x + Random.Range(-spawnWidth,spawnWidth);

        float spawnHeight = terrain.transform.localScale.z/4;
        float z = terrain.transform.position.z + Random.Range(-spawnHeight,spawnHeight);

        _ballObject = Instantiate(ballPrefab, transform);
        _ballObject.GetComponent<TouchCounter>().OnTouch += TouchBall;

        float y = _ballObject.transform.position.y;

        _ballObject.GetComponent<Rigidbody>().position = new Vector3(x,y,z);
    }

    
}
