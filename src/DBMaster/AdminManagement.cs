using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Security.Cryptography;
using System.IO;
using System.Data.SqlClient;

namespace DB_Manager
{
    public class AdminManagement : IBO23KioskDbCommand, IDisposable
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

        public void AddAdmin(string userName, string pwd, int iAuth, bool isActive, string rfid, DateTime expireDate, string comment)
        {
            try
            {
                stm = "INSERT INTO tbUser (UserName, Password, Auth, IsActive, RFID, ExpireDate, Comment, CreateDate) VALUES (@UserName, @Password, @Auth, @IsActive, @RFID, @ExpireDate, @Comment, GETDATE())";
                DbCallback.SetCommandText(stm);
                DbCallback.AddInputParameter("@UserName", SqlDbType.NVarChar, userName);
                DbCallback.AddInputParameter("@Password", SqlDbType.NVarChar, pwd);
                DbCallback.AddInputParameter("@Auth", SqlDbType.Int, iAuth);
                DbCallback.AddInputParameter("@IsActive", SqlDbType.NVarChar, isActive);
                DbCallback.AddInputParameter("@RFID", SqlDbType.NVarChar, rfid);
                DbCallback.AddInputParameter("@ExpireDate", SqlDbType.DateTime, expireDate);
                DbCallback.AddInputParameter("@Comment", SqlDbType.NVarChar, comment);
                DbCallback.ExecuteNonQuery();
            }
            catch (Exception ex) { throw ex; }
        }

        public void ChangePassword(string userName, string oldPwd, string newPwd)
        {
            try
            {
                int currentAuth;
                if (!Login(userName, oldPwd, out currentAuth))
                {
                    throw new Exception("ไม่สามารถบันทึกรหัสผ่านใหม่ได้! เนื่องจากชื่อผู้ใช้ไม่ถูกต้องหรือรหัสผ่านเดิมไม่ถูกต้อง");
                }
                else
                {
                    stm = "UPDATE tbUser SET Password=@Password WHERE UserName=@UserName";
                    DbCallback.SetCommandText(stm);
                    DbCallback.AddInputParameter("@UserName", SqlDbType.NVarChar, userName);
                    DbCallback.AddInputParameter("@Password", SqlDbType.NVarChar, newPwd);
                    DbCallback.ExecuteNonQuery();
                }
            }
            catch (Exception ex) { throw ex; }
        }

        public void DeactivateUser(string userName)
        {
            try
            {
                stm = "UPDATE tbUser SET IsActive='N' WHERE UserName=@UserName";
                DbCallback.SetCommandText(stm);
                DbCallback.AddInputParameter("@UserName", SqlDbType.NVarChar, userName);
                DbCallback.ExecuteNonQuery();
            }
            catch (Exception ex) { throw ex; }
        }

        public void ActivateUser(string userName, DateTime expireDate)
        {
            try
            {
                if (expireDate == null)
                {
                    stm = "UPDATE tbUser SET IsActive='Y' WHERE UserName=@UserName";
                    DbCallback.SetCommandText(stm);
                    DbCallback.AddInputParameter("@UserName", SqlDbType.NVarChar, userName);
                    DbCallback.ExecuteNonQuery();
                }
                else
                {
                    Exception _ex1 = null;
                    SqlTransaction tx;
                    SqlConnection conn = new SqlConnection();
                    conn.ConnectionString = DbCallback.ConnectionString;
                    if (conn.State == ConnectionState.Open) conn.Close();
                    conn.Open();
                    tx = conn.BeginTransaction();
                    try
                    {
                        stm = "UPDATE tbUser SET IsActive1='Y', ExpireDate=@ExpireDate WHERE UserName=@UserName";
                        DbCallback.SetCommandText(stm);
                        DbCallback.AddInputParameter("@UserName", SqlDbType.NVarChar, userName);
                        DbCallback.AddInputParameter("@ExpireDate", SqlDbType.DateTime, expireDate);
                        DbCallback.ExecuteNonQuery(conn, tx);

                        tx.Commit();
                    }
                    catch (Exception ex1)
                    {
                        tx.Rollback();
                        _ex1 = ex1;
                    }
                    conn.Close();
                    if (_ex1 != null) throw _ex1;
                }
            }
            catch (Exception ex) { throw ex; }
        }

        public string Encrypt(string key, string str)
        {
            try
            {
                byte[] CurrentIV = new byte[] { 51, 52, 53, 54, 55, 56, 57, 58 };
                byte[] CurrentKey = { };
                if (key.Length == 8)
                {
                    CurrentKey = Encoding.ASCII.GetBytes(key);
                }
                else if (key.Length > 8)
                {
                    CurrentKey = Encoding.ASCII.GetBytes(key.Substring(0, 8));
                }
                else
                {
                    string AddString = key.Substring(0, 1);
                    int TotalLoop = 8 - Convert.ToInt32(key.Length);
                    string tmpKey = key;

                    for (int i = 1; i <= TotalLoop; i++)
                    {
                        tmpKey = tmpKey + AddString;
                    }
                    CurrentKey = Encoding.ASCII.GetBytes(tmpKey);
                }

                DESCryptoServiceProvider desCrypt = new DESCryptoServiceProvider();
                desCrypt.IV = CurrentIV;
                desCrypt.Key = CurrentKey;

                MemoryStream ms = new MemoryStream();
                ms.Position = 0;

                ICryptoTransform ce = desCrypt.CreateEncryptor();
                CryptoStream cs = new CryptoStream(ms, ce, CryptoStreamMode.Write);
                byte[] arrByte = Encoding.ASCII.GetBytes(str);
                cs.Write(arrByte, 0, arrByte.Length);
                cs.FlushFinalBlock();
                cs.Close();

                return Convert.ToBase64String(ms.ToArray());
            }
            catch (Exception ex) { throw ex; }
        }

        public bool Login(string userName, string pwd, out int currentAuthentication)
        {
            try
            {
                stm = "SELECT * FROM tbUser WHERE (UserName=@UserName) AND (Password=@Password) AND (IsActive='Y')";

                DbCallback.SetCommandText(stm);
                DbCallback.AddInputParameter("@UserName", SqlDbType.NVarChar, userName);
                DbCallback.AddInputParameter("@Password", SqlDbType.NVarChar, pwd);
                DataSet ds = DbCallback.ExecuteToDataSet();
                if (ds != null)
                    if (ds.Tables != null)
                        if (ds.Tables.Count > 0)
                            if (ds.Tables[0].Rows != null)
                                if (ds.Tables[0].Rows.Count > 0)
                                {
                                    currentAuthentication = Convert.ToInt32(ds.Tables[0].Rows[0]["Auth"]);
                                    return true;
                                }
                currentAuthentication = 0;
                return false;
            }
            catch (Exception ex)
            {
                DbCallback.CloseConnection();
                throw ex;
            }
        }
    }
}