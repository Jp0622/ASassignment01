 <%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="SITConnect.Login" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>

    <script src="https://www.google.com/recaptcha/api.js?render=6Levux0eAAAAAECRcDxSIzSxSGw26EiaMSnbdZiT"></script>

</head>
<body>
    <form id="form1" runat="server">
        <asp:Label runat="server" ID="ErrorMsg"> </asp:Label>
        <div>
            <input type="hidden" id="g-recaptcha-response" name="g-recaptcha-response" />
            <div>

                <asp:Label runat="server" Text="Email address"></asp:Label>
                <asp:TextBox runat="server" ID="Email" onkeyup="javascript:emailvalidate()"></asp:TextBox>
                <asp:Label runat="server" ID="emailchecker"></asp:Label>
            </div>
            <div>

                <asp:Label runat="server" Text="Password"> </asp:Label>
                <asp:TextBox runat="server" TextMode="Password" ID="PassWord" onkeyup="javascript:passwordvalidate()"></asp:TextBox>
                <asp:Label runat="server" ID="pwdchecker"></asp:Label>

            </div>
            <div>
                <asp:Button runat="server" Text="Login" ID="btn_Login" OnClick="btn_Login_Click" />
            </div>
             <div>
                <asp:LinkButton ID="btn_reg" Text="Registration" PostBackUrl="~/Registration.aspx" runat="server"></asp:LinkButton>
            </div>
         
        </div>
    </form>

    <script> 
        grecaptcha.ready(function () {
            grecaptcha.execute('6Levux0eAAAAAECRcDxSIzSxSGw26EiaMSnbdZiT', { action: 'Login' }).then(function (token) {
                document.getElementById("g-recaptcha-response").value = token;
            });
        });
    </script>

</body>
</html>

<script> 

</script>

