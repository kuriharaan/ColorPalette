using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(ColorPalette))]
public class ColorPalleteInspector : Editor
{
    public override void OnInspectorGUI()
    {
        var palette = target as ColorPalette;

        for( int i = 0; i < palette.Colors.Length; ++i )
        {
            var oldColor = palette.Colors[i];
            var newColor = EditorGUILayout.ColorField(palette.Colors[i]);

            if( oldColor != newColor )
            {
                palette.UpdateMaterial(newColor, i);
            }
        }
    }
}
