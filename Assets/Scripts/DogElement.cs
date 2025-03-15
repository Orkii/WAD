using System;
using TMPro;
using UnityEngine;

public class DogElement : MonoBehaviour {
    [SerializeField] TextMeshProUGUI text;
    public Action<Dog> onPress;
    Dog dog;

    public string dogId { get => dog.id; }

    public void setDog(Dog dog) {
        this.dog = dog;
        text.text = dog.name;
    }

    public void press() {
        if (onPress != null) onPress.Invoke(dog);
    }

    public void startLoad() {
        gameObject.SetActive(true);
    }
    public void stopLoad() {
        gameObject.SetActive(false);
    }
}
