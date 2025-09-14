using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SensitivitySettings : MonoBehaviour
{
    [SerializeField] Slider sensitivitySlider;
    [SerializeField] cameraController cam;
    [SerializeField] TMP_Text sensitivityValueText;
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

            if(cam) cam.SetSensitivity(saved);
            UpdateLabel(saved);
        }
    }
    public void OnSensitivityChange (float value)
    {
        int val = Mathf.RoundToInt(value);
        if (cam) cam.SetSensitivity(val);
        PlayerPrefs.SetInt(SensKey, val);
        PlayerPrefs.Save();
        UpdateLabel(val);
    }

    private void UpdateLabel(int val)
    {
        if (sensitivityValueText)
            sensitivityValueText.text = val.ToString();
            
        
    }
}
