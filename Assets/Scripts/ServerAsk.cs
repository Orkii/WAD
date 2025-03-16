

using SimpleJSON;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using Unity.VisualScripting;
using UnityEditor.Search;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Rendering;
using UnityEngine.UI;
using static Loadable;
using static UnityEngine.Rendering.HDROutputUtils;
public class ServerAsk : MonoBehaviour {


    Queue<LoadProcess> toLoad = new Queue<LoadProcess>();
    [SerializeField] List<LoadProcess> list;
    private object yueld;

    private long nextID = 0;

    private long getNextID() {
        nextID++;
        return nextID;
    }


    public void stopLoad(long id) {
        Debug.Log("stopLoad");
        foreach (LoadProcess process in toLoad) {
            if (process.id == id) {
                process.continueLoad = false;
            }
        }
    }



    LoadProcess currentLoad { get { return toLoad.Count == 0 ? null : toLoad.Peek(); } }

    private void Start() {
        //Debug.Log("ServerAsk is alive!");
        List<LoadProcess> list = new List<LoadProcess>(toLoad); // serialize toLoad
    }
    private void FixedUpdate() {
        if (currentLoad == null) return;
        if (currentLoad.isLoading == false) {
            //if (currentLoad.continueLoad) == false 
            //Debug.Log("Start cour 1");
            Debug.Log(toLoad.Count);
            StartCoroutine(currentLoad.Start(onLoadEndCallback));
        }
    }

    private void onLoadEndCallback() {
        toLoad.Dequeue();
    }

    public long texture2DGet(string imagePath, Action<Texture2D, long> loadHandler, Action<string, long> statusHandler) {
        //Debug.Log("texture2DGet");
        LoadProcessTexture2D loadProcess = new LoadProcessTexture2D();
        loadProcess.OnLoadStatus += statusHandler;
        loadProcess.OnLoadFinish += loadHandler;
        loadProcess.id = getNextID();
        Uri requestURI = new Uri(imagePath, UriKind.Absolute);
        loadProcess.request = UnityWebRequestTexture.GetTexture(requestURI);
        toLoad.Enqueue(loadProcess);
        return loadProcess.id;
    }


    public long JSONNodeGet(string path, Action<JSONNode, long> loadHandler, Action<string, long> statusHandler) {
        Debug.Log("JSONNodeGet");
        LoadProcessJSONNode loadProcess = new LoadProcessJSONNode();
        loadProcess.OnLoadFinish += loadHandler;
        loadProcess.OnLoadStatus += statusHandler;
        loadProcess.request = UnityWebRequest.Get(path);
        loadProcess.id = getNextID();
        toLoad.Enqueue(loadProcess);
        return loadProcess.id;
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
public abstract class LoadProcess {
    //public Type type;

    public long id;

    public UnityWebRequest request;
    public event Action<string, long> OnLoadStatus;
    public bool isLoading = false;
    public bool continueLoad = true;
    int timeToWait = 100;

    public void OnLoadStatusInvoke(string str) {
        OnLoadStatus.Invoke(str, id);
    }
    public void OnLoadStatusClear() {
        OnLoadStatus = null;
    }
    /// <summary>
    /// Load data
    /// </summary>
    /// <param name="onExit"></param>
    /// <returns></returns>
    public IEnumerator Start(Action onExit) {
        if (!continueLoad) {
            onExit();
            yield break;
        }
        isLoading = true;
        //Debug.Log("Start cour 2");
        UnityWebRequestAsyncOperation operation = request.SendWebRequest();
        if (!request.isNetworkError ) {
            while (!operation.isDone && continueLoad) {
                OnLoadStatusInvoke(operation.progress.ToString());
                //Debug.Log("cour 3");
                yield return null;
            }
            if (request.isDone) {
                //Debug.Log("cour 4");
                while (timeToWait > 0) {// Loading is too fast, slow it down
                    Thread.Sleep(10);
                    timeToWait--;
                    yield return null;
                }
                if (!continueLoad) {
                    errorAccuired();
                    onExit();
                    yield break;
                }
                dataTransform();
            }
        }
        else {
            //Debug.Log("cour 5");
            errorAccuired();
        }
        if (!continueLoad) {
            request.Abort();
            errorAccuired();
        }
        //Debug.Log("cour 6");
        clear();
        onExit();
        isLoading = false;
    }
    protected virtual void clear() {
        request.Dispose();
        OnLoadStatusClear();
    }
    /// <summary>
    /// Finish callback invoke with data
    /// </summary>
    protected abstract void dataTransform();
    /// <summary>
    /// If smth go wrong, this is being invoked
    /// </summary>
    protected virtual void errorAccuired() {
        isLoading = false;
    }
}

public class LoadProcessTexture2D : LoadProcess {
    public event Action<Texture2D, long> OnLoadFinish;
    public void OnLoadFinishInvoke(Texture2D loadable) {
        OnLoadFinish.Invoke(loadable, id);
    }
    public void OnLoadFinishClear() {
        OnLoadFinish = null;
    }
    protected override void clear() {
        base.clear();
        OnLoadFinishClear();
    }

    protected override void dataTransform() {
        Texture2D texture = DownloadHandlerTexture.GetContent(request);
        OnLoadFinishInvoke(texture);
    }

    protected override void errorAccuired() {
        base.errorAccuired();
        OnLoadFinishInvoke(null);
    }
}

public class LoadProcessJSONNode : LoadProcess {
    public event Action<JSONNode, long> OnLoadFinish;
    public void OnLoadFinishInvoke(JSONNode loadable) {
        OnLoadFinish.Invoke(loadable, id);
    }
    public void OnLoadFinishClear() {
        OnLoadFinish = null;
    }
    protected override void clear() {
        base.clear();
        OnLoadFinishClear();
    }

    protected override void dataTransform() {
        JSONNode node = JSON.Parse(request.downloadHandler.text);
        OnLoadFinishInvoke(node);
    }

    protected override void errorAccuired() {
        base.errorAccuired();
        OnLoadFinishInvoke(null);
    }
}