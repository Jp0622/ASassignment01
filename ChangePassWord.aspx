<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ChangePassWord.aspx.cs" Inherits="SITConnect.ChangePassWord" %>

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

                <asp:Label runat="server" Text="LastPassWord"> </asp:Label>
                <asp:TextBox runat="server" TextMode="Password" ID="LastPassWord" ></asp:TextBox>
                
            </div>
            <div>

                <asp:Label runat="server" Text="Password"> </asp:Label>
                <asp:TextBox runat="server" TextMode="Password" ID="PassWord" onkeyup="javascript:passwordvalidate()"></asp:TextBox>
                <asp:Label runat="server" ID="pwdchecker"></asp:Label>
                
            </div>
            
            <div>

                <asp:Label runat="server" Text="Confirm"> </asp:Label>
                <asp:TextBox runat="server" TextMode="Password" ID="ConfirmPassWord" onkeyup="javascript:confirmvalidate()"></asp:TextBox>
                <asp:Label runat="server" ID="confirmpwdchecker"></asp:Label>
                
            </div>
           
            <asp:Button runat="server" Text="Submit" ID="btn_Submit"  OnClick="btn_Submit_Click" />
        </div>
    </form>
</body>
</html>
<script>
    //Password validation
    function passwordvalidate() {
        var password = document.getElementById('<%=PassWord.ClientID%>').value;
        //Length
        if (password.length < 12) {

            document.getElementById("pwdchecker").innerHTML = "Length shorter than 12!";
            document.getElementById("pwdchecker").style.color = "red";
            return false;

        }
        //Number
        if (password.search(/[0-9]/) == -1) {

            document.getElementById("pwdchecker").innerHTML = "Require number!";
            document.getElementById("pwdchecker").style.color = "red";
            return false;
        }
        //Cap
        if (password.search(/[A-Z]/) == -1) {

            document.getElementById("pwdchecker").innerHTML = "Require capital letters!";
            document.getElementById("pwdchecker").style.color = "red";
            return false;
        }
        //Small
        if (password.search(/[a-z]/) == -1) {

            document.getElementById("pwdchecker").innerHTML = "Require small letters!";
            document.getElementById("pwdchecker").style.color = "red";
            return false;
        }
        //Special
        if (password.search(/[\!<>,.@#\$%&]/) == -1) {

            document.getElementById("pwdchecker").innerHTML = "Require special characters!";
            document.getElementById("pwdchecker").style.color = "red";
            return false;
        }
        document.getElementById("pwdchecker").innerHTML = "Correct format!";
        document.getElementById("pwdchecker").style.color = "blue";
        return true;
    }

    function confirmvalidate() {
        if (document.getElementById('<%=PassWord.ClientID%>').value != document.getElementById('<%=ConfirmPassWord.ClientID%>').value) {
            document.getElementById("confirmpwdchecker").innerHTML = "Password does not match!";
            document.getElementById("confirmpwdchecker").style.color = "red";
            return false;
        }
        document.getElementById("confirmpwdchecker").innerHTML = "Correct!";
        document.getElementById("confirmpwdchecker").style.color = "blue";
    }
</script>
