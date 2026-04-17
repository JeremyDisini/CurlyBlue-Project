using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class ScoreKeeper : MonoBehaviour
{
    [SerializeField]
    TextMeshPro scoreboard;

    int score = 0;
    void OnTriggerEnter(Collider col)
    {
        score++;
        scoreboard.text = score.ToString();
    }
}
