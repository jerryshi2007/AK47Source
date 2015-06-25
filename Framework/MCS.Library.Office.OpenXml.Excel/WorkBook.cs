using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.IO.Packaging;
using System.Xml;
using System.Text.RegularExpressions;
using System.Globalization;
using MCS.Library.Core;
using System.Security;
using System.Xml.Linq;
using MCS.Library.Office.OpenXml.Excel.Encryption;
using System.Reflection;

namespace MCS.Library.Office.OpenXml.Excel
{
    public sealed class WorkBook
    {
        public const string DefaultSheetName = "sheet1";

        /// <summary>
        /// 新创建一个Excel，里面包含一个工作簿sheet1
        /// </summary>
        /// <returns></returns>
        public static WorkBook CreateNew()
        {
            WorkBook result = new WorkBook();
            WorkBookView wbView = new WorkBookView(result);
            WorkSheet ws = new WorkSheet(result, DefaultSheetName);

            result.Views.Add(wbView);
            result.Sheets.Add(ws);

            return result;
        }

        /// <summary>
        /// 通过流数据加载WorkBook
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static WorkBook Load(Stream input)
        {
            WorkBook wb = new WorkBook();
            wb.InnerLoad(input);
            return wb;
        }

        public static WorkBook Load(Stream input, string password)
        {
            if (!string.IsNullOrEmpty(password))
            {
                EncryptedPackageHandler encrHandler = new EncryptedPackageHandler();
                ExcelEncryption fileEncryption = new ExcelEncryption();
                fileEncryption.IsEncrypted = true;
                fileEncryption.Password = password;
                using (MemoryStream copyStream = new MemoryStream())
                {
                    input.CopyTo(copyStream);
                    //ExcelHelper.CopyStream(input, copyStream);
                    MemoryStream decrytStrem = encrHandler.DecryptPackage(copyStream, fileEncryption);
                    return Load(decrytStrem);
                }
            }
            else
            {
                return null;
            }

        }

        /// <summary>
        /// 通过文件路径加载WorkBook
        /// </summary>
        /// <param name="excelFilePath"></param>
        /// <returns></returns>
        public static WorkBook Load(string excelFilePath)
        {
            FileInfo excelFile = new FileInfo(excelFilePath);
            ExceptionHelper.FalseThrow(excelFile.Exists, "文件不存在！");

            using (FileStream stream = excelFile.OpenRead())
            {
                return Load(stream);
            }
        }

        public static WorkBook Load(string excelFilePath, string password)
        {
            FileInfo excelFile = new FileInfo(excelFilePath);
            ExceptionHelper.FalseThrow(excelFile.Exists, "文件不存在！");
            excelFile.Refresh();

            if (!string.IsNullOrEmpty(password))
            {
                EncryptedPackageHandler encrHandler = new EncryptedPackageHandler();
                ExcelEncryption fileEncryption = new ExcelEncryption();
                fileEncryption.IsEncrypted = true;
                fileEncryption.Password = password;
                using (MemoryStream excelStream = encrHandler.DecryptPackage(excelFile, fileEncryption))
                {
                    return Load(excelStream);
                }
            }
            else
            {
                using (FileStream stream = excelFile.OpenRead())
                {
                    return Load(stream);
                }
            }
        }

        internal WorkBook() { }

        #region properties
        private WorkSheetCollection _Sheets;
        /// <summary>
        /// 工作表中的工作簿
        /// </summary>
        public WorkSheetCollection Sheets
        {
            get
            {
                if (this._Sheets == null)
                {
                    this._Sheets = new WorkSheetCollection();
                }

                return this._Sheets;
            }
        }

        private AppTheme _Theme;
        /// <summary>
        /// 工作表主题
        /// </summary>
        public AppTheme Theme
        {
            get
            {
                if (this._Theme == null)
                {
                    this._Theme = new AppTheme(this);
                }
                return this._Theme;
            }
        }

        internal WorkBookProtection _Protection;
        /// <summary>
        /// 工作簿保护信息
        /// </summary>
        public WorkBookProtection Protection
        {
            get
            {
                if (this._Protection == null)
                {
                    this._Protection = new WorkBookProtection();
                }
                return this._Protection;
            }
        }

        private WorkBookViewCollection _Views = null;
        /// <summary>
        /// 工作簿界面信息
        /// </summary>
        public WorkBookViewCollection Views
        {
            get
            {
                if (this._Views == null)
                {
                    this._Views = new WorkBookViewCollection();
                }

                return this._Views;
            }
        }

