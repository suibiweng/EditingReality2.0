using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RealityEditor;
using UnityEngine.Networking;
using System;
using TMPro;
using UnityEngine.UI;
using DimBoxes;
using Oculus.Interaction;

public class ReConstructSpot : MonoBehaviour
{
    public RealityEditorManager manager;

    public TMP_Text promptText;

    public string prompt;


    public GameObject Target;
    public ModelDownloader modelDownloader;

    public Fast3dFunctions fast3DFunctions;


    public string URLID;

    public string serverURL;
    // Start is called before the first frame update


   Coroutine FileCheck;

   public string DownloadURL="";
   public string UploadURL="";

   public BoundBox boundBox;
   public bool isselsected=false;
  public Grabbable _grabbable;

  public Toggle debugShow;

  public VoiceLabel voiceLabel;

   

    void Start()
    {
        manager = FindObjectOfType<RealityEditorManager>();
        modelDownloader = FindObjectOfType<ModelDownloader>();
        fast3DFunctions= FindObjectOfType<Fast3dFunctions>();
        DownloadURL=manager.ServerURL;
        UploadURL=manager.ServerURL;
        DownloadURL+=":"+manager.downloadPort+"/";
        UploadURL+=":"+manager.uploadPort+"/upload";
      //  ServerURL+=":"+downloadPort+"/";
       // fast3DFunctions.StartCapture();
//    _grabbable = GetComponent<Grabbable>();
        _grabbable.WhenPointerEventRaised += HandlePointerEventRaised;


  


        
    }

    // Update is called once per frame
    void Update()
    {
        if(debugShow!=null)
        debugShow.isOn = isselsected;

        if(isselsected){

        if(OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger)){

            StartGeneration();

        }


        }

    



       // prompt= manager.VoiceToPrompt;

        
        
        prompt=promptText.text;



    }






    public void setVoiceInput(){

        prompt= voiceLabel.Label.text;
        





    }


    private void HandlePointerEventRaised(PointerEvent evt)
    {
        switch (evt.Type)
        {
            case PointerEventType.Select:
                OnSelect();
                
                break;
            case PointerEventType.Unselect:

            // PreviewWindow.gameObject.SetActive(false);
             Release();

                break;
        }
    }


        public void OnSelect()
    {

        manager.updateSelected( URLID);
        isselsected = true;
       

    }

    public void   Release(){


    }


    

Vector2 ObjectScreenPosition()
{
    // Convert this GameObject's position to 2D screen coordinates
    Vector3 screenPosition = fast3DFunctions.MaskCamera.WorldToScreenPoint(transform.position);

    // Check if the object is in front of the camera
    if (screenPosition.z > 0)
    {
        Vector2 screenPosition2D = new Vector2(screenPosition.x, screenPosition.y);
        Debug.Log("Screen Position (2D): " + screenPosition2D);
        return screenPosition2D;
    }
    else
    {
        Debug.Log("Object is behind the camera.");
        return Vector2.zero; // Or any value you choose to represent an invalid position
    }
}




bool Capturing=false;

    public void StartGeneration(){

        if(!isselsected) return;

        if(!Capturing)
            StartCoroutine(CaptureRouting());

        // fast3DFunctions.Capture(UploadURL,URLID+".png",ObjectScreenPosition(),URLID);
        // fast3DFunctions.UploadMask(UploadURL,URLID+"_Mask.png","MaskTest",ObjectScreenPosition(),URLID);         
      //FileCheck= StartCoroutine(CheckURLPeriodically(DownloadURL + URLID + "_reconstruct.zip"));


    
    
    
    }

        public void modifywithPrompt(){



            if(!isselsected) return;

            if(!Capturing)
                StartCoroutine(MaskWithPrompt());









        }




        IEnumerator MaskWithPrompt(){


                Capturing=true;

        Vector2 TargetPos =ObjectScreenPosition();
        fast3DFunctions.ToggleCullingMask();
        yield return new WaitForSeconds(0.3f);
        fast3DFunctions.Capture(UploadURL,URLID+".png",TargetPos,URLID);
        yield return new WaitForSeconds(0.3f);
        fast3DFunctions.UploadMask(UploadURL,URLID+"_Mask.png",prompt,TargetPos,URLID);   
         //yield return new WaitForSeconds(0.3f);
        //fast3DFunctions.CaptureDepth(UploadURL,URLID+"_Depth.png",TargetPos,URLID);
        yield return new WaitForSeconds(0.3f);
        fast3DFunctions.ToggleCullingMask();
        // if(FileCheck==null)
        //     FileCheck= StartCoroutine(CheckURLPeriodically(DownloadURL + URLID + "_reconstruct.zip"));


        Capturing=false;







        }






    IEnumerator CaptureRouting(){
        Capturing=true;


        Vector2 TargetPos =ObjectScreenPosition();
        fast3DFunctions.ToggleCullingMask();
        yield return new WaitForSeconds(0.3f);
        fast3DFunctions.Capture(UploadURL,URLID+".png",TargetPos,URLID);
          //yield return new WaitForSeconds(0.3f);
          //fast3DFunctions.UploadMask(UploadURL,URLID+"_Mask.png","MaskTest",TargetPos,URLID);   
         //yield return new WaitForSeconds(0.3f);
        //fast3DFunctions.CaptureDepth(UploadURL,URLID+"_Depth.png",TargetPos,URLID);
        yield return new WaitForSeconds(0.3f);
        fast3DFunctions.ToggleCullingMask();
        if(FileCheck==null)
            FileCheck= StartCoroutine(CheckURLPeriodically(DownloadURL + URLID + "_reconstruct.zip"));


        Capturing=false;




    }




    public void Delete(){

        manager.RemoveReConSpot(URLID);


    }




    








    public void downloadModel(string url, GameObject warp)
    {
        modelDownloader.AddTask(
            new ModelIformation()
            {
                ModelURL = url,
                gameobjectWarp = warp
            }
        );

        // loadingIcon.SetActive(false);
        // loadingParticles.Stop();
        // SmoothCubeRenderer.enabled = false;

        modelDownloader.startDownload();
    }




      IEnumerator CheckURLPeriodically(string urltocheck)
    {
        yield return new WaitForSeconds(10f);
        while (true)
        {
            yield return CheckURL(urltocheck);
            yield return new WaitForSeconds(checkInterval);
        }
    }

     public float checkInterval = 5f; // Check the URL every 5 seconds
    public event Action<bool> OnURLResponse = delegate { };

     IEnumerator CheckURL(string url)
    {
        UnityWebRequest www = UnityWebRequest.Get(url);
        UnityWebRequestAsyncOperation requestAsyncOperation = www.SendWebRequest();

        while (!requestAsyncOperation.isDone)
        {
            yield return null;
        }

        if (www.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("URL is responding!");



            StopCoroutine(FileCheck);
            FileCheck=null;

            downloadModel(url, Target);







            OnURLResponse(true);
        }
        else
        {
            //  Debug.LogError("Error checking URL: " + www.error);
            OnURLResponse(false);
        }

        www.Dispose();
    }



}
