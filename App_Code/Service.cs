using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

// NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service" in code, svc and config file together.
public class Service : IService
{
    public string vratiPodatke(int value)
    {
        return string.Format("You entered: {0}", value);
    }

    public string obradiPodatke(string podatak)
    {
        return podatak;
    }

    public DataSet readMultipleSets(string sql)
    {
        SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["baza"].ToString());
        SqlCommand cmd = new SqlCommand();

        try
        {
            if (conn.State == ConnectionState.Closed) conn.Open();

            cmd = new SqlCommand(sql, conn);
            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = 800;
            SqlDataAdapter ad = new SqlDataAdapter(cmd);
            DataSet dt = new DataSet();
            ad.Fill(dt, "podaci");

            conn.Close();
            return dt;
        }
        catch
        {
            return null;
        }
        finally
        {
            conn.Close();
        }
    }

    public string renderDtToJason(DataTable dt)
    {
        System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
        serializer.MaxJsonLength = 2147483644;
        List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
        Dictionary<string, object> row;
        foreach (DataRow dr in dt.Rows)
        {
            row = new Dictionary<string, object>();
            foreach (DataColumn col in dt.Columns)
            {
                TypeCode yourTypeCode = Type.GetTypeCode(col.DataType);
                bool num = false; bool b = false;
                switch (yourTypeCode)
                {
                    case TypeCode.Byte:
                    case TypeCode.SByte:
                    case TypeCode.Int16:
                    case TypeCode.UInt16:
                    case TypeCode.Int32:
                    case TypeCode.UInt32:
                    case TypeCode.Int64:
                    case TypeCode.UInt64:
                    case TypeCode.Single:
                    case TypeCode.Double:
                    case TypeCode.Decimal:
                        num = true;
                        break;

                    default:    // TypeCode.DBNull, TypeCode.Char and TypeCode.Object
                        if (yourTypeCode == TypeCode.Boolean) b = true;
                        num = false;
                        break;
                }

                row.Add(col.ColumnName, num ? dr[col].ToString().Replace(',', '.') : b ? dr[col].ToString().ToLower() : dr[col].ToString());
            }
            rows.Add(row);
        }
        return serializer.Serialize(rows);
    }


}
