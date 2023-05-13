using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class Cinematic : MonoBehaviour
{
    public Transform target;
    public float smoothSpeed = 0.125f;
    public List<Vector3> offsets;
    public float interval = 5.0f;
    public float minDistance = 1f;
    public float maxDistance = 5f;
    public float pivotDuration = 3f;

    public GameObject LetterBox;

    private float timer = 0f;
    private Coroutine cameraCoroutine;

    public PostProcessVolume postProcessVolume;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            if(cameraCoroutine != null)
            {
                StopCoroutine(cameraCoroutine);
            }

            postProcessVolume.enabled = true;
            LetterBox.SetActive(true);
            cameraCoroutine = StartCoroutine(MoveCamera());
        }

        if(Input.GetKeyDown(KeyCode.D))
        {
            if (cameraCoroutine != null)
            {
                StopCoroutine(cameraCoroutine);
                postProcessVolume.enabled = false;
                LetterBox.SetActive(false);
            }
        }
    }

    private IEnumerator MoveCamera()
    {
        Vector3 smoothPosition = Vector3.zero;

        int offsetIndex = 0;
        Vector3 currentOffset = offsets[offsetIndex];

        float smoothStep = 0f;

        while (true)
        {
            Vector3 desiredPosition = target.position + currentOffset;

            smoothStep += smoothSpeed * Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, smoothStep);

            smoothPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
            transform.position = smoothPosition;
            transform.LookAt(target.position);

            // Switch to the next offset every interval seconds
            if (timer < interval) { timer += Time.deltaTime; }
            else
            {
                offsetIndex = (offsetIndex + 1) % offsets.Count;
                currentOffset = offsets[offsetIndex];
                timer = 0.0f;
                smoothStep = 0f;
            }

            yield return null;
        }
    }

}
