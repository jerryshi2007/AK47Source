#region
// -------------------------------------------------
// Assembly	��	DeluxeWorks.Library
// FileName	��	FileCacheDependency.cs
// Remark	��	�ļ�������
// -------------------------------------------------
// VERSION  	AUTHOR		DATE			CONTENT
// 1.0		    ������	    20070430		����
// -------------------------------------------------
#endregion

using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using MCS.Library.Core;
using MCS.Library.Properties;

namespace MCS.Library.Caching
{
    /// <summary>
    /// �ļ��������ͻ��˴����ڳ�ʼ������Ķ���ʱ����Ҫ�ṩһ���ļ������飬
    /// ���������е��κ�һ���ļ������仯ʱ����Ϊ�����������ص�Cache����ڡ�
    /// </summary>
    public sealed class FileCacheDependency : DependencyBase
    {
        /// <summary>
        /// Ϊ��ʵ�ִ�Сд���⣬
        /// </summary>
        public class FileDependencyReadOnlyCollection : ReadOnlyCollection<string>
        {
            /// <summary>
            /// ���캯��
            /// </summary>
            /// <param name="ls">��ʼ���õ�List</param>
            public FileDependencyReadOnlyCollection(List<string> ls)
                : base(ls)
            { }

            /// <summary>
            /// �жϼ������Ƿ���ĳ���ַ����������ж�֮ǰ���ַ���ת��Ϊ��д
            /// </summary>
            /// <param name="key">��Ҫ�������ַ���</param>
            /// <returns>�Ƿ����</returns>
            public new bool Contains(string key)
            {
                key = key.ToUpper();
                return base.Contains(key);
            }

            /// <summary>
            /// ����ָ����������
            /// </summary>
            /// <param name="key">��ֵ</param>
            /// <returns>��ֵ����</returns>
            public new int IndexOf(string key)
            {
                key = key.ToUpper();
                return base.IndexOf(key);
            }
        }

        #region ˽����/�ṹ����
        //������Ŀ¼��FileSystemWatcher��ӳ��
        private class DirToFileSystemWatcherItem : IDisposable
        {
            private int referenceCount = 0;
            private readonly string directory = string.Empty;
            private readonly FileSystemWatcher watcher = null;

            /// <summary>
            /// �鿴ϵͳ�ļ���
            /// </summary>
            /// <param name="directory">Ŀ¼</param>
            /// <param name="fse">�ļ�ϵͳ�¼����</param>
            /// <param name="re">Reamed�¼����</param>
            public DirToFileSystemWatcherItem(string directory, FileSystemEventHandler fse, RenamedEventHandler re)
            {
                this.directory = directory;

                this.watcher = new FileSystemWatcher(directory);
                this.watcher.Changed += fse;
                this.watcher.Deleted += fse;
                this.watcher.Renamed += re;
                this.watcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.DirectoryName | NotifyFilters.FileName;

                this.watcher.EnableRaisingEvents = true;
            }

            public FileSystemWatcher Watcher
            {
                get { return this.watcher; }
            }

            public string Directory
            {
                get { return this.directory; }
            }

            public int ReferenceCount
            {
                get { return this.referenceCount; }
                set { this.referenceCount = value; }
            }

            public void Dispose()
            {
                if (this.watcher != null)
                    this.watcher.Dispose();

                GC.SuppressFinalize(true);
            }
        }

        /// <summary>
        /// �ļ����Ƶ�FileDependency��ӳ��
        /// </summary>
        private class FileNameToFileCacheDependenciesItem
        {
            private string fileName = string.Empty;
            private Dictionary<FileCacheDependency, FileCacheDependency> dependencies = new Dictionary<FileCacheDependency, FileCacheDependency>();

            /// <summary>
            /// ��ȡ�ļ�����
            /// </summary>
            /// <param name="dependenceFileName">���������ļ�������</param>
            public FileNameToFileCacheDependenciesItem(string dependenceFileName)
            {
                this.fileName = dependenceFileName;
            }

            public string FileName
            {
                get { return this.fileName; }
            }

            public Dictionary<FileCacheDependency, FileCacheDependency> Dependencies
            {
                get { return this.dependencies; }
            }
        }
        #endregion ˽���ඨ��

