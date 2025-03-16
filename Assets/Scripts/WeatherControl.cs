using SimpleJSON;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using Zenject;



public class WeatherControl : MonoBehaviour {
    private const string WEATHER_URL = "https://api.weather.gov/gridpoints/TOP/32,81/forecast";

    DateTime lastUpdate;
    [Inject] ServerAsk server;
    [SerializeField] WeatherView weatherView;
    List<long> nowLaodID = new List<long>();

    void Start() {
        
    }

    
    void Update() {
        if (DateTime.Now - lastUpdate > TimeSpan.FromSeconds(5)) {
            lastUpdate = DateTime.Now;
            weatherChange();
        }    
    }
    public async void weatherChange() {
        Debug.Log("weatherChange 0");
        nowLaodID.Add(server.JSONNodeGet(WEATHER_URL, onWeatherLoaded, onWeatherStatus));
        Debug.Log("weatherChange 1");
    }
    void onWeatherLoaded(JSONNode node, long id) {
        nowLaodID.Remove(id);
        if (Utils.isNull(node)) { errorOccurredWhileLoad(); return; }
        JSONNode currentWeather = extractWeatherNode(node);
        if (Utils.isNull(currentWeather)) { errorOccurredWhileLoad(); return; }
        string imagePath = extractImagePath(currentWeather);
        if (Utils.isNull(imagePath)) { errorOccurredWhileLoad(); return; }
        weatherView.setText(makeBeautifulText(extractDegrees(currentWeather)));
        weatherView.removeImage();
        nowLaodID.Add(server.texture2DGet(imagePath, onImageLoaded, onWeatherStatus)); 
    }
    void onImageLoaded(Texture2D texture, long id) {
        nowLaodID.Remove(id);
        if (Utils.isNull(texture)) { errorOccurredWhileLoad(); return; }
        weatherView.setImage(texture);
    }
    void onWeatherStatus(string loadable, long id) {
        //Debug.Log("Status = " + loadable);
    }
    private string extractImagePath(JSONNode node) {
        
        return Utils.removeQuotationMarks(node["icon"].ToString());
    }
    private string extractDegrees(JSONNode node) {
        string deg = node["temperature"].ToString();
        if (Utils.isNull(deg)) return Utils.NULL_STR;
        string tUnit = Utils.removeQuotationMarks(node["temperatureUnit"].ToString());
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
        Debug.LogWarning("errorOccurredWhileLoad");
    }

    void OnDisable() {
        foreach (long a in nowLaodID) {
            server.stopLoad(a);
        }
        nowLaodID.Clear();
        weatherView.clear();
    }

    void OnEnable() {
        lastUpdate = DateTime.Now;
        weatherChange();
    }

}




