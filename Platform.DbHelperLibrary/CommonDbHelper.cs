using System;
using System.Collections;
using System.Collections.Specialized;
using System.Data;
using System.Data.SqlClient;
using System.Data.OleDb;
using Oracle.DataAccess.Client;
using System.Configuration;
using System.Reflection;

namespace Platform.DbHelperLibrary
{
    public class CommonDbHelper
    {
        #region 属性
        /// <summary>
        /// 数据库连接字符串
        /// </summary>
        private string connectionString;
        public string ConntionString
        {
            get
            {
                return connectionString;
            }
            set
            {
                connectionString = value;
            }
        }
        /// <summary>
        /// 数据库类型 
        /// </summary>
        private string dbType;
        public string DbType
        {
            get
            {
                if (string.IsNullOrEmpty(dbType))
                    return "Access";
                else
                    return dbType;
            }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    dbType = value;
                }
                else
                {
                    dbType = ConfigurationManager.AppSettings["DataType"];
                }
            }
        }
        #endregion

        #region 构造方法
        /// <summary>
        /// 设置默认参数数据类型及连接字符串
        /// </summary>
        public CommonDbHelper(string strConnect, string dbType)
        {
            this.ConntionString = strConnect;
            this.DbType = dbType;
        }
        /// <summary>
        /// 从配置中自动读取数据库类型及连接字符串
        /// </summary>
        public CommonDbHelper()
        {
            this.connectionString = ConfigurationManager.AppSettings["ConnectionString"];
            this.dbType = ConfigurationManager.AppSettings["DbType"];
        }
        #endregion

        #region 转换参数
        /// <summary>
        /// 对应数据库数据类型参数转换
        /// </summary>
        /// <param name="ParaName">数据类型名称</param>
        /// <param name="DataType">数据库类型</param>
        /// <returns></returns>
        private IDbDataParameter iDbParameter(string ParaName, string DataType)
        {
            switch (this.DbType)
            {
                case "SqlServer":
                    return GetSqlParameter(ParaName, DataType);
                case "Oracle":
                    return GetOracleParameter(ParaName, DataType);
                case "Access":
                    return GetOleDbParameter(ParaName, DataType);
                default:
                    return GetSqlParameter(ParaName, DataType);
            }
        }
        /// <summary>
        /// 获取Sql数据库数据类型
        /// </summary>
        private SqlParameter GetSqlParameter(string ParaName, string DataType)
        {
            switch (DataType)
            {
                case "Decimal":
                    return new System.Data.SqlClient.SqlParameter(ParaName, System.Data.SqlDbType.Decimal);
                case "Varchar":
                    return new System.Data.SqlClient.SqlParameter(ParaName, System.Data.SqlDbType.VarChar);
                case "DateTime":
                    return new System.Data.SqlClient.SqlParameter(ParaName, System.Data.SqlDbType.DateTime);
                case "Iamge":
                    return new System.Data.SqlClient.SqlParameter(ParaName, System.Data.SqlDbType.Image);
                case "Int":
                    return new System.Data.SqlClient.SqlParameter(ParaName, System.Data.SqlDbType.Int);
                case "Text":
                    return new System.Data.SqlClient.SqlParameter(ParaName, System.Data.SqlDbType.NText);
                default:
                    return new System.Data.SqlClient.SqlParameter(ParaName, System.Data.SqlDbType.VarChar);
            }
        }
        /// <summary>
        /// 获取Oracle数据库字段类型
        /// </summary>
        private OracleParameter GetOracleParameter(string ParaName, string DataType)
        {
            switch (DataType)
            {
                case "Decimal":
                    return new OracleParameter(ParaName, OracleDbType.Double);
                case "Varchar":
                    return new OracleParameter(ParaName, OracleDbType.Varchar2);
                case "DateTime":
                    return new OracleParameter(ParaName, OracleDbType.Date);
                case "Iamge":
                    return new OracleParameter(ParaName, OracleDbType.BFile);
                case "Int":
                    return new OracleParameter(ParaName, OracleDbType.Int32);
                case "Text":
                    return new OracleParameter(ParaName, OracleDbType.Clob);
                default:
                    return new OracleParameter(ParaName, OracleDbType.Varchar2);
            }
        }
        /// <summary>
        /// 获取Assecss数据库字段类型
        /// </summary>
        private OleDbParameter GetOleDbParameter(string ParaName, string DataType)
        {
            switch (DataType)
            {
                case "Decimal":
                    return new OleDbParameter(ParaName, System.Data.DbType.Decimal);
                case "Varchar":
                    return new OleDbParameter(ParaName, System.Data.DbType.String);
                case "DateTime":
                    return new OleDbParameter(ParaName, System.Data.DbType.DateTime);
                case "Iamge":
                    return new OleDbParameter(ParaName, System.Data.DbType.Binary);
                case "Int":
                    return new OleDbParameter(ParaName, System.Data.DbType.Int32);
                case "Text":
                    return new OleDbParameter(ParaName, System.Data.DbType.String);
                default:
                    return new OleDbParameter(ParaName, System.Data.DbType.String);
            }
        }

        #endregion

        #region 创建 Connection 和 Command
        /// <summary>
        /// 获取数据库连接
        /// </summary>
        private IDbConnection GetConnection()
        {
            switch (this.DbType)
            {
                case "SqlServer":
                    return new SqlConnection(this.ConntionString);
                case "Oracle":
                    return new OracleConnection(this.ConntionString);
                case "Access":
                    return new OleDbConnection(this.ConntionString);
                default:
                    return new SqlConnection(this.ConntionString);
            }
        }
        /// <summary>
        /// 获取数据执行命令
        /// </summary>
        private IDbCommand GetCommand(string Sql, IDbConnection iConn)
        {
            switch (this.DbType)
            {
                case "SqlServer":
                    return new SqlCommand(Sql, (SqlConnection)iConn);
                case "Oracle":
                    return new OracleCommand(Sql, (OracleConnection)iConn);
                case "Access":
                    return new OleDbCommand(Sql, (OleDbConnection)iConn);
                default:
                    return new SqlCommand(Sql, (SqlConnection)iConn);
            }
        }
        /// <summary>
        /// 获取数据执行命令
        /// </summary>
        private IDbCommand GetCommand()
        {
            switch (this.DbType)
            {
                case "SqlServer":
                    return new SqlCommand();
                case "Oracle":
                    return new OracleCommand();
                case "Access":
                    return new OleDbCommand();
                default:
                    return new SqlCommand();
            }
        }
        /// <summary>
        /// 获取数据读取适配器
        /// </summary>
        private IDataAdapter GetAdapater(string Sql, IDbConnection iConn)
        {
            switch (this.DbType)
            {
                case "SqlServer":
                    return new SqlDataAdapter(Sql, (SqlConnection)iConn);
                case "Oracle":
                    return new OracleDataAdapter(Sql, (OracleConnection)iConn);
                case "Access":
                    return new OleDbDataAdapter(Sql, (OleDbConnection)iConn);
                default:
                    return new SqlDataAdapter(Sql, (SqlConnection)iConn); ;
            }
        }
        /// <summary>
        /// 获取数据读取适配器
        /// </summary>
        private IDataAdapter GetAdapater()
        {
            switch (this.DbType)
            {
                case "SqlServer":
                    return new SqlDataAdapter();
                case "Oracle":
                    return new OracleDataAdapter();
                case "Access":
                    return new OleDbDataAdapter();
                default:
                    return new SqlDataAdapter();
            }
        }
        /// <summary>
        /// 获取数据读取适配器
        /// </summary>
        private IDataAdapter GetAdapater(IDbCommand iCmd)
        {
            switch (this.DbType)
            {
                case "SqlServer":
                    return new SqlDataAdapter((SqlCommand)iCmd);
                case "Oracle":
                    return new OracleDataAdapter((OracleCommand)iCmd);
                case "Access":
                    return new OleDbDataAdapter((OleDbCommand)iCmd);
                default:
                    return new SqlDataAdapter((SqlCommand)iCmd);
            }
        }
        #endregion

        #region  执行简单SQL语句
        /// <summary>
        /// 执行SQL语句，返回影响的记录数
        /// </summary>
        /// <param name="SQLString">SQL语句</param>
        /// <returns>影响的记录数</returns>
        public int ExecuteSql(string SqlString)
        {
            using (IDbConnection iConn = this.GetConnection())
            {
                using (IDbCommand iCmd = GetCommand(SqlString, iConn))
                {
                    iConn.Open();
                    try
                    {
                        int rows = iCmd.ExecuteNonQuery();
                        return rows;
                    }
                    catch (System.Exception E)
                    {
                        throw new Exception(E.Message);
                    }
                    finally
                    {
                        if (iConn.State != ConnectionState.Closed)
                        {
                            iConn.Close();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 执行多条SQL语句，实现数据库事务。
        /// </summary>
        /// <param name="SQLStringList">多条SQL语句</param>        
        public void ExecuteSqlTran(ArrayList SQLStringList)
        {
            using (IDbConnection iConn = this.GetConnection())
            {
                iConn.Open();
                using (IDbCommand iCmd = GetCommand())
                {
                    iCmd.Connection = iConn;
                    using (IDbTransaction iDbTran = iConn.BeginTransaction())
                    {
                        iCmd.Transaction = iDbTran;
                        try
                        {
                            for (int n = 0; n < SQLStringList.Count; n++)
                            {
                                string strsql = SQLStringList[n].ToString();
                                if (strsql.Trim().Length > 1)
                                {
                                    iCmd.CommandText = strsql;
                                    iCmd.ExecuteNonQuery();
                                }
                            }
                            iDbTran.Commit();
                        }
                        catch (System.Exception E)
                        {
                            iDbTran.Rollback();
                            throw new Exception(E.Message);
                        }
                        finally
                        {
                            if (iConn.State != ConnectionState.Closed)
                            {
                                iConn.Close();
                            }
                        }
                    }
                }
            }
        }
        /// <summary>
        /// 执行带一个存储过程参数的的SQL语句。
        /// </summary>
        /// <param name="SQLString">SQL语句</param>
        /// <param name="content">参数内容,比如一个字段是格式复杂的文章，有特殊符号，可以通过这个方式添加</param>
        /// <returns>影响的记录数</returns>
        public int ExecuteSql(string SqlString, string content)
        {
            using (IDbConnection iConn = this.GetConnection())
            {
                using (IDbCommand iCmd = GetCommand(SqlString, iConn))
                {
                    IDataParameter myParameter = this.iDbParameter("@content", "Text");
                    myParameter.Value = content;
                    iCmd.Parameters.Add(myParameter);
                    iConn.Open();
                    try
                    {
                        int rows = iCmd.ExecuteNonQuery();
                        return rows;
                    }
                    catch (System.Exception e)
                    {
                        throw new Exception(e.Message);
                    }
                    finally
                    {
                        if (iConn.State != ConnectionState.Closed)
                        {
                            iConn.Close();
                        }
                    }
                }
            }
        }


        /**/
        /// <summary>
        /// 向数据库里插入图像格式的字段(和上面情况类似的另一种实例)
        /// </summary>
        /// <param name="strSQL">SQL语句</param>
        /// <param name="fs">图像字节,数据库的字段类型为image的情况</param>
        /// <returns>影响的记录数</returns>
        public int ExecuteSqlInsertImg(string SqlString, byte[] fs)
        {
            using (IDbConnection iConn = this.GetConnection())
            {
                using (IDbCommand iCmd = GetCommand(SqlString, iConn))
                {
                    System.Data.IDataParameter myParameter = this.iDbParameter("@content", "Image");
                    myParameter.Value = fs;
                    iCmd.Parameters.Add(myParameter);
                    iConn.Open();
                    try
                    {
                        int rows = iCmd.ExecuteNonQuery();
                        return rows;
                    }
                    catch (System.Exception e)
                    {
                        throw new Exception(e.Message);
                    }
                    finally
                    {
                        if (iConn.State != ConnectionState.Closed)
                        {
                            iConn.Close();
                        }
                    }
                }
            }
        }

        /**/
        /// <summary>
        /// 执行一条计算查询结果语句，返回查询结果（object）。
        /// </summary>
        /// <param name="SQLString">计算查询结果语句</param>
        /// <returns>查询结果（object）</returns>
        public object GetSingle(string SqlString)
        {
            using (IDbConnection iConn = GetConnection())
            {
                using (IDbCommand iCmd = GetCommand(SqlString, iConn))
                {
                    iConn.Open();
                    try
                    {
                        object obj = iCmd.ExecuteScalar();
                        if ((Object.Equals(obj, null)) || (Object.Equals(obj, System.DBNull.Value)))
                        {
                            return null;
                        }
                        else
                        {
                            return obj;
                        }
                    }
                    catch (System.Exception e)
                    {
                        throw new Exception(e.Message);
                    }
                    finally
                    {
                        if (iConn.State != ConnectionState.Closed)
                        {
                            iConn.Close();
                        }
                    }
                }
            }
        }
        /**/
        /// <summary>
        /// 执行查询语句，返回IDataAdapter
        /// </summary>
        /// <param name="strSQL">查询语句</param>
        /// <returns>IDataAdapter</returns>
        public IDataAdapter ExecuteReader(string strSQL)
        {
            using (IDbConnection iConn = this.GetConnection())
            {
                iConn.Open();
                try
                {
                    System.Data.IDataAdapter iAdapter = this.GetAdapater(strSQL, iConn);
                    return iAdapter;
                }
                catch (System.Exception e)
                {
                    throw new Exception(e.Message);
                }
                finally
                {
                    if (iConn.State != ConnectionState.Closed)
                    {
                        iConn.Close();
                    }
                }
            }
        }
        /**/
        /// <summary>
        /// 执行查询语句，返回DataSet
        /// </summary>
        /// <param name="SQLString">查询语句</param>
        /// <returns>DataSet</returns>
        public DataSet Query(string sqlString)
        {
            using (IDbConnection iConn = this.GetConnection())
            {
                using (IDbCommand iCmd = GetCommand(sqlString, iConn))
                {
                    DataSet ds = new DataSet();
                    iConn.Open();
                    try
                    {
                        System.Data.IDataAdapter iAdapter = this.GetAdapater(sqlString, iConn);
                        iAdapter.Fill(ds);
                        return ds;
                    }
                    catch (System.Exception ex)
                    {
                        throw new Exception(ex.Message);
                    }
                    finally
                    {
                        if (iConn.State != ConnectionState.Closed)
                        {
                            iConn.Close();
                        }
                    }
                }
            }
        }

        /**/
        /// <summary>
        /// 执行查询语句，返回DataSet
        /// </summary>
        /// <param name="sqlString">查询语句</param>
        /// <param name="dataSet">要填充的DataSet</param>
        /// <param name="tableName">要填充的表名</param>
        /// <returns>DataSet</returns>
        public DataSet Query(string sqlString, DataSet dataSet, string tableName)
        {
            using (IDbConnection iConn = this.GetConnection())
            {
                using (IDbCommand iCmd = GetCommand(sqlString, iConn))
                {
                    iConn.Open();
                    try
                    {
                        System.Data.IDataAdapter iAdapter = this.GetAdapater(sqlString, iConn);
                        ((OleDbDataAdapter)iAdapter).Fill(dataSet, tableName);
                        return dataSet;
                    }
                    catch (System.Exception ex)
                    {
                        throw new Exception(ex.Message);
                    }
                    finally
                    {
                        if (iConn.State != ConnectionState.Closed)
                        {
                            iConn.Close();
                        }
                    }
                }
            }
        }


        /**/
        /// <summary>
        /// 执行SQL语句 返回存储过程
        /// </summary>
        /// <param name="sqlString">Sql语句</param>
        /// <param name="dataSet">要填充的DataSet</param>
        /// <param name="startIndex">开始记录</param>
        /// <param name="pageSize">页面记录大小</param>
        /// <param name="tableName">表名称</param>
        /// <returns>DataSet</returns>
        public DataSet Query(string sqlString, DataSet dataSet, int startIndex, int pageSize, string tableName)
        {
            using (IDbConnection iConn = this.GetConnection())
            {
                iConn.Open();
                try
                {
                    System.Data.IDataAdapter iAdapter = this.GetAdapater(sqlString, iConn);

                    ((OleDbDataAdapter)iAdapter).Fill(dataSet, startIndex, pageSize, tableName);

                    return dataSet;
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
                finally
                {
                    if (iConn.State != ConnectionState.Closed)
                    {
                        iConn.Close();
                    }
                }
            }
        }


        /**/
        /// <summary>
        /// 执行查询语句，向XML文件写入数据
        /// </summary>
        /// <param name="sqlString">查询语句</param>
        /// <param name="xmlPath">XML文件路径</param>
        public void WriteToXml(string sqlString, string xmlPath)
        {
            Query(sqlString).WriteXml(xmlPath);
        }

        /**/
        /// <summary>
        /// 执行查询语句
        /// </summary>
        /// <param name="SqlString">查询语句</param>
        /// <returns>DataTable </returns>
        public DataTable ExecuteQuery(string sqlString)
        {
            using (IDbConnection iConn = this.GetConnection())
            {
                //IDbCommand iCmd  =  GetCommand(sqlString,iConn);
                DataSet ds = new DataSet();
                try
                {
                    System.Data.IDataAdapter iAdapter = this.GetAdapater(sqlString, iConn);
                    iAdapter.Fill(ds);
                }
                catch (System.Exception e)
                {
                    throw new Exception(e.Message);
                }
                finally
                {
                    if (iConn.State != ConnectionState.Closed)
                    {
                        iConn.Close();
                    }
                }
                return ds.Tables[0];
            }
        }

        /**/
        /// <summary>
        /// 执行查询语句
        /// </summary>
        /// <param name="SqlString">查询语句</param>
        /// <returns>DataTable </returns>
        public DataTable ExecuteQuery(string SqlString, string Proc)
        {
            using (IDbConnection iConn = this.GetConnection())
            {
                using (IDbCommand iCmd = GetCommand(SqlString, iConn))
                {
                    iCmd.CommandType = CommandType.StoredProcedure;
                    DataSet ds = new DataSet();
                    try
                    {
                        System.Data.IDataAdapter iDataAdapter = this.GetAdapater(SqlString, iConn);
                        iDataAdapter.Fill(ds);
                    }
                    catch (System.Exception e)
                    {
                        throw new Exception(e.Message);
                    }
                    finally
                    {
                        if (iConn.State != ConnectionState.Closed)
                        {
                            iConn.Close();
                        }
                    }
                    return ds.Tables[0];
                }


            }
        }

        /**/
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Sql"></param>
        /// <returns></returns>
        public DataView ExeceuteDataView(string Sql)
        {
            using (IDbConnection iConn = this.GetConnection())
            {
                using (IDbCommand iCmd = GetCommand(Sql, iConn))
                {
                    DataSet ds = new DataSet();
                    try
                    {
                        System.Data.IDataAdapter iDataAdapter = this.GetAdapater(Sql, iConn);
                        iDataAdapter.Fill(ds);
                        return ds.Tables[0].DefaultView;
                    }
                    catch (System.Exception e)
                    {
                        throw new Exception(e.Message);
                    }
                    finally
                    {
                        if (iConn.State != ConnectionState.Closed)
                        {
                            iConn.Close();
                        }
                    }
                }
            }
        }

        #endregion

        #region 执行带参数的SQL语句
        /**/
        /// <summary>
        /// 执行SQL语句，返回影响的记录数
        /// </summary>
        /// <param name="SQLString">SQL语句</param>
        /// <returns>影响的记录数</returns>
        public int ExecuteSql(string SQLString, params IDataParameter[] iParms)
        {
            using (IDbConnection iConn = this.GetConnection())
            {
                IDbCommand iCmd = GetCommand();
                {
                    try
                    {
                        PrepareCommand(out iCmd, iConn, null, SQLString, iParms);
                        int rows = iCmd.ExecuteNonQuery();
                        iCmd.Parameters.Clear();
                        return rows;
                    }
                    catch (System.Exception E)
                    {
                        throw new Exception(E.Message);
                    }
                    finally
                    {
                        iCmd.Dispose();
                        if (iConn.State != ConnectionState.Closed)
                        {
                            iConn.Close();
                        }
                    }
                }
            }
        }


        /**/
        /// <summary>
        /// 执行多条SQL语句，实现数据库事务。
        /// </summary>
        /// <param name="SQLStringList">SQL语句的哈希表（key为sql语句，value是该语句的SqlParameter[]）</param>
        public void ExecuteSqlTran(Hashtable SQLStringList)
        {
            using (IDbConnection iConn = this.GetConnection())
            {
                iConn.Open();
                using (IDbTransaction iTrans = iConn.BeginTransaction())
                {
                    IDbCommand iCmd = GetCommand();
                    try
                    {
                        //循环
                        foreach (DictionaryEntry myDE in SQLStringList)
                        {
                            string cmdText = myDE.Key.ToString();
                            IDataParameter[] iParms = (IDataParameter[])myDE.Value;
                            PrepareCommand(out iCmd, iConn, iTrans, cmdText, iParms);
                            int val = iCmd.ExecuteNonQuery();
                            iCmd.Parameters.Clear();
                        }
                        iTrans.Commit();
                    }
                    catch
                    {
                        iTrans.Rollback();
                        throw;
                    }
                    finally
                    {
                        iCmd.Dispose();
                        if (iConn.State != ConnectionState.Closed)
                        {
                            iConn.Close();
                        }
                    }

                }
            }
        }


        /**/
        /// <summary>
        /// 执行一条计算查询结果语句，返回查询结果（object）。
        /// </summary>
        /// <param name="SQLString">计算查询结果语句</param>
        /// <returns>查询结果（object）</returns>
        public object GetSingle(string SQLString, params IDataParameter[] iParms)
        {
            using (IDbConnection iConn = this.GetConnection())
            {
                IDbCommand iCmd = GetCommand();
                {
                    try
                    {
                        PrepareCommand(out iCmd, iConn, null, SQLString, iParms);
                        object obj = iCmd.ExecuteScalar();
                        iCmd.Parameters.Clear();
                        if ((Object.Equals(obj, null)) || (Object.Equals(obj, System.DBNull.Value)))
                        {
                            return null;
                        }
                        else
                        {
                            return obj;
                        }
                    }
                    catch (System.Exception e)
                    {
                        throw new Exception(e.Message);
                    }
                    finally
                    {
                        iCmd.Dispose();
                        if (iConn.State != ConnectionState.Closed)
                        {
                            iConn.Close();
                        }
                    }
                }
            }
        }

        /**/
        /// <summary>
        /// 执行查询语句，返回IDataReader
        /// </summary>
        /// <param name="strSQL">查询语句</param>
        /// <returns> IDataReader </returns>
        public IDataReader ExecuteReader(string SQLString, params IDataParameter[] iParms)
        {
            IDbConnection iConn = this.GetConnection();
            {
                IDbCommand iCmd = GetCommand();
                {
                    try
                    {
                        PrepareCommand(out iCmd, iConn, null, SQLString, iParms);
                        System.Data.IDataReader iReader = iCmd.ExecuteReader();
                        iCmd.Parameters.Clear();
                        return iReader;
                    }
                    catch (System.Exception e)
                    {
                        throw new Exception(e.Message);
                    }
                    finally
                    {
                        iCmd.Dispose();
                        if (iConn.State != ConnectionState.Closed)
                        {
                            iConn.Close();
                        }
                    }
                }
            }
        }

        /**/
        /// <summary>
        /// 执行查询语句，返回DataSet
        /// </summary>
        /// <param name="SQLString">查询语句</param>
        /// <returns>DataSet</returns>
        public DataSet Query(string sqlString, params IDataParameter[] iParms)
        {
            using (IDbConnection iConn = this.GetConnection())
            {
                IDbCommand iCmd = GetCommand();
                {
                    PrepareCommand(out iCmd, iConn, null, sqlString, iParms);
                    try
                    {
                        IDataAdapter iAdapter = this.GetAdapater(sqlString, iConn);
                        DataSet ds = new DataSet();
                        iAdapter.Fill(ds);
                        iCmd.Parameters.Clear();
                        return ds;
                    }
                    catch (System.Exception ex)
                    {
                        throw new Exception(ex.Message);
                    }
                    finally
                    {
                        iCmd.Dispose();
                        if (iConn.State != ConnectionState.Closed)
                        {
                            iConn.Close();
                        }
                    }
                }
            }
        }


        /**/
        /// <summary>
        /// 初始化Command
        /// </summary>
        /// <param name="iCmd"></param>
        /// <param name="iConn"></param>
        /// <param name="iTrans"></param>
        /// <param name="cmdText"></param>
        /// <param name="iParms"></param>
        private void PrepareCommand(out IDbCommand iCmd, IDbConnection iConn, System.Data.IDbTransaction iTrans, string cmdText, IDataParameter[] iParms)
        {
            if (iConn.State != ConnectionState.Open)
                iConn.Open();
            iCmd = this.GetCommand();
            iCmd.Connection = iConn;
            iCmd.CommandText = cmdText;
            if (iTrans != null)
                iCmd.Transaction = iTrans;
            iCmd.CommandType = CommandType.Text;//cmdType;
            if (iParms != null)
            {
                foreach (IDataParameter parm in iParms)
                    iCmd.Parameters.Add(parm);
            }
        }

        #endregion

        #region SQLServer存储过程操作

        /**/
        /// <summary>
        /// 执行存储过程
        /// </summary>
        /// <param name="storedProcName">存储过程名</param>
        /// <param name="parameters">存储过程参数</param>
        /// <returns>SqlDataReader</returns>
        public SqlDataReader RunProcedure(string storedProcName, IDataParameter[] parameters)
        {
            IDbConnection iConn = this.GetConnection();
            {
                iConn.Open();

                using (SqlCommand sqlCmd = BuildQueryCommand(iConn, storedProcName, parameters))
                {
                    return sqlCmd.ExecuteReader(CommandBehavior.CloseConnection);
                }
            }
        }

        /**/
        /// <summary>
        /// 执行存储过程
        /// </summary>
        /// <param name="storedProcName">存储过程名</param>
        /// <param name="parameters">存储过程参数</param>
        /// <param name="tableName">DataSet结果中的表名</param>
        /// <returns>DataSet</returns>
        public DataSet RunProcedure(string storedProcName, IDataParameter[] parameters, string tableName)
        {

            using (IDbConnection iConn = this.GetConnection())
            {
                DataSet dataSet = new DataSet();
                iConn.Open();
                System.Data.IDataAdapter iDA = this.GetAdapater();
                iDA = this.GetAdapater(BuildQueryCommand(iConn, storedProcName, parameters));

                ((SqlDataAdapter)iDA).Fill(dataSet, tableName);
                if (iConn.State != ConnectionState.Closed)
                {
                    iConn.Close();
                }
                return dataSet;
            }
        }



        /**/
        /// <summary>
        /// 执行存储过程
        /// </summary>
        /// <param name="storedProcName">存储过程名</param>
        /// <param name="parameters">存储过程参数</param>
        /// <param name="tableName">DataSet结果中的表名</param>
        /// <param name="startIndex">开始记录索引</param>
        /// <param name="pageSize">页面记录大小</param>
        /// <returns>DataSet</returns>
        public DataSet RunProcedure(string storedProcName, IDataParameter[] parameters, int startIndex, int pageSize, string tableName)
        {

            using (IDbConnection iConn = this.GetConnection())
            {
                DataSet dataSet = new DataSet();
                iConn.Open();
                System.Data.IDataAdapter iDA = this.GetAdapater();
                iDA = this.GetAdapater(BuildQueryCommand(iConn, storedProcName, parameters));

                ((SqlDataAdapter)iDA).Fill(dataSet, startIndex, pageSize, tableName);
                if (iConn.State != ConnectionState.Closed)
                {
                    iConn.Close();
                }
                return dataSet;
            }
        }

        /**/
        /// <summary>
        /// 执行存储过程 填充已经存在的DataSet数据集 
        /// </summary>
        /// <param name="storeProcName">存储过程名称</param>
        /// <param name="parameters">存储过程参数</param>
        /// <param name="dataSet">要填充的数据集</param>
        /// <param name="tablename">要填充的表名</param>
        /// <returns></returns>
        public DataSet RunProcedure(string storeProcName, IDataParameter[] parameters, DataSet dataSet, string tableName)
        {
            using (IDbConnection iConn = this.GetConnection())
            {
                iConn.Open();
                System.Data.IDataAdapter iDA = this.GetAdapater();
                iDA = this.GetAdapater(BuildQueryCommand(iConn, storeProcName, parameters));

                ((SqlDataAdapter)iDA).Fill(dataSet, tableName);

                if (iConn.State != ConnectionState.Closed)
                {
                    iConn.Close();
                }

                return dataSet;
            }
        }

        /**/
        /// <summary>
        /// 执行存储过程并返回受影响的行数
        /// </summary>
        /// <param name="storedProcName"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public int RunProcedureNoQuery(string storedProcName, IDataParameter[] parameters)
        {

            int result = 0;
            using (IDbConnection iConn = this.GetConnection())
            {
                iConn.Open();
                using (SqlCommand scmd = BuildQueryCommand(iConn, storedProcName, parameters))
                {
                    result = scmd.ExecuteNonQuery();
                }

                if (iConn.State != ConnectionState.Closed)
                {
                    iConn.Close();
                }
            }

            return result;
        }

        public string RunProcedureExecuteScalar(string storeProcName, IDataParameter[] parameters)
        {
            string result = string.Empty;
            using (IDbConnection iConn = this.GetConnection())
            {

                iConn.Open();
                using (SqlCommand scmd = BuildQueryCommand(iConn, storeProcName, parameters))
                {
                    object obj = scmd.ExecuteScalar();
                    if (obj == null)
                        result = null;
                    else
                        result = obj.ToString();
                }

                if (iConn.State != ConnectionState.Closed)
                {
                    iConn.Close();
                }

            }

            return result;
        }

        /**/
        /// <summary>
        /// 构建 SqlCommand 对象(用来返回一个结果集，而不是一个整数值)
        /// </summary>
        /// <param name="connection">数据库连接</param>
        /// <param name="storedProcName">存储过程名</param>
        /// <param name="parameters">存储过程参数</param>
        /// <returns>SqlCommand</returns>
        private SqlCommand BuildQueryCommand(IDbConnection iConn, string storedProcName, IDataParameter[] parameters)
        {

            IDbCommand iCmd = GetCommand(storedProcName, iConn);
            iCmd.CommandType = CommandType.StoredProcedure;
            if (parameters == null)
            {
                return (SqlCommand)iCmd;
            }
            foreach (IDataParameter parameter in parameters)
            {
                iCmd.Parameters.Add(parameter);
            }
            return (SqlCommand)iCmd;
        }

        /**/
        /// <summary>
        /// 执行存储过程，返回影响的行数        
        /// </summary>
        /// <param name="storedProcName">存储过程名</param>
        /// <param name="parameters">存储过程参数</param>
        /// <param name="rowsAffected">影响的行数</param>
        /// <returns></returns>
        public int RunProcedure(string storedProcName, IDataParameter[] parameters, out int rowsAffected)
        {
            using (IDbConnection iConn = this.GetConnection())
            {
                int result;
                iConn.Open();
                using (SqlCommand sqlCmd = BuildIntCommand(iConn, storedProcName, parameters))
                {
                    rowsAffected = sqlCmd.ExecuteNonQuery();
                    result = (int)sqlCmd.Parameters["ReturnValue"].Value;

                    if (iConn.State != ConnectionState.Closed)
                    {
                        iConn.Close();
                    }
                    return result;
                }
            }
        }

        /**/
        /// <summary>
        /// 创建 SqlCommand 对象实例(用来返回一个整数值)    
        /// </summary>
        /// <param name="storedProcName">存储过程名</param>
        /// <param name="parameters">存储过程参数</param>
        /// <returns>SqlCommand 对象实例</returns>
        private SqlCommand BuildIntCommand(IDbConnection iConn, string storedProcName, IDataParameter[] parameters)
        {
            SqlCommand sqlCmd = BuildQueryCommand(iConn, storedProcName, parameters);
            sqlCmd.Parameters.Add(new SqlParameter("ReturnValue",
                SqlDbType.Int, 4, ParameterDirection.ReturnValue,
                false, 0, 0, string.Empty, DataRowVersion.Default, null));
            return sqlCmd;
        }
        #endregion

        #region 存储过程操作

        /// <summary>
        /// 执行存储过程 返回SqlDataReader ( 注意：调用该方法后，一定要对SqlDataReader进行Close )
        /// </summary>
        /// <param name="storedProcName">存储过程名</param>
        /// <param name="parameters">存储过程参数</param>
        /// <returns>OracleDataReader</returns>
        public OracleDataReader OraRunProcedure(string storedProcName, IDataParameter[] parameters)
        {
            OracleConnection connection = this.GetConnection() as OracleConnection;
            OracleDataReader returnReader;
            connection.Open();
            OracleCommand command = OraBuildQueryCommand(connection, storedProcName, parameters);
            command.CommandType = CommandType.StoredProcedure;
            returnReader = command.ExecuteReader(CommandBehavior.CloseConnection);
            return returnReader;
        }

        public OracleDataReader OraRunProcedure(string storedProcName)
        {
            OracleConnection connection = this.GetConnection() as OracleConnection;
            OracleDataReader returnReader;
            connection.Open();
            OracleCommand command = OraBuildQueryCommand(connection, storedProcName);
            command.CommandType = CommandType.StoredProcedure;
            returnReader = command.ExecuteReader(CommandBehavior.CloseConnection);
            return returnReader;
        }

        private static OracleCommand OraBuildQueryCommand(OracleConnection connection, string storedProcName)
        {
            OracleCommand command = new OracleCommand(storedProcName, connection);
            command.CommandType = CommandType.StoredProcedure;

            return command;
        }


        /// <summary>
        /// 执行存储过程
        /// </summary>
        /// <param name="storedProcName">存储过程名</param>
        /// <param name="parameters">存储过程参数</param>
        /// <param name="tableName">DataSet结果中的表名</param>
        /// <returns>DataSet</returns>
        public DataSet OraRunProcedure(string storedProcName, IDataParameter[] parameters, string tableName)
        {
            using (OracleConnection connection = this.GetConnection() as OracleConnection)
            {
                DataSet dataSet = new DataSet();
                connection.Open();
                OracleDataAdapter sqlDA = new OracleDataAdapter();
                sqlDA.SelectCommand = OraBuildQueryCommand(connection, storedProcName, parameters);
                sqlDA.Fill(dataSet, tableName);
                connection.Close();
                return dataSet;
            }
        }


        /// <summary>
        /// 构建 OracleCommand 对象(用来返回一个结果集，而不是一个整数值)
        /// </summary>
        /// <param name="connection">数据库连接</param>
        /// <param name="storedProcName">存储过程名</param>
        /// <param name="parameters">存储过程参数</param>
        /// <returns>OracleCommand</returns>
        private static OracleCommand OraBuildQueryCommand(OracleConnection connection, string storedProcName, IDataParameter[] parameters)
        {
            OracleCommand command = new OracleCommand(storedProcName, connection);
            command.CommandType = CommandType.StoredProcedure;
            foreach (OracleParameter parameter in parameters)
            {
                command.Parameters.Add(parameter);
            }
            return command;
        }

        /// <summary>
        /// 执行存储过程，返回影响的行数		
        /// </summary>
        /// <param name="storedProcName">存储过程名</param>
        /// <param name="parameters">存储过程参数</param>
        /// <param name="rowsAffected">影响的行数</param>
        /// <returns></returns>
        public int OraRunProcedure(string storedProcName, IDataParameter[] parameters, out int rowsAffected)
        {
            using (OracleConnection connection = this.GetConnection() as OracleConnection)
            {
                int result;
                connection.Open();
                OracleCommand command = BuildIntCommand(connection, storedProcName, parameters);
                rowsAffected = command.ExecuteNonQuery();
                result = (int)command.Parameters["ReturnValue"].Value;
                //Connection.Close();
                return result;
            }
        }

        /// <summary>
        /// 创建 OracleCommand 对象实例(用来返回一个整数值)	
        /// </summary>
        /// <param name="storedProcName">存储过程名</param>
        /// <param name="parameters">存储过程参数</param>
        /// <returns>OracleCommand 对象实例</returns>
        private static OracleCommand BuildIntCommand(OracleConnection connection, string storedProcName, IDataParameter[] parameters)
        {
            OracleCommand command = OraBuildQueryCommand(connection, storedProcName, parameters);
            command.Parameters.Add(new OracleParameter("ReturnValue",
                OracleDbType.Int32, 4, ParameterDirection.ReturnValue,
                false, 0, 0, string.Empty, DataRowVersion.Default, null));
            return command;
        }
        #endregion
    }
}
