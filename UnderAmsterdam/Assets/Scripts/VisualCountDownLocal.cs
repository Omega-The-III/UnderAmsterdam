using UnityEngine;
using TMPro;
using System.Collections;

public class VisualCountDownLocal : MonoBehaviour
{
    private AudioSource audioSource;

    [SerializeField] private AudioClip countDown, countDownStart;
    [SerializeField] private TextMeshProUGUI counter;
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        Gamemanager.Instance.CountDownStart.AddListener(delegate { StartCoroutine(StartLocalCountDown()); } );
    }

    private IEnumerator StartLocalCountDown()
    {
        counter.gameObject.SetActive(true);
        counter.text = "3";
        audioSource.PlayOneShot(countDown);
        yield return new WaitForSeconds(1);
        counter.text = "2";
        audioSource.PlayOneShot(countDown);
        yield return new WaitForSeconds(1);
        counter.text = "1";
        audioSource.PlayOneShot(countDown);
        yield return new WaitForSeconds(1);
        counter.text = "Start!";
        audioSource.PlayOneShot(countDownStart);
        yield return new WaitForSeconds(1);
        counter.gameObject.SetActive(false);
    }
}
