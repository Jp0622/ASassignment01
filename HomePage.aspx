<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="HomePage.aspx.cs" Inherits="SITConnect.HomePage" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <nav>
              <asp:LinkButton runat="server" ID="btn_link" Text="Change PassWord" PostBackUrl="/ChangePassWord.aspx"></asp:LinkButton>
            <br />
              <asp:LinkButton runat="server" ID="btn_log" Text="Audit Log" PostBackUrl="/AuditLog.aspx">Log</asp:LinkButton>
            <br />
                  <asp:Button runat="server" ID="btn_LogOut" Text="LogOut" OnClick="btn_LogOut_Click" />
        </nav>
        <h2>Welcome!!!</h2>
      
        <div>
          
        </div>
    </form>
</body>
</html>
