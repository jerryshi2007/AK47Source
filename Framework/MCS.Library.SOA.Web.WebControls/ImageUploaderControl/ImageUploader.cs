using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using MCS.Web.Library.Script;
using MCS.Web.Library;
using MCS.Library.SOA.DataObjects;
using MCS.Library.Core;
using MCS.Library.Security;
using MCS.Library.Globalization;

[assembly: WebResource("MCS.Web.WebControls.ImageUploaderControl.ImageUploader.js", "application/x-javascript")]
[assembly: WebResource("MCS.Web.WebControls.ImageUploaderControl.ImageUploader.css", "text/css")]
[assembly: WebResource("MCS.Web.WebControls.ImageUploaderControl.noLogo.png", "image/png")]
[assembly: WebResource("MCS.Web.WebControls.ImageUploaderControl.defaultImage_bw.jpg", "image/jpeg")]

namespace MCS.Web.WebControls
{
	[RequiredScript(typeof(HBCommonScript), 2)]
	[RequiredScript(typeof(SubmitButtonScript), 8)]
	[ClientScriptResource("MCS.Web.WebControls.ImageUploader", "MCS.Web.WebControls.ImageUploaderControl.ImageUploader.js")]
	[ClientCssResource("MCS.Web.WebControls.ImageUploaderControl.ImageUploader.css")]
	public class ImageUploader : ScriptControlBase
	{
		private ImageProperty imgProp = null;

		private HtmlInputFile uploader;
		private HtmlImage imageControl;
		private HtmlInputButton btnUpload;
		private HtmlInputButton btnDelete;

		private HtmlGenericControl loading;

		private const string INNERFRAMEID = "__imageUploaderInnerFrame";
		private const string INNERFRAME_CONTAINER = "__imageUploaderInnerFrameContainer";

        private string postedControlID;

		public ImageUploader()
			: base(true, HtmlTextWriterTag.Span)
		{ }

		private HttpPostedFile GetPostFile(string inputName)
		{
			var files = HttpContext.Current.Request.Files;
			HttpPostedFile file = null;

			for (int i = 0; i < files.AllKeys.Count(); i++)
			{
				if (string.Compare(files.AllKeys[i], inputName, true) == 0)
				{
					var item = files[files.AllKeys[i]];
					string fileName = Path.GetFileName(item.FileName);

					if (!string.IsNullOrEmpty(fileName))
					{
						file = item;
						break;
					}
				}
			}

			return file;
		}

		protected override void OnInit(EventArgs e)
		{
            postedControlID = GetPostedControlID();

            if (ProcessShowImageRequest())
                return;

            if (ProcessUploadImageRequest())
                return;

            if (postedControlID != string.Empty && GetPostedControlID() != this.ClientID)
			{
				return;
			}
            
			base.OnInit(e);
		}

		private bool ProcessShowImageRequest()
		{
			string contentID = WebUtility.GetRequestQueryValue("imagePropID", "");
			bool result = contentID.IsNotEmpty();

			if (result)
			{
				OnBeforeDownload();

				string filePath = WebUtility.GetRequestQueryValue("filePath", "").Decrypt();
				bool cache = WebUtility.GetRequestQueryValue("cache", false);

				HttpContext.Current.Response.Clear();

				ShowImage(contentID, filePath, cache);
			}

			return result;
		}

		private bool ProcessUploadImageRequest()
		{
			string opText = WebUtility.GetRequestFormString("clientOPHidden", string.Empty);
			string op = !string.IsNullOrEmpty(opText) ? opText.Split(',')[0] : "";          //WebUtility.GetRequestFormString("clientOPHidden", string.Empty).Split(',')[0];
			string controlID = !string.IsNullOrEmpty(opText) ? opText.Split(',')[1] : ""; //WebUtility.GetRequestFormString("clientOPHidden", string.Empty).Split(',')[1];

			bool result = (op == "upload");
			if (result)
                UploadImage();

            return result;
		}

