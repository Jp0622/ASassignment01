using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SITConnect
{
    public partial class AuditLog : System.Web.UI.Page
    {

        string MYDBConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["MYDBConnection"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {



            GetList();




        }


        private void GetList()
        {
            using (SqlConnection con = new SqlConnection(MYDBConnectionString))
            {

                try
                {
                    con.Open();
                    DataSet dset = new DataSet();
                    SqlDataAdapter adapter = new SqlDataAdapter();
                    string sqlQuery = string.Format("SELECT  Email,Event,Change,Time  from AuditLog a inner join ACCOUNT  b on  a.UserID=b.UserID");
                    SqlCommand cmd = new SqlCommand(sqlQuery, con);
                    cmd.CommandType = CommandType.Text;
                  

                    adapter.SelectCommand = cmd;
                    adapter.Fill(dset);
                    grid_auditlog.DataSource = dset.Tables[0];
                    grid_auditlog.DataBind();


                }
                catch (Exception ex)
                {
               
                }
            }


        }


    }
}