using UnityEngine;
using UnityEditor;

public class BorderTextureGenerator : EditorWindow
{
    private Texture2D sourceTexture;
    private string savePath = "Assets/Resources/BorderTexture.png";
    
    [MenuItem("Tools/Border Texture Generator")]
    public static void ShowWindow()
    {
        GetWindow<BorderTextureGenerator>("Border Texture Generator");
    }
    
    private void OnGUI()
    {
        GUILayout.Label("Border Texture Generator", EditorStyles.boldLabel);
        
        sourceTexture = (Texture2D)EditorGUILayout.ObjectField(
            "Source Texture", 
            sourceTexture, 
            typeof(Texture2D), 
            false
        );
        
        savePath = EditorGUILayout.TextField("Save Path", savePath);
        
        if (GUILayout.Button("Generate") && sourceTexture != null)
        {
            string sourcePath = AssetDatabase.GetAssetPath(sourceTexture);
            GenerateBorderTexture(sourcePath);
        }
    }
    
    private void GenerateBorderTexture(string sourcePath)
    {
        // 원본 텍스처의 임포터 설정 가져오기
        TextureImporter importer = AssetImporter.GetAtPath(sourcePath) as TextureImporter;
        if (importer == null) return;

        // 원본 설정 백업
        bool originalReadable = importer.isReadable;
        TextureImporterCompression originalCompression = importer.textureCompression;

        try
        {
            // 텍스처를 읽기 가능하게 설정
            importer.isReadable = true;
            importer.textureCompression = TextureImporterCompression.Uncompressed;
            importer.SaveAndReimport();

            // 텍스처 다시 로드
            Texture2D originalTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(sourcePath);
            int width = originalTexture.width;
            int height = originalTexture.height;

            // 새 텍스처 생성
            Texture2D newTexture = new Texture2D(width, height, TextureFormat.RGBA32, false);
            
            // 중심점 계산
            Vector2 center = new Vector2(width * 0.5f, height * 0.5f);
            float maxDistance = Vector2.Distance(Vector2.zero, center);
            
            // 픽셀 데이터 가져오기
            Color[] pixels = originalTexture.GetPixels();
            
            // 각 픽셀의 알파값을 중심으로부터의 거리에 따라 조절
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int index = y * width + x;
                    Color originalColor = pixels[index];
                    
                    if (originalColor.a > 0)
                    {
                        // 중심으로부터의 거리 계산
                        float distance = Vector2.Distance(new Vector2(x, y), center);
                        float normalizedDistance = distance / maxDistance;
                        
                        // 알파값 조절 (중심에서 멀어질수록 더 불투명하게)
                        float newAlpha = Mathf.Lerp(0f, originalColor.a, normalizedDistance);
                        pixels[index] = new Color(1, 1, 1, newAlpha);
                    }
                    else
                    {
                        pixels[index] = Color.clear;
                    }
                }
            }
            
            newTexture.SetPixels(pixels);
            newTexture.Apply();
            
            // PNG로 저장
            byte[] bytes = newTexture.EncodeToPNG();
            System.IO.File.WriteAllBytes(savePath, bytes);
            
            // 새 텍스처 임포트 설정
            AssetDatabase.ImportAsset(savePath);
            TextureImporter newImporter = AssetImporter.GetAtPath(savePath) as TextureImporter;
            if (newImporter != null)
            {
                newImporter.textureType = TextureImporterType.Sprite;
                newImporter.alphaIsTransparency = true;
                newImporter.filterMode = FilterMode.Bilinear;
                newImporter.textureCompression = TextureImporterCompression.Compressed;
                newImporter.SaveAndReimport();
            }
            
            // 메모리 정리
            DestroyImmediate(newTexture);
            
            Debug.Log("Border texture saved to: " + savePath);
        }
        finally
        {
            // 원본 텍스처 설정 복구
            importer.isReadable = originalReadable;
            importer.textureCompression = originalCompression;
            importer.SaveAndReimport();
        }
        
        AssetDatabase.Refresh();
    }
}