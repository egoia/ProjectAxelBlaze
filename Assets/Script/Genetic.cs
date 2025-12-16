using System.Collections.Generic;
using UnityEngine;

public class Genetic : MonoBehaviour
{ 
    [Header("Population Settings")]
    [Min(2)]public int populationSize;
    [Min(1)]public int nbGeneration;
    public float mutationRate;
    public float time;
    [Range(0f, 1f)] public float conservationRate;


    private int weightCount;
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
        //weightCount = Simulation.GetWeightCount();


        // Initialisation logic
        InitPopulation();
    }

    private void Update()
    {
        if (Time.time > timeStamp + time && !isGenerating)
            SwapGeneration();
    }

    private void InitPopulation() {
        // Create new pop
        for (int i = 0; i < populationSize; i++)
            population.Add(new GameObject()); // NOTE : Default constructor will auto-create random weight
    }

    private void RecreatePopulation()
    {
        // Reset List
        for (int i = 0; i < population.Count; i++)
            Destroy(population[i]);
        population.Clear();

        // Create new pop
        for (int i = 0; i < populationSize; i++)
            population.Add(new GameObject()); // TODO weightsList[i]
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
        CrossBreeding();
    }

    private void NewGeneration() {

        // Create the new pop
        RecreatePopulation();

        // Update the var
        generation += 1;
        timeStamp = Time.time;
        isGenerating = false;
    }

    private void CrossBreeding() {

        // CrossBreed 2 by 2 every kept agent
        for (int i = 0; i < keepCount; i++) {
            float[] weights = new float[weightCount];

            for (int j = 1; j < keepCount; j++)
                for (int k = 0; k < weightCount; k++)
                    continue;
            //weights[k] = (weightsList[i].GetWeights() + weightsList[j].GetWeights()) / 2; // TODO Faudrait rajouter de la mutation (mutationRate)

            weightsList.Add(weights);
        }

    }


}
