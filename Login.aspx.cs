using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web.Script.Serialization;
namespace SITConnect
{
    public partial class Login : System.Web.UI.Page
    {
        string MYDBConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["MYDBConnection"].ConnectionString;
        static string finalHash;
        static string salt;
        byte[] Key;
        byte[] IV;
        protected void Page_Load(object sender, EventArgs e)
        {

        }
        public bool ValidateCaptcha()
        {
            bool result = true;

            //When user submits the recaptcha form, the user gets a response POST parameter. 
            //captchaResponse consist of the user click pattern. Behaviour analytics! AI :) 
            string captchaResponse = Request.Form["g-recaptcha-response"];

            //To send a GET request to Google along with the response and Secret key.
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create
           ("https://www.google.com/recaptcha/api/siteverify?secret=6Levux0eAAAAAPNlO_BcU5LTpIqk_JOtZYZuvT8Y&response=" + captchaResponse);


            try
            {

                //Codes to receive the Response in JSON format from Google Server
                using (WebResponse wResponse = req.GetResponse())
                {
                    using (StreamReader readStream = new StreamReader(wResponse.GetResponseStream()))
                    {
                        //The response in JSON format
                        string jsonResponse = readStream.ReadToEnd();

                        //To show the JSON response string for learning purpose
                        //lbl_gScore.Text = jsonResponse.ToString();


                        JavaScriptSerializer js = new JavaScriptSerializer();

                        //Create jsonObject to handle the response e.g success or Error
                        //Deserialize Json
                        MyObject jsonObject = js.Deserialize<MyObject>(jsonResponse);

                        //Convert the string "False" to bool false or "True" to bool true
                        result = Convert.ToBoolean(jsonObject.success);//

                    }
                }

                return result;
            }
            catch (WebException ex)
            {
                throw ex;
            }
        }

        protected void btn_Login_Click(object sender, EventArgs e)
        {

            if (ValidateCaptcha())
            {   //Login
                var Account = Email.Text.Trim();
                var Password = PassWord.Text.Trim();

                if (string.IsNullOrEmpty(Account) || string.IsNullOrEmpty(Password))
                {
                    ErrorMsg.ForeColor = System.Drawing.Color.Red;
                    ErrorMsg.Text = "Account or password cannot be empty!";
                    return;
                }

                SHA512Managed hashing = new SHA512Managed();



                string dbHash = getDBHash(Account);
                string dbSalt = getDBSalt(Account);
                try
                {
                    if (dbSalt != null && dbSalt.Length > 0 && dbHash != null && dbHash.Length > 0)
                    {

                        //check if account is locked
                        if (islock(Account))

                        {
                            ErrorMsg.ForeColor = System.Drawing.Color.Red;
                            ErrorMsg.Text = "Account is locked!";
                            return;
                        }


                        string pwdWithSalt = Password + dbSalt;
                        byte[] hashWithSalt = hashing.ComputeHash(Encoding.UTF8.GetBytes(pwdWithSalt));
                        string userHash = Convert.ToBase64String(hashWithSalt);

                        if (userHash.Equals(dbHash))
                        {
                            var UserID = 0;
                            using (SqlConnection connection = new SqlConnection(MYDBConnectionString))
                            {
                                string sql = "select UserID FROM Account WHERE Email=@Email";
                                SqlCommand command = new SqlCommand(sql, connection);
                                command.Parameters.AddWithValue("@Email", Account);
                                try
                                {
                                    connection.Open();
                                    using (SqlDataReader reader = command.ExecuteReader())
                                    {

                                        while (reader.Read())
                                        {

                                            if (reader["UserID"] != null)
                                            {
                                                if (reader["UserID"] != DBNull.Value)
                                                {
                                                    UserID = Convert.ToInt32(reader["UserID"].ToString());

                                                }


                                            }
                                        }


                                    }


                                }
                                catch (Exception ex)
                                {
                                    throw new Exception(ex.ToString());
                                }

                                finally { connection.Close(); }
                            }




                            Session["UserID"] = UserID;
                            Response.Redirect("HomePage.aspx", false);
                        }
                        else
                        {
                            ErrorMsg.ForeColor = System.Drawing.Color.Red;
                            ErrorMsg.Text = "Userid or password is not valid. Please try again.";

                            erroLogin(Account);
                            return;
                            //Response.Redirect("Login.aspx");
                        }
                    }
                    else
                    {
                        ErrorMsg.ForeColor = System.Drawing.Color.Red;
                        ErrorMsg.Text = "Userid or password is not valid. Please try again.";
                        return;
                    }

                }
                catch (Exception ex)
                {
                    ErrorMsg.Text = ex.Message;

                }
                finally { }

            }
            else
            {
            }




        }
        /// <summary>
        /// go back to account lock checker
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        private bool islock(string email)
        {
            var flag = false;
            using (SqlConnection connection = new SqlConnection(MYDBConnectionString))
            {
                string sql = "select IsLock FROM Account WHERE Email=@Email";
                SqlCommand command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@Email", email);
                try
                {
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {

                        while (reader.Read())
                        {

                            if (reader["IsLock"] != null)
                            {
                                if (reader["IsLock"] != DBNull.Value)
                                {
                                    flag = Convert.ToBoolean(reader["IsLock"].ToString());

                                }


                            }
                        }


                    }




                }
                catch (Exception ex)
                {
                    throw new Exception(ex.ToString());
                }

                finally
                {
                    connection.Close();


                }
                return flag;

            }

        }

