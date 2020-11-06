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
using System.Linq;
using System.Reflection;
using UnityEditor;

namespace SODB
{
    /// <summary>
    /// 
    /// </summary>
    public class SODB_Setting : ScriptableObject
    {
        private IReadOnlyDictionary<ELabelName, float> const_LabelSize = new Dictionary<ELabelName, float>
        {
            {ELabelName.Type, 150}, {ELabelName.Column, 150},
            {ELabelName.PK, 25}, {ELabelName.NN, 25}, {ELabelName.UQ, 25}
        };

        public static SODB_Setting instance
        {
            get
            {
                if (_instance == null)
                {
                    SODB_Setting[] arrSetting = Resources.LoadAll<SODB_Setting>("");

                    int iCurrentSettingCount = arrSetting.Count(p => p.bIsCurrent);
                    if (iCurrentSettingCount > 1)
                    {
                        Debug.LogWarning($"{nameof(SODB_Setting)} - iCurrentSettingCount({iCurrentSettingCount}) > 1");
                    }

                    _instance = arrSetting.FirstOrDefault(p => p.bIsCurrent);
                    if (_instance == null)
                    {
                        // 일단 현재 존재하는 것 중에 찾아서 넣습니다.
                        _instance = arrSetting.FirstOrDefault();
                        if (_instance == null)
                        {
                            if (Application.isEditor)
                            {
                                _instance = SODB_Utility.CreateAsset<SODB_Setting>();
                                Debug.Log($"{nameof(SODB_Setting)} is null / auto create default setting", _instance);
                            }
                            else
                            {
                                _instance = CreateInstance<SODB_Setting>();
                                Debug.LogWarning($"{nameof(SODB_Setting)} is null / auto create default setting, but is not file");
                            }
                        }

                        _instance.bIsCurrent = true;
                    }
                }

                return _instance;
            }
        }



        static SODB_Setting _instance;

        [SerializeField]
        List<TableConfig> _listTableConfig = new List<TableConfig>();

        [SerializeField]
        protected bool bIsCurrent = true;

        public string strLastEditScript_GUID { get; private set; }


        public void DoDrawTable(MonoScript pScript, float fScrollHeight, out TableConfig pTableConfig)
        {
            Record_LastScriptGUID(pScript);
            EditorGUILayout.LabelField("Columns");
            DrawHeader();

            Type pType = pScript.GetClass();
            pTableConfig = GetOrNew_TableConfig(pType);

            DrawFields(pTableConfig, pType.GetFields(BindingFlags.Public | BindingFlags.Instance), fScrollHeight);
            CleanUpTable(pTableConfig);
        }

        public TableConfig GetTableConfig(string strTypeName)
        {
            TableConfig pTableConfig = _listTableConfig.FirstOrDefault(p => p.strClassName == strTypeName);
            if (pTableConfig == null)
                pTableConfig = new TableConfig(strTypeName);

            return pTableConfig;
        }

        #region Private

        private void Record_LastScriptGUID(MonoScript pScript)
        {
            // AssetPath는 파일의 위치가 변경될 수 있기 때문에, GUID를 저장합니다.
            string strAssetPath = AssetDatabase.GetAssetPath(pScript);
            strLastEditScript_GUID = AssetDatabase.AssetPathToGUID(strAssetPath);
        }


