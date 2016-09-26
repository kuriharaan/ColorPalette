using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class ColorPaletteEditorWindow : EditorWindow
{
    [MenuItem("Window/ColorPalette")]
    static void Open()
    {
        EditorWindow.GetWindow<ColorPaletteEditorWindow>("ColorPalette");
    }

    void OnGUI()
    {
        if (GUILayout.Button("Create alpha texture"))
        {
            if( null != Selection.objects[0] )
            {
                CreateAlphaTexture(Selection.objects[0]);
            }
        }
    }

    Texture2D CreateAlphaTexture(Object obj)
    {
        var sourceAssetPath = AssetDatabase.GetAssetPath(obj);
        var filename = System.IO.Path.GetFileName(sourceAssetPath);
        var directoryPath = System.IO.Path.GetDirectoryName(sourceAssetPath) + "/Alpha";

        // Create alpha texture directory
        if (!System.IO.Directory.Exists(directoryPath))
        {
            System.IO.Directory.CreateDirectory(directoryPath);
        }

        // set source texture as readable
        var sourceImporter = TextureImporter.GetAtPath(sourceAssetPath) as TextureImporter;
        bool sourceReadableFlag = sourceImporter.isReadable;

        sourceImporter.textureType = TextureImporterType.Advanced;
        sourceImporter.npotScale = TextureImporterNPOTScale.None;
        sourceImporter.filterMode = FilterMode.Point;
        sourceImporter.spriteImportMode = SpriteImportMode.Single;

        TextureImporterSettings st = new TextureImporterSettings();
        sourceImporter.ReadTextureSettings(st);
        st.textureFormat = TextureImporterFormat.RGBA32;
        st.readable = true;
        sourceImporter.SetTextureSettings(st);

        AssetDatabase.ImportAsset(sourceAssetPath);


        var sourceTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(sourceAssetPath);
        var sourcePixels   = sourceTexture.GetPixels();

        // create texture pixels
        var tex2d = new Texture2D(sourceTexture.width, sourceTexture.height, TextureFormat.Alpha8, false);
        var pixels = tex2d.GetPixels();

        List<Color> colorTable = new List<Color>();
        colorTable.Add(new Color(0.0f, 0.0f, 0.0f, 0.0f));

        float minimumGap     = 1.0f / 256.0f;
        float minimumGapHalf = minimumGap * 0.5f;
        for (int iy = 0; iy < sourceTexture.height; ++iy )
        {
            for( int ix = 0; ix < sourceTexture.width; ++ix )
            {
                int index = iy * sourceTexture.width + ix;

                var color = sourcePixels[index];
                if (0 == color.a)
                {
                    pixels[index].a = 0;
                }
                else
                {
                    if( colorTable.Contains(color) )
                    {
                        pixels[index].a = (colorTable.IndexOf(color) + minimumGapHalf) * minimumGap;
                    }
                    else
                    {
                        colorTable.Add(color);
                        pixels[index].a = ((colorTable.Count - 1) + minimumGapHalf) * minimumGap;
                    }
                }

                //pixels[index].a = sourcePixels[index].a;
            }
        }

        // for log
        for (int i = 0; i < colorTable.Count; ++i )
        {
            var c = colorTable[i];
            Debug.Log(c.ToString());
        }

        tex2d.SetPixels(pixels);

        // write pixels & import texture asset
        var outputPath = AssetDatabase.GenerateUniqueAssetPath(directoryPath + "/" + filename);
        System.IO.File.WriteAllBytes(outputPath, tex2d.EncodeToPNG());
        DestroyImmediate(tex2d);

        var option = ImportAssetOptions.ForceSynchronousImport | ImportAssetOptions.ForceUncompressedImport;
        AssetDatabase.ImportAsset(outputPath, option);

        // revert source texture readable setting
        sourceImporter.isReadable = sourceReadableFlag;
        AssetDatabase.ImportAsset(sourceAssetPath);

        // texture importer settings
        var importer = TextureImporter.GetAtPath(outputPath) as TextureImporter;
        importer.isReadable    = false;
        importer.mipmapEnabled = false;
        importer.filterMode    = FilterMode.Point;
        importer.textureFormat = TextureImporterFormat.Alpha8;
        AssetDatabase.ImportAsset(outputPath);

        return AssetDatabase.LoadAssetAtPath<Texture2D>(outputPath);
    }
}
