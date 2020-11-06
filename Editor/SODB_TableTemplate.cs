/*	============================================
 *	Author   			    : Strix
 *  Generated Time          : ${DATE}
 *	Summary 		        : 
 *
 *  툴로 자동으로 생성되는 코드입니다.
 *  Tool 위치 : Tools/Strix/SODB Editor
 *
 *  이 파일을 직접 수정하시면 나중에 툴로 생성할 때 날아갑니다.
   ============================================= */

using UnityEngine;
using System.Collections.Generic;
using SODB;

#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// 
/// </summary>
[CreateAssetMenu(menuName = "SODB/" + nameof(SODB_TableTemplate))]
public partial class SODB_TableTemplate : TableBase<SOExample>
{
    public static SODB_TableTemplate Create(string strCSV)
    {
        SODB_TableTemplate pTable = CreateInstance<SODB_TableTemplate>();
        pTable.DoInsert(strCSV);

        return pTable;
    }

//Field

    protected override void OnInsert(List<SOExample> listInsert)
    {
        base.OnInsert(listInsert);
        
//Insert
    }

    protected override void OnClear()
    {
        base.OnClear();

//Clear
    }
}



#if UNITY_EDITOR

[CustomEditor(typeof(SODB_TableTemplate))]
public class SODB_TableTemplate_Drawer : SOTableBase_Drawer<SOExample>
{
}

#endif