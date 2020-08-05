using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;

namespace DB_Manager
{
    public class BO23TestManagement : IBO23KioskDbCommand, IDisposable
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

        public DataSet GetBO23(string carTag)
        {
            DataSet ds = new DataSet();
            try
            {
                //stm = "SELECT TOP 100 [BO23],[BoDate] AS [วันที่],[B_Type] AS [ชนิด],[B_Name] AS [ชื่อชนิด],[B_Number] AS [จำนวน] FROM tbBo23Test ";
                //stm += "WHERE CarTag=@CarTag";
                stm = "SELECT TOP 100 * FROM tbBo23Test ";
                stm += "WHERE CarTag=@CarTag";
                DbCallback.SetCommandText(stm);
                DbCallback.AddInputParameter("@CarTag", SqlDbType.NVarChar, carTag);
                return DbCallback.ExecuteToDataSet();
            }
            catch (Exception ex)
            {
                DbCallback.CloseConnection();
                throw ex;
            }
        }
    }
}
