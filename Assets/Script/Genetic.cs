using System.Collections.Generic;
using UnityEngine;

public class Genetic : MonoBehaviour
{
    [Header("Population Settings")]
    public int populationSize;
    public int weightCount;
    public int nbGeneration;
    public float mutationRate;
    public float time;
    [Range(0f, 1f)] public float conservationRate;

    private int keepWeight;
    private int newWeight;
    private float timeStamp;
    private int generation = 0;
    private List<GameObject> population;
    private List<float[]> weightsList;

    // NOTE : Remplacer GameObject par la classe de notre objet

    private void Start()
    {

        // Variables Initialisation
        weightsList = new List<float[]>(populationSize);
        timeStamp = Time.time;
        keepWeight = Mathf.FloorToInt(weightCount * conservationRate);
        newWeight = weightCount - keepWeight;

        // Initialisation logic
        FirstGenerationRandomWeights();
        InitPopulation();

        Time.timeScale = 10;
    }

    private void Update()
    {
        if (Time.time > timeStamp + time)
            SwapGeneration();
    }

    private void FirstGenerationRandomWeights() {
        for (int i = 0; i < populationSize; i++) {
            float[] weights = new float[weightCount];

            for (int j = 0; j < weightCount; j++)
                weights[j] = Random.Range(-1f, 1f); // TODO Mettre les bons poids 

            weightsList.Add(weights);
        }
    }

    private void InitPopulation() {

        population = new List<GameObject>();
        for (int i = 0; i < populationSize; i++)
            population.Add(new GameObject()); // TODO weights[i]

    }


    // TODO Comment les noms sont à chier faudra que j'en mette des meilleurs
    private void SwapGeneration() {

        // Early exit (faudra surement mettre une fonction qui gère la fin)
        if (generation >= nbGeneration)
            return;

        EndGeneration();
        Reproduce();
        NewGeneration();
    }
    private void EndGeneration() {

        // TODO Fitness est pas encore implémentée

        // Compute all fitness
        for(int i = 0;i < populationSize;i++)
            continue;
            //population[i].ComputeFitness(); 

        // Sort by fitness
        //population.Sort((a, b) => b.GetFitness().CompareTo(a.GetFitness()));

    }

    private void Reproduce() {

        // Keep only the best
        weightsList.RemoveRange(keepWeight, newWeight);

        // Compute new weight based on the best individuals
        for (int i = 0; i < newWeight; i++)
            weightsList.Add(CrossBreeding());
    }

    private void NewGeneration() {

        // Create the new pop
        InitPopulation();

        // Update the var
        generation += 1;
        timeStamp = Time.time;
    }

    private float[] CrossBreeding() {
        return weightsList[0];
    }
}
