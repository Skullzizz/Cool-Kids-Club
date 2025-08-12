using UnityEngine;
using UnityEngine.UI;

public class UISmoothFillBar : MonoBehaviour
{

    [SerializeField] private Image fillImage;
    [SerializeField] private float lerpSpeed = 5f;

    [SerializeField] private Color fullColor = Color.green;
    [SerializeField] private Color lowColor = Color.red;    

    private float targetFill = 1f;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void SetFill(float value)
    {
        targetFill = Mathf.Clamp01(value);
    }
    
    void Awake()
    {
        if (!fillImage) fillImage = GetComponent<Image>();
        if (fillImage) targetFill = fillImage.fillAmount;
    }

    // Update is called once per frame
    void Update()
    {
        if (!fillImage) return;

        float t = Mathf.Clamp01(Time.unscaledDeltaTime * lerpSpeed);
        fillImage.fillAmount = Mathf.Lerp(fillImage.fillAmount, targetFill, t); //makes it so that the fill animation will work no matter fps
        fillImage.color = Color.Lerp(lowColor, fullColor, fillImage.fillAmount);
    }
}
