using System;
using System.Runtime.InteropServices;
using UnityEngine;

public struct XY
{
    public int x;
    public int y;
}

public class BasicTest : MonoBehaviour
{
    private WebCamTexture _inImage;
    private Texture2D _outImage;
    private Color32[] _pixels;

    [DllImport("OpenCVWrapper", EntryPoint = "Test")]
    private static extern IntPtr Test(IntPtr imagePtr,
        [MarshalAs(UnmanagedType.I4)]int length,
        [MarshalAs(UnmanagedType.I4)]out int resLength);

    // Use this for initialization
    private void Start()
    {
        _pixels = new Color32[800 * 600];
        _outImage = new Texture2D(800, 600);
        var device = WebCamTexture.devices[0];
        _inImage = new WebCamTexture(device.name, 800, 600);
        _inImage.Play();
        GetComponent<Renderer>().material.mainTexture = _outImage;
    }

    // Update is called once per frame
    private void Update()
    {
        _inImage.GetPixels32(_pixels);
        _outImage.SetPixels32(_pixels);
        var haldle = GCHandle.Alloc(_pixels, GCHandleType.Pinned);

        int resLength;
        var resPtr = Test(haldle.AddrOfPinnedObject(), _pixels.Length, out resLength);
        if (resLength <= 0)
        {
            _outImage.Apply();
            return;
        }
        XY[] output;
        MarshalUnmananagedArray2Struct(resPtr, resLength, out output);

        foreach (var xy in output)
        {
            PrintCrossOnTex(_outImage, xy, Color.cyan);
        }
        _outImage.Apply();
    }

    private void PrintCrossOnTex(Texture2D tex, XY xy, Color col)
    {
        tex.SetPixel(xy.x, xy.y, col);
        tex.SetPixel(xy.x + 1, xy.y, col);
        tex.SetPixel(xy.x + 2, xy.y, col);
        tex.SetPixel(xy.x, xy.y + 1, col);
        tex.SetPixel(xy.x, xy.y + 2, col);
        tex.SetPixel(xy.x - 1, xy.y, col);
        tex.SetPixel(xy.x - 2, xy.y, col);
        tex.SetPixel(xy.x, xy.y - 1, col);
        tex.SetPixel(xy.x, xy.y - 2, col);
    }

    private void MarshalUnmananagedArray2Struct<T>(IntPtr unmanagedArray, int length, out T[] mangagedArray)
    {
        var size = Marshal.SizeOf(typeof(T));
        mangagedArray = new T[length];

        for (var i = 0; i < mangagedArray.Length; i++)
        {
            mangagedArray[i] = (T)Marshal.PtrToStructure(new IntPtr(unmanagedArray.ToInt64() + i * size), typeof(T));
        }
    }
}
