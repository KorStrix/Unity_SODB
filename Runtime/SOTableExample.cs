#region Header
/*	============================================
 *	Author 			      : Strix
 *	Initial Creation Date : 2020-11-02
 *	Summary 			  : 
 *  Template 		      : Visual Studio ItemTemplate For Unity V7
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using SODB;

#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// 
/// </summary>
[CreateAssetMenu(menuName = "SODB/SOTableExample")]
public partial class SOTableExample : SOTableBase<SOExample>
{
    protected override void OnInsert(Dictionary<string, ColumnConfig> mapColumnConfig, Dictionary<string, FieldInfo> mapFieldInfo, List<SOExample> listInsert, out bool bIsSuccess)
    {
        base.OnInsert(mapColumnConfig, mapFieldInfo, listInsert, out bIsSuccess);

        foreach (var pElement in listInsert)
        {
            foreach (var pColumnConfig in mapColumnConfig)
            {
                if (mapFieldInfo.TryGetValue(pColumnConfig.Key, out var pFieldInfo) == false)
                {
                    Debug.LogError("Error");
                    continue;
                }

                EColumnTypeFlags eTypeFlags = pColumnConfig.Value.eTypeFlags;

                bool bIsNotNull = (eTypeFlags & EColumnTypeFlags.NN) != 0;
                object pFieldValue = pFieldInfo.GetValue(pElement);
            }
        }
    }
}