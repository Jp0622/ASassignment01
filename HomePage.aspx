<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="HomePage.aspx.cs" Inherits="SITConnect.HomePage" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
</head>
<body>
    <form id="form1" runat="server">

        <h2>Welcome!!!</h2>
        <asp:LinkButton runat="server" ID="btn_link" Text="404 page" PostBackUrl="/404">404 page</asp:LinkButton>
        <div>
            <asp:Button runat="server" ID="btn_LogOut" Text="LogOut" OnClick="btn_LogOut_Click" />
        </div>
    </form>
</body>
</html>
