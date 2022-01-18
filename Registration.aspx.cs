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
    public partial class Registration : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }
        string MYDBConnectionString =System.Configuration.ConfigurationManager.ConnectionStrings["MYDBConnection"].ConnectionString;
        static string finalHash;
        static string imgurl="";
        static string salt;
        byte[] Key; 
        byte[] IV;

        protected void createAccount()
        {
            try
            {
                int count = 0;
                using (SqlConnection con = new SqlConnection(MYDBConnectionString))
                {
                 
                  
                    using (SqlCommand cmd = new SqlCommand("INSERT INTO " +
                        "Account(FirstName,LastName,Email,PasswordHash,PasswordSalt,IV,[Key],IsLock,ErrorLoginCount,DateofBirth,ImgUrl,[Card])" +
                        " VALUES(@FirstName, @LastName,@Email,@PasswordHash,@PasswordSalt,@IV,@Key,@IsLock,@ErrorLoginCount,@DateofBirth,@ImgUrl,@Card)"))
                    {
                        using (SqlDataAdapter sda = new SqlDataAdapter())
                        {
                            cmd.CommandType = CommandType.Text;
                            cmd.Parameters.AddWithValue("@FirstName", FirstName.Text.Trim());
                            cmd.Parameters.AddWithValue("@LastName", LastName.Text.Trim());
                  
                            cmd.Parameters.AddWithValue("@Email", Email.Text.Trim());
                            cmd.Parameters.AddWithValue("@PasswordHash", finalHash);
                            cmd.Parameters.AddWithValue("@PasswordSalt", salt);
                            cmd.Parameters.AddWithValue("@IV", Convert.ToBase64String(IV)); 
                            cmd.Parameters.AddWithValue("@Key", Convert.ToBase64String(Key));
                         
                            cmd.Parameters.AddWithValue("@IsLock", false);
                            cmd.Parameters.AddWithValue("@ErrorLoginCount",0);
                            cmd.Parameters.AddWithValue("@DateofBirth",Date.Text.Trim());
                            cmd.Parameters.AddWithValue("@ImgUrl",imgurl);
                            cmd.Parameters.AddWithValue("@Card", Convert.ToBase64String( encryptData(Card.Text.Trim())));
                            cmd.Connection = con; 
                            
                            con.Open();
                            count = cmd.ExecuteNonQuery();
                           
                            con.Close();
                        }
                    }
                }
                if (count == 1)
                {
                    Session["Email"] = Email.Text.Trim();
                    Response.Redirect("HomePage.aspx", false);
                }
                else
                {
                    ErrorMsg.Text = "Email creation failed";

                }
            }
            catch (Exception ex)
            {
                ErrorMsg.Text = ex.Message;
                
            }

        }

       
        private  bool IsEmail(string email)
        {
            Regex RegEmail = new Regex(@"^\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$");
            Match m = RegEmail.Match(email);
            return m.Success;
        }
        private bool IsPassWord(string password)
        {
            if (password.Length < 12)
            {
                pwdchecker.ControlStyle.ForeColor = System.Drawing.Color.Red;
                pwdchecker.Text = "Length shorter than 12";
                return false;
            }


            if (!new Regex(@"[0-9]").IsMatch(password))
            {
                pwdchecker.ControlStyle.ForeColor = System.Drawing.Color.Red;
                pwdchecker.Text = "Require number";
                return false;
            }

            if (!new Regex(@"[A-Z]").IsMatch(password))
            {
                pwdchecker.ControlStyle.ForeColor = System.Drawing.Color.Red;
                pwdchecker.Text = "Require capital letters";
                return false;
            }

            if (!new Regex(@"[a-z]").IsMatch(password))
            {
                pwdchecker.ControlStyle.ForeColor = System.Drawing.Color.Red;
                pwdchecker.Text = "Require small letters";
                return false;
            }
   
            if (!new Regex(@"[\.@#\$%&]").IsMatch(password))
            {
                pwdchecker.ControlStyle.ForeColor = System.Drawing.Color.Red;
                pwdchecker.Text = "Require special characters";
                return false;
            }
            return true;
        }

        protected void btn_Registration_Click(object sender, EventArgs e)
        {

            //Email validation
            var email = Email.Text.Trim();
            string pwd = PassWord.Text.ToString().Trim();
            if (!IsEmail(email))
            {
                emailchecker.ControlStyle.ForeColor = System.Drawing.Color.Red;
                emailchecker.Text = "Wrong email format";
                return;

            }
            if (!IsPassWord(pwd))
            {
                return;

            }


            using (SqlConnection con = new SqlConnection(MYDBConnectionString))
            {

                try
                {
                    con.Open();
                    DataSet dset = new DataSet();
                    SqlDataAdapter adapter = new SqlDataAdapter();
                    string sqlQuery = string.Format("SELECT * FROM ACCOUNT WHERE Email=@Email");
                    SqlCommand cmd = new SqlCommand(sqlQuery, con);
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@Email", Email.Text.Trim());
                  
                    adapter.SelectCommand = cmd;
                    adapter.Fill(dset);
                    if (dset.Tables[0].Rows.Count > 0)
                    {
                        //Account already exist
                        ErrorMsg.ControlStyle.ForeColor = System.Drawing.Color.Red;
                        ErrorMsg.Text = "Account already exist";
                        return;

                    }


                }//try 
                catch (Exception ex)
                {
                    ErrorMsg.Text = ex.Message;


                }
            }
            
            //Upload file
            if (Upload.HasFile)
            {
                var upload = Upload.FileName;

                string filename = Upload.FileName;
                string imgPath = "/images/" + filename;
                string path = Server.MapPath("") + imgPath;

                Upload.SaveAs(path);
                imgurl = imgPath;
            }
          
          


            //Generate random "salt"  
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            byte[] saltByte = new byte[8];

            //Fills array of bytes with a cryptographically strong sequence of random values.      rng.GetBytes(saltByte); 
            salt = Convert.ToBase64String(saltByte);

            SHA512Managed hashing = new SHA512Managed();

            string pwdWithSalt = pwd + salt;
            byte[] plainHash = hashing.ComputeHash(Encoding.UTF8.GetBytes(pwd));
            byte[] hashWithSalt = hashing.ComputeHash(Encoding.UTF8.GetBytes(pwdWithSalt));

            finalHash = Convert.ToBase64String(hashWithSalt);


            RijndaelManaged cipher = new RijndaelManaged();
            cipher.GenerateKey();
            Key = cipher.Key;
            IV = cipher.IV;

            createAccount();
        }

        protected byte[] encryptData(string data)
        {
            byte[] cipherText = null; 
            try
            {
                RijndaelManaged cipher = new RijndaelManaged();
                cipher.IV = IV; 
                cipher.Key = Key;
                ICryptoTransform encryptTransform = cipher.CreateEncryptor();
                byte[] plainText = Encoding.UTF8.GetBytes(data); 
                cipherText = encryptTransform.TransformFinalBlock(plainText, 0, plainText.Length);
            }
            catch (Exception ex)
            {
                ErrorMsg.Text = ex.Message;

            }
            finally { }
            return cipherText;
        }


    }
}