        #region ˽�о�̬��Ա
        private static Dictionary<string, DirToFileSystemWatcherItem>
            dirToFileSystemWatcherDict = new Dictionary<string, DirToFileSystemWatcherItem>(StringComparer.OrdinalIgnoreCase);

        private static Dictionary<string, FileNameToFileCacheDependenciesItem>
            fileNameToFileCacheDependenciesDict = new Dictionary<string, FileNameToFileCacheDependenciesItem>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// ͬ������
        /// </summary>
        private static object syncObject = new object();

        #endregion ˽�о�̬��Ա

        #region ˽�г�Ա

        private string[] dirs;
        private FileDependencyReadOnlyCollection fileNameCollection;
        private bool changed = false;
        private bool ignoreFileNotExist = false;

#if DELUXEWORKSTEST
		/// <summary>
		/// ȡ�ڲ����ϵĸ��������ڲ���
		/// </summary>
		private int DirToFileSystemWatcherDictCount
		{
			get
			{
				return FileCacheDependency.dirToFileSystemWatcherDict.Count;
			}
		}

		/// <summary>
		/// ȡ�ڲ����ϵĸ��������ڲ���
		/// </summary>
		private int FileNameToFileCacheDependenciesDictCount
		{
			get
			{
				return FileCacheDependency.fileNameToFileCacheDependenciesDict.Count;
			}
		}
#endif
        #endregion ˽������

        #region ���з���
        /// <summary>
        /// ���췽��
        /// </summary>
        /// <param name="fileNames">Cache���������ļ�����</param>
        /// <remarks>
        /// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Test\Caching\FileCacheDependencyTest.cs" region="FileCacheDependencyConstructorTest" lang="cs" title="ָ���������ļ����б�" />
        /// </remarks>
        public FileCacheDependency(params string[] fileNames)
        {
            Init(fileNames);
        }

        /// <summary>
        /// ���췽��
        /// </summary>
        /// <param name="ifne">�Ƿ���Բ����ڵ��ļ�������Ϊfalse�����ļ������ڻ��׳��쳣</param>
        /// <param name="fileNames">Cache���������ļ�����</param>
        public FileCacheDependency(bool ifne, params string[] fileNames)
        {
            this.ignoreFileNotExist = ifne;

            Init(fileNames);
        }

        /// <summary>
        /// �ļ�������
        /// </summary>
        /// <remarks>
        /// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Test\Caching\FileCacheDependencyTest.cs" region="FileCacheDependencyFileNamesTest" lang="cs" title="�ļ����б�ֻ��" />
        /// </remarks>
        public FileDependencyReadOnlyCollection FileNames
        {
            get
            {
                return this.fileNameCollection;
            }
        }
        /// <summary>
        /// �Ƿ����ı�
        /// </summary>
        /// <remarks>
        /// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Test\Caching\FileCacheDependencyTest.cs" region="FileDependencyChangeContentTest" lang="cs" title="��ȡ��Dependency�Ƿ����" />
        /// </remarks>
        public override bool HasChanged
        {
            get
            {
                return this.changed;
            }
        }
        #endregion ���з���

        #region �����ķ���
        /// <summary>
        /// 
        /// </summary>
        internal protected override void SetChanged()
        {
            this.changed = true;
        }

        /// <summary>
        /// �ͷ���Դ�������ڲ���FileSystemWatcher
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                lock (FileCacheDependency.syncObject)
                {
                    foreach (string dir in this.dirs)
                    {
                        DirToFileSystemWatcherItem item;

                        if (FileCacheDependency.dirToFileSystemWatcherDict.TryGetValue(dir, out item))
                        {
                            item.ReferenceCount--;

                            if (item.ReferenceCount == 0)
                            {
                                item.Watcher.Dispose();
                                FileCacheDependency.dirToFileSystemWatcherDict.Remove(dir);
                            }
                        }
                    }

                    foreach (string file in this.fileNameCollection)
                    {
                        FileNameToFileCacheDependenciesItem f2dItem;

                        if (FileCacheDependency.fileNameToFileCacheDependenciesDict.TryGetValue(file, out f2dItem))
                        {
                            f2dItem.Dependencies.Remove(this);

                            if (f2dItem.Dependencies.Count == 0)
                                FileCacheDependency.fileNameToFileCacheDependenciesDict.Remove(file);
                        }
                    }
                }
            }

