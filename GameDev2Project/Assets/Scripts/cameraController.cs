using UnityEngine;

public class cameraController : MonoBehaviour
{
    [SerializeField, Range(1, 10)] int sens = 3;
    [SerializeField] int lockVertMin = -90, lockVertMax = 90;
    [SerializeField] bool invertY;

    float rotX;
    const string SensKey = "Sensitivity";

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        int saved = PlayerPrefs.GetInt(SensKey, sens);
        sens = Mathf.Clamp(saved, 1, 10); 
    }

    // Update is called once per frame
    void Update()
    {

        if(Time.timeScale == 0f || Cursor.lockState != CursorLockMode.Locked) return;

        // get input
        float mouseX = Input.GetAxisRaw("Mouse X") * sens;
        float mouseY = Input.GetAxisRaw("Mouse Y") * sens;

        // use invertY to give option of y look inversion
        if (invertY)
            rotX += mouseY;
        else
            rotX -= mouseY;

        // clamp camera on the X axis
        rotX = Mathf.Clamp(rotX, lockVertMin, lockVertMax);


        // rotate the camera to look up and down
        transform.localRotation = Quaternion.Euler(rotX, 0, transform.localEulerAngles.z);
        transform.parent.Rotate(Vector3.up * mouseX);

    }
public void SetSensitivity(int value)
    {
        sens = Mathf.Clamp(value, 1, 10);
        PlayerPrefs.SetInt(SensKey, sens);
        PlayerPrefs.Save();
    }
}
