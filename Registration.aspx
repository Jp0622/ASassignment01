<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Registration.aspx.cs" Inherits="SITConnect.Registration" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <asp:Label runat="server" ID="ErrorMsg"></asp:Label>
        <div>
            <div>

                <asp:Label runat="server" Text="First Name"></asp:Label>
                <asp:TextBox runat="server" ID="FirstName"></asp:TextBox>
            </div>
            <div>

                <asp:Label runat="server" Text="Last Name"></asp:Label>
                <asp:TextBox runat="server" ID="LastName"></asp:TextBox>
            </div>
            <div>

                <asp:Label runat="server" Text="Credit Card Info"></asp:Label>
                <asp:TextBox runat="server" ID="Card"></asp:TextBox>
            </div>
            <div>

                <asp:Label runat="server" Text="Email address" ></asp:Label>
                <asp:TextBox runat="server" ID="Email" onkeyup="javascript:emailvalidate()"></asp:TextBox>
                  <asp:Label runat="server" ID="emailchecker"></asp:Label>
            </div>
            <div>

                <asp:Label runat="server" Text="Password"> </asp:Label>
                <asp:TextBox runat="server" TextMode="Password" ID="PassWord" onkeyup="javascript:passwordvalidate()"></asp:TextBox>
                <asp:Label runat="server" ID="pwdchecker"></asp:Label>
                
            </div>
            <div>

                <asp:Label runat="server" Text="Date of Birth"></asp:Label>
                <asp:TextBox runat="server" ID="Date" TextMode="Date"></asp:TextBox>
            </div>
            <div>

                <asp:Label runat="server" Text="Image"> </asp:Label>
                <asp:FileUpload runat="server" ID="Upload" />
            </div>
            <asp:Button runat="server" Text="Registration" ID="btn_Registration"  OnClick="btn_Registration_Click" />
        </div>
    </form>
</body>
</html>
<script>
    //Password validation
    function passwordvalidate() {
        var password = document.getElementById('<%=PassWord.ClientID%>').value;
        //Length validation
        if (password.length < 12) {

            document.getElementById("pwdchecker").innerHTML = "Length shorter than 12";
            document.getElementById("pwdchecker").style.color = "red";
            return false;

        }
        //number validation
        if (password.search(/[0-9]/) == -1) {

            document.getElementById("pwdchecker").innerHTML = "Require number";
            document.getElementById("pwdchecker").style.color = "red";
            return false;
        }
        //Cap letter validation
        if (password.search(/[A-Z]/) == -1) {

            document.getElementById("pwdchecker").innerHTML = "Require capital letters";
            document.getElementById("pwdchecker").style.color = "red";
            return false;
        }
        //Small letter validation
        if (password.search(/[a-z]/) == -1) {

            document.getElementById("pwdchecker").innerHTML = "Require small letters";
            document.getElementById("pwdchecker").style.color = "red";
            return false;
        }
        //Special character validation
        if (password.search(/[\.@#\$%&]/) == -1) {

            document.getElementById("pwdchecker").innerHTML = "Require special characters";
            document.getElementById("pwdchecker").style.color = "red";
            return false;
        }
        document.getElementById("pwdchecker").innerHTML = "Correct";
        document.getElementById("pwdchecker").style.color = "blue";
        return true;
    }
    //if email matches

    function emailvalidate() {
        var email = document.getElementById('<%=Email.ClientID%>').value;
        var pattern = /^\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$/;

        //Email validation

        if (email.search(pattern) == -1) {
            document.getElementById("emailchecker").innerHTML = "Email format is wrong!";
            document.getElementById("emailchecker").style.color = "red";
            return false;

        }
        document.getElementById("emailchecker").innerHTML = "Email format is correct!";
        document.getElementById("emailchecker").style.color = "blue";
        return true;
    }

    
</script>
