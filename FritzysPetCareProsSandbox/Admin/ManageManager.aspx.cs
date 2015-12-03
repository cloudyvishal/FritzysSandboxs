﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using BCL.Users.Management;
using BCL.Utility.CommonMethods;

namespace FritzysPetCareProsSandbox.Admin
{
    public partial class ManageManager : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                Bind();
            }
        }

        public void ErrorMessage(string Message)
        {
            divError.Visible = true;

            lblError.Attributes.Add("Class", "errorTable");

            lblError.Visible = true;

            lblError.Text = Message;
        }

        public void SuccessMessage(string Message)
        {
            divError.Visible = true;

            lblError.Attributes.Add("Class", "successTable");

            lblError.Visible = true;

            lblError.Text = Message;
        }

        private string SortExpression
        {
            get
            {
                if (ViewState["SortExpression"] != null)
                    return (string)ViewState["SortExpression"];
                else
                    return string.Empty;
            }
            set
            {
                if (ViewState["SortExpression"] == null)
                    ViewState.Add("SortExpression", value);
                else
                    ViewState["SortExpression"] = value;
            }
        }

        private string SortDirection
        {
            get
            {
                if (ViewState["SortDirection"] != null)
                    return (string)ViewState["SortDirection"];
                else
                    return "ASC";
            }
            set
            {
                if (ViewState["SortDirection"] == null)
                    ViewState.Add("SortDirection", value);
                else
                    ViewState["SortDirection"] = value;
            }
        }

        public void Bind()
        {
            User ObjUser = null;

            DataSet ds = null;

            DataTable dt = null;

            DataView dv = null;

            try
            {
                ObjUser = new User();

                ds = new DataSet();

                dt = new DataTable();

                dv = new DataView();

                ds = ObjUser.GetAllManager(Request.QueryString["SearchFor"].ToString(), Request.QueryString["SearchText"].ToString());

                ddlSearch.SelectedValue = Request.QueryString["SearchFor"].ToString();

                txtSearch.Text = Request.QueryString["SearchText"].ToString();

                if (ds.Tables[0].Rows.Count > 0)
                {
                    GrdUser.Visible = true;

                    dt = ds.Tables[0];

                    dv = dt.DefaultView;

                    if ((SortExpression != string.Empty) && (SortDirection != string.Empty))
                        dv.Sort = SortExpression + " " + SortDirection;

                    GrdUser.DataSource = dv;

                    GrdUser.DataBind();

                    CheckAll();

                    check();

                    CommonFunctions.Setserial(GrdUser, "srno");

                    divsearch.Visible = true;
                }
                else
                {
                    if ((Convert.ToInt32(ddlSearch.SelectedIndex) > 0) && (txtSearch.Text != ""))
                    {
                        txtSearch.Text = "";
                        
                        ddlSearch.SelectedIndex = 0;
                        
                        lnkNorec.Visible = true;

                        ErrorMessage("Sorry no recordes avalible");
                    }

                    divsearch.Visible = false;
                    
                    GrdUser.Visible = false;
                    
                    ErrorMessage("Sorry no recordes avalible");
                }
            }
            finally
            {
                ObjUser = null;
            }
        }

        public void CheckAll()
        {
            CheckBox chkall;
            chkall = (CheckBox)GrdUser.HeaderRow.FindControl("chkall");
            chkall.Attributes.Add("onclick", "checkall()");
            string str;
            str = "function checkall()";
            str = str + "{if(document.getElementById('" + chkall.ClientID + "').checked==true)";
            str = str + "{";
            foreach (GridViewRow gvr in GrdUser.Rows)
            {
                CheckBox checkall;
                checkall = (CheckBox)gvr.FindControl("chkSelect");
                str = str + "document.getElementById('" + checkall.ClientID + "').checked=true;";
            }
            str = str + "}";
            str = str + "else";
            str = str + "{";
            foreach (GridViewRow grv in GrdUser.Rows)
            {
                CheckBox chk1;
                chk1 = (CheckBox)grv.FindControl("chkSelect");
                str = str + "document.getElementById('" + chk1.ClientID + "').checked=false;";
            }
            str = str + "}}";
            Page.ClientScript.RegisterStartupScript(GetType(), "sss", str, true);
        }

        protected void check()
        {
            string checkid = "";
            checkid += "function val(id){";
            checkid += "var flg=0;";

            foreach (GridViewRow grv in GrdUser.Rows)
            {
                CheckBox chk1;
                chk1 = (CheckBox)grv.FindControl("chkSelect");
                checkid += "if(document.getElementById('" + chk1.ClientID + "').checked==true){";
                checkid += "flg=1;}";
            }
            checkid += "if(flg!=1){";
            checkid += "alert('Select Atleast One User');return false;}";
            checkid += "if(flg==1 && id==1){";
            checkid += "if(!confirm('Do You Want To Delete Selected User(s) ?')){return false;}}";
            checkid += "if(flg==1 && id==2){";
            checkid += "if(!confirm('Do You Want To Change Status of User(s) ?')){return false;}}";
            checkid = checkid + "}";
            Page.ClientScript.RegisterClientScriptBlock(GetType(), "myscript2", checkid, true);

            btnDelete.Attributes.Add("onclick", "return val(1);");
            btnStatus.Attributes.Add("onclick", "return val(2);");
        }

        protected void btnDelete_Click(object sender, EventArgs e)
        {
            string UserID = CommonFunctions.GetCheckedRow(GrdUser, "chkSelect");

            if (UserID != "")
            {
                User ObjUser = new User();

                ObjUser.DeleteUser(UserID);

                Bind();
            }
            SuccessMessage("User(s) deleted successfully");
        }

        protected void btnStatus_Click(object sender, EventArgs e)
        {
            string UserID = CommonFunctions.GetCheckedRow(GrdUser, "chkSelect");

            if (UserID != "")
            {
                User ObjUser = new User();

                ObjUser.ChangeUserStatus(UserID);

                Bind();
            }
        }

        protected void GrdUser_Sorting(object sender, GridViewSortEventArgs e)
        {
            if (SortExpression != e.SortExpression)
            {
                SortExpression = e.SortExpression;
                SortDirection = "ASC";
            }
            else
            {
                if (SortDirection == "ASC")
                {
                    SortDirection = "DESC";
                }
                else
                {
                    SortDirection = "ASC";
                }
            }
            GrdUser.PageIndex = 0;
            Bind();
        }

        protected void GrdUser_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            GrdUser.PageIndex = e.NewPageIndex;

            Bind();
        }

        protected void GrdUser_DataBound(object sender, EventArgs e)
        {
            GridViewRow gvr = GrdUser.BottomPagerRow;
            Label lb1 = (Label)gvr.Cells[0].FindControl("CurrentPage");
            lb1.Text = Convert.ToString(GrdUser.PageIndex + 1);
            int[] page = new int[7];
            page[0] = GrdUser.PageIndex - 2;
            page[1] = GrdUser.PageIndex - 1;
            page[2] = GrdUser.PageIndex;
            page[3] = GrdUser.PageIndex + 1;
            page[4] = GrdUser.PageIndex + 2;
            page[5] = GrdUser.PageIndex + 3;
            page[6] = GrdUser.PageIndex + 4;
            for (int i = 0; i < 7; i++)
            {
                if (i != 3)
                {
                    if (page[i] < 1 || page[i] > GrdUser.PageCount)
                    {
                        LinkButton lb = (LinkButton)gvr.Cells[0].FindControl("p" + Convert.ToString(i));
                        lb.Visible = false;
                    }
                    else
                    {
                        LinkButton lb = (LinkButton)gvr.Cells[0].FindControl("p" + Convert.ToString(i));
                        lb.Text = Convert.ToString(page[i]);

                        lb.CommandName = "PageNo";
                        lb.CommandArgument = lb.Text;

                    }
                }
            }
            if (GrdUser.PageIndex == 0)
            {
                LinkButton lb = (LinkButton)gvr.Cells[0].FindControl("LinkButton1");
                lb.Visible = false;
                lb = (LinkButton)gvr.Cells[0].FindControl("LinkButton2");
                lb.Visible = false;

            }
            if (GrdUser.PageIndex == GrdUser.PageCount - 1)
            {
                LinkButton lb = (LinkButton)gvr.Cells[0].FindControl("LinkButton3");
                lb.Visible = false;
                lb = (LinkButton)gvr.Cells[0].FindControl("LinkButton4");
                lb.Visible = false;

            }
            if (GrdUser.PageIndex > GrdUser.PageCount - 5)
            {
                Label lbmore = (Label)gvr.Cells[0].FindControl("nmore");
                lbmore.Visible = false;
            }
            if (GrdUser.PageIndex < 4)
            {
                Label lbmore = (Label)gvr.Cells[0].FindControl("pmore");
                lbmore.Visible = false;
            }
        }

        protected void GrdUser_RowCreated(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Pager)
            {
                GridViewRow gvr = e.Row;
                LinkButton lb = (LinkButton)gvr.Cells[0].FindControl("p0");
                lb.Command += new CommandEventHandler(lb_Command);
                lb = (LinkButton)gvr.Cells[0].FindControl("p1");
                lb.Command += new CommandEventHandler(lb_Command);
                lb = (LinkButton)gvr.Cells[0].FindControl("p2");
                lb.Command += new CommandEventHandler(lb_Command);
                lb = (LinkButton)gvr.Cells[0].FindControl("p4");
                lb.Command += new CommandEventHandler(lb_Command);
                lb = (LinkButton)gvr.Cells[0].FindControl("p5");
                lb.Command += new CommandEventHandler(lb_Command);
                lb = (LinkButton)gvr.Cells[0].FindControl("p6");
                lb.Command += new CommandEventHandler(lb_Command);
            }
        }

        void lb_Command(object sender, CommandEventArgs e)
        {
            GrdUser.PageIndex = Convert.ToInt32(e.CommandArgument) - 1;
            Bind();
        }

        protected void lnkNorec_Click(object sender, EventArgs e)
        {
            Response.Redirect("ManageManager.aspx?SearchFor=0&SearchText=");
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            Response.Redirect("ManageManager.aspx?SearchFor=" + ddlSearch.SelectedValue + "&SearchText=" + txtSearch.Text.Trim());
        }

        protected void btnViewall_Click(object sender, EventArgs e)
        {
            Response.Redirect("ManageManager.aspx?SearchFor=0" + "&SearchText=");
        }
    }
}