        //error login function
        private void erroLogin(string email)
        {

            int count = 0;

            using (SqlConnection connection = new SqlConnection(MYDBConnectionString))
            {
                string sql = "select ErrorLoginCount FROM Account WHERE Email=@Email";
                SqlCommand command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@Email", email);
                try
                {
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {

                        while (reader.Read())
                        {

                            if (reader["ErrorLoginCount"] != null)
                            {
                                if (reader["ErrorLoginCount"] != DBNull.Value)
                                {
                                    count = Convert.ToInt32(reader["ErrorLoginCount"].ToString());

                                }


                            }
                        }


                    }
                    //lock account;
                    if (count >= 3)
                    {
                       
                        SqlCommand cmd = new SqlCommand("Update ACCOUNT SET IsLock=1 WHERE Email=@Email", connection);
                        cmd.CommandType = CommandType.Text;

                        cmd.Parameters.AddWithValue("@Email", email);

                        cmd.ExecuteNonQuery();


                    }
                    else
                    {

                        SqlCommand cmd = new SqlCommand("Update ACCOUNT SET ErrorLoginCount=ErrorLoginCount+1 WHERE Email=@Email", connection);

                        cmd.CommandType = CommandType.Text;

                        cmd.Parameters.AddWithValue("@Email", email);

                        cmd.ExecuteNonQuery();

                    }





                }
                catch (Exception ex)
                {
                    throw new Exception(ex.ToString());
                }

                finally { connection.Close(); }
            }

            //Audit Log
            var UserID = 0;
            using (SqlConnection connection = new SqlConnection(MYDBConnectionString))
            {
                string sql = "select UserID FROM Account WHERE Email=@Email";
                SqlCommand command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@Email", email);
                try
                {
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {

                        while (reader.Read())
                        {

                            if (reader["UserID"] != null)
                            {
                                if (reader["UserID"] != DBNull.Value)
                                {
                                    UserID = Convert.ToInt32(reader["UserID"].ToString());

                                }


                            }
                        }


                    }


                    SqlCommand cmd = new SqlCommand("insert into AUDITLOG (UserID,Time,Event,Change) " +
                        "values (@UserID,@Time,@Event,@Change)", connection);
                    cmd.CommandType = CommandType.Text;

                    cmd.Parameters.AddWithValue("@UserID", UserID);
                    cmd.Parameters.AddWithValue("@Time", DateTime.Now);
                    cmd.Parameters.AddWithValue("@Event", "Login");
                    cmd.Parameters.AddWithValue("@Change", "Login failed");

                    cmd.ExecuteNonQuery();






                }
                catch (Exception ex)
                {
                    throw new Exception(ex.ToString());
                }

                finally { connection.Close(); }
            }


        }

        protected string getDBHash(string email)
        {

            string h = null;

            SqlConnection connection = new SqlConnection(MYDBConnectionString);
            string sql = "select PasswordHash FROM Account WHERE Email=@Email";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@Email", email);
            try
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader["PasswordHash"] != null)
                        {
                            if (reader["PasswordHash"] != DBNull.Value)
                            {
                                h = reader["PasswordHash"].ToString();
                            }
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }

            finally { connection.Close(); }
            return h;
        }





        protected string getDBSalt(string email)
        {
            string s = null;

            SqlConnection connection = new SqlConnection(MYDBConnectionString);
            string sql = "select PASSWORDSALT FROM ACCOUNT WHERE Email=@Email";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@Email", email);
            try
            {
                connection.Open();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader["PASSWORDSALT"] != null)
                        {
                            if (reader["PASSWORDSALT"] != DBNull.Value)
                            {
                                s = reader["PASSWORDSALT"].ToString();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally { connection.Close(); }
            return s;

        }

        private class MyObject
        {
            public string success { get;  set; }
            public List<string> ErrorMessage { get; set; }
        }
    }
}