using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Door : InteractableObject
{
    public bool isOpen;
    [SerializeField] bool isRotating = true;
    [SerializeField] float speed = 1f;

    [Header("Rotation")]
    [SerializeField] float openAngle = 90f;
    [SerializeField] float forwardDirection = 0;

    [Header("Sliding")]
    [SerializeField] Vector3 slideDirection = Vector3.back;
    [SerializeField] float slideAmt = 1.9f;
    
    private Vector3 StartRotation;
    private Vector3 StartPosition;
    //technically points into door frame, so right will be forward instead
    private Vector3 forward;

    Coroutine AnimationCoroutine;

    private void Awake()
    {
        StartRotation = transform.rotation.eulerAngles;
        StartPosition = transform.position;
        forward = transform.right;
    }

    public void Open(Vector3 userPos)
    {
        if (!isOpen)
        {
            if (AnimationCoroutine != null)
            {
                StopCoroutine(AnimationCoroutine);
            }
            if (isRotating)
            {
                float dot = Vector3.Dot(forward, (userPos - transform.position).normalized);
                Debug.Log($"Dot: {dot.ToString("N3")}");
                AnimationCoroutine = StartCoroutine(RotateOpen(dot));
            }
            else
            {
                AnimationCoroutine = StartCoroutine(SlideOpen());
            }
        }
    }

    private IEnumerator RotateOpen(float forwardAmt)
    {
        Quaternion startRotation = transform.rotation;
        Quaternion endRotation;

        if (forwardAmt >= forwardDirection)
        {
            endRotation = Quaternion.Euler(new Vector3(0, startRotation.y - openAngle, 0));
        }
        else
        {
            {
                endRotation = Quaternion.Euler(new Vector3(0, startRotation.y + openAngle, 0));
            }
        }
        
        isOpen = true;
        float timeElapsed = 0;
        while(timeElapsed < 1)
        {
            transform.rotation = Quaternion.Slerp(startRotation, endRotation, timeElapsed);
            yield return null;
            timeElapsed += Time.deltaTime * speed;
        }
    }

    private IEnumerator SlideOpen()
    {
        Vector3 endPosition = StartPosition + slideAmt * slideDirection.normalized;
        Vector3 startPosition = transform.position;

        isOpen = true;
        float timeElapsed = 0;
        while (timeElapsed < 1)
        {
            transform.position = Vector3.Lerp(startPosition, endPosition, timeElapsed);
            yield return null;
            timeElapsed += Time.deltaTime * speed;
        }
    }

    private void Close()
    {
        if (isOpen)
        {
            if (AnimationCoroutine != null)
            {
                StopCoroutine(AnimationCoroutine);
            }

            if (isRotating)
            {
                AnimationCoroutine = StartCoroutine(RotateClose());
            }
            else
            {
                AnimationCoroutine = StartCoroutine(SlideClose());
            }
        }
    }

    private IEnumerator RotateClose()
    {
        Quaternion startRotation = transform.rotation;
        Quaternion endRotation = Quaternion.Euler(StartRotation);

        isOpen = false;
        float timeElapsed = 0;  
        while (timeElapsed < 1)
        {
            transform.rotation = Quaternion.Slerp(startRotation, endRotation, timeElapsed);
            yield return null;
            timeElapsed += Time.deltaTime * speed;
        }
    }

    private IEnumerator SlideClose()
    {
        Vector3 endPosition = StartPosition;
        Vector3 startPosition = transform.position;

        isOpen = false;
        float timeElapsed = 0;
        while (timeElapsed < 1)
        {
            transform.position = Vector3.Lerp(startPosition, endPosition, timeElapsed);
            yield return null;
            timeElapsed += Time.deltaTime * speed;
        }
    }

    public override bool ActivateInteract(PlayerInteract interactor)
    {
        if (isOpen)
        {
            Close();
        }
        else
        {
            Open(gamemanager.instance.player.transform.position);
        }
        return true;
    }
}
