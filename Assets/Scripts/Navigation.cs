using UnityEngine;

public class Navigation : MonoBehaviour {


    [SerializeField] Window dogWindow;
    [SerializeField] Window weatherWindow;

    void Start() {
        Debug.Log("Navigation is alive!");
    }

    public void dogButton() {
        dogWindow.open();
        weatherWindow.close();
        Debug.Log("Dog pressed");
    }

    public void weatherButton() {
        weatherWindow.open();
        dogWindow.close();
        Debug.Log("Weather pressed");
    }
}
