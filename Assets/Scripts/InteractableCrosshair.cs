using UnityEngine;

public class InteractableCrosshair : MonoBehaviour
{
    [SerializeField]
    Animator anim;

    [SerializeField]
    PlayerController player; 

    void Update()
    {
        anim.SetBool("isHovering", player.hoveredObject != null);
    }
}
