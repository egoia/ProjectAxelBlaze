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

    public GameObject simulationPrefab;


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
            population.Add(Instantiate(simulationPrefab));// NOTE : Default constructor will auto-create random weight

        weightCount = population[0].GetComponent<Simulation>().GetWeightCount();
    }

    private void RecreatePopulation()
    {
        // Reset List
        for (int i = 0; i < population.Count; i++)
            Destroy(population[i]);
        population.Clear();

        // Create new pop
        for (int i = 0; i < populationSize; i++) 
            population.Add(Instantiate(simulationPrefab));

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

        for (int i = 0; i < population.Count; i++)
            population[i].GetComponent<Simulation>().EndSimulation();

        // Sort by fitness
        population.Sort((a, b) => b.GetComponent<Simulation>().GetFitness().CompareTo(a.GetComponent<Simulation>().GetFitness()));

    }

    private void Reproduce() {

        // Keep only the best 
        weightsList.Clear();
        for (int i = 0; i < keepCount; i++)
            weightsList.Add(population[i].GetComponent<Simulation>().GetWeights());

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
                    // DEBUG J'ai mis les index à 0 (azparce que le "bug" de player movement crée un effet de bord
                    // qui crée un oob ici (flatten peut pas être appelé, donc on récupère jamais la liste de poids)
                    weights[k] = (weightsList[i][0] + weightsList[j][0]) / 2; // TODO Faudrait rajouter de la mutation (mutationRate)

            weightsList.Add(weights);
        }

    }


}