        private void UploadImage()
		{

			HttpResponse response = HttpContext.Current.Response;

			try
			{
				ImageProperty imageProperty = GetPostedImageProperty();
				string opText = WebUtility.GetRequestFormString("clientOPHidden", string.Empty);
				string op = !string.IsNullOrEmpty(opText) ? opText.Split(',')[0] : "";          //WebUtility.GetRequestFormString("clientOPHidden", string.Empty).Split(',')[0];
				string inputName = !string.IsNullOrEmpty(opText) ? opText.Split(',')[2] : ""; //WebUtility.GetRequestFormString("clientOPHidden", string.Empty).Split(',')[1];

				HttpPostedFile file = GetPostFile(inputName);

				if (file != null)
				{
                    if (this.FileMaxSize > 0 && file.ContentLength > this.FileMaxSize)
                    {
                        response.Write(GetResponseTextScript(string.Format("您上传的文件超过了{0}字节。", this.FileMaxSize)));
                        response.Write(GetClientControlInvokeStript(GetPostedControlID(), "uploadFail", "document.getElementById('responseInfo').value", ""));
                        return;
                    }

				    string filePath = string.Empty;

                    ImageUploadHelper.UploadFile(this, file, imageProperty.OriginalName, imageProperty.NewName, out filePath);
                    imageProperty.FilePath = filePath.Encrypt();

					string imgPropJsonStr = JSONSerializerExecute.Serialize(imageProperty);

                    string uploadImageShowenUrl = CurrentPageUrl + string.Format("?imagePropID={0}&filePath={1}", imageProperty.ID, filePath.Encrypt());

					response.Write(GetResponseTextScript(imgPropJsonStr));

                    response.Write(GetUploadImageUrlByFile(uploadImageShowenUrl));

                    string paramsData = string.Format("['{0}','{1}']", "document.getElementById('responseInfo').value", uploadImageShowenUrl);

                    response.Write(GetClientControlInvokeStript(GetPostedControlID(), "uploadSuccess", "document.getElementById('responseInfo').value", "document.getElementById('uploadImageUrlByFile').value"));
				}
			}
			catch (System.Exception ex)
			{
				response.Write(GetResponseTextScript(ex.Message));
				response.Write(GetClientControlInvokeStript(GetPostedControlID(), "uploadFail", "document.getElementById('responseInfo').value",""));
			}
			finally
			{
				response.End();
			}
		}

        //2012-03-13 randy add

        private static string GetUploadImageUrlByFile(string text)
        {
            string result = string.Format("<input type='hidden' id='uploadImageUrlByFile' value='{0}' />\n",
                HttpUtility.HtmlAttributeEncode(text));

            return result;
        }

		private static string GetResponseTextScript(string text)
		{
			string result = string.Format("<input type='hidden' id='responseInfo' value='{0}' />\n",
				HttpUtility.HtmlAttributeEncode(text));

			return result;
		}

		private string GetClientControlInvokeStript(string controlID, string method, string paramsData,string imageUrl)
		{
			StringBuilder strB = new StringBuilder();

			strB.AppendLine("<script type='text/javascript'>");

            strB.AppendFormat("if (window.parent.$find('{0}')) window.parent.$find('{0}').{1}({2},{3}); ",
                controlID, method, paramsData, imageUrl == "" ? "''" : imageUrl);

			strB.AppendLine("</script>");

			return strB.ToString();
		}

		protected override void Render(HtmlTextWriter writer)
		{
			if (this.DesignMode)
				writer.Write("Image Uploader Control");
			else
				base.Render(writer);
		}

		protected override void OnPreRender(EventArgs e)
		{
            if (!string.IsNullOrEmpty(postedControlID) && postedControlID != this.ClientID)
            {
                return;
            }

		    base.OnPreRender(e);

			this.Controls.Add(GetContainer());

			//注册隐藏域，用于客户端上传图片时的属性信息
			Page.ClientScript.RegisterHiddenField("clientImagePropertiesHidden", string.Empty);
			Page.ClientScript.RegisterHiddenField("clientOPHidden", string.Empty);

			Page.ClientScript.RegisterStartupScript(this.GetType(), "InnerFrame",
				string.Format("<div id=\"{0}\" style=\"display:none\"><iframe id=\"{1}\" name=\"{2}\"></iframe></div>",
				INNERFRAME_CONTAINER, INNERFRAMEID, INNERFRAMEID));

			//Page.Form.Enctype = "multipart/form-data";
		}

