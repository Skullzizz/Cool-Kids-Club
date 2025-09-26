using UnityEngine;

public class WebGLMainmenu : MonoBehaviour
{
    [SerializeField] GameObject quitToDesktop;
    void Start()
    {
#if UNITY_WEBGL
        quitToDesktop.SetActive(false);
#endif
    }
}
