using TMPro;
using UnityEngine;

public class PromptUI : MonoBehaviour
{
    private Camera mainCamera;
    [SerializeField] private GameObject uiPanel;
    [SerializeField] private TextMeshProUGUI promptText;

    public bool isDisplayed = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        mainCamera = Camera.main;
        uiPanel.SetActive(false);
    }

    // Update is called once per frame
    void LateUpdate()
    {
        var rotation = mainCamera.transform.rotation;
        transform.LookAt(transform.position + rotation * Vector3.forward, rotation * Vector3.up);
    }

    public void SetText(string text)
    {
        promptText.text = text;
        uiPanel.SetActive(true);
        isDisplayed = true;
    }

    public void SetPos(Transform position)
    {
        transform.position = position.position;
    }
    public void Close()
    {
        uiPanel.SetActive(false);
        isDisplayed = false;
    }
}
