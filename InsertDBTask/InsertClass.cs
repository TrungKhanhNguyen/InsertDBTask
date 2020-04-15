using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
namespace InsertDBTask
{
    public class InsertClass
    {
        public void InsertCELL(string ngaygio,string CD,string CR,string sender,string receiver,string cellID)
        {
            SqlParameter[] p =
           {
               new SqlParameter("@ngaygio",SqlDbType.NVarChar,250),
               new SqlParameter("@CD",SqlDbType.NVarChar,50),
               new SqlParameter("@CR",SqlDbType.NVarChar,50),
               new SqlParameter("@sender",SqlDbType.NVarChar,50),
               new SqlParameter("@receiver",SqlDbType.NVarChar,50),
               new SqlParameter("@cellID",SqlDbType.NVarChar,50),
           };
            p[0].Value = ngaygio;
            p[1].Value = CD;
            p[2].Value = CR;
            p[3].Value = sender;
            p[4].Value = receiver;
            p[5].Value = cellID;

            DB.ExecuteNonQuery("INSERTCellContent", p);
        }
    }
}
