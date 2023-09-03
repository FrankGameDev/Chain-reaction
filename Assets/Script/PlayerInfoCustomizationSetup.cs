using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class PlayerInfoCustomizationSetup : MonoBehaviour
{

    [Header("Name settings")]
    public TextMeshProUGUI playerTitle;
    public TMP_InputField inputNameField;

    [Header("Color section")]
    public Slider colorPicker;
    public Image sliderBackground;
    public Image colorImagePreview;

    private Color choosenColor;

    private void Start()
    {
        colorPicker.minValue = 0;
        colorPicker.maxValue = 1;


        RandomColor();
        colorPicker.value = choosenColor.r;
    }


    public void PreviewColor()
    {
        choosenColor = Color.HSVToRGB(colorPicker.value, 1, 1);
        colorImagePreview.color = choosenColor;
        inputNameField.GetComponent<Image>().color = choosenColor;
        playerTitle.color = choosenColor;
    }

    public void RandomColor()
    {
        choosenColor = Random.ColorHSV(0, 1, .5f, 1, .5f, 1);
        colorImagePreview.color = choosenColor;
        inputNameField.GetComponent<Image>().color = choosenColor;
        playerTitle.color = choosenColor;
    }


    //UTILS



    //GETTER & SETTER   
    public void SetPlayerNumber(int num) => playerTitle.text = $"Player {num}";

    public string GetPlayerName() => inputNameField.text;
    public Color GetColor() => choosenColor;

}
