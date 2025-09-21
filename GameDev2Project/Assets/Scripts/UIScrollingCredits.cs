using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIScrollingCredits : MonoBehaviour
{
    public RectTransform viewport;
    public float speed = 80f;
    public float topPadding = 80f;
    public bool loop = false;

    RectTransform content;
    float startY, endY;
    bool done;

    private void Awake()
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate(content);
        float contentHeight = content.rect.height;
        float viewHeight = viewport.rect.height;

        startY = -(viewHeight / 2f) - (contentHeight / 2f);

        endY = (viewHeight / 2f) + (contentHeight / 2f) + topPadding;

        var pos = content.anchoredPosition;
        pos.y = startY;
        content.anchoredPosition = pos;
    }

    private void Update()
    {
        if(done) return;
        var pos = content.anchoredPosition;
        pos.y += speed * Time.unscaledDeltaTime;
        content.anchoredPosition = pos;

        if (pos.y >= endY)
        {
            if (loop)
            {
                pos.y = -startY;
                content.anchoredPosition = pos;
            }
            else
            {
                done = true;
            }
        }
    }
}
