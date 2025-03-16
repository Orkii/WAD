using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ViewList : MonoBehaviour{

    [SerializeField] GameObject content;
    [SerializeField] RectTransform contentRectTransform;
    [SerializeField] GridLayoutGroup grid;
    [SerializeField] GameObject loadIcon;
    List<DogElement> DogElementList = new List<DogElement>();

    void Start() {
        //content.transform.childCount
        
    }

    public void addElement(GameObject go) {
        loadIcon.SetActive(false);
        go.transform.SetParent(content.transform);
        DogElementList.Add(go.GetComponent<DogElement>());
        contentRectTransform.sizeDelta = new Vector2(contentRectTransform.sizeDelta.x, (grid.cellSize.y + grid.spacing.y) * content.transform.childCount + 100);
    }

    public void clear() {
        foreach (Transform child in content.transform) {
            Destroy(child.gameObject);
        }
    }

    public void contentIsLoading() {
        loadIcon.SetActive(true);
    }

    public void startLoad(Dog dog) {
        foreach (DogElement element in DogElementList) {
            if (element.dogId == dog.id) {
                Debug.Log("LOAD");
                element.startLoad();
            }
        }
    }
    public void stopLoad(string id) {
        foreach (DogElement element in DogElementList) {
            if (element.dogId == id) element.stopLoad();
        }
    }
    public void stopLoadAll() {
        foreach (DogElement element in DogElementList) {
            element.stopLoad();
        }
    }

}