		private void ShowImage(string id, string filePath, bool cache)
		{
			HttpResponse response = HttpContext.Current.Response;

			try
			{
				byte[] content = null;

				string fileName = string.Empty;

				if (filePath.IsNotEmpty())
					content = GetImageBytesFromFilePath(filePath, out fileName);
				else
					content = GetImageBytesFromID(id, out fileName);

				if (content == null)
				{
					fileName = "cannotShow.png";
					content = ResourceHelper.GetResourceBytes(Assembly.GetExecutingAssembly(),
						"MCS.Web.WebControls.ImageUploaderControl.cannotShow.png");
				}

				if (cache == false)
					response.Cache.SetCacheability(HttpCacheability.NoCache);

				response.ContentType = WebUtility.GetContentTypeByFileName(fileName);
				response.BinaryWrite(content);
			}
			catch (System.Exception ex)
			{
				response.Write(ex.Message);
			}
			finally
			{
				response.End();
			}
		}

		private static byte[] GetImageBytesFromFilePath(string filePath, out string fileName)
		{
			byte[] content = null;

			fileName = Path.GetFileName(filePath);

			if (File.Exists(filePath))
			{
				using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
				{
					content = fs.ToBytes();
				}
			}

			return content;
		}

		private static byte[] GetImageBytesFromID(string id, out string fileName)
		{
			MaterialContentCollection mcs = MaterialContentAdapter.Instance.Load(builder => builder.AppendItem("CONTENT_ID", id));

			byte[] content = null;
			fileName = string.Empty;

			if (mcs.Count > 0)
			{
				fileName = mcs[0].FileName;
				content = mcs[0].ContentData;
			}

			return content;
		}

		private string GetContentType(string fileName)
		{
			return Path.GetExtension(fileName);
		}

		private static ImageProperty GetPostedImageProperty()
		{
			string data = HttpContext.Current.Request.Form.GetValue("clientImagePropertiesHidden", string.Empty);

			ImageProperty imgProp = JSONSerializerExecute.Deserialize<ImageProperty>(data);

			if (imgProp.ID.IsNullOrEmpty())
				imgProp.ID = Guid.NewGuid().ToString();

			imgProp.NewName = UuidHelper.NewUuidString() + Path.GetExtension(imgProp.OriginalName);
			imgProp.Changed = true;
			imgProp.UpdateTime = DateTime.Now;

			return imgProp;
		}

		protected override string SaveClientState()
		{
			if (this.ResourceID.IsNotEmpty())
				this.ImgProp.ResourceID = this.ResourceID;

			if (this.Class.IsNotEmpty())
				this.ImgProp.Class = this.Class;

			return JSONSerializerExecute.Serialize(this.ImgProp);
		}

		protected override void LoadClientState(string clientState)
		{
			object[] state = (object[])JSONSerializerExecute.DeserializeObject(clientState);

            object tempState;

            if (state[0] is object[])
            {
                object[] tempResult = (object[])state[0];
                tempState = tempResult[0];
            }
            else
            {
                tempState = state[0];
            }
            this.imgProp = JSONSerializerExecute.Deserialize<ImageProperty>(tempState);

			string postedControlID = GetPostedControlID();

			if (postedControlID.IsNullOrEmpty())
				postedControlID = ((string)state[1]).Replace('_', '$');

			HttpPostedFile file = GetPostFile(postedControlID);

			if (file != null)
			{
				if (this.imgProp.ID.IsNullOrEmpty())
					this.imgProp.ID = UuidHelper.NewUuidString();

				this.imgProp.NewName = UuidHelper.NewUuidString() + Path.GetExtension(imgProp.OriginalName);
				this.imgProp.Changed = true;

                string filePath;

                ImageUploadHelper.UploadFile(this, file, this.imgProp.OriginalName, this.imgProp.NewName, out filePath);
			}
		}

