using TMPro;
using UnityEngine;

public class ScoreDisplayer : MonoBehaviour
{
    public TextMeshProUGUI scoreText;
    public Simulation sim;

    // Update is called once per frame
    void Update()
    {
        scoreText.text = sim.goalsScored.ToString();      
    }
}
