using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioTestScript : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private Button button;
    [SerializeField] private Button button2;
    private AudioManager audioManager;
    void Start()
    {
        audioManager = AudioManager.instance;
        button.onClick.AddListener(OnButtonclick);
        button2.onClick.AddListener(OnButton2click);
    }

    void OnButtonclick()
    {
        audioManager.TutorialTalker("what is going on here?");
    }

    void OnButton2click() {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
