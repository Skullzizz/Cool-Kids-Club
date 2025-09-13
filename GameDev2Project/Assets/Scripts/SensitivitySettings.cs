using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SensitivitySettings : MonoBehaviour
{
    [SerializeField] Slider sensitivitySlider;
    [SerializeField] private TextMeshProUGUI valueLabel;
    const string SensKey = "Sensitivity";

    

    void OnEnable()
    {
        if (sensitivitySlider)
        {
            sensitivitySlider.wholeNumbers = true;
            sensitivitySlider.minValue = 1;
            sensitivitySlider.maxValue = 10;

        int saved = PlayerPrefs.GetInt(SensKey, 3);
        sensitivitySlider.SetValueWithoutNotify(saved);
           sensitivitySlider.onValueChanged.AddListener(OnSensitivityChange);
        }
    }
    public void OnSensitivityChange (float value)
    {
        int val = Mathf.RoundToInt(value);
        PlayerPrefs.SetInt(SensKey, val);
        PlayerPrefs.Save();
        UpdateLabel(val);
    }

    private void UpdateLabel(int val)
    {
        if (valueLabel) valueLabel.text = $"Sensitivity: {val}";
        {
            
        }
    }
}
