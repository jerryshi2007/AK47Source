using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Northwoods.GoXam.Model;
using Designer.Models;
using Northwoods.GoXam;
using Designer.Commands;
using System.Collections.ObjectModel;
using System.Xml.Linq;
using System.Linq;
using Designer.Utils;
using System.Windows.Browser;
using Newtonsoft.Json;

namespace Designer.ViewModels
{
    public class DiagramPageViewModel : ViewModelBase
    {
        public DiagramPageViewModel(WorkflowInfo info, IWebInterAction client)
        {
            this._clientProxy = client;
            InitCommand();
            this.WfInfo = info;
            this.Key = info.Key;
            this.Name = info.Name;

            if (!string.IsNullOrEmpty(info.GraphDescription))
            {
                DiagramModel.Load<ActivityNode, ActivityLink>(
                    XElement.Parse(info.GraphDescription),
                    WorkflowUtils.DIAGRAM_XELEMENT_NODENAME,
                    WorkflowUtils.DIAGRAM_XELEMENT_LINKNAME);

                foreach (var node in (ObservableCollection<ActivityNode>)DiagramModel.NodesSource)
                {
                    var actInfo = info.Activities.SingleOrDefault(p => p.Key == node.Key);
                    if (actInfo != null)
                    {
                        node.Category = actInfo.ActivityType.ToString();

                        if (actInfo.BranchProcessTemplates == null)
                            continue;

                        if (actInfo.BranchProcessTemplates.Count > 0)
                        {
                            node.WfHasBranchProcess = true;
                        }
                    }
                }
                //为兼容2011-04-22之前保存的数据
                foreach (var link in (ObservableCollection<ActivityLink>)DiagramModel.LinksSource)
                {
                    if (link.From == "N0" && link.FromPort == "portTop")
                    {
                        link.FromPort = "portBottom";
                    }
                }
                return;
            }

            foreach (ActivityInfo activity in info.Activities)
            {
                ActivityNode item = new ActivityNode()
                {
                    Key = activity.Key,
                    Category = activity.ActivityType.ToString(),
                    WfName = activity.Name,
                    WfDescription = activity.Description,
                    WfEnabled = activity.Enabled
                };

                this.DiagramModel.AddNode(item);
            }

            foreach (var transition in info.Transitions)
            {
                this.DiagramModel.AddLink(new ActivityLink()
                {
                    Key = transition.Key,
                    Text = transition.Name,
                    From = transition.FromActivityKey,
                    To = transition.ToActivityKey
                });
            }
        }

        #region Fields
        private IWebInterAction _clientProxy;

        private string _key;
        /// <summary>
        /// 流程KEY
        /// </summary>
        public string Key
        {
            get
            {
                return _key;
            }
            set
            {
                _key = value;
                OnPropertyChanged("Key");
            }
        }
        private string _name;
        /// <summary>
        /// 流程名称
        /// </summary>
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                OnPropertyChanged("Name");
            }
        }

        public string CurrentChildSelectedKey
        {
            get;
            set;
        }

        public WorkflowInfo WfInfo;

        private GraphLinksModel<ActivityNode, string, string, ActivityLink> _diagramModel;
        public GraphLinksModel<ActivityNode, string, string, ActivityLink> DiagramModel
        {
            get
            {
                if (_diagramModel == null)
                {
                    _diagramModel = new GraphLinksModel<ActivityNode, string, string, ActivityLink>()
                    {
                        Modifiable = true,
                        HasUndoManager = true
                    };
                }

                return _diagramModel;
            }
        }

        #endregion

        public event EventHandler RequestClose;

        #region Methodes

        public RelayCommand<object> CloseItemCommand
        {
            get;
            private set;
        }

        private void CloseItemCommandExecuted(object para)
        {
            if (RequestClose != null)
            {
                RequestClose(this, EventArgs.Empty);
            }
        }

        private void InitCommand()
        {
            CloseItemCommand = new RelayCommand<object>(CloseItemCommandExecuted, p => { return true; });
        }

        private RelayCommand<string> _ProcessChangeCommand = null;

        public RelayCommand<string> ProcessChangeCommand
        {
            get
            {
                if (this._ProcessChangeCommand == null)
                    this._ProcessChangeCommand = new RelayCommand<string>(ProcessChangeCommandExecuted, CanProcessChangeCommand);

                return this._ProcessChangeCommand;
            }
        }

        private static bool CanProcessChangeCommand(string para)
        {
            return true;
        }

        private static void ProcessChangeCommandExecuted(string para)
        {
            if (string.IsNullOrEmpty(para) == false)
            {
                HtmlPage.Window.Invoke("OpenEditor", para);
            }
        }

        private void Process_MouseButton(object sender, MouseButtonEventArgs e)
        {
            HtmlPage.Window.Invoke("LoadProperty", WorkflowUtils.CLIENTSCRIPT_PARAM_WORKFLOW, this.Key, JsonConvert.SerializeObject(new WorkflowInfo() { Key = this.Key }));
            MessageBox.Show((e.OriginalSource as FrameworkElement).Tag.ToString());
            /*	
            this.WebMethod.LoadProperty(WorkflowUtils.CLIENTSCRIPT_PARAM_WORKFLOW,
                this.mainDiagram.Tag.ToString(),
                WorkflowUtils.ExtractWorkflowInfoJson(this.mainDiagram)); */
        }

        private void LoadPropertyCommandExecuted(ActivityNode acNode)
        {
            this.CurrentChildSelectedKey = acNode.Key;
            MessageBox.Show(this.CurrentChildSelectedKey);

        }

        private bool CanLoadPropertyCommand(ActivityNode acNode)
        {
            return string.Compare(this.CurrentChildSelectedKey, acNode.Key) != 0;
        }

        private RelayCommand<ActivityNode> _LoadActivityPropertyCommand = null;

        public RelayCommand<ActivityNode> LoadActivityPropertyCommand
        {
            get
            {
                if (this._LoadActivityPropertyCommand == null)
                    this._LoadActivityPropertyCommand = new RelayCommand<ActivityNode>(this.LoadPropertyCommandExecuted, this.CanLoadPropertyCommand);

                return this._LoadActivityPropertyCommand;
            }
        }

        private RelayCommand<MouseButtonEventArgs> _ProcessPropertyLoadCommand = null;
        public RelayCommand<MouseButtonEventArgs> ProcessPropertyLoadCommand
        {
            get
            {
                if (this._ProcessPropertyLoadCommand == null)
                    this._ProcessPropertyLoadCommand = new RelayCommand<MouseButtonEventArgs>(
                        s =>
                        {
                            HtmlPage.Window.Invoke("LoadProperty", WorkflowUtils.CLIENTSCRIPT_PARAM_WORKFLOW, this.Key, JsonConvert.SerializeObject(new WorkflowInfo() { Key = this.Key }));
                            MessageBox.Show((s.OriginalSource as FrameworkElement).Tag.ToString());
                        },
                        e =>
                        {
                            if (this.Key != (e.OriginalSource as FrameworkElement).Tag.ToString())
                                return true;

                            return false;
                        });

                return this._ProcessPropertyLoadCommand;
            }
        }

        public RelayCommand<string> _CurrentSelectActivity = null;

        public RelayCommand<string> CurrentSelectActivity
        {
            get
            {
                if (this._CurrentSelectActivity == null)
                    this._CurrentSelectActivity = new RelayCommand<string>(
                    s =>
                    {
                        MessageBox.Show(s);
                    },
                    c =>
                    {
                        return true;
                    });

                return this._CurrentSelectActivity;
            }
        }

        #endregion
    }
}