		private string GetPostedControlID()
		{
			string opText = WebUtility.GetRequestFormString("clientOPHidden", string.Empty);
			string op = !string.IsNullOrEmpty(opText) ? opText.Split(',')[0] : "";          //WebUtility.GetRequestFormString("clientOPHidden", string.Empty).Split(',')[0];
			string controlID = !string.IsNullOrEmpty(opText) ? opText.Split(',')[1] : ""; //WebUtility.GetRequestFormString("clientOPHidden", string.Empty).Split(',')[1];

			return controlID;
		}

		private Table GetContainer()
		{
			Table tbl = new Table();
			tbl.Width = this.Width;
			tbl.Height = this.Height;
			tbl.CellPadding = 0;
			tbl.CellSpacing = 0;
			tbl.Attributes["class"] = "ImageUploader_Table";

			TableRow row = new TableRow();
			TableCell cell = new TableCell();
			cell.Attributes["class"] = "ImageUploader_Input_File_Tablecell";
			SetUploadControlProperty();
			SetImageContainerProperty();
			cell.Controls.Add(uploader);
			cell.Controls.Add(GetLoading());

			row.Attributes["class"] = "ImageUploader_Input_File_Tablerow";
			row.Controls.Add(cell);

			TableRow imgRow = new TableRow();

			imgRow.HorizontalAlign = HorizontalAlign.Center;

			TableCell imgCell = new TableCell();

			imgCell.Controls.Add(imageControl);

			imgRow.Controls.Add(imgCell);
			imgRow.Attributes["class"] = "ImageUploader_ImageRow";

			tbl.Controls.Add(row);
			tbl.Controls.Add(imgRow);

			TableRow btnRow = new TableRow();
			if (!this.ReadOnly)
			{
				btnRow.Attributes["class"] = "ImageUploader_Footer";
				row.Attributes["class"] = "";
			}
			else
			{
				btnRow.Attributes["class"] = "ImageUploader_Footer invisible";
				row.Attributes["class"] = "invisible";
			}


			TableCell btnCell = new TableCell();
			btnCell.Attributes["class"] = "ImageUploader_Button_Tablecell";

			btnCell.Controls.Add(GetButtonTable());
			btnRow.Controls.Add(btnCell);

			tbl.Controls.Add(btnRow);

			return tbl;
		}

		private HtmlGenericControl GetLoading()
		{
			this.loading = new HtmlGenericControl("div");
			loading.Attributes["height"] = "10";
			loading.ID = "Loading";
			loading.Attributes["class"] = "ImageUploader_Loading_Invisible";

			return loading;
		}

		private TableRow GetHeader()
		{
			TableRow row = new TableRow();
			row.Attributes["class"] = "ImageUploader_Header";
			TableCell cell = new TableCell();
			cell.Controls.Add(new LiteralControl(string.Format("<span>{0}<span>", "请选择图片")));
			row.Controls.Add(cell);

			return row;
		}

		private Table GetButtonTable()
		{
			Table tbl = new Table();
			tbl.CellSpacing = 0;
			tbl.CellPadding = 0;
			TableRow row = new TableRow();
			TableCell btnSaveCell = new TableCell();
			TableCell btnDeleteCell = new TableCell();

			btnUpload = new HtmlInputButton();
			btnUpload.ID = "btnSave";
			btnUpload.Value = Translator.Translate(Define.DefaultCulture, "上传");

			btnDeleteCell.Attributes["class"] = "ImageUploader_Button_Cell";
			btnDelete = new HtmlInputButton();
			btnDelete.ID = "btnDelete";
			btnDelete.Attributes["value"] = Translator.Translate(Define.DefaultCulture, "删除");



			btnSaveCell.Attributes["class"] = "ImageUploader_Button_Cell";
			btnSaveCell.Controls.Add(btnUpload);
			btnDeleteCell.Controls.Add(btnDelete);

			row.Controls.Add(btnSaveCell);
			row.Controls.Add(btnDeleteCell);

			tbl.Controls.Add(row);
			tbl.Attributes["class"] = "ImageUploader_Button_Table";

            if (this.AutoUpload)
            {
                if (btnSaveCell.Attributes["class"] != "ImageUploader_Button_Cell invisible")
                    btnSaveCell.Attributes["class"] = "ImageUploader_Button_Cell invisible";
            }

			return tbl;
		}

