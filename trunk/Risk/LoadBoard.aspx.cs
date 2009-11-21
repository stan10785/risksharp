using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using RiskLib;
using System.Drawing;

namespace Risk
{    
    public partial class LoadBoard : System.Web.UI.Page
    {
        RiskGame Game 
        {
            /// Store game in session so it retains its object instance

            get { return (RiskGame)Session["Game"]; }
            set { Session["Game"] = value; }
        }
        
        // Dynamically created controls
        Dictionary<string, LinkButton> TerritoryLinks;


        protected void Page_Init(object sender, EventArgs e)
        {   
            /// Create the board controls at runtime
            
            CreateBoard();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            /// Add events to controls created at runtime
            
            foreach (LinkButton lb in TerritoryLinks.Values)
            {
                lb.Click += TerritoryClick;
            }

            if (!Page.IsPostBack)
            {
                /// Test script

                Game = new RiskGame(Server.MapPath("Risk.xml"));
                Game.AddPlayer("Joe");
                Game.AddPlayer("Buddy");
                Game.AddPlayer("Gus");
                Game.AssignTerritoriesRandomly(new Random());

                UpdateLabels();
            }
        }

        protected void TerritoryClick(object sender, EventArgs e)
        {
            LinkButton lb = (LinkButton)sender;
            Game.TerritorySelected(lb.CommandArgument);

            UpdateLabels();
        }

        #region <UI methods>

        private void CreateBoard()
        {
            TerritoryLinks = new Dictionary<string, LinkButton>();

            RiskBoard EmptyBoard = new RiskBoard(Server.MapPath("Risk.xml"));

            foreach (BoardTerritory t in EmptyBoard.Territories
                                            .OrderBy(n => n.Name)
                                            .OrderBy(n => n.Continent.Name))
            {
                LinkButton lb = new LinkButton();
                lb.ID = t.Name;
                lb.CommandArgument = t.Name;
                lb.Text = "0";
                lb.CssClass = GetCssClass(t.Name);
                lb.ToolTip = t.Name;
                
                TerritoryLinks.Add(t.Name, lb);
                PlaceHolder2.Controls.Add(lb);
            }
        }

        private void UpdateLabels()
        {
            StateLabel.Text = Game.State.ToString();
            TurnStateLabel.Text = Game.TurnState();
            PlayersLabel.Text = Game.PlayersAsList();

            AddPlayerLb.Visible = Game.CanAddPlayer();
            AddPlayerTb.Visible = Game.CanAddPlayer();
            AssignTerrLb.Visible = Game.CanRandomlyAssignTerritories();
            EndAttackLb.Visible = Game.CanDoneAttacking();

            if (Game.TurnInProgress)
            {
                List<string> Selectable = Game.GetSelectable();

                foreach (LinkButton lb in TerritoryLinks.Values)
                {
                    if (Selectable.Contains(lb.CommandArgument))
                    {
                        lb.BorderColor = Color.White;
                        lb.BorderWidth = 3;
                    }
                    else
                    {
                        lb.BorderWidth = 0;
                    }
                }
            }

            // 'Game Board' Labels

            foreach (BoardTerritory t in Game.Board.Territories)
            {
                try
                {
                    PlayerTerritory pt = Game.PlayerTerritories.Where(x => x.boardTerritory.Name == t.Name).Single();
                    //TerritoryLinks[t.Name].CssClass = GetCssClass(t.Name);
                    //PlayerNameLabels[t.Name].Text = pt.Player.Name;
                    TerritoryLinks[t.Name].Text = pt.Troops.ToString();
                    TerritoryLinks[t.Name].ToolTip = pt.boardTerritory.Name + " - " + pt.Player.Name;
                    TerritoryLinks[t.Name].BackColor = pt.Player.color;
                }
                catch
                {
                    TerritoryLinks[t.Name].Text = "0";
                    //TerritoryLinks[t.Name].CssClass = GetCssClass(t.Name) + " empty";
                    //TroopLabels[t.Name].Text = "0";
                    //((TableRow)PlayerNameLabels[t.Name].Parent.Parent).BackColor = Color.White;
                }
            }
        }

        private string GetCssClass(string name)
        {
            return name.ToLower().Replace(' ', '_');
        }

        #endregion


        #region  <game setup, simple calls to the RiskGame object>

        protected void NewGame(object sender, EventArgs e)
        {
            Game = new RiskGame(Server.MapPath("Risk.xml"));

            UpdateLabels();
        }

        protected void AddPlayer(object sender, EventArgs e)
        {
            TextBox t = (TextBox)UpdatePanel1.FindControl("TextBox1");
            Game.AddPlayer(t.Text);

            UpdateLabels();
        }

        protected void AssignTerritories(object sender, EventArgs e)
        {
            Game.AssignTerritoriesRandomly(new Random(DateTime.Now.Second));

            UpdateLabels();
        }

        protected void EndAttack(object sender, EventArgs e)
        {
            Game.EndAttack();

            UpdateLabels();
        }

        #endregion

    }
}
