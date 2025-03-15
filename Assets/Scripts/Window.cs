using UnityEngine;

public class Window : MonoBehaviour {
    void Start() {
        //Debug.Log(name + " is alive!");
    }

    public void open() {
        //enabled = true;
        gameObject.SetActive(true);
        //Debug.Log(name + " is opend!");
    }

    public void close () {
        //enabled = false;
        gameObject.SetActive(false);
        //Debug.Log(name + " is closed!");
    }
}
