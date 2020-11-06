#region Header

/*	============================================
 *	Author 			    	: strix
 *	Initial Creation Date 	: 2020-11-06
 *	Summary 		        : 
 *  Template 		        : New Behaviour For ReSharper
   ============================================ */

#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SODB;

/// <summary>
/// 
/// </summary>
public class TableValidChecker_PK<T, TTargetFieldType> : TableValidCheckerBase<T>
{
    public override void OnInsert(Dictionary<string, TableBase<T>.ColumnInfo> mapColumnInfo, List<T> listInsert, out string strErrorMsg)
    {
        throw new System.NotImplementedException();
    }
}