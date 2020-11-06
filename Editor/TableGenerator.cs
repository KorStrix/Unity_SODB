#region Header

/*	============================================
 *	Author 			    	: strix
 *	Initial Creation Date 	: 2020-11-03
 *	Summary 		        :
 *  Template 		        : New Behaviour For ReSharper
   ============================================ */

#endregion Header

using System;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SODB
{
    public static class TableGenerator
    {
        public enum EditCodeWhere
        {
            Field,
            Insert,
            Clear,
        }

        public class CodeGenerator
        {
            public string strReplace { get; private set; }
            public StringBuilder pBuilder { get; private set; } = new StringBuilder();

            public CodeGenerator(EditCodeWhere eCodeWhere)
            {
                this.strReplace = "//" + eCodeWhere.ToString();
            }
        }

        private static Dictionary<EditCodeWhere, CodeGenerator> mapCodeGenerator = new Dictionary<EditCodeWhere, CodeGenerator>()
        {
            { EditCodeWhere.Field, new CodeGenerator(EditCodeWhere.Field) },
            { EditCodeWhere.Insert, new CodeGenerator(EditCodeWhere.Insert) },
            { EditCodeWhere.Clear, new CodeGenerator(EditCodeWhere.Clear) }
        };

        private static string const_strTablePostfix = "_Table";
        private static string const_strDateTime = "${DATE}";

        private static string const_strTabOnce = "    ";
        private static string const_strTabTwo = "        ";

        /// <summary>
        /// Table과 TargetScript를 감싸는 Wrapper를 만듭니다.
        /// </summary>
        /// <param name="pTableConfig"></param>
        /// <param name="strAbsoluteFilePath"></param>
        public static void DoGenerate_CSFile(MonoScript pScriptTarget, TableConfig pTableConfig, string strAbsoluteFilePath)
        {
            // string.Format이 안됨;
            //string strFileContent = string.Format(const_strPrefix, 
            //    nameof(CustomLogType),
            //    _strBuilder_Class.ToString());

            string strPath = $"{strAbsoluteFilePath}{const_strTablePostfix}.cs";
            string[] arrAlreadyExistGUID = AssetDatabase.FindAssets($"t:Script {pTableConfig.strClassName}{const_strTablePostfix}");
            if (arrAlreadyExistGUID.Length != 0)
            {
                strPath = $"{Application.dataPath.Replace("/Assets", "")}/" +
                          $"{(AssetDatabase.GUIDToAssetPath(arrAlreadyExistGUID.First()))}";
            }

            var strFileContent = Generate_CSFileContents(pScriptTarget, pTableConfig);
            try
            {
                File.WriteAllText(strPath, strFileContent, Encoding.UTF8);
            }
            catch (DirectoryNotFoundException e)
            {
                string strNewPath = $"{Application.dataPath}/{pTableConfig.strClassName}{const_strTablePostfix}.cs";
                File.WriteAllText(strNewPath, strFileContent, Encoding.UTF8);

                Debug.LogWarning("해당 경로에 .cs 파일 생성을 실패하여 다른 경로에 저장했습니다..\n" +
                                 $"원래 경로 :{strPath}\n" +
                                 $"저장한 경로 : {strNewPath}");
                strPath = strNewPath;
            }

#if UNITY_EDITOR
            AssetDatabase.Refresh();
            Selection.activeObject = AssetDatabase.LoadAssetAtPath(strPath.Replace(Application.dataPath, "Assets"), typeof(MonoScript));
#endif
        }

        private static string Generate_CSFileContents(MonoScript pScriptTarget, TableConfig pTableConfig)
        {
            Type pTypeClass = pScriptTarget.GetClass();
            Dictionary<string, FieldInfo> mapFields = pTypeClass.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).ToDictionary(p => p.Name);

            foreach (var pCodeGenerator in mapCodeGenerator.Values)
                pCodeGenerator.pBuilder.Clear();

            foreach (var pColumnConfig in pTableConfig.listColumnConfig)
            {
                if (pColumnConfig.eTypeFlags.ContainEnumFlag(EColumnTypeFlags.PK))
                {
                    if (mapFields.TryGetValue(pColumnConfig.strColumnName, out FieldInfo pFieldInfo) == false)
                    {
                        Debug.LogError("Error");
                        continue;
                    }

                    string strFieldName = "Dictionary_PK";
                    WriteMember_Dictionary(mapCodeGenerator[EditCodeWhere.Field].pBuilder, pFieldInfo, pTypeClass, strFieldName);
                    mapCodeGenerator[EditCodeWhere.Insert].pBuilder.AppendLine($"{const_strTabTwo}listInsert.ForEach(p => _{strFieldName}.Add(p.{pFieldInfo.Name}, p));");
                    mapCodeGenerator[EditCodeWhere.Clear].pBuilder.AppendLine($"{const_strTabTwo}_{strFieldName}.Clear();");
                }
            }


            MonoScript pMonoScript = MonoScript.FromScriptableObject(ScriptableObject.CreateInstance<SODB_TableTemplate>());
            string strFileContent = pMonoScript.text
                .Replace($"{nameof(SODB_TableTemplate)}", $"{pTableConfig.strClassName}{const_strTablePostfix}")
                .Replace($"{nameof(SOExample)}", $"{pTableConfig.strClassName}")
                .Replace(const_strDateTime, DateTime.Now.ToString("yy-MM-dd HH:mm:ss"));

            foreach (var pCodeGenerator in mapCodeGenerator.Values)
                strFileContent = strFileContent.Replace(pCodeGenerator.strReplace, pCodeGenerator.pBuilder.ToString());

            return strFileContent;
        }

        private static void WriteMember_Dictionary(StringBuilder strCodeBuilder_Field, FieldInfo pFieldInfo, Type pTypeClass, string strFieldName)
        {
            string strGenericArg = $"<{pFieldInfo.FieldType.FullName}, {pTypeClass.Name}>";

            strCodeBuilder_Field.AppendLine($"{const_strTabOnce}public IReadOnlyDictionary{strGenericArg} {strFieldName} => _{strFieldName};");
            strCodeBuilder_Field.AppendLine($"{const_strTabOnce}SerializableDictionary{strGenericArg} _{strFieldName} = new SerializableDictionary{strGenericArg}();");
        }
    }
}


