using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;

namespace DB_Manager
{
    public class CardAndCarManagement : IBO23KioskDbCommand, IDisposable
    {
        #region Dispose

        // Implement IDisposable. 
        // Do not make this method virtual. 
        // A derived class should not be able to override this method. 
        private bool disposed = false;
        public void Dispose()
        {
            Dispose(true);
            // This object will be cleaned up by the Dispose method. 
            // Therefore, you should call GC.SupressFinalize to 
            // take this object off the finalization queue 
            // and prevent finalization code for this object 
            // from executing a second time.
            GC.SuppressFinalize(this);
        }

        // Dispose(bool disposing) executes in two distinct scenarios. 
        // If disposing equals true, the method has been called directly 
        // or indirectly by a user's code. Managed and unmanaged resources 
        // can be disposed. 
        // If disposing equals false, the method has been called by the 
        // runtime from inside the finalizer and you should not reference 
        // other objects. Only unmanaged resources can be disposed. 
        protected virtual void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called. 
            if (!this.disposed)
            {
                // If disposing equals true, dispose all managed 
                // and unmanaged resources. 
                if (disposing)
                {
                    // Dispose managed resources.
                    base.Dispose();
                }

                // Call the appropriate methods to clean up 
                // unmanaged resources here. 
                // If disposing is false, 
                // only the following code is executed.                                


                // Note disposing has been done.
                disposed = true;
            }
        }

        #endregion

        public static Dictionary<string, string> TableDict = new Dictionary<string, string>() {
                                                                {"OID", "ID"},
                                                                { "RFIDCode", "RFID" }, 
                                                                { "CarTag", "ทะเบียนรถ" },
                                                                {"CarDescription", "ลักษณะรถ"},
                                                                {"CreatedDate", "วันที่บันทึก"},
                                                                {"ExpiryDate", "วันที่หมดอายุ"},
                                                                {"IsActive","สถานะ"},
                                                                {"Comment", "หมายเหตุ"}};

        public bool GetCarTag(string cardId, out string carTag)
        {
            DataSet ds = new DataSet();
            try
            {
                stm = "SELECT * FROM tbOperator WHERE RFIDCode=@RFIDCode AND IsActive='ACTIVE'";
                DbCallback.SetCommandText(stm);
                DbCallback.AddInputParameter("@RFIDCode", SqlDbType.NVarChar, cardId);
                ds = DbCallback.ExecuteToDataSet();
                if (ds != null)
                    if (ds.Tables != null)
                        if (ds.Tables.Count > 0)
                            if (ds.Tables[0].Rows != null)
                                if (ds.Tables[0].Rows.Count > 0)
                                {
                                    carTag = Convert.ToString(ds.Tables[0].Rows[0]["CarTag"]);
                                    return true;
                                }
                carTag = string.Empty;
                return false;
            }
            catch (Exception ex)
            {
                DbCallback.CloseConnection();
                throw ex;
            }
        }

        public bool CheckCarTag(string cardId)
        {
            DataSet ds = new DataSet();
            try
            {
                stm = "SELECT * FROM tbOperator WHERE CarTag=@CarTag AND IsActive='ACTIVE'";
                DbCallback.SetCommandText(stm);
                DbCallback.AddInputParameter("@CarTag", SqlDbType.NVarChar, cardId);
                ds = DbCallback.ExecuteToDataSet();
                if (ds != null)
                    if (ds.Tables != null)
                        if (ds.Tables.Count > 0)
                            if (ds.Tables[0].Rows != null)
                                if (ds.Tables[0].Rows.Count > 0)
                                    return true;
                return false;
            }
            catch (Exception ex)
            {
                DbCallback.CloseConnection();
                throw ex;
            }
        }

        public DataTable GetData(string cardId)
        {
            DataSet ds = new DataSet();
            try
            {
                stm = "SELECT [OID] AS [ID], [RFIDCode] AS [RFID],[CarTag] AS [ทะเบียนรถ],[CreatedDate] AS [วันที่บันทึก],[ExpiryDate] AS [วันที่หมดอายุ],[IsActive] AS [สถานะ] FROM tbOperator WHERE RFIDCode=@RFIDCode";
                DbCallback.SetCommandText(stm);
                DbCallback.AddInputParameter("@RFIDCode", SqlDbType.NVarChar, cardId);
                ds = DbCallback.ExecuteToDataSet();
                if (ds != null)
                    if (ds.Tables != null)
                        if (ds.Tables.Count > 0)
                            if (ds.Tables[0].Rows != null)
                                if (ds.Tables[0].Rows.Count > 0)
                                    return ds.Tables[0];
                return null;
            }
            catch (Exception ex)
            {
                DbCallback.CloseConnection();
                throw ex;
            }
        }

