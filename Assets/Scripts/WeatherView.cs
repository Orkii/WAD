using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using static System.Net.Mime.MediaTypeNames;

public class WeatherView : MonoBehaviour {
    //[SerializeField] GameObject imageGO;
    [SerializeField] GameObject loadImageGO;
    [SerializeField] TextMeshProUGUI text;

    [SerializeField] UnityEngine.UI.Image weatherImage;

    private void Start() {
        setText("");
        //imageGO.SetActive(false);
        weatherImage.sprite = null;
        loadImageGO.SetActive(false);
    }
    public void setImage(Texture2D texture) {
        if (texture == null) removeImage();
        weatherImage.sprite = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100.0f);
        weatherImage.gameObject.SetActive(true);
        loadImageGO.SetActive(false);
    }

    public void removeImage() {
        weatherImage.sprite = null;
        weatherImage.gameObject.SetActive(false);
        loadImageGO.SetActive(true);
    }

    public void clear() {
        text.text = "";
        removeImage();
    }
    public void setText(string txt) {
        Debug.Log("Set text = " + txt);
        text.text = txt;
        Debug.Log("Set text end");
    }

}
