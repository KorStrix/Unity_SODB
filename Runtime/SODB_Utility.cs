#region Header

/*	============================================
 *	Author 			    	: strix
 *	Initial Creation Date 	: 2020-11-03
 *	Summary 		        :
 *  Template 		        : New Behaviour For ReSharper
   ============================================ */

#endregion Header

using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;

namespace SODB
{
    /// <summary>
    /// 
    /// </summary>
    public static class SODB_Utility
    {
        public static T CreateAsset<T>() where T : ScriptableObject
        {
            T pAsset = ScriptableObject.CreateInstance<T>();

#if UNITY_EDITOR
            const string strCreateAssetPath = "Resources";

            string strAbsoluteDirectory = Application.dataPath + $"/{strCreateAssetPath}";
            if (Directory.Exists(strAbsoluteDirectory) == false)
                Directory.CreateDirectory(strAbsoluteDirectory);

            string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath($"Assets/{strCreateAssetPath}/New {typeof(T)}.asset");

            AssetDatabase.CreateAsset(pAsset, assetPathAndName);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorUtility.FocusProjectWindow();

            pAsset = (T)AssetDatabase.LoadAssetAtPath(assetPathAndName, typeof(T));
            Selection.activeObject = pAsset;
#endif

            return pAsset;
        }

        public static int RemoveAll<TKey, TValue>(this Dictionary<TKey, TValue> mapTarget, System.Func<TKey, TValue, bool> Check_IsRemoveItem)
        {
            KeyValuePair<TKey, TValue>[] arrRemoveList = mapTarget.Where(p => Check_IsRemoveItem(p.Key, p.Value)).ToArray();
            foreach (var pItem in arrRemoveList)
                mapTarget.Remove(pItem.Key);

            return arrRemoveList.Length;
        }
    }
}