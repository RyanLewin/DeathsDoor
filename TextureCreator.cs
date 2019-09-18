///Credit to https://catlikecoding.com/unity/tutorials/noise/ for the perlin noise 
///script.

using UnityEngine;
using System.Collections.Generic;

public class TextureCreator : MonoBehaviour {

    public static TextureCreator GetMap { get; private set; }
    [Range(2, 512)]
	public int resolution = 256;

	public float frequency = 1f;

	[Range(1, 8)]
	public int octaves = 1;

	[Range(1f, 4f)]
	public float lacunarity = 2f;

	[Range(0f, 1f)]
	public float persistence = 0.5f;

	[Range(1, 3)]
	public int dimensions = 3;

	public NoiseMethodType type;

	public Gradient coloring;

	public Texture2D texture { get; private set; }

    public List<Tile> mapTiles = new List<Tile>();

    private void OnEnable () {
		if (texture == null)
        {
            GetMap = this;
            texture = new Texture2D(resolution, resolution, TextureFormat.RGB24, true);
			texture.name = "Procedural Texture";
			texture.wrapMode = TextureWrapMode.Clamp;
			texture.filterMode = FilterMode.Trilinear;
			texture.anisoLevel = 9;
			GetComponent<MeshRenderer>().material.mainTexture = texture;
		}
		FillTexture();
	}

	private void FixedUpdate () {
		if (transform.hasChanged) {
			transform.hasChanged = false;
			FillTexture();
		}
	}
	
	public void FillTexture ()
    {
		if (texture.width != resolution) {
			texture.Resize(resolution, resolution);
		}
		
		Vector3 point00 = transform.TransformPoint(new Vector3(-0.5f,-0.5f));
		Vector3 point10 = transform.TransformPoint(new Vector3( 0.5f,-0.5f));
		Vector3 point01 = transform.TransformPoint(new Vector3(-0.5f, 0.5f));
		Vector3 point11 = transform.TransformPoint(new Vector3( 0.5f, 0.5f));

		NoiseMethod method = Noise.methods[(int)type][dimensions - 1];
		float stepSize = 1f / resolution;
		for (int y = 0; y < resolution; y++)
        {
			Vector3 point0 = Vector3.Lerp(point00, point01, (y + 0.5f) * stepSize);
			Vector3 point1 = Vector3.Lerp(point10, point11, (y + 0.5f) * stepSize);
			for (int x = 0; x < resolution; x++)
            {
				Vector3 point = Vector3.Lerp(point0, point1, (x + 0.5f) * stepSize);
				float sample = Noise.Sum(method, point, frequency, octaves, lacunarity, persistence);
				if (type != NoiseMethodType.Value)
                {
					sample = sample * 0.5f + 0.5f;
				}
				texture.SetPixel(x, y, coloring.Evaluate(sample));
			}
		}
		texture.Apply();
	}

    public Color GetTexture (Vector2 pos)
    {
        Vector3 point00 = transform.TransformPoint(new Vector3(-0.5f, -0.5f));
        Vector3 point10 = transform.TransformPoint(new Vector3(0.5f, -0.5f));
        Vector3 point01 = transform.TransformPoint(new Vector3(-0.5f, 0.5f));
        Vector3 point11 = transform.TransformPoint(new Vector3(0.5f, 0.5f));

        NoiseMethod method = Noise.methods[(int)type][dimensions - 1];
        float stepSize = 1f / resolution;

        Vector3 point0 = Vector3.Lerp(point00, point01, (pos.y + 0.5f) * stepSize);
        Vector3 point1 = Vector3.Lerp(point10, point11, (pos.y + 0.5f) * stepSize);
        Vector3 point = Vector3.Lerp(point0, point1, (pos.x + 0.5f) * stepSize);
        float sample = Noise.Sum(method, point, frequency, octaves, lacunarity, persistence);
        if (type != NoiseMethodType.Value)
        {
            sample = sample * 0.5f + 0.5f;
        }
        return coloring.Evaluate(sample);
    }
}