		private void SetImageContainerProperty()
		{
			imageControl = new HtmlImage();

			if (this.ImgProp != null && this.ImgProp.IsEmpty() == false)
			{
				string filePath = string.Empty;

				if (this.ImgProp.Content != null)
					filePath = this.ImgProp.Content.PhysicalSourceFilePath.ToString().Encrypt();

				imageControl.Src = CurrentPageUrl + string.Format("?imagePropID={0}&filePath={1}",
					this.ImgProp.ID, filePath);
				if (this.ReadOnly == false)
					imageControl.Src += "&cache=false";
                if (this.Page.IsPostBack)
                {
                    if (!string.IsNullOrEmpty(this.ImgProp.Src) && this.ImgProp.Content == null)
                        imageControl.Src = this.ImgProp.Src;
                }
			}
			else
			{
				imageControl.Src = DefaultImg;
			}

			imageControl.ID = "PreviewImage";
			imageControl.Attributes["class"] = "ImageUploader_Image";
			imageControl.Attributes["title"] = Translator.Translate(Define.DefaultCulture, "点击此处在新窗口中打开图片");
			imageControl.Style["width"] = this.ImageWidth.ToString();
			imageControl.Style["height"] = this.ImageHeight.ToString();
		}

        /// <summary>
        /// 处理请求的页面地址
        /// </summary>
        [ScriptControlProperty, ClientPropertyName("currentPageUrl")]
		private string CurrentPageUrl
		{
			get
			{
				Page page = (Page)HttpContext.Current.CurrentHandler;

				return page.ResolveUrl(page.AppRelativeVirtualPath);
			}
		}

		private void SetUploadControlProperty()
		{
			this.uploader = new HtmlInputFile();
			this.uploader.Attributes["class"] = "ImageUploader_Input_File";
			this.uploader.ID = "ImgUpload";
		}

		public ImageProperty ImgProp
		{
			get
			{
				if (this.imgProp == null)
					this.imgProp = new ImageProperty();

				return this.imgProp;
			}
			set
			{
				this.imgProp = value;
			}
		}

        public string UploadImageShowenUrl
        {
            get;
            set;

        }

		public delegate void UploadHandler(object sender, UploadEventArgs e);

		#region 事件
		public event UploadHandler BeforeUpload;
		public event UploadHandler AfterUpload;
		public event EventHandler BeforeDownload;

		//public event ImageContentHandler ImageContent;
		#endregion

		internal protected void OnBeforeUploadFile(UploadEventArgs e)
		{
			if (this.BeforeUpload != null)
				this.BeforeUpload(this, e);
		}

		internal protected void OnAfterUploadFile(UploadEventArgs e)
		{
			if (this.AfterUpload != null)
				this.AfterUpload(this, e);
		}

		internal protected void OnBeforeDownload()
		{
			if (this.BeforeDownload != null)
			{
				EventArgs e = new EventArgs();

				this.BeforeDownload(this, e);
			}
		}
		#region 客户端属性

		[Browsable(false)]
		[ScriptControlProperty()]
		[ClientPropertyName("inputFileID")]
		private string InputControlID
		{
			get
			{
				return uploader.ClientID;
			}

		}

		[Browsable(false)]
		[ScriptControlProperty()]
		[ClientPropertyName("imageID")]
		private string ImageID
		{
			get
			{
				return imageControl.ClientID;
			}

		}

		public Unit ImageWidth
		{
			get
			{
				return ViewState.GetViewStateValue("ImageWidth", Unit.Pixel(120));
			}
			set
			{
				ViewState.SetViewStateValue("ImageWidth", value);
			}
		}

