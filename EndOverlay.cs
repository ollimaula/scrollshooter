using UnityEngine;

public class EndOverlay : MonoBehaviour
{
    [SerializeField] private SpriteRenderer rend;
    public void SetAlpha(float alpha)
    {
        Color spriteColor = rend.color;
        spriteColor.a = alpha;
        rend.color = spriteColor;
    }
}

