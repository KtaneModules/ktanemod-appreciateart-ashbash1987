using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class ArtFrame : MonoBehaviour
{
    public Color[] BaseColors;
    public Color ColorVariance;

    private void Awake()
    {
        MeshRenderer renderer = GetComponent<MeshRenderer>();
        MaterialPropertyBlock block = new MaterialPropertyBlock();
        block.SetColor("_Color", GenerateColor());

        renderer.SetPropertyBlock(block);
    }

    private Color GenerateColor()
    {
        Color color = BaseColors.RandomPick();
        color.r += Random.Range(-ColorVariance.r, ColorVariance.r);
        color.g += Random.Range(-ColorVariance.g, ColorVariance.g);
        color.b += Random.Range(-ColorVariance.b, ColorVariance.b);

        return color;
    }
}
