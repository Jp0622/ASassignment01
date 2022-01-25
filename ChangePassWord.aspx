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
    //密码 校验
    function passwordvalidate() {
        var password = document.getElementById('<%=PassWord.ClientID%>').value;
        //长度校验
        if (password.length < 12) {

            document.getElementById("pwdchecker").innerHTML = "长度小于12";
            document.getElementById("pwdchecker").style.color = "red";
            return false;

        }
        //数字校验
        if (password.search(/[0-9]/) == -1) {

            document.getElementById("pwdchecker").innerHTML = "缺少数字";
            document.getElementById("pwdchecker").style.color = "red";
            return false;
        }
        //大写字母校验
        if (password.search(/[A-Z]/) == -1) {

            document.getElementById("pwdchecker").innerHTML = "缺少大写字母";
            document.getElementById("pwdchecker").style.color = "red";
            return false;
        }
        //小写字母校验
        if (password.search(/[a-z]/) == -1) {

            document.getElementById("pwdchecker").innerHTML = "缺少小写字母";
            document.getElementById("pwdchecker").style.color = "red";
            return false;
        }
        //特殊字符校验
        if (password.search(/[\.@#\$%&]/) == -1) {

            document.getElementById("pwdchecker").innerHTML = "缺少特殊字符";
            document.getElementById("pwdchecker").style.color = "red";
            return false;
        }
        document.getElementById("pwdchecker").innerHTML = "正确";
        document.getElementById("pwdchecker").style.color = "blue";
        return true;
    }

    function confirmvalidate() {
        if (document.getElementById('<%=PassWord.ClientID%>').value != document.getElementById('<%=ConfirmPassWord.ClientID%>').value) {
            document.getElementById("confirmpwdchecker").innerHTML = "两次密码不一致";
            document.getElementById("confirmpwdchecker").style.color = "red";
            return false;
        }
        document.getElementById("confirmpwdchecker").innerHTML = "正确";
        document.getElementById("confirmpwdchecker").style.color = "blue";
    }
</script>
