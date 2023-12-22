using UnityEngine;

public class Scroller : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private Material _background;
    void Update() => _background.mainTextureOffset += new Vector2(0, speed * Time.deltaTime);
}
