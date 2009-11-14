<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="LoadBoard.aspx.cs" Inherits="Risk.LoadBoard" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title></title>
    <style type="text/css">
        .status-panel
        {
        	position: fixed;
        	bottom: 1px;
        	right: 1px;
        	border: 1px solid black;
        	padding: 10px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <asp:ScriptManager ID="ScriptManager1" runat="server">
        </asp:ScriptManager>
        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
            <ContentTemplate>
                <asp:Panel ID="StatusPanel" runat="server" CssClass="status-panel">
                    <asp:Label ID="StateLabel" runat="server" Text="Label"></asp:Label><br />
                    <asp:Label ID="TurnStateLabel" runat="server" Text="Label"></asp:Label><br />
                    <asp:Label ID="PlayersLabel" runat="server" Text="Label"></asp:Label>
                    
                    <br /><br /><br />
                
                    <asp:LinkButton ID="LinkButton1" runat="server" OnClick="NewGame">New Game</asp:LinkButton><br />
                    
                    <asp:TextBox ID="TextBox1" runat="server">
                    </asp:TextBox><asp:LinkButton ID="LinkButton2" runat="server" OnClick="AddPlayer">Add Player</asp:LinkButton><br />
                    
                    <asp:LinkButton ID="LinkButton3" runat="server" OnClick="AssignTerritories">Randomly Assign Territories</asp:LinkButton>
                    <br />
                    <asp:LinkButton ID="LinkButton4" runat="server" OnClick="EndAttack">Done Attacking</asp:LinkButton>
                </asp:Panel>
                
                
                <asp:PlaceHolder ID="PlaceHolder1" runat="server"></asp:PlaceHolder>
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
    </form>
</body>
</html>
