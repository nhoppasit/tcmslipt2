using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace DB_Manager
{
    public class VisionTestManagement : IBO23KioskDbCommand, IDisposable
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

        public void GetData(int id, out string message, out int number)
        {
            {
                message = "NG";
                number = 0;
            }
            try
            {
                DataSet ds = new DataSet();
                stm = "SELECT * FROM tbVisionTest WHERE ID=@ID";
                DbCallback.SetCommandText(stm);
                DbCallback.AddInputParameter("@ID", SqlDbType.NVarChar, id);
                ds = DbCallback.ExecuteToDataSet();
                if (ds != null)
                    if (ds.Tables != null)
                        if (ds.Tables.Count > 0)
                            if (ds.Tables[0].Rows != null)
                                if (ds.Tables[0].Rows.Count > 0)
                                {
                                    message = (string)(ds.Tables[0].Rows[0]["Message"]);
                                    try { number = (int)(ds.Tables[0].Rows[0]["GoodBasket"]); }
                                    catch { number = 0; }
                                }
            }
            catch (Exception ex)
            {
                DbCallback.CloseConnection();
                throw ex;
            }
        }

    }
}
