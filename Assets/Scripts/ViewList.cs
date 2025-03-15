using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ViewList : MonoBehaviour{

    [SerializeField] GameObject content;
    [SerializeField] RectTransform contentRectTransform;
    [SerializeField] GridLayoutGroup grid;
    List<DogElement> DogElementList = new List<DogElement>();

    void Start() {
        //content.transform.childCount
        
    }

    public void addElement(GameObject go) {
        go.transform.SetParent(content.transform);
        DogElementList.Add(go.GetComponent<DogElement>());
        contentRectTransform.sizeDelta = new Vector2(contentRectTransform.sizeDelta.x, (grid.cellSize.y + grid.spacing.y) * content.transform.childCount + 100);
    }

    public void clear() {
        while (content.transform.childCount != 0) {
            Destroy(content.transform.GetChild(0));
        }
    }

    public void startLoad(Dog dog) {
        foreach (DogElement element in DogElementList) {
            if (element.dogId == dog.id) {
                element.startLoad();
            }
        }
    }
    public void stopLoad() {
        foreach (DogElement element in DogElementList) {
            element.stopLoad();
        }
    }


}
