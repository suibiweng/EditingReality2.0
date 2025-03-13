using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using Anaglyph.DisplayCapture;
using System;
using UnityEngine.UI;
using UnityEngine.Android;

public class Fast3dFunctions : MonoBehaviour
{

    public Camera MaskCamera;

    public Camera CaptureCamera;
    private int originalCullingMask;
    private bool isRenderingNothing = false; //

    public RawImage DepthTextureIMG;


    public static Texture2D streamingTexture;
    public RenderTexture Mask,Depth;

    public DisplayCaptureManager displayCaptureManager;

    void Start() {
        displayCaptureManager= FindAnyObjectByType<DisplayCaptureManager>();


         RequestMicrophonePermission();

        StartCapture();
       

         InitCameraMask();



    }




    void RequestMicrophonePermission()
    {
        if (!Permission.HasUserAuthorizedPermission(Permission.Microphone))
        {
            Permission.RequestUserPermission(Permission.Microphone);
        }
    }

    



    public void  CaptureDepth(string url,string filename,Vector2 objectPosition,string urlid){


        var texture2D = ConvertRenderTextureToTexture2D(Depth);
            StartCoroutine(UploadPNG(texture2D, url, filename, "", false, 0, objectPosition, false, "Depth", urlid));








    }



    Texture2D ConvertToTexture2D(Texture texture)
    {
        // Ensure the input texture is readable (or handle RenderTexture)
        RenderTexture renderTexture = RenderTexture.GetTemporary(
            texture.width,
            texture.height,
            0,
            RenderTextureFormat.Default,
            RenderTextureReadWrite.Linear
        );

        // Blit the texture into the RenderTexture
        Graphics.Blit(texture, renderTexture);

        // Create a new Texture2D
        Texture2D texture2D = new Texture2D(texture.width, texture.height, TextureFormat.RGBA32, false);

        // Read the RenderTexture contents into the Texture2D
        RenderTexture.active = renderTexture;
        texture2D.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        texture2D.Apply();

        // Clean up
        RenderTexture.active = null;
        RenderTexture.ReleaseTemporary(renderTexture);

        return texture2D;
    }


void InitCameraMask(){

           if (CaptureCamera == null)
        {
           // CaptureCamera = GetComponent<Camera>(); // Attempt to auto-assign
        }

        if (CaptureCamera != null)
        {
            // Store the original culling mask of the camera
            originalCullingMask = CaptureCamera.cullingMask;
        }
        else
        {
            Debug.LogError("No camera assigned to CameraCullingSwitcher!");
        }




}


    public void ToggleCullingMask()
    {
        if (CaptureCamera == null) return;

        if (isRenderingNothing)
        {
            // Restore the original culling mask
            CaptureCamera.cullingMask = originalCullingMask;
        }
        else
        {
            // Set the culling mask to Nothing
            CaptureCamera.cullingMask = 0;
        }

        // Toggle the state
        isRenderingNothing = !isRenderingNothing;
    }



    void Update() {
              if(OVRInput.GetUp(OVRInput.RawButton.A)){
                 ToggleCullingMask();
          
        }



    }




    public void StartCapture(){

        displayCaptureManager.StartScreenCapture();


    }

    public void StopCapture(){


        displayCaptureManager.StopScreenCapture();


    }

    public void sendCommand(string url,string command ,string urlid)
    {

        StartCoroutine(SendtheCommand(url,command,urlid));




    }







    // Update the texture to be uploaded
    public static void UpdateTexture(Texture2D texture)
    {
        streamingTexture = texture;
    }



        public void CaptureIpCam(string url, string filename,Vector2 objPosition,string urlid)
    {
        if (streamingTexture == null)
        {
            Debug.LogError("No texture set for streaming. Use UpdateTexture to set a texture first.");
            return;
        }

        StartCoroutine(UploadPNG(streamingTexture, url, filename,"",false,0,objPosition,false,"IP_RGB",urlid));
    }


        public void ModifyCapture(string url, string filename,Vector2 objPosition,string urlid)
    {
        if (streamingTexture == null)
        {
            Debug.LogError("No texture set for streaming. Use UpdateTexture to set a texture first.");
            return;
        }

        StartCoroutine(UploadPNG(streamingTexture, url, filename,"",false,0,objPosition,false,"RGB_modify",urlid));
    }


    // Capture and upload the current streaming texture with a custom filename
    public void Capture(string url, string filename,Vector2 objPosition,string urlid)
    {
        if (streamingTexture == null)
        {
            Debug.LogError("No texture set for streaming. Use UpdateTexture to set a texture first.");
            return;
        }

        StartCoroutine(UploadPNG(streamingTexture, url, filename,"",false,0,objPosition,false,"RGB",urlid));
    }

    // Overloaded Capture function to handle RenderTexture input
    public void UploadMask(string url, string filename, string prompt,Vector2 objPosition,string urlid)
    {
        Texture2D texture2D = ConvertRenderTextureToTexture2D(Mask);
        StartCoroutine(UploadPNG(texture2D, url, filename, prompt,true,40,objPosition,false,"Mask",urlid));
        Destroy(texture2D); // Clean up after upload
    }   



        public void UploadDepthMap(string url, string filename,Vector2 objPosition,string urlid)
    {
        Texture2D texture2D = ConvertRenderTextureToTexture2D(Depth);
        StartCoroutine(UploadPNG(texture2D, url, filename, "",true,0,objPosition,true,"Depth",urlid));
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
public IEnumerator UploadPNG(Texture2D texture, string url, string filename, string prompt, bool flipY, int xOffset, Vector2 objectPosition, bool debugDraw, string type,string urlid)
{
    byte[] pngData = texture.EncodeToPNG();
    if (pngData != null)
    {
        WWWForm form = new WWWForm();
        form.AddBinaryData("file", pngData, filename, "image/png");
        form.AddField("prompt", prompt);
        form.AddField("flipY", flipY ? "true" : "false"); 
        form.AddField("xOffset", xOffset.ToString()); 
        form.AddField("objectPosition", $"({(int)objectPosition.x},{(int)objectPosition.y})"); // Send as (x,y)
        form.AddField("debugDraw", debugDraw ? "true" : "false"); 
        form.AddField("type", type);
        form.AddField("URLID",urlid);

        UnityWebRequest request = UnityWebRequest.Post(url, form);
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


public IEnumerator SendtheCommand( string url,string command ,string urlid)
{
    
    if (command != "")
    {
        WWWForm form = new WWWForm();

        form.AddField("Command", command);
        form.AddField("URLID",urlid);

        UnityWebRequest request = UnityWebRequest.Post(url, form);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Command sent: " + command);
        }
        else
        {
            Debug.LogError("Error: " + request.error);
        }
    }
}

}
