using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEditorInternal;

namespace XPalette
{
    [CustomEditor(typeof(XColorPalette))]
    public class XColorPaletteEditor : Editor
    {
        private XColorPalette palette;
        private SerializedProperty colorSet;

        private ReorderableList reorderableList;

        private List<UnityEngine.Color> color;
        private List<AnimBool> showFade;

        private static float lineHeight = EditorGUIUtility.singleLineHeight;
        private static float lineSpace = lineHeight + 5;

        private void OnEnable()
        {
            palette = target as XColorPalette;
            colorSet = serializedObject.FindProperty(XColorPalette.ColorSet_Prop_Name);
            reorderableList = new ReorderableList(serializedObject, colorSet);
            reorderableList.draggable = false;

            reorderableList.drawHeaderCallback = DrawHeaderCallback;

            reorderableList.elementHeightCallback = ElementHeightCallback;
            reorderableList.drawElementCallback = DrawElementCallback;

            reorderableList.onAddCallback = OnAddCallback;
            reorderableList.onRemoveCallback = OnRemoveCallback;

            initiallizeAttribute();
        }


        private void DrawHeaderCallback(Rect rect)
        {
            GUI.Label(rect, "ColorSet", EditorStyles.boldLabel);
        }

        private float ElementHeightCallback(int index)
        {
            int num = 2;
            if(showFade.Count > 0)
            {
                if (!showFade[index].value)
                {
                    return lineSpace * num + 5;
                }
                else
                {
                    num = 6;
                    return lineSpace * num + 5;
                }
            }
            return lineSpace * num;
        }

        private void DrawElementCallback(Rect rect, int index, bool isActive, bool isFocused)
        {
            palette.ColorSet[index].ColorName = EditorGUI.TextField(new Rect(rect.x, rect.y + 5, rect.width, lineHeight), $"Color {index}", palette.ColorSet[index].ColorName);

            EditorGUI.BeginChangeCheck();
            color[index] = EditorGUI.ColorField(new Rect(rect.x, rect.y + lineSpace + 5, rect.width - 100, lineHeight), color[index]);
            if (EditorGUI.EndChangeCheck())
            {
                palette.ColorSet[index].r = color[index].r;
                palette.ColorSet[index].g = color[index].g;
                palette.ColorSet[index].b = color[index].b;
                palette.ColorSet[index].a = color[index].a;
            }

            showFade[index].target = EditorGUI.ToggleLeft(new Rect(rect.x + rect.width - 90, rect.y + lineSpace + 5, 100, lineHeight), "Show detail", showFade[index].target);
            if (EditorGUILayout.BeginFadeGroup(showFade[index].faded))
            {
                EditorGUI.BeginChangeCheck();
                palette.ColorSet[index].r = EditorGUI.Slider(new Rect(rect.x, rect.y + lineSpace * 2 + 5, rect.width, lineHeight), "R", palette.ColorSet[index].r, 0f, 1f);
                palette.ColorSet[index].g = EditorGUI.Slider(new Rect(rect.x, rect.y + lineSpace * 3 + 5, rect.width, lineHeight), "G", palette.ColorSet[index].g, 0f, 1f);
                palette.ColorSet[index].b = EditorGUI.Slider(new Rect(rect.x, rect.y + lineSpace * 4 + 5, rect.width, lineHeight), "B", palette.ColorSet[index].b, 0f, 1f);
                palette.ColorSet[index].a = EditorGUI.Slider(new Rect(rect.x, rect.y + lineSpace * 5 + 5, rect.width, lineHeight), "A", palette.ColorSet[index].a, 0f, 1f);
                if (EditorGUI.EndChangeCheck())
                {
                    color[index] = palette.ColorSet[index].ToColor();
                }
            }
            EditorGUILayout.EndFadeGroup();

        }

        private void OnAddCallback(ReorderableList list)
        {
            palette.ColorSet.Add(new XColor());
            colorSet = serializedObject.FindProperty(XColorPalette.ColorSet_Prop_Name);
            color.Add(palette.ColorSet[palette.ColorSet.Count - 1].ToColor());
            showFade.Add(new AnimBool(false));
        }

        private void OnRemoveCallback(ReorderableList list)
        {
            color.RemoveAt(list.index);
            showFade.RemoveAt(list.index);
            palette.ColorSet.RemoveAt(list.index);
            colorSet = serializedObject.FindProperty(XColorPalette.ColorSet_Prop_Name);
        }

        private void initiallizeAttribute()
        {
            showFade = new List<AnimBool>();
            color = new List<UnityEngine.Color>();
            int index = 0;
            foreach (XColor xcolor in palette.ColorSet)
            {
                color.Add(xcolor.ToColor());
                showFade.Add(new AnimBool(false));
                showFade[index].valueChanged.AddListener(Repaint);
                index++;
            }
        }


        public override void OnInspectorGUI()
        {
            //base.OnInspectorGUI();
            GUILayout.Space(5);
            palette.PaletteName = EditorGUILayout.TextField("XColorPalette Name", palette.PaletteName);
            GUILayout.Space(5);
            //EditorGUILayout.LabelField($"{reorderableList.index}");

            serializedObject.Update();

            reorderableList.DoLayoutList();

            serializedObject.ApplyModifiedProperties();
        }
    }
}