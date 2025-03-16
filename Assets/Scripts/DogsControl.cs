using ModestTree;
using SimpleJSON;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine;
using Zenject;

public class DogsControl : MonoBehaviour {
    private const string DOGS_URL = "https://dogapi.dog/api/v2/breeds";
    private const int DOGS_COUNT_TO_SHOW = 10;

    [Inject] ServerAsk server;
    [SerializeField] ViewList dogViewList;
    [SerializeField] GameObject dogPopUp;
    [SerializeField] GameObject dogElement;
    List<long> nowLaodID = new List<long>();

    



    

    void onDogLoaded(JSONNode node, long id) {
        nowLaodID.Remove(id);   
        Debug.Log("onDogLoaded");
        if (Utils.isNull(node)) { errorOccurredWhileLoad(); return; }
        List<Dog> dogs = extractDogs(node);
        if (Utils.isNull(dogs)) { errorOccurredWhileLoad(); return; }

        
        foreach (Dog el in dogs) {

            GameObject dog = Instantiate(dogElement, transform);
            dog.GetComponent<DogElement>().setDog(el);
            dog.GetComponent<DogElement>().onPress += onDogPressed;
            Debug.Log("HERE");
            dogViewList.addElement(dog);
        }
    }
    private void stopLoadAll() {
        foreach (long id in nowLaodID) {
            server.stopLoad(id);
        }
        dogViewList.stopLoadAll();
        nowLaodID.Clear();
    }
    private void onDogPressed(Dog dog) {
        Debug.Log("onDogPressed");
        if (nowLaodID.Count > 0) stopLoadAll();
        dogViewList.startLoad(dog);
        nowLaodID.Add(server.JSONNodeGet(DOGS_URL + "/" + dog.id, showPopUp, dogFactLoadStatus));
    }
    private void showPopUp(JSONNode node, long id) {
        nowLaodID.Remove(id);
        if (Utils.isNull(node)) { errorOccurredWhileLoad(); return; }
        
        

        Dog dog = extractDog(node["data"]);
        if (dog == null) { errorOccurredWhileLoad(); return; }

        dogViewList.stopLoad(dog.id);
        string facts = extractDogFacts(node["data"]);
        //if (facts.Contains("null")) return;

        GameObject pop = Instantiate(dogPopUp);
        pop.GetComponent<PopUp>().setText(facts);
        pop.GetComponent<PopUp>().setTitle(dog.name);

        pop.transform.SetParent(transform);
        pop.transform.position = dogViewList.transform.position;
        Debug.Log("Fact loaded = " + node.ToString());
    }
    
    private void dogFactLoadStatus(string status, long id) {

    }

    void onDogStastus(string node, long id) {
        if (Utils.isNull(node)) { errorOccurredWhileLoad(); return; }

    }


    public void dogStart() {
        Debug.Log("dogStart 0");
        dogViewList.contentIsLoading();
        nowLaodID.Add(server.JSONNodeGet(DOGS_URL, onDogLoaded, onDogStastus));
        Debug.Log("dogStart 1");
    }



    private void errorOccurredWhileLoad() {
        Debug.LogError("errorOccurredWhileLoad");
    }

    private string extractDogFacts(JSONNode node) {
        if (Utils.isNull(node)) { return "null"; }
        string res = "";
        res += "\nОписание:" + node["attributes"]["description"];


        res += "\nМаксимальная продолжительность жизни:" + node["attributes"]["life"]["max"];
        res += "\nСредняя продолжительность жизни:" + node["attributes"]["life"]["min"];

        res += "\nМаксимальный вес самца:" + node["attributes"]["male_weight"]["max"];
        res += "\nСредний вес самца:" + node["attributes"]["male_weight"]["min"];

        res += "\nМаксимальынй вес самки:" + node["attributes"]["female_weight"]["max"];
        res += "\nСредний вес самки:" + node["attributes"]["female_weight"]["min"];

        JSONNode hypoallergenicNode = node["attributes"]["hypoallergenic"];
        if (hypoallergenicNode.IsNull) { return "null"; }
        res += "\nГипоаллергенный: " + (hypoallergenicNode.AsBool ? "Да" : "Нет");
        return res;
    }
private List<Dog> extractDogs(JSONNode node) {
        List<Dog> dogs = new List<Dog>();
        //Debug.Log("data = " + node["data"][1].ToString());
        for (int i = 0; i < DOGS_COUNT_TO_SHOW; i++) {
            //Debug.Log("data" + i + " " + node["data"][i].ToString());
            JSONNode dogNode = node["data"][i];
            if (Utils.isNull(dogNode)) return dogs;
            Dog dog = extractDog(dogNode);
            if (Utils.isNull(dog)) return dogs;
            dogs.Add(dog);
        }
        return dogs;
    }
    private Dog extractDog(JSONNode node) {
        string id = node["id"].ToString();
        string name = node["attributes"]["name"].ToString();
        if (Utils.isNull(id)) return null;
        if (Utils.isNull(name)) return null;
        return new Dog(Utils.removeQuotationMarks(name), Utils.removeQuotationMarks(id));
    }



    void OnDisable() {
        foreach (long a in nowLaodID) {
            server.stopLoad(a);
        }
        nowLaodID.Clear();
        dogViewList.clear();
    }

    void OnEnable() {
        dogStart();
    }
}



public class Dog {
    public string name;
    public string id;
    public Dog(string name, string id) {
        this.name = name;
        this.id = id;
    }
    public override string ToString() {
        return "(" + name + ")";
    }

}