        private FileProperties _FileDetails;
        /// <summary>
        /// Excel文件属性信息
        /// </summary>
        public FileProperties FileDetails
        {
            get
            {
                if (this._FileDetails == null)
                {
                    this._FileDetails = new FileProperties();
                }

                return this._FileDetails;
            }
        }

        private WorkbookFileVersion _FileVersion;
        /// <summary>
        /// Excel文件版本信息
        /// </summary>
        public WorkbookFileVersion FileVersion
        {
            get
            {
                if (this._FileVersion == null)
                {
                    this._FileVersion = new WorkbookFileVersion();
                }

                return this._FileVersion;
            }
        }

        internal WorkBookProperties _Properties;

        internal WorkBookProperties Properties
        {
            get
            {
                if (this._Properties == null)
                    this._Properties = new WorkBookProperties();

                return this._Properties;
            }
        }

        private CompressionOption _Compression = CompressionOption.Normal;
        /// <summary>
        /// 工作表压缩方式，默认为Normal
        /// </summary>
        public CompressionOption Compression
        {
            get
            {
                return this._Compression;
            }
            set
            {
                this._Compression = value;
            }
        }

        /// <summary>
        /// Excel计算模式
        /// </summary>
        public ExcelCalcMode CalcMode
        {
            get;
            set;
        }

        private CalculationChain _CalculationChain;
        public CalculationChain CalculationChain
        {
            get
            {
                if (this._CalculationChain == null)
                {
                    this._CalculationChain = new CalculationChain();
                }

                return this._CalculationChain;
            }
        }

        private decimal _MaxFontWidth = decimal.MaxValue;
        internal decimal MaxFontWidth
        {
            get
            {
                if (this._MaxFontWidth == decimal.MaxValue)
                {
                    System.Drawing.Font f = new System.Drawing.Font("宋体", 11);
                    this._MaxFontWidth = (decimal)(System.Windows.Forms.TextRenderer.MeasureText("00", f).Width - System.Windows.Forms.TextRenderer.MeasureText("0", f).Width);
                }
                return this._MaxFontWidth;
            }
        }
        #endregion

        #region "Load"
        private void InnerLoad(Stream input)
        {
            ExceptionHelper.FalseThrow(input.CanRead, "流必须可读与可写！");

            using (Package package = Package.Open(input, FileMode.Open, FileAccess.Read))
            {
                ExcelLoadContext context = new ExcelLoadContext(package);
                context.Reader = new ExcelReader(this);
                context.Reader.Context = context;

                LoadWorkBookInfo(context);

                LoadSharedStrings(context);

                LoadStyles(context);

                LoadAppTheme(context);

                LoadWorkSheets(context);

                LoadCalculationChains(context);
            }
        }

        private void LoadStyles(ExcelLoadContext context)
        {
            ((IPersistable)context.GlobalStyles).Load(context);
        }

        private void LoadAppTheme(ExcelLoadContext context)
        {
            ((IPersistable)this.Theme).Load(context);
        }

        private void LoadWorkSheets(ExcelLoadContext context)
        {
            this.Sheets.ForEach(sheet =>
            {
                ((IPersistable)sheet).Load(context);
                sheet.Names.AddRange(context.DefinedNames.Where(nameRange =>
                {
                    if (nameRange._WorkSheet != null)
                        return string.Compare(nameRange._WorkSheet.Name, sheet.Name) == 0;
                    else
                        return false;
                }));
            });
        }

        private void LoadSharedStrings(ExcelLoadContext context)
        {
            if (context.Package.PartExists(ExcelCommon.Uri_SharedStrings))
            {
                var sharedStringsRoot = context.Package.GetXElementFromUri(ExcelCommon.Uri_SharedStrings);
                context.Reader.ReadSharedStrings(sharedStringsRoot);
            }
        }

