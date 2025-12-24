using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Genetic : MonoBehaviour
{

    #region ===== JSON =====
    [Header("JSON Settings")]
    public string JsonName = "weightDump";
    [Tooltip("The lower the more step, the higher the less step (percentage)")]
    [Range(0f, 1f)] public float generationStepRate;
    [Tooltip("The percentage of agents to dump (ranked from best to worst")]
    [Range(0f, 1f)] public float dumpConservationRate;
    private int dumpConservationCount;
    private int dumpGenerationStep;
    // TODO Add an option to always force dump of first generation

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
    #endregion

    #region ===== POPULATION =====
    [Header("Population Settings")]
    [Min(2)]public int populationSize;
    [Min(1)]public int nbGeneration;
    public float mutationRate;
    public float time;
    [Range(0f, 1f)] public float conservationRate;
    public GameObject simulationPrefab;
    [Min(1)] public int simulationPerRow;

    private int weightCount;
    private int keepCount;
    private float timeStamp;
    [SerializeField] private int generation = 1;
    private List<Simulation> population;
    private List<float[]> weightsList;
    private bool isGenerating;
    #endregion

    // TODO C'est un peu sale d'utiliser une seule weightlist, je devrais différencier les currents et futures
    // J'ai pas d'enformcement dur au niveau du keepCount et conservationRate, ce qui pourrait cause des problèmes mais ça m'étonnerait que ça arrive donc osef

    private void Start()
    {

        // Variables Initialisation
        weightsList = new List<float[]>(populationSize);
        timeStamp = Time.time;
        keepCount = Mathf.Max(2 ,Mathf.FloorToInt(populationSize * conservationRate)); // NOTE : Guaranty we keep at least two agent to be able to cross breed
        population = new List<Simulation>();
        dumpConservationCount = Mathf.RoundToInt(populationSize * dumpConservationRate);
        dumpGenerationStep = Mathf.Max(1, Mathf.RoundToInt(nbGeneration * generationStepRate)); // NOTE : Guaranty it's at least one or we will have division by 0 problem

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
            population.Add(Instantiate(simulationPrefab, new Vector3(60 * (i / simulationPerRow), 0, 30 * (i%simulationPerRow)), Quaternion.identity).GetComponent<Simulation>());

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
            Simulation simul = Instantiate(simulationPrefab, new Vector3(60 * (i / simulationPerRow), 0, 30 * (i%simulationPerRow)), Quaternion.identity).GetComponent<Simulation>();
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

            // TODO Le mieux ça serait d'avoir une fonction à appeler qui ferait tout ça et qui pourrait aussi afficher un UI final

            // Clear the agents one last time
            for (int i = 0; i < population.Count; i++)
                Destroy(population[i].gameObject);
            population.Clear();

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

        // Skip the dump of generation based on the step
        if ((generation) % dumpGenerationStep != 0)
            return;

        // New generation json
        GenerationData genData = new GenerationData();
        genData.generation = generation;

        for (int i = 0;i < dumpConservationCount; i++) {
            // Create the simulation json
            RankData rankData = new RankData();
            rankData.rank = i + 1;
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

            // Loop Initialisation
            float[] weigthParent1 = population[Random.Range(0, keepCount)].GetWeights();
            float[] weigthParent2 = population[Random.Range(0, keepCount)].GetWeights();
            float[] weights = new float[weightCount];
            int cutOffIndex = Random.Range(0, weightCount);

            // Weight from parent 1
            for (int j = 0; j < cutOffIndex; j++)
                weights[j] = weigthParent1[j];

            // Weight from parent 2
            for (int j = cutOffIndex; j < weightCount; j++)
                weights[j] = weigthParent2[j];

            weightsList.Add(weights);

        }

    }

    // NOTE : Works only in editor, won't work in build
    private void DumpJson() {
        // JSONIFY
        string json = JsonUtility.ToJson(jsonDump, true);

        // PATH
        string projectRoot = Directory.GetParent(Application.dataPath).FullName;
        string path = Path.Combine(projectRoot, JsonName + ".json");

        // WRITE
        File.WriteAllText(path, json);
    }

}
