using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class Art : MonoBehaviour
{
    public Texture2D[] Paintings;

    private MeshRenderer _renderer = null;
    private Material _material = null;

    private void Awake()
    {
        _renderer = GetComponent<MeshRenderer>();
        _material = _renderer.material;

        SetArt(Paintings.RandomPick());
    }

    public void SetArt(Texture texture)
    {
        _material.mainTexture = texture;
    }
}
