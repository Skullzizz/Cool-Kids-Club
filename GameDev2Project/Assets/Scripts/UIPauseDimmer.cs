using UnityEngine;
using UnityEngine.UI;

public class PauseDimmer : MonoBehaviour
{
    [SerializeField] private Image dimImage;
    [SerializeField] private float fadeSpeed = 5f;
    private float targetAlpha = 0f;


    void Awake()
    {
        if (!dimImage) dimImage = GetComponent<Image>();
        SetAlpha(0f);
    }

    // Update is called once per frame
    void Update()
    {
        Color c = dimImage.color;
        c.a = Mathf.Lerp(c.a, targetAlpha, Time.unscaledDeltaTime * fadeSpeed);
        dimImage.color = c;
    }

    public void ShowDim()
    {
        targetAlpha = 0.5f;
    }

    public void HideDim()
    {
        targetAlpha = 0f;
    }

    private void SetAlpha(float a)
    {
        Color c = dimImage.color;
        c.a = a;
        dimImage.color = c;
    }
}
