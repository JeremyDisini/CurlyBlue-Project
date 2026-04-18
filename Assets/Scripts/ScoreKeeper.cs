using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class ScoreKeeper : MonoBehaviour
{
    [SerializeField]
    Animator anim;

    [SerializeField]
    TextMeshPro scoreboard;

    int score = 0;
    void OnTriggerEnter(Collider col)
    {
        score++;
        scoreboard.text = score.ToString();

        anim.Play("OnScore", 0, 0);
    }
}