            base.Dispose(disposing);
        }

        /// <summary>
        /// ����Dependency�󶨵�CacheItemʱ
        /// </summary>
        protected internal override void CacheItemBinded()
        {
            InitFileWatcher();
        }

        #endregion �����ķ���

        #region ˽�з���
        private void Init(string[] fileNames)
        {
            this.UtcLastModified = DateTime.UtcNow;
            this.UtcLastAccessTime = DateTime.UtcNow;

            this.InitFileNames(fileNames);
            this.InitDirs(this.fileNameCollection);
        }

        private static void FileSystemWatcher_Changed(object sender, FileSystemEventArgs e)
        {
            DoFileChange(e.FullPath);
        }

        private static void FileSystemWatcher_Renamed(object sender, RenamedEventArgs e)
        {
            DoFileChange(e.OldFullPath);
        }

        private static void DoFileChange(string fullPath)
        {
            try
            {
                lock (FileCacheDependency.syncObject)
                {
                    FileNameToFileCacheDependenciesItem f2dItem;
                    List<CacheItemBase> list = new List<CacheItemBase>();

                    if (FileCacheDependency.fileNameToFileCacheDependenciesDict.TryGetValue(fullPath, out f2dItem))
                    {
                        foreach (KeyValuePair<FileCacheDependency, FileCacheDependency> kp in f2dItem.Dependencies)
                        {
                            kp.Value.changed = true;

                            if (kp.Value.CacheItem != null)
                                list.Add(kp.Value.CacheItem);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Trace.Write(ex.Message);
            }
        }

        private void InitFileNames(string[] fileNames)
        {
            List<string> list = new List<string>(fileNames.Length);

            for (int i = 0; i < fileNames.Length; i++)
            {
                string fileName = fileNames[i].ToUpper();

                if (string.IsNullOrEmpty(fileName) == false)
                {
                    if (Path.IsPathRooted(fileName) == false)
                        fileName = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "\\" + fileName;

                    if (File.Exists(fileName) == false)
                        ExceptionHelper.FalseThrow<FileNotFoundException>(this.ignoreFileNotExist, Resource.FileNotFound, fileName);
                    else
                        list.Add(fileName);
                }
            }

            this.fileNameCollection = new FileDependencyReadOnlyCollection(list);
        }

        private void InitDirs(ReadOnlyCollection<string> fileNames)
        {
            Dictionary<string, string> dirDict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            foreach (string fileName in fileNames)
            {
                if (string.IsNullOrEmpty(fileName) == false)
                {
                    string dir = Path.GetDirectoryName(fileName);

                    ExceptionHelper.TrueThrow<ArgumentOutOfRangeException>(dir == null, "����ʹ�ø�Ŀ¼��ΪCache�������");

                    if (dirDict.ContainsKey(dir) == false)
                        dirDict.Add(dir, dir);
                }
            }

            string[] dires = new string[dirDict.Count];

            int j = 0;

            foreach (KeyValuePair<string, string> kp in dirDict)
                dires[j++] = kp.Value;

            this.dirs = dires;
        }

        private void InitFileWatcher()
        {
            lock (FileCacheDependency.syncObject)
            {
                foreach (string dir in this.dirs)
                {
                    DirToFileSystemWatcherItem item;

                    if (FileCacheDependency.dirToFileSystemWatcherDict.TryGetValue(dir, out item) == false)
                    {
                        item = new DirToFileSystemWatcherItem(dir,
                            new FileSystemEventHandler(FileSystemWatcher_Changed),
                            new RenamedEventHandler(FileSystemWatcher_Renamed));
                        FileCacheDependency.dirToFileSystemWatcherDict.Add(dir, item);
                    }

                    item.ReferenceCount++;
                }

                foreach (string fileName in this.fileNameCollection)
                {
                    FileNameToFileCacheDependenciesItem item;

                    if (FileCacheDependency.fileNameToFileCacheDependenciesDict.TryGetValue(fileName, out item) == false)
                    {
                        item = new FileNameToFileCacheDependenciesItem(fileName);
                        FileCacheDependency.fileNameToFileCacheDependenciesDict.Add(fileName, item);
                    }

                    item.Dependencies[this] = this;
                }
            }
        }
        #endregion ˽�з���
    }
}