		[DefaultValue("")]
		[ScriptControlProperty]
		[ClientPropertyName("imageWidth")]
		public string ClientImageWidth
		{
			get
			{
				return ImageWidth.ToString();
			}
		}

		public Unit ImageHeight
		{
			get
			{
				return ViewState.GetViewStateValue("ImageHeight", Unit.Pixel(120));
			}
			set
			{
				ViewState.SetViewStateValue("ImageHeight", value);
			}
		}

		[ScriptControlProperty]
		[ClientPropertyName("imageHeight")]
		public string ClientImageHeight
		{
			get
			{
				return ImageHeight.ToString();
			}
		}

		[DefaultValue("")]
		[ScriptControlProperty]
		[ClientPropertyName("defaultImg")]
		public string DefaultImg
		{
			get
			{
				string defaultImg = "MCS.Web.WebControls.ImageUploaderControl.noLogo.png";

				if (WebUtility.IsWebApplicationCompilationDebug())
					defaultImg = "MCS.Web.WebControls.ImageUploaderControl.defaultImage_bw.jpg";

				return ViewState.GetViewStateValue("DefaultImg",
					this.Page.ClientScript.GetWebResourceUrl(this.GetType(), defaultImg));
			}
			set
			{
				ViewState.SetViewStateValue("DefaultImg", value);
			}
		}

		[DefaultValue("")]
		[ScriptControlProperty]
		[ClientPropertyName("uploadButtonId")]
		private string UploadButtonID
		{
			get
			{
				return this.btnUpload.ClientID;
			}
		}

		[DefaultValue("")]
		[ScriptControlProperty]
		[ClientPropertyName("deleteButtonID")]
		private string DeleteButtonID
		{
			get
			{
				return this.btnDelete.ClientID;
			}
		}

		[DefaultValue("")]
		[ScriptControlProperty]
		[ClientPropertyName("innerFrameID")]
		private string InnerFrameID
		{
			get
			{
				return INNERFRAMEID;
			}
		}

		[DefaultValue("")]
		[ScriptControlProperty]
		[ClientPropertyName("innerFrameContainerID")]
		private string InnerFrameContainer
		{
			get
			{
				return INNERFRAME_CONTAINER;
			}
		}

		[DefaultValue("")]
		[ScriptControlProperty]
		[ClientPropertyName("fileMaxSize")]
		public int FileMaxSize
		{
			get
			{
				return GetPropertyValue<int>("FileMaxSize", 4194304);
			}
			set
			{
				SetPropertyValue<int>("FileMaxSize", value);
			}
		}

		[DefaultValue("")]
		[ScriptControlProperty]
		[ClientPropertyName("controlID")]
		public string ControlID
		{
			get
			{
				return this.ClientID;
			}
		}

		public string Class
		{
			get;
			set;
		}

		public string ResourceID
		{
			get;
			set;
		}

        [DefaultValue(true)]
        [ScriptControlProperty]
        [ClientPropertyName("autoUpload")]
        public bool AutoUpload
        {
            get
            {
                return GetPropertyValue<bool>("autoUpload", true);
            }
            set
            {
                SetPropertyValue<bool>("autoUpload", value);
            }
        }

        /// <summary>
        /// 上传后触发的客户端事件
        /// </summary>
        [DefaultValue("")]
        [Category("Action")]
        [ScriptControlEvent()]
        [ClientPropertyName("imageUploaded")]
        [Description("上传后触发的客户端事件")]
        public string OnClientImageUploaded
        {
            get { return GetPropertyValue("OnClientImageUploaded", string.Empty); }
            set { SetPropertyValue("OnClientImageUploaded", value); }
        }

        /// <summary>
        /// 客户端删除图片后事件
        /// </summary>
        [DefaultValue("")]
        [Category("Action")]
        [ScriptControlEvent()]
        [ClientPropertyName("imageDeleted")]
        [Description("客户端删除图片后事件")]
        public string OnClientImageDeleted
        {
            get { return GetPropertyValue("OnClientImageDeleted", string.Empty); }
            set { SetPropertyValue("OnClientImageDeleted", value); }
        }

		#endregion
	}
}
