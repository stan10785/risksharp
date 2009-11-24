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
            get { return (RiskGame)Session["Game"]; }
            set { Session["Game"] = value; }
        } // Store game in session so it retains its object instance
        Dictionary<string, LinkButton> TerritoryLinks; // Dynamically created controls
        DateTime StartTime;

        private void DebugPrint(string s, bool b)
        {
            string ss;

            ss = b ? "IN " : "OUT ";
            ss += s + " ";
            ss += DateTime.Now.Subtract(StartTime).TotalMilliseconds.ToString() + " ms";
            StartTime = DateTime.Now;

            AiEventLa.Text = ss + "<br>" + AiEventLa.Text;
        }

        // Life Cycle Events

        protected void Page_Init(object sender, EventArgs e)
        {
            StartTime = DateTime.Now;

            /// Create the board controls at runtime
            
            CreateBoard();

            DebugPrint("Page_Init", false);
        }

        protected void Page_Load(object sender, EventArgs e) 
        {
            DebugPrint("Page_Load", true);

            /// Add events to controls created at runtime
            
            foreach (LinkButton lb in TerritoryLinks.Values)
            {
                lb.Click += TerritoryClick;
            }

            if (Page.IsPostBack)
            {
                Game.AiMoving += BeforeAiMove;
                Game.AiMoved += AfterAiMove;
            }
            else
            {
                InitializeGame();
            }

            DebugPrint("Page_Load", false);
        }

        protected void Page_LoadComplete(object sender, EventArgs e)
        {
            AiCheck();
        }

        private void AiCheck()
        {
            if (Game.IsAiTurn)
            {
                Game.ExecuteAiTurn();
            }

            UpdateLabels();
        }

        private void InitializeGame()
        {
            Game = new RiskGame(Server.MapPath("Risk.xml"));
            Game.AiMoving += BeforeAiMove;
            Game.AiMoved += AfterAiMove;

            /// Test script
            Game.AddPlayer("Joe");
            Game.AddAiPlayer("Buddy");
            Game.AddAiPlayer("Gus");
            Game.AssignTerritoriesRandomly(new Random());
            AiCheck();

            UpdateLabels();
        }


        // UI Methods
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
            DebugPrint("UpdateLabels", true);

            StateLabel.Text = Game.State.ToString();
            TurnStateLabel.Text = Game.TurnState;
            PlayersLabel.Text = Game.GetPlayersAsList();

            AddPlayerLb.Visible = Game.CanAddPlayer;
            AddPlayerTb.Visible = Game.CanAddPlayer;
            AssignTerrLb.Visible = Game.CanRandomlyAssignTerritories;
            EndAttackLb.Visible = Game.CanEndAttack;
            TurnInSetLb.Visible = Game.CanTurnInCards;
            BonusTroopsLa.Text = Game.CurrentBonusTroopLevel.ToString();

            if (Game.IsTurnInProgress)
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
                    TerritoryLinks[t.Name].Text = pt.Troops.ToString();
                    TerritoryLinks[t.Name].ToolTip = pt.boardTerritory.Name + " - " + pt.Player.Name;
                    TerritoryLinks[t.Name].BackColor = pt.Player.color;
                }
                catch
                {
                    TerritoryLinks[t.Name].Text = "0";
                }
            }

            DebugPrint("UpdateLabels", false);
        }

        protected void BeforeAiMove(object sender, EventArgs e)
        {
        }
        protected void AfterAiMove(object sender, AiMoveEventArgs e)
        {
            AiEventLa.Text = e.Summary + "<br>" + AiEventLa.Text;
            AiCheck();
        }

        private string GetCssClass(string name)
        {
            return name.ToLower().Replace(' ', '_');
        }

        #endregion



        // UI Events
        #region  <game setup, simple calls to the RiskGame object>

        protected void NewGame(object sender, EventArgs e)
        {
            Game = new RiskGame(Server.MapPath("Risk.xml"));
        }

        protected void AddPlayer(object sender, EventArgs e)
        {
            TextBox t = (TextBox)UpdatePanel1.FindControl("TextBox1");
            Game.AddPlayer(t.Text);
        }

        protected void AssignTerritories(object sender, EventArgs e)
        {
            Game.AssignTerritoriesRandomly(new Random(DateTime.Now.Second));
        }

        protected void TerritoryClick(object sender, EventArgs e)
        {
            LinkButton Lb = (LinkButton)sender;
            Game.TerritorySelected(Lb.CommandArgument);
        }

        protected void EndAttack(object sender, EventArgs e)
        {
            Game.EndAttack();
        }

        protected void TurnInCards(object sender, EventArgs e)
        {
            Game.TurnInCards();
        }

        #endregion
    }
}