        Vector2 _vecScroll_ForFields;
        private void DrawFields(TableConfig pTableConfig, FieldInfo[] arrMember, float fScrollHeight)
        {
            EditorGUILayout.BeginScrollView(_vecScroll_ForFields, 
                GUILayout.ExpandHeight(true), GUILayout.MinHeight(fScrollHeight));

            ColumnConfig pPKColumn = pTableConfig.listColumnConfig.FirstOrDefault(p => p.eTypeFlags == EColumnTypeFlags.PK);

            for (int i = 0; i < arrMember.Length; i++)
            {
                FieldInfo pFieldInfo = arrMember[i];
                var pColumnConfig = GetOrNew_ColumnConfig(pTableConfig, pFieldInfo);

                bool bIsPK = (pColumnConfig.eTypeFlags & EColumnTypeFlags.PK) != 0;
                // bool bIsNN = (pColumnConfig.eTypeFlags & EColumnTypeFlags.NN) != 0;
                // bool bIsUQ = (pColumnConfig.eTypeFlags & EColumnTypeFlags.UQ) != 0;

                EditorGUILayout.BeginHorizontal();
                {
                    EditorGUILayout.LabelField(pFieldInfo.FieldType.Name, GUILayout.MaxWidth(const_LabelSize[ELabelName.Type]));
                    EditorGUILayout.LabelField(pFieldInfo.Name, GUILayout.MaxWidth(const_LabelSize[ELabelName.Column]));

                    EColumnTypeFlags eColumnTypeFlags = EColumnTypeFlags.None;

                    bool bIgnoreSet_PK = pPKColumn != null && pPKColumn.strColumnName != pColumnConfig.strColumnName;
                    GUI.enabled = bIgnoreSet_PK == false;
                    {
                        if (EditorGUILayout.Toggle(bIsPK, GUILayout.MaxWidth(const_LabelSize[ELabelName.PK])))
                        {
                            if (bIgnoreSet_PK)
                            {
                                EditorUtility.DisplayDialog("Error", "this table already has PK", "ok");
                                continue;
                            }
                            eColumnTypeFlags |= EColumnTypeFlags.PK;
                        }
                    }
                    GUI.enabled = true;

                    // PK는 반드시 NN을 포함해야 합니다.
                    //if (EditorGUILayout.Toggle(bIsPK || bIsNN, GUILayout.MaxWidth(const_LabelSize[ELabelName.NN])))
                    //    eColumnTypeFlags |= EColumnTypeFlags.NN;

                    //if (EditorGUILayout.Toggle(bIsUQ, GUILayout.MaxWidth(const_LabelSize[ELabelName.UQ])))
                    //    eColumnTypeFlags |= EColumnTypeFlags.UQ;

                    pColumnConfig.eTypeFlags = eColumnTypeFlags;
                }
                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.Space();
            EditorGUILayout.EndScrollView();
        }

        private void CleanUpTable(TableConfig pTableConfig)
        {
            List<ColumnConfig> listColumnConfig = pTableConfig.listColumnConfig;
            listColumnConfig.RemoveAll(p => p.eTypeFlags == EColumnTypeFlags.None);
            
            if (listColumnConfig.Count == 0)
                _listTableConfig.Remove(pTableConfig);
        }

        private void DrawHeader()
        {
            GUIStyle pLabelStyle = new GUIStyle();
            pLabelStyle.normal.textColor = Color.green;

            EditorGUILayout.BeginHorizontal();
            {
                DrawHeaderLabel(ELabelName.Type, pLabelStyle);
                DrawHeaderLabel(ELabelName.Column, pLabelStyle);

                var arrNames = Enum.GetNames(typeof(EColumnTypeFlags));
                for (int i = 0; i < arrNames.Length; i++)
                {
                    string strLabelName = arrNames[i];
                    if (Enum.TryParse(strLabelName, out ELabelName eLabelName) == false)
                        continue;

                    DrawHeaderLabel(eLabelName, pLabelStyle);
                }
            }
            EditorGUILayout.EndHorizontal();
        }

        private void DrawHeaderLabel(ELabelName eLabelName, GUIStyle pLabelStyle)
        {
            EditorGUILayout.LabelField(eLabelName.ToString(), pLabelStyle, GUILayout.Width(const_LabelSize[eLabelName]));
        }

        private TableConfig GetOrNew_TableConfig(Type pDrawTarget)
        {
            TableConfig pTableConfig = _listTableConfig.FirstOrDefault(p => p.strClassName == pDrawTarget.Name);
            if (pTableConfig == null)
            {
                pTableConfig = new TableConfig(pDrawTarget.Name);
                _listTableConfig.Add(pTableConfig);
            }

            return pTableConfig;
        }

        private static ColumnConfig GetOrNew_ColumnConfig(TableConfig pTableConfig, FieldInfo pFieldInfo)
        {
            ColumnConfig pColumnConfig = pTableConfig.listColumnConfig.FirstOrDefault(p => p.strColumnName == pFieldInfo.Name);
            if (pColumnConfig == null)
            {
                pColumnConfig = new ColumnConfig(pFieldInfo.Name);
                pTableConfig.listColumnConfig.Add(pColumnConfig);
            }

            return pColumnConfig;
        }

        #endregion
    }
}