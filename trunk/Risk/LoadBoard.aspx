<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="LoadBoard.aspx.cs" Inherits="Risk.LoadBoard" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title></title>
    <style type="text/css">
        
        body
        {
            margin: 0px;
            padding: 0px;	
            background-color: Black;
        }
        
        .status-panel
        {
        	position: fixed;
        	top: 0px;
        	right: 0px;
        	width: 100%;
        	height: 40px;
        	background-color: Black;
        	border-bottom: 1px solid orange;
        }
        .status-panel a
        {
            margin-left: 5px;
            margin-right: 5px;
            color: Blue;	
        }
        .map-outer-panel
        {
            height: 100%;
            width: 100%;
        	margin-top: 40px;
        	text-align: center;
        	
        }
        .map-inner-panel
        {
        	position: relative;
        	width: 1000px;
        	border: 1px solid orange;
        	border-top: none;
        }
        
        .map-inner-panel a
        {
        	font-family: Sans-Serif;
        	position: absolute;
        	color: #FFFFFF;
            text-decoration: none;
            font-weight: bold;
            width: 25px;
            padding: 2px;
            vertical-align: middle;
        }
        
        .greenland              { top: 71px;  left: 370px;}
        .alaska                 { top: 73px;  left: 67px; }
        .quebec                 { top: 134px; left: 278px; }
        .northwest_territory    { top: 77px;  left: 157px; }
        .western_united_states  { top: 168px; left: 112px; }
        .eastern_united_states  { top: 189px; left: 195px; }
        .central_america        { top: 247px; left: 133px; }
        .alberta                { top: 124px; left: 137px; }
        .ontario                { top: 137px; left: 216px; }
        
        .iceland                { top: 108px; left: 425px; }
        .british_isles          { top: 193px; left: 404px; }
        .western_europe         { top: 237px; left: 440px; }
        .northern_europe        { top: 188px; left: 488px; }
        .southern_europe        { top: 224px; left: 525px; }
        .scandinavia            { top: 125px; left: 490px; }
        .ukraine                { top: 150px; left: 565px; }
        
        .middle_east            { top: 265px; left: 599px; }
        .india                  { top: 303px; left: 727px; }
        .china                  { top: 240px; left: 796px; }
        .afghanistan            { top: 192px; left: 660px; }
        .mongolia               { top: 168px; left: 806px; }
        .siam                   { top: 338px; left: 830px; }
        .ural                   { top: 119px; left: 671px; }
        .siberia                { top: 90px;  left: 734px; }
        .japan                  { top: 193px; left: 942px; }
        .kamchatka              { top: 54px;  left: 898px; }
        .irkutsk                { top: 123px; left: 797px; }
        .yakutsk                { top: 63px;  left: 805px; }
        
        .peru                   { top: 385px; left: 235px; }
        .venezuela              { top: 309px; left: 230px; }
        .brazil                 { top: 365px; left: 295px; }
        .argentina              { top: 448px; left: 246px; }
        
        .north_africa           { top: 358px; left: 466px; }
        .east_africa            { top: 395px; left: 607px; }
        .south_africa           { top: 497px; left: 544px; }
        .congo                  { top: 425px; left: 540px; }
        .egypt                  { top: 324px; left: 543px; }
        .madagascar             { top: 496px; left: 636px; }
        
        .western_australia      { top: 471px; left: 858px; }
        .eastern_australia      { top: 464px; left: 952px; }
        .new_guinea             { top: 375px; left: 920px; }
        .indonesia              { top: 391px; left: 826px; }
        
        .selectable
        {
        	font-size: 3.0em;
            border: 1px solid Red;	
        }
        
        /*
        .empty
        {
            background-color: Blue;	
        }
        .selectable
        {
            border: 3px solid green;
            background-color: Black;	
        }
        */
        
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <asp:ScriptManager ID="ScriptManager1" runat="server">
        </asp:ScriptManager>
        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
            <ContentTemplate>
                <asp:Panel ID="Panel1" runat="server" CssClass="map-outer-panel">
                <asp:Panel ID="MapInnerPanel" runat="server" CssClass="map-inner-panel">
                
                    <asp:PlaceHolder ID="PlaceHolder2" runat="server"></asp:PlaceHolder>
                    
                    <asp:Image ID="Image1" runat="server" ImageUrl="~/img/RiskBoard6.png" 
                       Width="1000"/>
                       
                </asp:Panel>
                </asp:Panel>
                
            
                <asp:Panel ID="StatusPanel" runat="server" CssClass="status-panel">
                    <asp:Label ID="StateLabel" runat="server" Text="Label" ForeColor="White"></asp:Label>
                    <asp:Label ID="TurnStateLabel" runat="server" Text="Label" ForeColor="White"></asp:Label>
                    <asp:Label ID="PlayersLabel" runat="server" Text="Label" ForeColor="White"></asp:Label>
                    
                    <asp:LinkButton ID="NewGameLb" runat="server" OnClick="NewGame">New Game</asp:LinkButton>
                    
                    <asp:TextBox ID="AddPlayerTb" runat="server">
                    </asp:TextBox><asp:LinkButton ID="AddPlayerLb" runat="server" OnClick="AddPlayer">Add Player</asp:LinkButton>
                    
                    <asp:LinkButton ID="AssignTerrLb" runat="server" OnClick="AssignTerritories">Randomly Assign Territories</asp:LinkButton>
                    
                    <asp:LinkButton ID="EndAttackLb" runat="server" OnClick="EndAttack">Done Attacking</asp:LinkButton>
                </asp:Panel>
                
                <asp:PlaceHolder ID="PlaceHolder1" runat="server" Visible="false"></asp:PlaceHolder>
                
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
    </form>
</body>
</html>
