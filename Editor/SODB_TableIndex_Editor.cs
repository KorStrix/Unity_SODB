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
using UnityEditor;

/// <summary>
/// 
/// </summary>
public class SODB_TableIndex_Editor : EditorWindow
{
    /* const & readonly declaration             */

    /* enum & struct declaration                */

    /* public - Field declaration               */


    /* protected & private - Field declaration  */

    private TableConfig _pTableConfig;

    // ========================================================================== //

    /* public - [Do~Something] Function 	        */

    public static void DoShow(TableConfig pTableConfig)
    {
        SODB_TableIndex_Editor pWindow = (SODB_TableIndex_Editor)GetWindow(typeof(SODB_TableIndex_Editor), true);

        pWindow._pTableConfig = pTableConfig;
        pWindow.minSize = new Vector2(300, 200);
        pWindow.Show();
    }

    // ========================================================================== //

    /* protected - [Override & Unity API]       */


    /* protected - [abstract & virtual]         */


    // ========================================================================== //

    #region Private

    #endregion Private
}