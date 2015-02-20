using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Services;
using MCS.Library.SOA.DataObjects.Workflow;
using System.Threading;
using System.Configuration;
using System.IO;
using System.DirectoryServices;
using System.Timers;
using MCS.Library;
using MCS.Library.SOA.DataObjects;
using MCS.Library.Data;
using System.Data;
using System.Data.Common;
using MCS.Library.Core;
using MCS.Library.Configuration;

namespace SyncEmployeePhotoService
{
    public class EmployeePhotoServiceThread : ThreadTaskBase
    {
        public override void OnThreadTaskStart()
        {
            Run();
        }

        public void Run()
        {
            FileInfo logfile = new FileInfo(Environment.CurrentDirectory.ToString() + "/log.txt");
            StreamWriter write = null;
            if (logfile.Exists)
            {
                write = logfile.AppendText();
                write.WriteLine("---------------------------------------------------------------------------------------");
            }
            else
            {
                write = logfile.CreateText();
            }

            write.WriteLine("用户名,导入结果,操作时间,失败原因");

            List<Picture> pictures = new List<Picture>();
            List<Picture> deletedPictures = new List<Picture>();
            Database database = null;
            try
            {
                #region 读取新加用户的图片的数据库信息
                database = DbHelper.GetDBDatabase("HB2008");
                string sql = @"select top 1000 c.LOGIN_NAME, a.FILE_NAME, a.CONTENT_DATA from WF.MATERIAL_CONTENT a join wf.IMAGE b " +
                              "on a.CONTENT_ID = b.ID join wf.INITIALIZATION_USER_AD_IMAGE c on b.RESOURCE_ID = c.PICTURE_ID where a.CLASS = 'HR_PHOTO' and c.UpdateStatus = 0 and c.VALIDSTATUS = 1 ";

                DbDataReader dr = database.ExecuteReader(CommandType.Text, sql);
                while (dr.Read())
                {
                    Picture pic = new Picture();

                    pic.LOGIN_NAME = dr["LOGIN_NAME"].ToString();
                    pic.FILE_NAME = dr["FILE_NAME"] != null ? dr["FILE_NAME"].ToString() : string.Empty;
                    pic.CONTENT_DATA = (byte[])dr["CONTENT_DATA"];
                    pictures.Add(pic);
                }
                dr.Close();
                #endregion

                #region 读取删除用户的图片的数据库信息
                sql = @"select LOGIN_NAME from WF.INITIALIZATION_USER_AD_IMAGE where VALIDSTATUS = 0 and UpdateStatus = 0  ";

                dr = database.ExecuteReader(CommandType.Text, sql);
                while (dr.Read())
                {
                    Picture pic = new Picture();

                    pic.LOGIN_NAME = dr["LOGIN_NAME"].ToString();
                    deletedPictures.Add(pic);
                }
                dr.Close();
                #endregion
            }
            catch
            {
                pictures.Clear();
                deletedPictures.Clear();
                write.WriteLine("SQL Server 服务器异常");
                return;
            }

            #region 向AD中写入图片信息
            foreach (Picture pic in pictures)
            {
                DirectoryEntry entity = null;
                List<SearchResult> result = null;
                try
                {
                    var exFilter = new ExtraFilter();
                    exFilter.UserFilter = "samAccountName=" + pic.LOGIN_NAME;
                    string filter = ADSearchConditions.GetFilterByMask(ADSchemaType.Users, exFilter);
                    ADSearchConditions conditons = new ADSearchConditions();
                    conditons.Scope = SearchScope.Subtree;
                    ServerInfo serverInfo = ServerInfoConfigSettings.GetConfig().ServerInfos["dc"].ToServerInfo();
                    MCS.Library.ADHelper adHelper = ADHelper.GetInstance(serverInfo);
                    result = adHelper.ExecuteSearch(adHelper.GetRootEntry(), filter, conditons, "distinguishedName");

					if (result.Count < 1)
                    {
                        write.WriteLine(pic.LOGIN_NAME + ",失败," + DateTime.Now + ",AD中找不到该账号");
                    }
                    else
                    {
                        #region 处理逻辑
                        entity = adHelper.NewEntry(result[0].Properties["distinguishedName"][0].ToString());
                        if (pic.CONTENT_DATA.Length < 102400)
                        {
                            if (entity.Properties.Contains("thumbnailPhoto"))
                            {
                                entity.Properties["thumbnailPhoto"][0] = pic.CONTENT_DATA;
                            }
                            else
                            {
                                entity.Properties["thumbnailPhoto"].Add(pic.CONTENT_DATA);
                            }
                        }
                        else
                        {
                            ImageHelper imgHelp = new ImageHelper(pic.CONTENT_DATA);
                            double percent = 80000.00 / pic.CONTENT_DATA.Length;

                            if (pic.FILE_NAME != string.Empty && pic.FILE_NAME != null)
                            {
                                if (pic.FILE_NAME.Split('.').Length > 1)
                                {
                                    if (pic.FILE_NAME.Split('.')[1].ToLower() == "jpg" || pic.FILE_NAME.Split('.')[1].ToLower() == "gif" || pic.FILE_NAME.Split('.')[1].ToLower() == "png" || pic.FILE_NAME.Split('.')[1].ToLower() == "bmp")
                                    {

                                        if (entity.Properties.Contains("thumbnailPhoto"))
                                        {
                                            entity.Properties["thumbnailPhoto"][0] = imgHelp.GetThumbnailImage(pic.FILE_NAME.Split('.')[1].ToLower(), percent);
                                        }
                                        else
                                        {
                                            entity.Properties["thumbnailPhoto"].Add(imgHelp.GetThumbnailImage(pic.FILE_NAME.Split('.')[1].ToLower(), percent));
                                        }
                                    }
                                    else
                                    {
                                        if (entity.Properties.Contains("thumbnailPhoto"))
                                        {
                                            entity.Properties["thumbnailPhoto"][0] = imgHelp.GetThumbnailImage("jpg", percent);
                                        }
                                        else
                                        {
                                            entity.Properties["thumbnailPhoto"].Add(imgHelp.GetThumbnailImage("jpg", percent));
                                        }
                                    }
                                }
                                else
                                {
                                    if (entity.Properties.Contains("thumbnailPhoto"))
                                    {
                                        entity.Properties["thumbnailPhoto"][0] = imgHelp.GetThumbnailImage("jpg", percent);
                                    }
                                    else
                                    {
                                        entity.Properties["thumbnailPhoto"].Add(imgHelp.GetThumbnailImage("jpg", percent));
                                    }
                                }
                            }
                            else
                            {
                                if (entity.Properties.Contains("thumbnailPhoto"))
                                {
                                    entity.Properties["thumbnailPhoto"][0] = imgHelp.GetThumbnailImage("jpg", percent);
                                }
                                else
                                {
                                    entity.Properties["thumbnailPhoto"].Add(imgHelp.GetThumbnailImage("jpg", percent));
                                }
                            }
                        }
                        #endregion
                        entity.CommitChanges();
                        entity.Close();
                    
                        string sql = @"update WF.INITIALIZATION_USER_AD_IMAGE set UPDATESTATUS = 1 , UPDATETIME = GETDATE() where LOGIN_NAME = '" + pic.LOGIN_NAME + "' ";
                        database.ExecuteNonQuery(CommandType.Text, sql);
                        write.WriteLine(pic.LOGIN_NAME + ",成功," + DateTime.Now + ",");
                    }
                }
                catch (Exception ex)
                {
                    if (entity != null)
                    {
                        entity.Close();
                    }

                    write.WriteLine(pic.LOGIN_NAME + ",失败," + DateTime.Now + "," + ex.Message);
                }
            }

            foreach (Picture pic in deletedPictures)
            {
                DirectoryEntry entity = null;
                List<SearchResult> result = null;
                try
                {
                    var exFilter = new ExtraFilter();
                    exFilter.UserFilter = "samAccountName=" + pic.LOGIN_NAME;
                    string filter = ADSearchConditions.GetFilterByMask(ADSchemaType.Users, exFilter);
                    ADSearchConditions conditons = new ADSearchConditions();
                    conditons.Scope = SearchScope.Subtree;
                    ServerInfo serverInfo = ServerInfoConfigSettings.GetConfig().ServerInfos["dc"].ToServerInfo();
                    MCS.Library.ADHelper adHelper = ADHelper.GetInstance(serverInfo);
                    result = adHelper.ExecuteSearch(adHelper.GetRootEntry(), filter, conditons, "distinguishedName");
                    if (result.Count < 1)
                    {
                        write.WriteLine(pic.LOGIN_NAME + ",失败," + DateTime.Now + ",AD中找不到该账号");
                    }
                    else
                    {
                        entity = adHelper.NewEntry(result[0].Properties["distinguishedName"][0].ToString());

                        if (entity.Properties.Contains("thumbnailPhoto"))
                        {
                            entity.Properties["thumbnailPhoto"].Clear();
                        }
                        
                        entity.CommitChanges();
                        entity.Close();

                        string sql = @"update WF.INITIALIZATION_USER_AD_IMAGE set UPDATESTATUS = 1, UPDATETIME = GETDATE() where LOGIN_NAME = '" + pic.LOGIN_NAME + "' ";
                        database.ExecuteNonQuery(CommandType.Text, sql);
                        write.WriteLine(pic.LOGIN_NAME + ",成功," + DateTime.Now + ",");
                    }
                }
                catch (Exception ex)
                {
                    if (entity != null)
                    {
                        entity.Close();
                    }

                    write.WriteLine(pic.LOGIN_NAME + ",失败," + DateTime.Now + "," + ex.Message);
                }
            }

            write.Close();
            #endregion

            pictures.Clear();
            deletedPictures.Clear();
        }

        //public static void Main() 
        //{
        //    Run();
        //}
    }
}
