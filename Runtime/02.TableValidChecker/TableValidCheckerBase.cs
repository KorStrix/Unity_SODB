#region Header

/*	============================================
 *	Author 			    	: strix
 *	Initial Creation Date 	: 2020-11-06
 *	Summary 		        : 
 *  Template 		        : New Behaviour For ReSharper
   ============================================ */

#endregion Header

using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SODB;

/// <summary>
/// 
/// </summary>
public abstract class TableValidCheckerBase<T> : ScriptableObject
{
    /* const & readonly declaration             */

    /* enum & struct declaration                */

    /* public - Field declaration               */


    /* protected & private - Field declaration  */


    // ========================================================================== //

    /* public - [Do~Something] Function 	        */


    // ========================================================================== //

    /* protected - [Override & Unity API]       */


    /* protected - [abstract & virtual]         */

    public abstract void OnInsert(Dictionary<string, TableBase<T>.ColumnInfo> mapColumnInfo, List<T> listInsert, out string strErrorMsg);

    // ========================================================================== //

    #region Private

    #endregion Private
}