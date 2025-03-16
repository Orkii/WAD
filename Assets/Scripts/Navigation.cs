using UnityEngine;

public class Navigation : MonoBehaviour {


    [SerializeField] Window dogWindow;
    [SerializeField] Window weatherWindow;

    void Start() {
        Debug.Log("Navigation is alive!");
    }

    public void dogButton() {
        weatherWindow.close();
        dogWindow.open();
        Debug.Log("Dog pressed");
    }

    public void weatherButton() {
        dogWindow.close();
        weatherWindow.open();
        Debug.Log("Weather pressed");
    }
}
