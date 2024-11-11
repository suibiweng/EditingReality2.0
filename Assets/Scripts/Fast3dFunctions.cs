using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using Anaglyph.DisplayCapture;

public class Fast3dFunctions : MonoBehaviour
{
    public static Texture2D streamingTexture;
    public RenderTexture Mask;

    public DisplayCaptureManager displayCaptureManager;

    void Start() {
        displayCaptureManager= FindAnyObjectByType<DisplayCaptureManager>();





    }

    void Update() {}




    public void StartCapture(){

        displayCaptureManager.StartScreenCapture();


    }

    public void StopCapture(){


        displayCaptureManager.StopScreenCapture();


    }







    // Update the texture to be uploaded
    public static void UpdateTexture(Texture2D texture)
    {
        streamingTexture = texture;
    }

    // Capture and upload the current streaming texture with a custom filename
    public void Capture(string url = "http://yourserver.com/upload", string filename = "default_image.png")
    {
        if (streamingTexture == null)
        {
            Debug.LogError("No texture set for streaming. Use UpdateTexture to set a texture first.");
            return;
        }

        StartCoroutine(UploadPNG(streamingTexture, url, filename));
    }

    // Overloaded Capture function to handle RenderTexture input
    public void UploadMask(RenderTexture renderTexture, string url, string filename, string prompt)
    {
        Texture2D texture2D = ConvertRenderTextureToTexture2D(renderTexture);
        StartCoroutine(UploadPNG(texture2D, url, filename));
        Destroy(texture2D); // Clean up after upload
    }

    // Converts RenderTexture to Texture2D
    private Texture2D ConvertRenderTextureToTexture2D(RenderTexture renderTexture)
    {
        Texture2D texture = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGB24, false);
        RenderTexture.active = renderTexture;
        texture.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        texture.Apply();
        RenderTexture.active = null;
        return texture;
    }

    // Upload the texture as PNG to the specified URL with a custom filename
    private IEnumerator UploadPNG(Texture2D texture, string url, string filename)
    {
        byte[] pngData = texture.EncodeToPNG();
        if (pngData != null)
        {
            UnityWebRequest request = new UnityWebRequest(url, "POST");
            request.uploadHandler = new UploadHandlerRaw(pngData);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "image/png");
            request.SetRequestHeader("Filename", filename); // Custom header for filename

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Upload complete with filename: " + filename);
            }
            else
            {
                Debug.LogError("Error: " + request.error);
            }
        }
    }
}
