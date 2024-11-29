using UnityEngine;
using System.IO;

public class SaveRenderTextureToPNG : MonoBehaviour
{
    public RenderTexture renderTexture; // Assign your RenderTexture here
    public KeyCode saveKey = KeyCode.S; // Key to press for saving
    public string filePath = "SavedImage.png"; // File name and path for the saved image

    void Update()
    {
        if (Input.GetKeyDown(saveKey))
        {
            SaveRenderTexture();
        }
    }

    void SaveRenderTexture()
    {
        // Ensure the RenderTexture is active
        RenderTexture currentRT = RenderTexture.active;
        RenderTexture.active = renderTexture;

        // Create a Texture2D to copy the RenderTexture
        Texture2D texture = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGB24, false);
        texture.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        texture.Apply();

        // Save the Texture2D as a PNG file
        byte[] bytes = texture.EncodeToPNG();
        File.WriteAllBytes(filePath, bytes);

        // Cleanup
        RenderTexture.active = currentRT;
        Destroy(texture);

        Debug.Log($"RenderTexture saved to {filePath}");
    }
}