        private void LoadWorkBookInfo(ExcelLoadContext context)
        {
            ((IPersistable)this.FileDetails).Load(context);

            if (!context.Package.PartExists(ExcelCommon.Uri_Workbook))
            {
                throw new Exception(string.Format("文档格式错误，未找到{0}", ExcelCommon.Uri_Workbook));
            }

            XElement root = context.Package.GetXElementFromUri(ExcelCommon.Uri_Workbook);

            XElement childNode = root.Element(ExcelCommon.Schema_WorkBook_Main + "bookViews");
            context.Reader.ReadWorkBook_bookViews(childNode);

            childNode = root.Element(ExcelCommon.Schema_WorkBook_Main + "sheets");
            context.Reader.ReadWorkBook_sheets(childNode);

            childNode = root.Element(ExcelCommon.Schema_WorkBook_Main + "definedNames");
            context.Reader.ReadWorkBook_definedNames(childNode, context);

            childNode = root.Element(ExcelCommon.Schema_WorkBook_Main + "fileVersion");
            context.Reader.ReadWorkBook_fileVersion(childNode);

            childNode = root.Element(ExcelCommon.Schema_WorkBook_Main + "workbookPr");
            context.Reader.ReadWorkBook_workbookPr(childNode);

            childNode = root.Element(ExcelCommon.Schema_WorkBook_Main + "workbookProtection");
            context.Reader.ReadWorkBook_workbookProtection(childNode);
        }

        private void LoadCalculationChains(ExcelLoadContext context)
        {
            ((IPersistable)this.CalculationChain).Load(context);
        }
        #endregion

        #region "Save"
        private void SaveWorkSheets(ExcelSaveContext context)
        {
            foreach (var sheet in this.Sheets)
            {
                ((IPersistable)sheet).Save(context);
            }
        }

        private void SaveTheme(ExcelSaveContext context)
        {
            if (this._Theme != null)
            {
                ((IPersistable)this._Theme).Save(context);
            }
        }

        private void InnerSave(Package package)
        {
            // 创建一个新的 workbookpart 
            ExcelSaveContext context = new ExcelSaveContext(this) { Package = package };

            context.LinqWriter.WriteWorkBook();
            SaveWorkSheets(context);

            context.LinqWriter.WriteSharedStrings();

            ((IPersistable)this.FileDetails).Save(context);

            ((IPersistable)context.GlobalStyles).Save(context);

            ((IPersistable)this.CalculationChain).Save(context);

            SaveTheme(context);

            package.CreateRelationship(PackUriHelper.GetRelativeUri(new Uri("/xl", UriKind.Relative),
                ExcelCommon.Uri_Workbook),
                TargetMode.Internal,
                ExcelCommon.Schema_Relationships + "/officeDocument");

            package.Flush();
        }
        #endregion

        #region public method

        internal int GetTablesCount()
        {
            int resutl = 0;

            foreach (WorkSheet worksheet in this.Sheets)
            {
                if (worksheet._Tables != null)
                    resutl += worksheet._Tables.Count;
            }

            return resutl;
        }

        /// <summary>
        /// 保存工作表到指定流中
        /// </summary>
        /// <param name="targetStream"></param>
        public void Save(Stream targetStream)
        {
            if (targetStream.CanRead == false)
            {
                using (MemoryStream temStream = new MemoryStream())
                {
                    this.SavePackage(temStream);
                    temStream.Seek(0, SeekOrigin.Begin);
                    temStream.CopyTo(targetStream);
                }
            }
            else
                this.SavePackage(targetStream);

            if (targetStream.CanSeek == true)
                targetStream.Seek(0, SeekOrigin.Begin);
        }

        private void SavePackage(Stream targetStream)
        {
            using (Package package = Package.Open(targetStream, FileMode.Create, FileAccess.ReadWrite))
            {
                InnerSave(package);
            }
        }

        public void Save(Stream targetStream, string password)
        {
            ExcelEncryption fileEncryption = new ExcelEncryption();
            fileEncryption.Password = password;

            if (fileEncryption.IsEncrypted)
            {
                EncryptedPackageHandler eph = new EncryptedPackageHandler();
                using (MemoryStream encryptStream = eph.EncryptPackage(SaveAsBytes(), fileEncryption))
                {
                    encryptStream.Seek(0, SeekOrigin.Begin);
                    encryptStream.CopyTo(targetStream);
                    //ExcelHelper.CopyStream(encryptStream, targetStream);
                }
            }
            else
            {
                Save(targetStream);
            }
        }

        /// <summary>
        /// 保存工作表为指定文件
        /// </summary>
        /// <param name="targetFilePath"></param>
        public void Save(string targetFilePath)
        {
            if (File.Exists(targetFilePath))
            {
                File.Delete(targetFilePath);
            }

            using (Package package = Package.Open(targetFilePath, FileMode.Create, FileAccess.ReadWrite))
            {
                InnerSave(package);
            }
        }