        public static DataTable FormatDataTable()
        {
            DataTable table = new DataTable();
            table.Columns.Add("OID", typeof(Guid));
            table.Columns.Add("RFIDCode", typeof(string));
            table.Columns.Add("CarTag", typeof(string));
            table.Columns.Add("CarDescription", typeof(string));
            table.Columns.Add("CreatedDate", typeof(DateTime));
            table.Columns.Add("ExpiryDate", typeof(DateTime));
            table.Columns.Add("IsActive", typeof(string));
            table.Columns.Add("Comment", typeof(string));
            return table;
        }

        public bool Update(DataRow dr, out int resCode, out string resDesc)
        {
            try
            {
                stm = "UPDATE tbOperator SET RFIDCode=@RFIDCode, CarTag=@CarTag, CarDescription=@CarDescription, CreatedDate=@CreatedDate, ExpiryDate=@ExpiryDate, IsActive=@IsActive, Comment=@Comment WHERE OID=@OID";
                DbCallback.SetCommandText(stm);
                DbCallback.AddInputParameter("@OID", SqlDbType.UniqueIdentifier, dr["OID"]);
                DbCallback.AddInputParameter("@RFIDCode", SqlDbType.NVarChar, dr["RFIDCode"]);
                DbCallback.AddInputParameter("@CarTag", SqlDbType.NVarChar, dr["CarTag"]);
                DbCallback.AddInputParameter("@CarDescription", SqlDbType.NVarChar, dr["CarDescription"]);
                DbCallback.AddInputParameter("@CreatedDate", SqlDbType.Date, dr["CreatedDate"]);
                DbCallback.AddInputParameter("@ExpiryDate", SqlDbType.Date, dr["ExpiryDate"]);
                DbCallback.AddInputParameter("@IsActive", SqlDbType.NVarChar, dr["IsActive"]);
                DbCallback.AddInputParameter("@Comment", SqlDbType.NVarChar, dr["Comment"]);
                DbCallback.ExecuteNonQuery();
                resCode = 0;
                resDesc = "DONE Update.";
                return true;
            }
            catch (Exception ex) { resCode = -99; resDesc = ex.Message; return false; }
        }

        public bool Insert(DataRow dr, out int resCode, out string resDesc)
        {
            try
            {
                //"INSERT INTO tbUser (UserName, Password, Auth, IsActive, RFID, ExpireDate, Comment, CreateDate) VALUES (@UserName, @Password, @Auth, @IsActive, @RFID, @ExpireDate, @Comment, GETDATE())";
                stm = "INSERT INTO tbOperator (RFIDCode, CarTag, CarDescription, CreatedDate, ExpiryDate, IsActive, Comment) VALUES (@RFIDCode, @CarTag, @CarDescription, GETDATE(), @ExpiryDate, @IsActive, @Comment)";
                DbCallback.SetCommandText(stm);
                DbCallback.AddInputParameter("@RFIDCode", SqlDbType.NVarChar, dr["RFIDCode"]);
                DbCallback.AddInputParameter("@CarTag", SqlDbType.NVarChar, dr["CarTag"]);
                DbCallback.AddInputParameter("@CarDescription", SqlDbType.NVarChar, dr["CarDescription"]);
                DbCallback.AddInputParameter("@ExpiryDate", SqlDbType.Date, dr["ExpiryDate"]);
                DbCallback.AddInputParameter("@IsActive", SqlDbType.NVarChar, dr["IsActive"]);
                DbCallback.AddInputParameter("@Comment", SqlDbType.NVarChar, dr["Comment"]);
                DbCallback.ExecuteNonQuery();
                resCode = 0;
                resDesc = "DONE Insertion.";
                return true;
            }
            catch (Exception ex) { resCode = -99; resDesc = ex.Message; return false; }
        }
    }
}
