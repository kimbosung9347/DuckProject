using TMPro;
using UnityEngine;
using System.Collections;

public class DuckSpeechBubble : MonoBehaviour
{ 
    [SerializeField] private GameObject speechObject;
    [SerializeField] private TextMeshProUGUI speechText;
    [SerializeField] float typeSpeed = 0.1f;
    [SerializeField] float autoDeleteDelay = 0.5f;

    private Coroutine coSpeech;
    private string speech;
    private bool isAutoDelete = false;
     
    private void Start()
    {
        speechText.text = "";
        speechObject.SetActive(false);
    } 

    public void Active(string _speech)
    {
        isAutoDelete = false;
        StartSpeech(_speech);
    }

    public void ActiveAutoDeleteSpeech(string _speech)
    {
        isAutoDelete = true;
        StartSpeech(_speech);
    }

    void StartSpeech(string _speech)
    {
        if (speech == _speech)
            return;
         
        speechObject.SetActive(true);
         
        // 이미 출력 중이라면 전부 초기화
        if (coSpeech != null)
        {
            StopCoroutine(coSpeech);
            speechText.text = ""; 
        }

        speech = _speech;

        // 다시 시작
        coSpeech = StartCoroutine(CoSpeech());
    }

    IEnumerator CoSpeech()
    {
        speechText.text = "";

        for (int i = 0; i < speech.Length; i++)
        {
            speechText.text += speech[i];
            yield return new WaitForSeconds(typeSpeed);
        }
          
        if (isAutoDelete)
        {
            yield return new WaitForSeconds(autoDeleteDelay);
            speechText.text = "";
            speech = "";
            speechObject.SetActive(false);
        } 
    }

    private void OnDisable()
    {
        if (coSpeech != null)
            StopCoroutine(coSpeech);
         
        speechText.text = "";
        speechObject.SetActive(false);
    }
} 
