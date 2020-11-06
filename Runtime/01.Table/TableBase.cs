#region Header
/*	============================================
 *	Author 			      : Strix
 *	Initial Creation Date : 2020-11-02
 *	Summary 			  : 
 *  Template 		      : Visual Studio ItemTemplate For Unity V7
   ============================================ */
#endregion Header

using System;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

#if UNITY_EDITOR
using SODB;
using UnityEditor;
#endif

namespace SODB
{
    /// <summary>
    /// 
    /// </summary>
    public class TableBase<T> : ScriptableObject, IReadOnlyList<T>
    {
        /* const & readonly declaration             */

        /* enum & struct declaration                */

        public struct ColumnInfo
        {
            public string strColumnName { get; private set; }
            public ColumnConfig pConfig { get; private set; }
            public FieldInfo pFieldInfo { get; private set; }

            public ColumnInfo(string strColumnName, ColumnConfig pConfig, FieldInfo pFieldInfo)
            {
                this.strColumnName = strColumnName;
                this.pConfig = pConfig;
                this.pFieldInfo = pFieldInfo;
            }
        }

        /* public - Field declaration            	*/

        [SerializeField]
        private List<T> rows = new List<T>();

        [SerializeField]
        private List<TableValidCheckerBase<T>> _listValidChecker = new List<TableValidCheckerBase<T>>();

        /* protected & private - Field declaration  */

        // ========================================================================== //

        /* public - [Do~Something] Function 	        */

        public void DoInsert(string strCSV)
        {
            Type pType = typeof(T);
            TableConfig pTableConfig = SODB_Setting.instance.GetTableConfig(pType.Name);
            Dictionary<string, ColumnInfo> mapColumnInfo = Generate_ColumnInfo(pTableConfig, pType);
            List<T> listInsert = CSVUtility.FromCSVText_List<T>(strCSV, Debug.LogError);

            ProcessInsert(mapColumnInfo, listInsert, out bool bIsSuccess);
            if (bIsSuccess)
            {
                OnInsert(listInsert);
                rows.AddRange(listInsert);
            }

            Debug.Log($"ProcessInsert {strCSV}");
        }

        public void DoClear()
        {
            rows.Clear();

            OnClear();
        }

        // ========================================================================== //

        /* protected - [Override & Unity API]       */

        #region IReadOnlyList

        public IEnumerator<T> GetEnumerator() => rows.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => rows.GetEnumerator();
        public int Count => rows.Count;
        public T this[int index] => rows[index];

        #endregion

        /* protected - [abstract & virtual]         */

        protected virtual void OnInsert(List<T> listInsert)
        {
        }

        protected virtual void OnClear()
        {

        }

        // ========================================================================== //

        #region Private

        protected void ProcessInsert(Dictionary<string, ColumnInfo> mapColumnInfo, List<T> listInsert, out bool bIsSuccess)
        {
            bIsSuccess = true;

            foreach (var pChecker in _listValidChecker)
            {
                pChecker.OnInsert(mapColumnInfo, listInsert, out string strErrorMsg);
                if (string.IsNullOrEmpty(strErrorMsg) == false)
                {
                    Debug.LogError($"{name} ProcessInsert Error - {strErrorMsg}");
                    break;
                }
            }
        }

        private static Dictionary<string, ColumnInfo> Generate_ColumnInfo(TableConfig pTableConfig, Type pType)
        {
            Dictionary<string, ColumnConfig> mapColumnConfig = pTableConfig.listColumnConfig.ToDictionary(p => p.strColumnName);
            Dictionary<string, FieldInfo> mapFieldInfo = pType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).ToDictionary(p => p.Name);
            Dictionary<string, ColumnInfo> mapColumnInfo = mapFieldInfo
                .Where(p => mapColumnConfig.ContainsKey(p.Key))
                .Select(p => new ColumnInfo(p.Key, mapColumnConfig[p.Key], p.Value))
                .ToDictionary(p => p.strColumnName);
            return mapColumnInfo;
        }

        #endregion Private
    }
}

#region Editor
#if UNITY_EDITOR
public class SOTableBase_Drawer<T> : Editor
{
    private TextAsset pTextAsset;

    public override void OnInspectorGUI()
    {
        TableBase<T> pTarget = target as TableBase<T>;
        if (pTarget == null) return; // For Disable ReSharper

        pTextAsset = (TextAsset)EditorGUILayout.ObjectField("Raw Data(.csv)", pTextAsset, typeof(TextAsset), false);

        GUI.enabled = pTextAsset != null;
        {
            if (GUILayout.Button("ProcessInsert Raw Data"))
            {
                pTarget.DoInsert(pTextAsset?.text);
            }
        }
        GUI.enabled = true;

        if (GUILayout.Button("Clear"))
        {
            // ������� �߰��ؾ���
            pTarget.DoClear();
        }

        base.OnInspectorGUI();
    }
}
#endif
#endregion