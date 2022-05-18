using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Linq;
using UnityEditor.Experimental.GraphView;

namespace XEditor.CustomSearchWindow
{
    public class ObjectSearchProvider : ScriptableObject, ISearchWindowProvider
    {
        Type assetType;
        public SerializedProperty serializedProperty;
        public ObjectSearchProvider(Type assetType, SerializedProperty serializedProperty)
        {
            this.assetType = assetType;
            this.serializedProperty = serializedProperty;
        }

        public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
        {
            List<SearchTreeEntry> list = new List<SearchTreeEntry>();

            string[] assetGuids = AssetDatabase.FindAssets($"t:{assetType.Name}");
            List<string> paths = new List<string>();
            foreach (string assetGuid in assetGuids)
            {
                paths.Add(AssetDatabase.GUIDToAssetPath(assetGuid));
            }
            paths.Sort((a, b) =>
            {
                string[] splits1 = a.Split('/');
                string[] splits2 = b.Split('/');
                for (int i = 0; i < splits1.Length; i++)
                {
                    if (i >= splits2.Length)
                    {
                        return 1;
                    }
                    int value = splits1[i].CompareTo(splits2[i]);
                    if (value != 0)
                    {
                        if (splits1.Length != splits2.Length && (i == splits1.Length - 1 || i == splits2.Length - 1))
                        {
                            return splits1.Length < splits2.Length ? 1 : -1;
                        }
                        return value;
                    }
                }
                return 0;
            });

            List<string> groups = new List<string>();
            foreach(string item in paths)
            {
                string[] entryTitle = item.Split('/');
                string groupName = "";
                for (int i = 0; i < entryTitle.Length - 1; i++)
                {
                    groupName += entryTitle[i];
                    if (!groups.Contains(groupName))
                    {
                        list.Add(new SearchTreeGroupEntry(new GUIContent(entryTitle[i]), i + 1));
                        groups.Add(groupName);
                    }
                    groupName += "/";
                }

                UnityEngine.Object obj = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(item);
                SearchTreeEntry entry = new SearchTreeEntry(new GUIContent(entryTitle.Last(), EditorGUIUtility.ObjectContent(obj, obj.GetType()).image));
                entry.level = entryTitle.Length;
                entry.userData = obj;
                list.Add(entry);
            }

            return list;
        }

        public bool OnSelectEntry(SearchTreeEntry SearchTreeEntry, SearchWindowContext context)
        {
            serializedProperty.objectReferenceValue = (UnityEngine.Object)SearchTreeEntry.userData;
            serializedProperty.serializedObject.ApplyModifiedProperties();
            return true;
        }
    }

}