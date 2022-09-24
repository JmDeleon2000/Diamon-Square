using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class DiamonSquare : MonoBehaviour
{
    [SerializeField] MeshFilter mf;
    [SerializeField] MeshRenderer renderer;
    [Range(1, 10)]
    [SerializeField] int n = 1;
    [Range(0.0f, 1)]
    [SerializeField] float min_sample;
    [Range(0.0f, 1)]
    [SerializeField] float max_sample;

    MaterialPropertyBlock _mpb;
    MaterialPropertyBlock MPB
    {
        get 
        {
            if (_mpb == null)
                _mpb = new MaterialPropertyBlock();
            return _mpb; 
        }
    }

    readonly int text_index = Shader.PropertyToID("_HeightMap");

    private void OnValidate()
    {
        int size = (int)Mathf.Pow(2, n) + 1;
        float[][] arr = new float[size][];
        for (int i = 0; i < size; i++)
            arr[i] = new float[size];

        arr[0][0] = Random.Range(min_sample, max_sample);
        arr[0][size - 1] = Random.Range(min_sample, max_sample);
        arr[size - 1][size - 1] = Random.Range(min_sample, max_sample);
        arr[size - 1][0] = Random.Range(min_sample, max_sample);

        ds(ref arr, size);

        byte[] map = new byte[size*size];
        for (int i = 0; i < size; i++)
            for (int j = 0; j < size; j++)
                    map[i * size + j] = (byte)(arr[i][j] * 255);
        Texture2D texture = new Texture2D(size, size, TextureFormat.R8, false);
        texture.LoadRawTextureData(map);
        texture.Apply();
        MPB.SetTexture(text_index, texture);
        renderer.SetPropertyBlock(MPB);
    }

    void ds(ref float[][] arr, int size) 
    {
        int half = size / 2;
        if (half <= 1)
            return;

        for (int y = half; y < arr[0].Length; y += size)
            for (int x = half; x < arr.Length; x += size)
                squareStep(ref arr, x%arr.Length, y % arr[0].Length, half);


        int col = 0;
        for (int x = 0; x < arr.Length; x += half)
        {
            col++;
            //If this is an odd column.
            if (col % 2 == 1)
                for (int y = half; y < arr[0].Length; y += size)
                    diamondStep(ref arr, x % arr.Length, y % arr[0].Length, half);
            else
                for (int y = 0; y < arr[0].Length; y += size)
                    diamondStep(ref arr, x % arr.Length, y % arr[0].Length, half);
        }
        ds(ref arr, size / 2);
    }

    void squareStep(ref float[][] arr, int x, int y, int reach) 
    {
        int count = 0;
        float avg = 0;
        if (x - reach >= 0 && y - reach >= 0)
        {
            avg += arr[x - reach][y - reach];
            count++;
        }
        if (x + reach < arr.Length && y - reach >= 0)
        {
            avg += arr[x + reach][y - reach];
            count++;
        }
        if (x + reach < arr.Length && y + reach < arr[0].Length)
        {
            avg += arr[x + reach][y + reach];
            count++;
        }
        avg += Random.Range(0.0f, reach);
        avg/= count;
        arr[x][y] = avg;
    }


    void diamondStep(ref float[][] arr, int x, int y, int reach)
    {
        int count = 0;
        float avg = 0.0f;
        if (x - reach >= 0)
        {
            avg += arr[x - reach][y];
            count++;
        }
        if (x + reach < arr.Length)
        {
            avg += arr[x + reach][y];
            count++;
        }
        if (y - reach >= 0)
        {
            avg += arr[x][y - reach];
            count++;
        }
        if (y + reach < arr[0].Length)
        {
            avg += arr[x][y + reach];
            count++;
        }
        avg += Random.Range(0.0f, reach);
        avg /= count;
        arr[x][y] = avg;
    }
}
