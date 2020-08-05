using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace DB_Manager
{
    public class ConfigManagement : IBO23KioskDbCommand, IDisposable
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

        public int SetIntValue(string name, long value, string description)
        {
            try
            {
                DataSet ds = new DataSet();
                stm = "UPDATE tbConfig SET IntVal=@IntVal, Description=@Description WHERE Name=@Name";
                DbCallback.SetCommandText(stm);
                DbCallback.AddInputParameter("@Name", SqlDbType.NVarChar, name);
                DbCallback.AddInputParameter("@IntVal", SqlDbType.BigInt, value);
                DbCallback.AddInputParameter("@Description", SqlDbType.NVarChar, description);
                return DbCallback.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                DbCallback.CloseConnection();
                throw ex;
            }
        }

        public int SetCharValue(string name, string value, string description)
        {
            try
            {
                DataSet ds = new DataSet();
                stm = "UPDATE tbConfig SET CharVal=@CharVal, Description=@Description WHERE Name=@Name";
                DbCallback.SetCommandText(stm);
                DbCallback.AddInputParameter("@Name", SqlDbType.NVarChar, name);
                DbCallback.AddInputParameter("@CharVal", SqlDbType.NVarChar, value);
                DbCallback.AddInputParameter("@Description", SqlDbType.NVarChar, description);
                return DbCallback.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                DbCallback.CloseConnection();
                throw ex;
            }
        }

        public long GetIntValue(string name)
        {
            try
            {
                DataSet ds = new DataSet();
                stm = "SELECT * FROM tbConfig WHERE Name=@Name";
                DbCallback.SetCommandText(stm);
                DbCallback.AddInputParameter("@Name", SqlDbType.NVarChar, name);
                ds = DbCallback.ExecuteToDataSet();
                if (ds != null)
                    if (ds.Tables != null)
                        if (ds.Tables.Count > 0)
                            if (ds.Tables[0].Rows != null)
                                if (ds.Tables[0].Rows.Count > 0)
                                    if (!DBNull.Value.Equals(ds.Tables[0].Rows[0]["IntVal"]))
                                        return Convert.ToInt32(ds.Tables[0].Rows[0]["IntVal"]);
                throw new Exception("GetIntValue error!");
            }
            catch (Exception ex)
            {
                DbCallback.CloseConnection();
                throw ex;
            }
        }

        public string GetCharValue(string name)
        {
            try
            {
                DataSet ds = new DataSet();
                stm = "SELECT * FROM tbConfig WHERE Name=@Name";
                DbCallback.SetCommandText(stm);
                DbCallback.AddInputParameter("@Name", SqlDbType.NVarChar, name);
                ds = DbCallback.ExecuteToDataSet();
                if (ds != null)
                    if (ds.Tables != null)
                        if (ds.Tables.Count > 0)
                            if (ds.Tables[0].Rows != null)
                                if (ds.Tables[0].Rows.Count > 0)
                                    if (!DBNull.Value.Equals(ds.Tables[0].Rows[0]["CharVal"]))
                                        return (string)(ds.Tables[0].Rows[0]["CharVal"]);
                return string.Empty;
            }
            catch (Exception ex)
            {
                DbCallback.CloseConnection();
                throw ex;
            }
        }

    }
}
