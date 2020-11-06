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
using System.Collections.Generic;
using System.IO;
using System.Text;
using Object = UnityEngine.Object;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SODB
{
    public static class SOTableGenerator
    {
        private static Type const_Type = typeof(SOTableBase<>);
        private static string const_strTablePostfix = "_Table";

        private static string const_strPrefix = @"
/*	============================================
 *	Author   			    : Strix
 *	Summary 		        : 
 *
 *  툴로 자동으로 생성되는 코드입니다.
 *  이 파일을 직접 수정하시면 나중에 툴로 생성할 때 날아갑니다.
   ============================================= */

using UnityEngine;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(menuName = ""SODB/{0}{1}"")]
public partial class {0}{1} : {2}
{
{3}

#region Editor
#if UNITY_EDITOR
    [CustomEditor(typeof({0}{1}))]
    public class {0}{1}_Drawer : Editor
    {
        private TextAsset pTextAsset;
        
        public override void OnInspectorGUI()
        {
            {0}{1} pTarget = target as {0}{1};

            pTextAsset = (TextAsset)EditorGUILayout.ObjectField(""Raw Data(.csv)"", pTextAsset, typeof(TextAsset), false);

        GUI.enabled = pTextAsset != null;
        {
            if (GUILayout.Button(""Insert Raw Data""))
            {
                pTarget.DoInsert(pTextAsset.text);
            }
        }
        GUI.enabled = true;

        if (GUILayout.Button(""Clear""))
        {
            // 실행취소 추가해야함
            pTarget.rows.Clear();
        }

        base.OnInspectorGUI();
    }
    }
#endif
#endregion
}
";

        /// <summary>
        /// Table과 TargetScript를 감싸는 Wrapper를 만듭니다.
        /// </summary>
        /// <param name="pTableConfig"></param>
        /// <param name="strAbsoluteFilePath"></param>
        public static void DoGenerate_CSFile(TableConfig pTableConfig, string strAbsoluteFilePath)
        {
            // string.Format이 안됨;
            //string strFileContent = string.Format(const_strPrefix, 
            //    nameof(CustomLogType),
            //    _strBuilder_Class.ToString());

            string strCached = "";

            string strFileContent = const_strPrefix
                .Replace("{0}", pTableConfig.strClassName)
                .Replace("{1}", const_strTablePostfix)
                .Replace("{2}", $"{const_Type.FullName.Replace("`1", "")}<{pTableConfig.strClassName}>")
                .Replace("{3}", strCached);


            string strPath = $"{strAbsoluteFilePath}{const_strTablePostfix}";
            try
            {
                File.WriteAllText(strPath, strFileContent, Encoding.UTF8);
            }
            catch (DirectoryNotFoundException e)
            {
                string strNewPath = $"{Application.dataPath}/{pTableConfig.strClassName}{const_strTablePostfix}.cs";
                File.WriteAllText(strNewPath, strFileContent, Encoding.UTF8);
                
                Object pGeneratedFile = AssetDatabase.LoadAssetAtPath(strNewPath.Replace(Application.dataPath, "Assets/"), typeof(MonoScript));
                Selection.activeObject = pGeneratedFile;

                Debug.LogWarning("해당 경로에 .cs 파일 생성을 실패하여 다른 경로에 저장했습니다..\n" +
                                 $"원래 경로 :{strPath}\n" +
                                 $"저장한 경로 : {strNewPath}", pGeneratedFile);
            }

#if UNITY_EDITOR
            AssetDatabase.Refresh();
#endif
        }
    }
}