        /// <summary>
        /// 保存工作表为指定文件设置密码
        /// </summary>
        /// <param name="targetFilePath"></param>
        /// <param name="password"></param>
        public void Save(string targetFilePath, string password)
        {
            FileInfo targetFile = new FileInfo(targetFilePath);

            if (targetFile.Exists)
                targetFile.Delete();

            ExcelEncryption fileEncryption = new ExcelEncryption();

            fileEncryption.Password = password;

            if (fileEncryption.IsEncrypted)
            {
                using (FileStream fi = new FileStream(targetFile.FullName, FileMode.Create))
                {
                    byte[] file = this.SaveAsBytes();
                    EncryptedPackageHandler eph = new EncryptedPackageHandler();
                    using (MemoryStream ms = eph.EncryptPackage(file, fileEncryption))
                    {
                        fi.Write(ms.GetBuffer(), 0, (int)ms.Length);
                    }
                }
            }
            else
            {
                using (Package package = Package.Open(targetFile.FullName, FileMode.Create, FileAccess.ReadWrite))
                {
                    InnerSave(package);
                }
            }
        }

        /// <summary>
        /// 将文件转换成二进制输出
        /// </summary>
        /// <returns></returns>
        public byte[] SaveAsBytes()
        {
            Byte[] byRet = null;
            using (MemoryStream excelStream = new MemoryStream())
            {
                Save(excelStream);

                byRet = new byte[excelStream.Length];
                long pos = excelStream.Position;
                //excelStream.Seek(0, SeekOrigin.Begin);
                excelStream.Read(byRet, 0, (int)excelStream.Length);
                excelStream.Seek(pos, SeekOrigin.Begin);

                return byRet;
            }
        }

        /// <summary>
        /// 加密后Excel文件
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        public byte[] SaveAsBytes(string password)
        {
            byte[] byRet = null;
            ExcelEncryption fileEncryption = new ExcelEncryption();
            fileEncryption.Password = password;
            if (fileEncryption.IsEncrypted)
            {
                EncryptedPackageHandler eph = new EncryptedPackageHandler();
                using (MemoryStream encryptStream = eph.EncryptPackage(SaveAsBytes(), fileEncryption))
                {
                    byRet = encryptStream.ToArray();
                }
            }
            else
            {
                byRet = SaveAsBytes();
            }

            return byRet;
        }
        #endregion

