using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SITConnect
{
    public partial class ChangePassWord : System.Web.UI.Page
    {
       
        string MYDBConnectionString =System.Configuration.ConfigurationManager.ConnectionStrings["MYDBConnection"].ConnectionString;

       
        int UserID;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserID"] != null)
            {
                UserID = Convert.ToInt32(Session["UserID"].ToString());
            }
            else
            {
                Response.Redirect("/Login.aspx");
            }
        }
        protected void changePassWord()
        {
            try
            {

                var pwd = PassWord.Text.Trim();
                RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
                byte[] saltByte = new byte[8];

                //Fills array of bytes with a cryptographically strong sequence of random values.      rng.GetBytes(saltByte); 
                var  salt = Convert.ToBase64String(saltByte);

                SHA512Managed hashing = new SHA512Managed();


                string pwdWithSalt = pwd + salt;
                byte[] plainHash = hashing.ComputeHash(Encoding.UTF8.GetBytes(pwd));
                byte[] hashWithSalt = hashing.ComputeHash(Encoding.UTF8.GetBytes(pwdWithSalt));

                var    finalHash = Convert.ToBase64String(hashWithSalt);
                int count = 0;
                using (SqlConnection con = new SqlConnection(MYDBConnectionString))
                {
                 
                  
                    using (SqlCommand cmd = new SqlCommand("Update Account set PasswordHash=@PasswordHash , PasswordSalt=@PasswordSalt WHERE UserID=@UserID"))
                    {
                        using (SqlDataAdapter sda = new SqlDataAdapter())
                        {
                            cmd.CommandType = CommandType.Text;
                        
                            cmd.Parameters.AddWithValue("@UserID",UserID);
                            cmd.Parameters.AddWithValue("@PasswordHash", finalHash);
                            cmd.Parameters.AddWithValue("@PasswordSalt", salt);
                           
                            cmd.Connection = con; 
                            
                            con.Open();
                            count = cmd.ExecuteNonQuery();
                           
                            con.Close();
                        }
                    }
                }
                if (count == 1)
                {

                    //Change password history
                   
                   
                    using (SqlConnection connection = new SqlConnection(MYDBConnectionString))
                    {
                       
                        try
                        {
                            connection.Open();
                          

                            SqlCommand cmd = new SqlCommand("insert into PASSWORDCHANGE (UserID,PasswordHash,PasswordSalt) " +
                                "values (@UserID,@PasswordHash,@PasswordSalt)", connection);
                            cmd.CommandType = CommandType.Text;

                            cmd.Parameters.AddWithValue("@UserID", UserID);
                            cmd.Parameters.AddWithValue("@PasswordHash", finalHash);
                            cmd.Parameters.AddWithValue("@PasswordSalt", salt);
                            
                            cmd.ExecuteNonQuery();


                        }
                        catch (Exception ex)
                        {
                            throw new Exception(ex.ToString());
                        }

                        finally { connection.Close(); }
                    }


                    //Change password time
                    using (SqlConnection connection = new SqlConnection(MYDBConnectionString))
                    {

                        try
                        {
                            connection.Open();


                            SqlCommand cmd = new SqlCommand("UPDATE ACCOUNT SET LastChangeDate=@LastChangeDate WHERE UserID=@UserID", connection);
                            cmd.CommandType = CommandType.Text;

                            cmd.Parameters.AddWithValue("@UserID", UserID);
                        
                            cmd.Parameters.AddWithValue("@LastChangeDate", DateTime.Now);

                            cmd.ExecuteNonQuery();


                        }
                        catch (Exception ex)
                        {
                            throw new Exception(ex.ToString());
                        }

                        finally { connection.Close(); }
                    }

                    Session.Clear();
                    Session.Abandon();
                    Session.RemoveAll();
                    Response.Redirect("Login.aspx");
                }
                else
                {
                    ErrorMsg.Text = "Fail to change password!";

                }
            }
            catch (Exception ex)
            {
                ErrorMsg.Text = ex.Message;
                
            }

        }


        private bool IsPassWord(string password)
        {
            if (password.Length < 12)
            {
                pwdchecker.ControlStyle.ForeColor = System.Drawing.Color.Red;
                pwdchecker.Text = "Length shorter than 12!";
                return false;
            }


            if (!new Regex(@"[0-9]").IsMatch(password))
            {
                pwdchecker.ControlStyle.ForeColor = System.Drawing.Color.Red;
                pwdchecker.Text = "Require number!";
                return false;
            }

            if (!new Regex(@"[A-Z]").IsMatch(password))
            {
                pwdchecker.ControlStyle.ForeColor = System.Drawing.Color.Red;
                pwdchecker.Text = "Require capital letters!";
                return false;
            }

            if (!new Regex(@"[a-z]").IsMatch(password))
            {
                pwdchecker.ControlStyle.ForeColor = System.Drawing.Color.Red;
                pwdchecker.Text = "Require small letters!";
                return false;
            }
   
            if (!new Regex(@"[!<>,\.@#\$%&]").IsMatch(password))
            {
                pwdchecker.ControlStyle.ForeColor = System.Drawing.Color.Red;
                pwdchecker.Text = "Require special characters!";
                return false;
            }
            return true;
        }

        protected void btn_Submit_Click(object sender, EventArgs e)
        {

          
            string pwd = PassWord.Text.ToString().Trim();

            if (!IsPassWord(pwd))
            {
                return;

            }
        


            SHA512Managed hashing = new SHA512Managed();


            //avoid reuse

         
            using (SqlConnection con = new SqlConnection(MYDBConnectionString))
            {

                try
                {
                    con.Open();
                    DataSet dset = new DataSet();
                    SqlDataAdapter adapter = new SqlDataAdapter();
                    string sqlQuery = string.Format("SELECT  PasswordHash,PASSWORDSALT  FROM ACCOUNT WHERE UserID=@UserID");
                    SqlCommand cmd = new SqlCommand(sqlQuery, con);
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@UserID", UserID);

                    adapter.SelectCommand = cmd;
                    adapter.Fill(dset);
                    if (dset.Tables[0].Rows.Count > 0)
                    {
                       
                            string dbHash = dset.Tables[0].Rows[0]["PasswordHash"].ToString();
                            string dbSalt = dset.Tables[0].Rows[0]["PASSWORDSALT"].ToString();
                            string pwdWithSalt = LastPassWord.Text.Trim() + dbSalt;
                            byte[] hashWithSalt = hashing.ComputeHash(Encoding.UTF8.GetBytes(pwdWithSalt));
                            string userHash = Convert.ToBase64String(hashWithSalt);

                            if (!userHash.Equals(dbHash))
                            {

                                ErrorMsg.Text = "Old password does not match!";
                                return;

                            }

                      

                    }



                }
                catch (Exception ex)
                {
                    ErrorMsg.Text = ex.Message;
                    return;

                }
            }




            //Check if new password is same as old password
            using (SqlConnection con = new SqlConnection(MYDBConnectionString))
            {

                try
                {
                    con.Open();
                    DataSet dset = new DataSet();
                    SqlDataAdapter adapter = new SqlDataAdapter();
                    string sqlQuery = string.Format("SELECT top 2 PasswordHash,PASSWORDSALT  FROM PassWordChange WHERE UserID=@UserID ORDER BY ID desc");
                    SqlCommand cmd = new SqlCommand(sqlQuery, con);
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@UserID", UserID);
                  
                    adapter.SelectCommand = cmd;
                    adapter.Fill(dset);
                    if (dset.Tables[0].Rows.Count > 0)
                    {
                        foreach (DataRow row in dset.Tables[0].Rows)
                        {
                            string dbHash = row["PasswordHash"].ToString();
                            string dbSalt = row["PASSWORDSALT"].ToString();
                            string pwdWithSalt = pwd + dbSalt;
                            byte[] hashWithSalt = hashing.ComputeHash(Encoding.UTF8.GetBytes(pwdWithSalt));
                            string userHash = Convert.ToBase64String(hashWithSalt);

                            if (userHash.Equals(dbHash))
                            {

                                ErrorMsg.Text = "New password cannot be the same as old password！";
                                return;

                            }

                        }

                    }
                   


                }
                catch (Exception ex)
                {
                    ErrorMsg.Text = ex.Message;
                    return;

                }
            }

            //check change password is over 5min
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

                            if ((DateTime.Now - LastChangeDate).TotalMinutes < 5)
                            {
                                ErrorMsg.Text = "You must wait longer to change or password!";
                                return;
                            }
                        }

                    }



                }
                catch (Exception ex)
                {
                    ErrorMsg.Text = ex.Message;
                    return;

                }
            }

            changePassWord();

        }

  
    }
}