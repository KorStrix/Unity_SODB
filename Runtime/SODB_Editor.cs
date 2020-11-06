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
using System.Reflection;
using UnityEditor;

namespace SODB
{
    /// <summary>
    /// 
    /// </summary>
    public class SODB_Editor : EditorWindow
    {
        /* const & readonly declaration             */

        /* enum & struct declaration                */

        /* public - Field declaration               */


        /* protected & private - Field declaration  */

        SODB_Setting _pSetting;
        MonoScript _pScriptTarget;

        // ========================================================================== //

        /* public - [Do~Something] Function 	        */

        [MenuItem("Tools/Strix/SODB/Editor")]
        public static void DoShow()
        {
            SODB_Editor pWindow = (SODB_Editor) GetWindow(typeof(SODB_Editor), false);

            pWindow._pSetting = SODB_Setting.instance;
            pWindow.minSize = new Vector2(500, 500);
            pWindow.Show();
        }

        // ========================================================================== //

        /* protected - [Override & Unity API]       */

        private void OnGUI()
        {
            EditorGUILayout.HelpBox("이 툴은 Data Script를 SO(ScriptableObject)로 DB(Database)화 해주는 툴입니다.\n" +
                                    "작업 순서\n" + 
                                    "1. 아무 Data Script를 Table Target에 넣어보세요\n" + 
                                    "2. 적당히 세팅해보고 Generate 버튼을 눌러봅니다.\n",
                                    MessageType.Info);
            EditorGUILayout.Space();
            Draw_CurrentSetting();
            Draw_TargetScript();
        }

        /* protected - [abstract & virtual]         */


        // ========================================================================== //

        #region Private

        private void Draw_TargetScript()
        {
            _pScriptTarget = (MonoScript)EditorGUILayout.ObjectField("Table Target", _pScriptTarget, typeof(MonoScript), false);
            EditorGUILayout.Space();

            if (_pScriptTarget == null)
                return;

            _pSetting.DoDrawTable(_pScriptTarget, out TableConfig pTableConfig);

            if (GUILayout.Button("Generate Table Code!", GUILayout.Height(30)))
            {
                SOTableGenerator.DoGenerate_CSFile(pTableConfig, GetAbsolutePath(_pScriptTarget));
                Debug.Log("Generate Table Code!");
            }
        }

        private void Draw_CurrentSetting()
        {
            _pSetting = (SODB_Setting)EditorGUILayout.ObjectField("Current Setting", _pSetting, typeof(SODB_Setting), false);
            if (string.IsNullOrEmpty(_pSetting.strLastEditScript_GUID) == false)
            {
                string strAssetPath = AssetDatabase.GUIDToAssetPath(_pSetting.strLastEditScript_GUID);
                _pScriptTarget = AssetDatabase.LoadAssetAtPath<MonoScript>(strAssetPath);
            }
        }

        public static string GetAbsolutePath(MonoScript pScript)
        {
            return $"{Application.dataPath}/{AssetDatabase.GetAssetPath(pScript)}";
        }

        #endregion Private
    }
}