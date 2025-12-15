
using UnityEngine;

public class Simulation : MonoBehaviour
{
    public GameObject ballPrefab;
    GameObject ballObject;
    public GameObject terrain;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
       SpawnBall(); 
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    void SpawnBall()
    {
        float spawnWidth = terrain.transform.localScale.x/4;
        float x = Random.Range(-spawnWidth,spawnWidth);

        float spawnHeight = terrain.transform.localScale.z/4;
        float z = Random.Range(-spawnHeight,spawnHeight);

        ballObject = Instantiate(ballPrefab, transform);
        float y = ballObject.transform.position.y;

        ballObject.transform.position = new Vector3(x,y,z);
    }
}
