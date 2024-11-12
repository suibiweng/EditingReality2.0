using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RealityEditor;
using UnityEngine.Networking;
using System;


public class ReConstructSpot : MonoBehaviour
{
    public RealityEditorManager manager;


    public GameObject Target;
    public ModelDownloader modelDownloader;

    public Fast3dFunctions fast3DFunctions;


    public string URLID;

    public string serverURL;
    // Start is called before the first frame update


   Coroutine FileCheck;

   public string DownloadURL="";
   public string UploadURL="";

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




        
    }

    // Update is called once per frame
    void Update()
    {

        if(OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger)){

            StartGeneration();






        }

        
        




    }

    public void StartGeneration(){


        fast3DFunctions.Capture(UploadURL,URLID+".png");
        fast3DFunctions.UploadMask(UploadURL,URLID+".png","MaskTest");         
      //FileCheck= StartCoroutine(CheckURLPeriodically(DownloadURL + URLID + "_construct.zip"));


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
