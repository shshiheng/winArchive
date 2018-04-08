using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.Odbc;
namespace winArchive
{
    class clsArchive
    {
        public OdbcConnection dbConnection;
        private OdbcCommand dbCommand;
        public winViewerSender.clsParameterSender clsDisplaySender = new winViewerSender.clsParameterSender();
        Hashtable ArchiveParameters = new Hashtable();

        public void Setup(string strConnectionString)
        {
            this.dbConnection = new OdbcConnection();
            this.dbConnection.ConnectionString = strConnectionString;
            this.dbCommand = new OdbcCommand();
            this.dbCommand.Connection = this.dbConnection;
        }
        public void ReadDataBase()
        {
            System.Data.Odbc.OdbcDataAdapter dbAdapter = new System.Data.Odbc.OdbcDataAdapter();
            dbAdapter.SelectCommand = this.dbCommand;
            DataSet dbDataSet = new DataSet();
            ushort dimL = 0;
            string strSQL;
            strSQL = "SELECT Name,Code FROM DisplayDefination ORDER BY Code";
            dbCommand.CommandText = strSQL;
            string strTableName = "DisplayDefination";
            dbAdapter.Fill(dbDataSet, strTableName);
            dimL = (ushort)dbDataSet.Tables[strTableName].Rows.Count;
            for (ushort i = 0; i < dimL; i++)
            {
                string strCode = dbDataSet.Tables[strTableName].Rows[i]["Code"].ToString();
                string[] strParameters = dbDataSet.Tables[strTableName].Rows[i]["Name"].ToString().Split('-');
                string strName = strParameters[1];
                ArchiveParameters.Add(strName, strCode);
            }
        }
        public void ExecuteNonQueryCommand(string strClear)
        {
            this.dbCommand.CommandText = strClear;
            try
            {
                this.dbCommand.ExecuteNonQuery();
            }
            catch (System.Exception err)
            {
                string msg = err.Message;
            }
        }
        public void CopeArchiveMessages(ref byte[] byteMessages, ushort usMessageLength)
        {
            byte byteBit = 0;
            long lValue = 0;
            double Value = 0.0;
            ushort usCode = 0;
            byte byteNumber = byteMessages[0];
            ulong ulType = BitConverter.ToUInt32(byteMessages, 1);
            byte byteLength = byteMessages[5];
            if (byteLength > 32 || byteLength == 0 ) return;
            string strInsert = @"INSERT INTO ";
            string strTableName = ASCIIEncoding.ASCII.GetString(byteMessages, 6, byteLength);
            strInsert += strTableName + @" VALUES (";
            byteLength += 6;
            for (byte i = 0; i < byteNumber; i++)
            {
                byteBit = (byte)((ulType >> i) & 0x01);
                if (byteBit == 0x01)
                {
                    lValue = BitConverter.ToInt32(byteMessages, byteLength);
                    byteLength += 4;
                    strInsert += lValue.ToString();
                    usCode = this.SearchCode((i + 1).ToString(), strTableName);
                    if (usCode != 0) this.clsDisplaySender.SendValue(usCode, lValue);
                }
                else
                {
                    Value = BitConverter.ToDouble(byteMessages, byteLength);
                    byteLength += 8;
                    strInsert += Value.ToString();
                    usCode = this.SearchCode((i + 1).ToString(), strTableName);
                    if (usCode != 0) this.clsDisplaySender.SendValue(usCode, Value);
                }
                if (i != byteNumber - 1) strInsert += @",";
            }
            strInsert += @")";
            this.dbCommand.CommandText = strInsert;
            try
            {
                this.dbCommand.ExecuteNonQuery();
            }
            catch (System.Exception err)
            {
                string msg = err.Message;
            }
        }
        private ushort SearchCode(string strColumnNO, string strTableName)
        {
            ushort usCode = 0;
            string strName = strTableName + strColumnNO;
            if (ArchiveParameters.Contains(strName))
                usCode = Convert.ToUInt16(ArchiveParameters[strName]);
            return usCode;
        }
    }
}
