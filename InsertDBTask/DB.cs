using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace InsertDBTask
{
    public class DB
    {
        private static SqlConnection _conection;

        private static void OpenConection()
        {
            try
            {
                //chuỗi kết nối
                var scon1 = @"Data Source=.;Initial Catalog=CellSave;Integrated Security=True";
                //  var scon1 = @"Data Source=.\SQLEXPRESS;Integrated Security=True;Connect Timeout=15;Encrypt=False;TrustServerCertificate=False"; 
                if (_conection == null)
                    _conection = new SqlConnection(scon1);
                if (_conection.State != ConnectionState.Open)
                    _conection.Open();
            }
            catch (Exception)
            {
                MessageBox.Show("Kết nối thất bại", "Lỗi");
            }
        }
        // ngắt kết nối
        private static void CloseConection()
        {
            if (_conection == null) return;
            if (_conection.State == ConnectionState.Open)
                _conection.Close();
        }

        //hàm chứa tên thủ tục và tham số
        private static SqlCommand BuildCommand(string proceduceName, SqlParameter[] sqlParameters)
        {
            var cmd = new SqlCommand
            {
                CommandText = proceduceName,
                Connection = _conection,
                CommandType = CommandType.StoredProcedure
            };
            foreach (var sqlParameter in sqlParameters)
            {
                cmd.Parameters.Add(sqlParameter);
            }
            return cmd;
        }

        private static SqlCommand BuildIntCommand(string proceduceName, SqlParameter[] parameters)
        {
            var cmd = BuildCommand(proceduceName, parameters);
            cmd.Parameters.Add(new SqlParameter("ReturnValue", SqlDbType.Int, 4, ParameterDirection.ReturnValue, false, 0, 0, string.Empty, DataRowVersion.Default, null));
            return cmd;
        }

        // hàm thực thi thủ tục và số dòng đã thực hiện (thành công >0, ngược lại)
        // hàm này được sử dụng lúc sửa dữ liệu, xóa dữ liệu...
        public static int ExecuteNonQuery(string proceduceName, SqlParameter[] parameters)
        {
            try
            {
                OpenConection();
                var cmd = BuildCommand(proceduceName, parameters);
                cmd.CommandType = CommandType.StoredProcedure;
                var rec = cmd.ExecuteNonQuery();
                CloseConection();
                return rec;
            }
            catch (Exception)
            {
                throw;
            }
        }

        //Hàm thực thi thủ tục và trả về một danh sách (DataTable)

        public static DataTable ExecuteQuery(string proceduceName, SqlParameter[] parameters)
        {
            try
            {
                OpenConection();
                using (var sqlDa = new SqlDataAdapter(BuildCommand(proceduceName, parameters)))
                {
                    using (var ds = new DataSet())
                    {
                        sqlDa.Fill(ds);
                        CloseConection();
                        return ds.Tables[0];
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

    }
}
