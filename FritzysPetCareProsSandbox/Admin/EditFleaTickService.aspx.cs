﻿using System;
using System.IO;
using System.Web;
using System.Data;
using System.Linq;
using System.Web.UI;
using System.Drawing;
using System.Web.UI.WebControls;
using System.Collections.Generic;

using BCL.Utility.Contents;
using BCL.Admin.HomeServices;
using BCL.Utility.RandomString;
using System.Configuration;

namespace FritzysPetCareProsSandbox.Admin
{
    public partial class EditFleaTickService : System.Web.UI.Page
    {
        int ServiceID;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Request.QueryString["ServiceID"] != "")
                {
                    try
                    {
                        ServiceID = int.Parse(Request.QueryString["ServiceID"]);

                        ViewState["ImageName"] = "";

                        BindService(ServiceID);
                    }
                    finally
                    {
 
                    }
                }
                else
                {
                    Response.Redirect("ManagesServices.aspx");
                }
            }
        }

        public void ErrMessage(string Message)
        {
            divError.Visible = true;
            lblError.Attributes.Add("Class", "errorTable");
            lblError.Visible = true;
            lblError.Text = Message;
        }
        public void SuccesfullMessage(string Message)
        {
            divError.Visible = true;
            lblError.Attributes.Add("Class", "successTable");
            lblError.Visible = true;
            lblError.Text = Message;
        }

        protected void BindService(int ServiceID)
        {
            HomeServices ObjService = null;

            DataSet ds = null;

            try
            {
                ObjService = new HomeServices();

                ds = new DataSet();

                ds = ObjService.GetFleatickServiceDetail(ServiceID);

                if (ds.Tables[0].Rows.Count > 0)
                {
                    txtServiceTitle.Text = ds.Tables[0].Rows[0]["Title"].ToString();

                    txtServiceDescription.Text = ds.Tables[0].Rows[0]["Description"].ToString();

                    if (ds.Tables[0].Rows[0]["ImageName"].ToString() != "")
                    {
                        ImgProduct.ImageUrl = ConfigurationManager.AppSettings["HomePathValue"] + "StoreData/HomeServices/" + ds.Tables[0].Rows[0]["ImageName"].ToString();

                        ViewState["ImageName"] = ds.Tables[0].Rows[0]["ImageName"].ToString();
                    }
                    else
                        ImgProduct.ImageUrl = ConfigurationManager.AppSettings["HomePathValue"] + "StoreData/Product/Not.jpg";
                }
                else
                {

                }
            }
            finally
            {
 
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            HomeServices ObjUpdate = null;

            HomeServices ObjService = null;

            DataSet ds = null;

            try
            {
                ObjUpdate = new HomeServices();

                string str = Request.QueryString["ServiceID"].ToString();

                int id = Convert.ToInt32(str);

                ObjUpdate.UpdateFleaandTickServices(id, txtServiceTitle.Text.Trim(), txtServiceDescription.Text.Trim(), ViewState["ImageName"].ToString());

                SuccesfullMessage("Updated successfully.");

                ObjService = new HomeServices();

                ds = new DataSet();

                ds = ObjService.GetFleatickServiceDetail(Convert.ToInt32(Request.QueryString["ServiceID"].ToString()));

                if (ds.Tables.Count > 0)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        Session["UserType"] = ds.Tables[0].Rows[0]["UserType"].ToString();
                    }
                }
            }
            finally
            {
                ObjUpdate = null;

                ObjService = null;

                ds.Dispose();
            }
        }

        protected void btnUpload_Click1(object sender, EventArgs e)
        {
            try
            {
                if (flUploadDetail.PostedFile != null)
                {
                    string ImageName2 = System.IO.Path.GetFileName(flUploadDetail.PostedFile.FileName);
                    if (ImageName2 != "")
                    {
                        string ext = System.IO.Path.GetExtension(flUploadDetail.FileName);
                        if (ImageName2 != "" && ext == ".gif" || ext == ".jpg" || ext == ".png" || ext == ".GIF" || ext == ".jpeg" || ext == ".JPEG" || ext == ".PNG" || ext == ".JPG")
                        {
                            string NewImageName = RandomStringsAndNumbers.Generate() + ext;

                            string virtualpath2 = ConfigurationManager.AppSettings["HomePathValue"] + "StoreData/HomeServices/" + NewImageName;

                            string fullpath2 = ContentManager.GetPhysicalPath(virtualpath2);


                            System.Drawing.Image image = System.Drawing.Image.FromStream(flUploadDetail.PostedFile.InputStream);

                            Bitmap myBitmap = ResizeImage(flUploadDetail.PostedFile.InputStream, 88, 144);

                            myBitmap.Save(fullpath2, System.Drawing.Imaging.ImageFormat.Jpeg);

                            myBitmap.Dispose();

                            ViewState["ImageName"] = NewImageName.ToString();

                            ImgProduct.Visible = true;

                            ImgProduct.ImageUrl = ConfigurationManager.AppSettings["HomePathValue"] + "StoreData/HomeServices/" + NewImageName;

                            divError.Visible = false;
                        }
                        else
                        { ErrMessage("File Format Not Supported."); }
                    }
                    else
                    {
                        ErrMessage("Please upload Image.");
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

        private Bitmap ResizeImage(Stream streamImage, int maxWidth, int maxHeight)
        {
            Bitmap originalImage = new Bitmap(streamImage);

            int sourceWidth = originalImage.Width;
            
            int sourceHeight = originalImage.Height;

            float nPercent = 0;
            
            float nPercentW = 0;
            
            float nPercentH = 0;

            nPercentW = ((float)maxWidth / (float)sourceWidth);
            
            nPercentH = ((float)maxHeight / (float)sourceHeight);


            if (nPercentH < nPercentW)
                nPercent = nPercentH;
            else
                nPercent = nPercentW;

            int destWidth = (int)(sourceWidth * nPercent);
            
            int destHeight = (int)(sourceHeight * nPercent);
            
            return new Bitmap(originalImage, destWidth, destHeight);
        }
    }
}