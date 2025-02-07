using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioTestScript : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private Button button;
    [SerializeField] private Button button2;
    [SerializeField] private Button button3;
    private AudioManager audioManager;
    void Start()
    {
        audioManager = AudioManager.instance;
        button.onClick.AddListener(OnButtonclick);
        button2.onClick.AddListener(OnButton2click);
        button3.onClick.AddListener(OnButton3click);
    }

    void OnButtonclick()
    {
        audioManager.SFXThrowItem();
    }

    void OnButton2click() {
        audioManager.SFXGameOver();
    }

    void OnButton3click() {
        audioManager.BeginBackgroundMusic();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
