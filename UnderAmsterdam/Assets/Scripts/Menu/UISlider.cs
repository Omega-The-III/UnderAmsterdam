using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UISlider : MonoBehaviour
{
    [SerializeField] public GameObject handle;
    [SerializeField] GameObject backgroundBase;
    [SerializeField] Image fillBase;
    [SerializeField] float minXPos = -0.4832999f;
    private Vector3 minPosition;
    private Vector3 maxPosition;
    private float minValueForMixer = 0.0001f;
    private bool touched;

    [SerializeField] private SettingsUI volumeMixer;
    [Tooltip("Find all volumeType's in the SettingsUI audiomixer in Sounds folder")]
    [SerializeField] private string volumeType;
    [Tooltip("Is this slider used for volume?")]
    [SerializeField] private bool volumeSlider = true;
    
    // Start is called before the first frame update
    void Start()
    {
        minPosition = new Vector3(minXPos, 0.961f, 0);
        maxPosition = new Vector3(-minXPos, 0.961f, 0);
        handle.transform.localPosition = maxPosition;
        handlePosition(handle.transform.localPosition.x);
    }

    private void Update()
    {
        if(touched)
        {
            handlePosition(handle.transform.localPosition.x);
            if (handle.transform.localPosition.x < minPosition.x)
            {
                handle.transform.localPosition = minPosition;
            }
            if (handle.transform.localPosition.x > maxPosition.x)
            {
                handle.transform.localPosition = maxPosition;
            }
        }
    }
    public void handlePosition(float localPositionX)
    {
        float volume = minValueForMixer + Mathf.InverseLerp(minPosition.x, maxPosition.x, localPositionX);
        fillBase.fillAmount = volume;
        if (volumeSlider && volumeType != "")
        volumeMixer.SetVolume(volumeType, volume);
        if (volumeType == "Master")
            volumeMixer.MasterVolume();
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer == 8)
        {
            touched = true;
            if (other.transform.position.x > handle.transform.position.x)
                handle.transform.localPosition += new Vector3(0.01f, 0, 0);
            else
                handle.transform.localPosition -= new Vector3(0.01f, 0, 0);
            //handle.transform.position = new Vector3(other.transform.position.x, handle.transform.position.y, handle.transform.position.z);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.gameObject.layer == 8)
        {
            touched = false;
        }
    }
}
