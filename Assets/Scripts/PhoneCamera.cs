using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class PhoneCamera : MonoBehaviour
{
    private bool camAvailable;
    private WebCamTexture backCamera;
    private Texture placeholder;

    [SerializeField] private RawImage background;
    [SerializeField] private AspectRatioFitter fit;




    // Start is called before the first frame update
    void Start()
    {
        placeholder = background.texture;
        WebCamDevice[] devices = WebCamTexture.devices;
        Debug.Log(devices.Length);

        if (devices.Length < 0)
        {
            Debug.Log(" NO camera detected");
            camAvailable = false;
            return;
            
        }

        for (int i = 0; i < devices.Length; i++)
        {
            Debug.Log(devices[i].name);
            if (!devices[i].isFrontFacing)
            {
                backCamera = new WebCamTexture(devices[i].name,Screen.width,Screen.height);
            }

        }

        if(backCamera == null)
        {
            Debug.Log("Unable to find back camera");
            return;
        }

        backCamera.Play();
        background.texture = backCamera;
        camAvailable = true;

    }

    // Update is called once per frame
    void Update()
    {
        if (!camAvailable) {
            return;
        }
        else
        {
            float ratio = (float)backCamera.width / backCamera.height;
            fit.aspectRatio = ratio;

            float scaleY = backCamera.videoVerticallyMirrored? -1f:1f;
            background.rectTransform.localScale = new Vector3(1f, scaleY, 1f);

            int orient = -backCamera.videoRotationAngle;
            background.rectTransform.localEulerAngles = new Vector3(0,0, orient);
        }
    }
}
