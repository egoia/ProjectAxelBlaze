using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Genetic : MonoBehaviour
{

    //region ===== JSON =====
    [System.Serializable]
    public class GenerationData{
        public int generation;
        public List<RankData> ranks = new();
    }

    [System.Serializable]
    public class RankData {
        public int rank;
        public float[] weights;
    }

    [System.Serializable]
    public class GeneticDump { public List<GenerationData> generations = new(); }

    private GeneticDump jsonDump = new GeneticDump();
    //endregion

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
    private List<Simulation> population;
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
        population = new List<Simulation>();

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
            // NOTE : Default constructor will auto-create random weight
            population.Add(Instantiate(simulationPrefab, new Vector3(0, 0, 30 * i), Quaternion.identity).GetComponent<Simulation>());

        weightCount = population[0].GetWeightCount();
    }

    private void RecreatePopulation()
    {
        // Reset List
        for (int i = 0; i < population.Count; i++)
            Destroy(population[i].gameObject);
        population.Clear();

        // Create new pop
        for (int i = 0; i < populationSize; i++) {
            Simulation simul = Instantiate(simulationPrefab, new Vector3(0, 0, 30 * i), Quaternion.identity).GetComponent<Simulation>();
            simul.InitWithWeights(weightsList[i]);
            population.Add(simul);

        }
    }

    // TODO Comment les noms sont à chier faudra que j'en mette des meilleurs
    private void SwapGeneration() {

        isGenerating = true;

        EndGeneration();

        // Early exit (faudra surement mettre une fonction qui gère la fin)
        if (generation >= nbGeneration) {
            DumpJson();
            return;
        }

        Reproduce();
        NewGeneration();
    }
    private void EndGeneration() {
        // NOTE : On trie avant de créer le JSON pour avoir un JSON rangé

        // Deactivate each simulation
        for (int i = 0; i < population.Count; i++)
            population[i].EndSimulation();

        // Sort by fitness
        population.Sort((a, b) => b.GetFitness().CompareTo(a.GetFitness()));

        // === JSON build ===

        // New generation json
        GenerationData genData = new GenerationData();
        genData.generation = generation;

        for (int i = 0;i < populationSize; i++) {
            // Create the simulation json
            RankData rankData = new RankData();
            rankData.rank = i;
            rankData.weights = population[i].GetWeights();

            genData.ranks.Add(rankData);
        }


        jsonDump.generations.Add(genData);
    }

    private void Reproduce() {

        // Keep only the best 
        weightsList.Clear();
        for (int i = 0; i < keepCount; i++)
            weightsList.Add(population[i].GetWeights());


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
            float[] weigthParent1 = population[Random.Range(0, keepCount)].GetWeights();
            float[] weigthParent2 = population[Random.Range(0, keepCount)].GetWeights();
            float[] weights = new float[weightCount];

            int cutOffIndex = Random.Range(0, weightCount);

            for (int j = 0; j < cutOffIndex; j++)
                weights[j] = weigthParent1[j];

            for (int j = cutOffIndex; j < weightCount; j++)
                weights[j] = weigthParent2[j];

            //for (int k = 0; k < weightCount; k++)
            //   weights[k] = (weigthParent1[k] + weigthParent2[k]) / 2; // TODO Faudrait rajouter de la mutation (mutationRate)

            weightsList.Add(weights);

        }

    }

    private void DumpJson() {
        string json = JsonUtility.ToJson(jsonDump, true);
        string path = Path.Combine(Application.persistentDataPath, "genetic_dump.json");
        File.WriteAllText(path, json);
    }

}
