using TMPro;
using UnityEngine;

public class PopUp : MonoBehaviour {
    [SerializeField] TextMeshProUGUI title;
    [SerializeField] TextMeshProUGUI text;
    float minSize = 120;


    private void Start() {
        RectTransform rt = GetComponent<RectTransform>();
        minSize = rt.sizeDelta.y;
        Debug.Log("MinSize = " + minSize);
    }

    bool wasChangeSize;
    private void Update() {
        if (wasChangeSize) return;
        wasChangeSize = true;
        changeSize();

    }
    private void OnEnable() {
        transform.localPosition = new Vector3(
            Camera.main.transform.position.x,
            Camera.main.transform.position.y,
            0);
    }
    public void setText(string text) {
        this.text.text = text;
        changeSize();
    }
    public void changeSize() {
        RectTransform rt = GetComponent<RectTransform>();
        rt.sizeDelta = new Vector3(
            rt.sizeDelta.x,
            minSize + this.text.mesh.bounds.size.y);
    }
    public void setTitle(string text) {
        title.text = text;
    }

    public void close() {
        Destroy(gameObject);
    }
}
