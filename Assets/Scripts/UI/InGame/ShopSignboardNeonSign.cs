using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static GameUIManager;

public class ShopSignboardNeonSign : MonoBehaviour
{
    [SerializeField] public bool on = true;
    [SerializeField] private Image image;

    private void Start()
    {
        StartCoroutine(OnOffCoroutine());
    }

    private IEnumerator OnOffCoroutine()
    {
        while (true)
        {
            if (on == true)
            {
                on = false;
                image.color = new Color(0.8f, 0.8f, 0.8f, 1.0f);
            }
            else if (on == false)
            {
                on = true;
                image.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
            }

            yield return new WaitForSeconds(0.4f);
        }
    }
}