        /*	#region "Custom Attribute Collection"
		/// <summary>
		/// 将数据的信息创建或加载到Excel当中
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="collection"></param>
		/// <param name="tableStyle"></param>
		public void LoadFromCollection<T>(IEnumerable<T> collection, SpreadSheetDescription sheetDescription, Action<T, TableCell, string, int> creatingDataCell) where T : class
		{
			ExceptionHelper.TrueThrow(sheetDescription.SheetName.IsNullOrEmpty(), "工作表名称不能为空");

			if (sheetDescription.TableDescriptions.Count == 0)
			{
				Type entityType = typeof(T);
				TableDescription tbdescription = this.FindTableDescripton(entityType);
				if (tbdescription != null)
					sheetDescription.TableDescriptions.Add(tbdescription);
			}

			WorkSheet currentSheet = this.GetLoadFromCollectionWroksheet(sheetDescription);

			foreach (TableDescription tDes in sheetDescription.TableDescriptions)
			{
				currentSheet.LoadFromCollection<T>(collection, tDes, creatingDataCell);
			}
		}

		private WorkSheet GetLoadFromCollectionWroksheet(SpreadSheetDescription sheetDescription)
		{
			WorkSheet currentSheet = null;
			if (this.Sheets.IsCreateEmpty())
			{
				currentSheet = this.Sheets[WorkBook.DefaultSheetName];
				currentSheet.Name = sheetDescription.SheetName;
				if (sheetDescription.SheetCode.IsNotEmpty())
					sheetDescription.SheetCode = sheetDescription.SheetCode;
			}
			else if (this.Sheets.TryGetWrokSheet(sheetDescription.SheetName, sheetDescription.SheetCode, out currentSheet) == false)
			{
				currentSheet = new WorkSheet(this, sheetDescription.SheetName, sheetDescription.SheetHide);
				this.Sheets.Add(currentSheet);
			}

			return currentSheet;
		}


		/// <summary>
		/// 将带有标签数据转换成指定类型集合
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <typeparam name="TValueCollection"></typeparam>
		/// <returns></returns>
		public TValueCollection ExcelTableToObjectCollection<T, TValueCollection>(SpreadSheetDescription sheetDescription)
			where T : class,new()
			where TValueCollection : ICollection<T>, new()
		{
			ExceptionHelper.TrueThrow(sheetDescription.SheetName.IsNullOrEmpty(), "工作表名称不能为空");

			if (sheetDescription.TableDescriptions.Count == 0)
			{
				Type entityType = typeof(T);
				TableDescription tbdescription = this.FindTableDescripton(entityType);
				if (tbdescription != null)
					sheetDescription.TableDescriptions.Add(tbdescription);
			}

			WorkSheet worksheet;

			bool isGetSheet = this.Sheets.TryGetWrokSheet(sheetDescription.SheetName, sheetDescription.SheetCode, out worksheet);

			ExceptionHelper.FalseThrow(isGetSheet, "找不到指定{0}的工作表", sheetDescription.SheetName);

			TValueCollection result = default(TValueCollection);

			foreach (TableDescription tdescr in sheetDescription.TableDescriptions)
			{
				result = worksheet.ExcelTableToObjectCollection<T, TValueCollection>(tdescr);
			}

			return result;

		}

		/// <summary>
		/// 逐行提取数据，并返回指定Table当中所有数据
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="tableName"></param>
		/// <param name="rowValidator"></param>
		/// <returns></returns>
		public string EachTableRows<T>(SpreadSheetDescription sheetDescription, Func<ExportTableRowContext<T>, string> rowValidator) where T : class,new()
		{
			WorkSheet worksheet;
			if (this.Sheets.TryGetWrokSheet(sheetDescription.SheetName, sheetDescription.SheetCode, out worksheet))
			{
				if (sheetDescription.TableDescriptions.Count == 0)
				{
					Type entityType = typeof(T);
					TableDescription tbdescription = this.FindTableDescripton(entityType);
					if (tbdescription != null)
						sheetDescription.TableDescriptions.Add(tbdescription);
				}

				StringBuilder customLog = new StringBuilder();
				foreach (TableDescription tbd in sheetDescription.TableDescriptions)
				{
					customLog.Append(worksheet.EachTableRows<T>(tbd, rowValidator));
				}
				return customLog.ToString();
			}
			else
			{
				return string.Format("没有找到指定{0}工作表名称", sheetDescription.SheetName);
			}
		}

		/// <summary>
		/// 将Excel里指定Table名称的数据统收集返回指定的集合
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <typeparam name="TValueCollection"></typeparam>
		/// <param name="rowValidator"></param>
		/// <param name="operlog"></param>
		/// <returns></returns>
		public IDictionary<string, TValueCollection> EachTableRowsToObjectCollection<T, TValueCollection>(SpreadSheetDescription sheetDescription, Func<ExportTableRowContext<T>, string> rowValidator, out string operlog)
			where T : class,new()
			where TValueCollection : ICollection<T>, new()
		{
			IDictionary<string, TValueCollection> result = new Dictionary<string, TValueCollection>();
			StringBuilder customLog = new StringBuilder();
			WorkSheet worksheet;
			if (this.Sheets.TryGetWrokSheet(sheetDescription.SheetName, sheetDescription.SheetCode, out worksheet))
			{
				if (sheetDescription.TableDescriptions.Count == 0)
				{
					Type entityType = typeof(T);
					TableDescription tbdescription = this.FindTableDescripton(entityType);
					if (tbdescription != null)
						sheetDescription.TableDescriptions.Add(tbdescription);
				}

				foreach (TableDescription tbd in sheetDescription.TableDescriptions)
				{
					string currentLog;
					result.Add(tbd.TableName, worksheet.EachTableRowsToObjectCollection<T, TValueCollection>(tbd, rowValidator, out currentLog));
					customLog.AppendFormat("{0}{1}", tbd.TableName, currentLog);
				}
			}
			operlog = customLog.ToString();

			return result;
		}

		private TableDescription FindTableDescripton(Type type)
		{
			object[] tbmembers = type.GetCustomAttributes(typeof(TableDescriptionAttribute), false);

			ExceptionHelper.TrueThrow(tbmembers == null || tbmembers.Count() == 0, "没有在定义TableDescriptionAttribute，将不能使用此方法");

			TableDescriptionAttribute tbdescription = tbmembers[0] as TableDescriptionAttribute;

			TableDescription result = new TableDescription();
			result.InitDescription(tbdescription);

			return result;
		}

		#endregion */
    }
}
