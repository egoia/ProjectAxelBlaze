using System.Collections.Generic;
using UnityEngine;

public class Genetic : MonoBehaviour
{
    [Header("Population Settings")]
    [Min(2)]public int populationSize;
    [Min(1)]public int weightCount;
    [Min(1)]public int nbGeneration;
    public float mutationRate;
    public float time;
    [Range(0f, 1f)] public float conservationRate;

    private int keepCount;
    private float timeStamp;
    private int generation = 1;
    private List<GameObject> population;
    private List<float[]> weightsList;
    private bool isGenerating;

    // NOTE : Remplacer GameObject par la classe de notre objet
    // TODO C'est un peu sale d'utiliser une seule weightlist, je devrais différencier les currents et futures
    // J'ai pas d'enformcement dur au niveau du keepCount et conservationRate, ce qui pourrait cause des problèmes mais ça m'étonnerait que ça arrive donc osef

    private void Start()
    {

        // Variables Initialisation
        weightsList = new List<float[]>(populationSize);
        timeStamp = Time.time;
        keepCount = Mathf.Max(2 ,Mathf.FloorToInt(populationSize * conservationRate)); // NOTE : Guaranty we keep at least two agent to be able to cross breed
        population = new List<GameObject>();


        // Initialisation logic
        FirstGenerationRandomWeights();
        InitPopulation();
    }

    private void Update()
    {
        if (Time.time > timeStamp + time && !isGenerating)
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
        // Reset List
        for (int i = 0; i < population.Count; i++)
            Destroy(population[i]);
        population.Clear();

        // Create new pop
        for (int i = 0; i < populationSize; i++)
            population.Add(new GameObject()); // TODO weights[i]

    }


    // TODO Comment les noms sont à chier faudra que j'en mette des meilleurs
    private void SwapGeneration() {

        // Early exit (faudra surement mettre une fonction qui gère la fin)
        if (generation >= nbGeneration)
            return;

        isGenerating = true;

        EndGeneration();
        Reproduce();
        NewGeneration();
    }
    private void EndGeneration() {

        // TODO Fitness est pas encore implémentéea

        // Compute all fitness
        for(int i = 0;i < populationSize;i++)
            continue;
            //population[i].ComputeFitness(); 

        // Sort by fitness
        //population.Sort((a, b) => b.GetFitness().CompareTo(a.GetFitness()));

    }

    private void Reproduce() {

        // Keep only the best 
        weightsList.Clear();
        for (int i = 0; i < keepCount; i++)
            continue;
            //weightsList.Add(population[i].GetWeights());

        // Compute new weight based on the best individuals
        for (int i = 0; i < populationSize - keepCount; i++)
            weightsList.Add(CrossBreeding());
    }

    private void NewGeneration() {

        // Create the new pop
        InitPopulation();

        // Update the var
        generation += 1;
        timeStamp = Time.time;
        isGenerating = false;
    }

    private float[] CrossBreeding() {


        float[] weights1 = weightsList[Random.Range(0, keepCount)];
        float[] weights2 = weightsList[Random.Range(0, keepCount)];

        float[] weights = new float[weightCount];

        for (int i = 0; i < weightCount; i++)
            weights[i] = (weights1[i] + weights2[i]) /2; // TODO Faudrait rajouter de la mutation (mutationRate)

        return weights;
    }
}
