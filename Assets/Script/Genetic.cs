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
    [SerializeField] private int generation = 1;
    private List<GameObject> population;
    private List<float[]> weightsList;
    private bool isGenerating;

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
        InitPopulation();
    }

    private void Update()
    {
        if (Time.time > timeStamp + time && !isGenerating)
            SwapGeneration();
    }

    private void InitPopulation() {
        // TODO Pour l'instant ça va créer toutes les simu en ligne on verra plus tard pour faire une grille
        // TODO La réutilisation est infame + le code est pas propre mais on fait avec

        // Create new pop
        for (int i = 0; i < populationSize; i++)
            population.Add(Instantiate(simulationPrefab, new Vector3(0, 0, 30 * i), Quaternion.identity));// NOTE : Default constructor will auto-create random weight

        weightCount = population[0].GetComponent<Simulation>().GetWeightCount();
    }

    private void RecreatePopulation()
    {
        // Reset List
        for (int i = 0; i < population.Count; i++)
            Destroy(population[i]);
        population.Clear();

        // Create new pop
        for (int i = 0; i < populationSize; i++) {
            GameObject simul = Instantiate(simulationPrefab, new Vector3(0, 0, 30 * i), Quaternion.identity);
            simul.GetComponent<Simulation>().InitWithWeights(weightsList[i]);
            population.Add(simul); // TODO Là on est censé fournir des poids, ou juste après

        }
        

    }

    // TODO Comment les noms sont à chier faudra que j'en mette des meilleurs
    private void SwapGeneration() {

        isGenerating = true;

        EndGeneration();

        // Early exit (faudra surement mettre une fonction qui gère la fin)
        if (generation >= nbGeneration)
            return;

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


        for (int i=0; i < populationSize - keepCount; i++) {
            float[] weigthParent1 = population[Random.Range(0, keepCount)].GetComponent<Simulation>().GetWeights();
            float[] weigthParent2 = population[Random.Range(0, keepCount)].GetComponent<Simulation>().GetWeights();
            float[] weights = new float[weightCount];

            for (int k = 0; k < weightCount; k++)
                weights[k] = (weigthParent1[k] + weigthParent2[k]) / 2; // TODO Faudrait rajouter de la mutation (mutationRate)

            weightsList.Add(weights);

        }

    }

}
