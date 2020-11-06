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
using UnityEditor;
#endif

namespace SODB
{
    /// <summary>
    /// 
    /// </summary>
    public class SOTableBase<T> : ScriptableObject, IReadOnlyList<T>
        where T : new()
    {
        /* const & readonly declaration             */

        /* enum & struct declaration                */

        /* public - Field declaration            	*/

        [SerializeField]
        private List<T> rows = new List<T>();

        /* protected & private - Field declaration  */

        // ========================================================================== //

        /* public - [Do~Something] Function 	        */

        public void DoInsert(string strCSV)
        {
            Type pType = typeof(T);
            TableConfig pTableConfig = SODB_Setting.instance.GetTableConfig(pType.Name);

            Dictionary<string, ColumnConfig> mapColumnConfig = pTableConfig.listColumnConfig.ToDictionary(p => p.strColumnName);
            Dictionary<string, FieldInfo> mapFieldInfo = pType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).ToDictionary(p => p.Name);
            mapColumnConfig.RemoveAll((Key, Value) => Value.eTypeFlags == EColumnTypeFlags.None);
            mapFieldInfo.RemoveAll((Key, Value) => mapColumnConfig.ContainsKey(Key));


            List<T> listInsert = CSVUtility.FromCSVText_List<T>(strCSV, Debug.LogError);
            
            OnInsert(mapColumnConfig, mapFieldInfo, listInsert, out bool bIsSuccess);
            if (bIsSuccess)
            {
                rows.AddRange(listInsert);
            }

            Debug.Log($"Insert {strCSV}");
        }

        public void DoClear()
        {
            rows.Clear();
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

        protected virtual void OnInsert(Dictionary<string, ColumnConfig> mapColumnConfig, Dictionary<string, FieldInfo> mapFieldInfo, List<T> listInsert, out bool bIsSuccess)
        {
            bIsSuccess = true;
        }

        // ========================================================================== //

        #region Private

        #endregion Private
    }

#if UNITY_EDITOR

    [CustomEditor(typeof(SOTableExample))]
    public class SOTableExample_Drawer : Editor
    {
        private TextAsset pTextAsset;
        
        public override void OnInspectorGUI()
        {
            SOTableExample pTarget = target as SOTableExample;

            pTextAsset = (TextAsset)EditorGUILayout.ObjectField("Raw Data(.csv)", pTextAsset, typeof(TextAsset), false);

            GUI.enabled = pTextAsset != null;
            {
                if (GUILayout.Button("Insert Raw Data"))
                {
                    pTarget.DoInsert(pTextAsset.text);
                }
            }
            GUI.enabled = true;

            if (GUILayout.Button("Clear"))
            {
                // 실행취소 추가해야함
                pTarget.DoClear();
            }

            base.OnInspectorGUI();
        }
    }

#endif
}