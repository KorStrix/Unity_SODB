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

namespace SODB
{
    [Flags]
    public enum EColumnTypeFlags
    {
        None = 0,

        /// <summary>
        /// Primary Key
        /// <para>Rows 중 유일한 키(=중복허용X), <see langword="null"/>이 허용되지 않습니다.</para>
        /// </summary>
        PK = 1 << 0,

        ///// <summary>
        ///// Not Null
        ///// </summary>
        //NN = 1 << 1,

        ///// <summary>
        ///// Unique Key
        ///// <para>Rows 중 유일한 키(=중복허용X), <see langword="null"/>이 허용됩니다.</para>
        ///// </summary>
        //UQ = 1 << 2,
    }

    enum ELabelName
    {
        Type,
        Column,
        PK,
        NN,
        UQ,
    }


    [Serializable]
    public class TableConfig
    {
        public string strClassName;
        public List<ColumnConfig> listColumnConfig;

        public TableConfig(string strClassName)
        {
            this.strClassName = strClassName;
            listColumnConfig = new List<ColumnConfig>();
        }
    }

    [Serializable]
    public class ColumnConfig
    {
        public string strColumnName;
        public EColumnTypeFlags eTypeFlags;

        public ColumnConfig(string strColumnName)
        {
            this.strColumnName = strColumnName;
            eTypeFlags = EColumnTypeFlags.None;
        }
    }
}