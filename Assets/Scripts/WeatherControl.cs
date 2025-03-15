using SimpleJSON;
using System;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using Zenject;



public class WeatherControl : MonoBehaviour {


    DateTime lastUpdate;
    [Inject] ServerAsk server;
    [Inject] WeatherView weatherView;


    void Start() {
        
    }

    
    void Update() {
        if (DateTime.Now - lastUpdate > TimeSpan.FromSeconds(5)) {

        }    
    }


    public void weatherChange() {
        Debug.Log("weatherChange 0");
        server.weatherGet(onWeatherLoaded, onWeatherStatus);
        Debug.Log("weatherChange 1");
    }




    void onWeatherLoaded(Loadable loadable) {

        Debug.Log("onWeatherLoaded 0");

        if (loadable == null) return;
        if (loadable.status != Loadable.LOAD_STATUS.OK) return;

        Debug.Log("Result = " + loadable.response);

        
        JSONNode node = JSON.Parse(loadable.response.text);

        if (Utils.isNull(node)) { errorOccurredWhileLoad(); return; }
        JSONNode currentWeather = extractWeatherNode(node);
        if (Utils.isNull(currentWeather)) { errorOccurredWhileLoad(); return; }
        string imagePath = extractImagePath(currentWeather);
        if (Utils.isNull(imagePath)) { errorOccurredWhileLoad(); return; }
        weatherView.setText(makeBeautifulText(extractDegrees(currentWeather)));
        server.imageGet(imagePath, onImageLoaded, onWeatherStatus);

        
    }

    void onImageLoaded(Loadable loadable) {
        if (loadable == null) return;
        if (loadable.status != Loadable.LOAD_STATUS.OK) return;

        Debug.Log("isDone = " + loadable.response.isDone);
        
        Texture2D texture = (loadable.response as DownloadHandlerTexture).texture;
        if (Utils.isNull(texture)) { errorOccurredWhileLoad(); return; }
        weatherView.setImage(texture);

    }

    void onWeatherStatus(string loadable) {
        //Debug.Log("Status = " + loadable);
    }

    void onWeatherImage(string loadable) {

    }

    private string extractImagePath(JSONNode node) {
        return node["icon"].ToString();
    }
    private string extractDegrees(JSONNode node) {
        string deg = node["temperature"].ToString();
        if (Utils.isNull(deg)) return Utils.NULL_STR;
        string tUnit = node["temperatureUnit"].ToString();
        if (Utils.isNull(tUnit)) return Utils.NULL_STR;
        return deg + tUnit;
    }
    private string makeBeautifulText(string degrees) {
        return "Сегодня " + degrees;
    }

    private JSONNode extractWeatherNode(JSONNode node, int i = 0) {
        return node["properties"]["periods"][i];
    }

    private void errorOccurredWhileLoad() {
        Debug.LogError("errorOccurredWhileLoad");
    }
}




