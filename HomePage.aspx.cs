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
    public partial class HomePage : System.Web.UI.Page
    {
        string MYDBConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["MYDBConnection"].ConnectionString;
        protected void Page_Load(object sender, EventArgs e)
        {


            if (Session["UserID"] == null)
            {
                Response.Redirect("Login.aspx");
            }
            else
            {

                //Maximum password time
                var UserID = Session["UserID"];


                using (SqlConnection con = new SqlConnection(MYDBConnectionString))
                {

                    try
                    {
                        con.Open();
                        DataSet dset = new DataSet();
                        SqlDataAdapter adapter = new SqlDataAdapter();
                        string sqlQuery = string.Format("SELECT  LastChangeDate  FROM ACCOUNT WHERE UserID=@UserID");
                        SqlCommand cmd = new SqlCommand(sqlQuery, con);
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.AddWithValue("@UserID", UserID);

                        adapter.SelectCommand = cmd;
                        adapter.Fill(dset);
                        if (dset.Tables[0].Rows.Count > 0)
                        {
                            if (!DBNull.Value.Equals(dset.Tables[0].Rows[0][0]))
                            {
                                var LastChangeDate = Convert.ToDateTime(dset.Tables[0].Rows[0][0]);

                                if ((DateTime.Now - LastChangeDate).TotalMinutes > 1)
                                {

                                    Response.Redirect("/ChangePassWord.aspx");
                                }
                            }

                        }



                    }
                    catch (Exception ex)
                    {

                    }
                }





            }

        }

        protected void btn_LogOut_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Session.Abandon();
            Session.RemoveAll();
            Response.Redirect("Login.aspx");
        }
    }
}