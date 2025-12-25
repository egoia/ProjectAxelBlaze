using TMPro;
using UnityEngine;

public class GenerationUI : MonoBehaviour
{
    public static GenerationUI Instance;
    public TextMeshProUGUI textUI;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void UpdateText(int genValue){ textUI.text = genValue.ToString(); }
}
