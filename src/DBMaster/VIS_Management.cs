using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace DB_Manager
{
    public class VIS_Management : IBO23KioskDbCommand, IDisposable
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

        string _tbName = "tbVisionAppData";

        public long GetMaxID()
        {
            try
            {
                DataSet ds = new DataSet();
                stm = string.Format("SELECT TOP 1 * FROM {0} ORDER BY VIS_ID DESC", _tbName);
                DbCallback.SetCommandText(stm);
                ds = DbCallback.ExecuteToDataSet();
                if (ds != null)
                    if (ds.Tables != null)
                        if (ds.Tables.Count > 0)
                            if (0 < ds.Tables[0].Rows.Count)
                                if (!DBNull.Value.Equals(ds.Tables[0].Rows[0]["VIS_ID"]))
                                    return (long)ds.Tables[0].Rows[0]["VIS_ID"];
                throw new Exception("Get maximum id failed!");
            }
            catch (Exception ex)
            {
                DbCallback.CloseConnection();
                throw ex;
            }
        }

        public int Insert(long visID, long journalID, string bType, string face)
        {
            try
            {
                DataSet ds = new DataSet();
                stm = string.Format("INSERT INTO {0} (VIS_ID, TimeText, JournalID, B_TYPE, FACE, STATE_ID) VALUES (@VIS_ID, @TimeText, @JournalID, @B_TYPE, @FACE, @STATE_ID)", _tbName);
                DbCallback.SetCommandText(stm);
                DbCallback.AddInputParameter("@VIS_ID", SqlDbType.BigInt, visID);
                DbCallback.AddInputParameter("@TimeText", SqlDbType.NVarChar, DateTime.Now.ToString("yyyy-MM-dd, HH:mm:ss"));
                DbCallback.AddInputParameter("@JournalID", SqlDbType.BigInt, journalID);
                DbCallback.AddInputParameter("@B_TYPE", SqlDbType.NVarChar, bType);
                DbCallback.AddInputParameter("@FACE", SqlDbType.NVarChar, face);
                DbCallback.AddInputParameter("@STATE_ID", SqlDbType.Int, 0);
                return DbCallback.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                DbCallback.CloseConnection();
                throw ex;
            }
        }

        public int Update(long vis_id, int state_id, string job, float score1, float score2, float score3, float score4, string runStatus)
        {
            try
            {
                DataSet ds = new DataSet();
                stm = string.Format("UPDATE {0} SET STATE_ID=@STATE_ID, {1}_SCORE1=@SCORE1, {1}_SCORE2=@SCORE2, {1}_SCORE3=@SCORE3, {1}_SCORE4=@SCORE4, {1}_RUN_STATUS=@RUN_STATUS WHERE VIS_ID=@VIS_ID", _tbName, job);
                DbCallback.SetCommandText(stm);
                DbCallback.AddInputParameter("@VIS_ID", SqlDbType.BigInt, vis_id);
                DbCallback.AddInputParameter("@STATE_ID", SqlDbType.Int, state_id);
                DbCallback.AddInputParameter("@SCORE1", SqlDbType.Float, score1);
                DbCallback.AddInputParameter("@SCORE2", SqlDbType.Float, score2);
                DbCallback.AddInputParameter("@SCORE3", SqlDbType.Float, score3);
                DbCallback.AddInputParameter("@SCORE4", SqlDbType.Float, score4);
                DbCallback.AddInputParameter("@RUN_STATUS", SqlDbType.NVarChar, runStatus);
                return DbCallback.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                DbCallback.CloseConnection();
                throw ex;
            }
        }

        public DataTable Select(long vis_id)
        {
            try
            {
                DataSet ds = new DataSet();
                stm = string.Format("SELECT * FROM {0} WHERE VIS_ID=@VIS_ID", _tbName);
                DbCallback.SetCommandText(stm);
                DbCallback.AddInputParameter("@VIS_ID", SqlDbType.BigInt, vis_id);
                ds = DbCallback.ExecuteToDataSet();
                if (ds != null)
                    if (ds.Tables != null)
                        if (ds.Tables.Count > 0)
                            return ds.Tables[0];
                return null;
            }
            catch (Exception ex)
            {
                DbCallback.CloseConnection();
                throw ex;
            }
        }
    }
}
