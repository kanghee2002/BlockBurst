using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;

public class FontChanger : EditorWindow
{
    private Font newFont;
    private TMP_FontAsset newTMPFont;

    [MenuItem("Tools/Change All Fonts")]
    static void Init()
    {
        FontChanger window = (FontChanger)EditorWindow.GetWindow(typeof(FontChanger));
        window.Show();
    }

    void OnGUI()
    {
        newFont = (Font)EditorGUILayout.ObjectField("New Font (UI.Text)", newFont, typeof(Font), false);
        newTMPFont = (TMP_FontAsset)EditorGUILayout.ObjectField("New Font (TextMeshPro)", newTMPFont, typeof(TMP_FontAsset), false);

        if(GUILayout.Button("Change All Fonts in Scene"))
        {
            ChangeAllFonts();
        }
    }

    void ChangeAllFonts()
    {
        // Legacy UI.Text 컴포넌트 변경
        if(newFont != null)
        {
            Text[] texts = GameObject.FindObjectsOfType<Text>();
            foreach(Text text in texts)
            {
                Undo.RecordObject(text, "Font Change");
                text.font = newFont;
            }
        }

        // TextMeshPro 컴포넌트 변경
        if(newTMPFont != null)
        {
            TextMeshProUGUI[] tmpTexts = GameObject.FindObjectsOfType<TextMeshProUGUI>();
            foreach(TextMeshProUGUI tmp in tmpTexts)
            {
                Undo.RecordObject(tmp, "Font Change");
                tmp.font = newTMPFont;
            }
        }
    }
}