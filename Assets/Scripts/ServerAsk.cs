

using SimpleJSON;
using System;
using System.Collections.Generic;
using System.Net.Http;
using UnityEngine;
using UnityEngine.Networking;
using static Loadable;
public class ServerAsk : MonoBehaviour {


    Queue<LoadProcess> toLoad = new Queue<LoadProcess>();
    [SerializeField] List<LoadProcess> list;
    LoadProcess currentLoad { get { return toLoad.Count == 0 ? null : toLoad.Peek(); } }

    private void Start() {
        Debug.Log("ServerAsk is alive!");
        
    }
    private void FixedUpdate() {
        list = new List<LoadProcess>(toLoad);
        if (currentLoad == null) return;
        if (currentLoad.request == null) return;

        if (currentLoad.request.isNetworkError) {
            Debug.LogWarning("Error = " + currentLoad.request.error);
            Debug.LogWarning("URL = " + currentLoad.request.url);
            errorStop();
            return;
        }

        //if (currentLoad.request) {
        //    if (currentLoad.request.downloadHandler.isDone) {
        //        Debug.Log("currentLoad = " + currentLoad);
        //        loadFinish(currentLoad.request);
        //    }
        //}
        //else{
        //    currentLoad.OnLoadStatusInvoke(currentLoad.request.downloadedBytes.ToString());
        //}
        
    }

    public async void imageGet(string imagePath, Action<Loadable> loadHandler, Action<string> statusHandler) {
        
        LoadProcess loadProcess = new LoadProcess();
        if (toLoad.Count > 1) return;
        loadProcess.OnLoadFinish += loadHandler;
        loadProcess.OnLoadStatus += statusHandler;
        Debug.Log("ImPath = " + imagePath);
        Uri requestURI = new Uri(imagePath);
        UnityWebRequestTexture.GetTexture(requestURI);
        //loadProcess.request = UnityWebRequestTexture.GetTexture(requestURI);
        //loadProcess.request = UnityWebRequestTexture.GetTexture(imagePath);
        
        
        //loadProcess.request = UnityWebRequestTexture.GetTexture("https://api.weather.gov/icons/land/night/wind_bkn?size=medium");

        Debug.Log("ImPathN = " + loadProcess.request.url);
        toLoad.Enqueue(loadProcess);
        if (toLoad.Count > 1) return;
        startLoad();
    }

    public void weatherGet(Action<Loadable> loadHandler, Action<string> statusHandler) {
        LoadProcess loadProcess = new LoadProcess();
        loadProcess.OnLoadFinish += loadHandler;
        loadProcess.OnLoadStatus += statusHandler;
        
        loadProcess.request = UnityWebRequest.Get("https://api.weather.gov/gridpoints/TOP/32,81/forecast");
        Debug.Log("WEPathN = " + loadProcess.request.url);
        toLoad.Enqueue(loadProcess);
        if (toLoad.Count > 1) return;
        startLoad();
        
    }

    protected void startLoad() {
        currentLoad.request.SendWebRequest();
        if (currentLoad.request.isNetworkError) errorStop();

    }

    protected void loadFinish(UnityWebRequest request) {
        Debug.Log("Finish");
        currentLoad.OnLoadStatusInvoke("Done");
        currentLoad.OnLoadFinishInvoke(new Loadable(LOAD_STATUS.OK, request.downloadHandler));
        clearLoad();
        if (toLoad.Count > 0) startLoad();
    }
    protected void errorStop(){
        Debug.Log("errorStop");
        currentLoad.OnLoadFinishInvoke(new Loadable(LOAD_STATUS.ERROR, null));
        clearLoad();
        if (toLoad.Count > 0) startLoad();
    }
    
    protected void clearLoad() {
        Debug.Log("clearLoad");
        Debug.Log("clearLoad = " + toLoad.Count);
        toLoad.Dequeue();
        Debug.Log("clearLoad = " + toLoad.Count);       
    }
}


public class Loadable {
    public LOAD_STATUS status;
    public DownloadHandler response;

    public Loadable(LOAD_STATUS status, DownloadHandler response) {
        this.status = status;
        this.response = response;
    }

    public enum LOAD_STATUS {
        OK,
        ERROR
    }
}

[Serializable]
public class LoadProcess {
    //public Type type;
    public UnityWebRequest request;
    public event Action<Loadable> OnLoadFinish;
    public event Action<string> OnLoadStatus;
    public void OnLoadFinishInvoke(Loadable loadable) {
        OnLoadFinish.Invoke(loadable);
    }
    public void OnLoadStatusInvoke(string str) {
        OnLoadStatus.Invoke(str);
    }
    public void OnLoadFinishClear() {
        OnLoadFinish = null;
    }
    public void OnLoadStatusClear() {
        OnLoadStatus = null;
    }
}