using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.SOA.DataObjects.Security;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PC = MCS.Library.SOA.DataObjects.Security;
using System.Threading;
using MCS.Library.Data.Builder;
using MCS.Library.OGUPermission;
using MCS.Library.SOA.DataObjects.Schemas.SchemaProperties;

namespace MCS.Library.SOA.DataObjects.Security.Test.SchemaObject
{
    class OguObjectGenerator1
    {
        const string wangliGuid = "22c3b351-a713-49f2-8f06-6b888a280fff";

        internal static readonly SCUser Wangli = new SCUser()
        {
            Name = "王黎",
            DisplayName = "王黎",
            CodeName = "wangli5",
            FirstName = "黎",
            LastName = "王",
            Status = SchemaObjectStatus.Normal,
            ID = wangliGuid,
        };

        static OguObjectGenerator1()
        {
            //Wangli.Properties["UserRank"].StringValue = "0";
            //Wangli.Properties["CadreType"].StringValue = "0";
            //Wangli.Properties["Mail"].StringValue = "wangli5@sinooceanland.com";
            //Wangli.Properties["Occupation"].StringValue = "";
        }

        internal static PC.Executors.ISCObjectOperations Facade
        {
            get { return PC.Executors.SCObjectOperations.Instance; }
        }

        private static ConnectiveSqlClauseCollection SchemaStrict(ConnectiveSqlClauseCollection c, string prefix, PC.StandardObjectSchemaType schema)
        {
            WhereSqlClauseBuilder where = new WhereSqlClauseBuilder();
            where.AppendItem(prefix + "SchemaType", schema);
            c.Add(where);
            return c;
        }

        public static void Generate()
        {
            /*
             * <组织机构>
             * ┗{标准管理维度}                                smd
             *      ┗{远洋地产}                               sinooceanland
             *          ┣{集团总部}                           groupHQ
             *          ┃  ┣{成本工程部}                     costEngineer
             *          ┃  ┃   ┣陈科                        chenke
             *          ┃  ┃   ┗王立臣                      fanhy
             *          ┃  ┣{养老地产业务发展中心}           
             *          ┃  ┃       ┣李业明                  liyem
             *          ┃  ┃       ┣张帅一（兼职）          zhangshuaiy
             *          ┃  ┃       ┗赵林                    zhaolin1
             *          ┃  ┃
             *          ┃  ┗{流程管理部}
             *          ┃          ┣[DemoGroup]
             *          ┃          ┃   ┗王立臣               fanhy
             *          ┃          ┣樊海云                    fanhy
             *          ┃          ┣杨睿1                     yangrui1
             *          ┃          ┣张帅一（主职）            zhangshuaiy
             *          ┃          ┗刘闽辉                    liumh
             *          ┣{城市公司}                            cityCompany 
             *          ┃   ┣{北京远联公司}                   beijingYuanlian
             *          ┃   ┃      ┣[群组A]                  groupA
             *          ┃   ┃      ┣{北京远联高管}           beijingYuanlianExecutives
             *          ┃   ┃      ┃      ┗ 刘闽辉(兼职)    liumh
             *          ┃   ┃      ┗{远洋傲北项目部}         yuanyangAobei
             *          ┃   ┃              ┣[群组B]          groupB
             *          ┃   ┃              ┃    ┗赵林       zhaolin1
             *          ┃   ┃              ┣王发平           wangfaping
             *          ┃   ┃              ┗金戈             jinge
             *          ┃   ┃
             *          ┃   ┣{北京中联公司}                   beijingZhonglian
             *          ┃   ┣{北京远豪公司}                   beijingYuanhao
             *          ┃   ┣{北京远胜公司}                   beijingYuansheng
             *          ┃   ┣{北京地区管理部}                 beijingArea
             *          ┃   ┣{大连明远公司}                   dalianMingyuan
             *          ┃   ┣{大连广宇公司}                   dalianGuangyu
             *          ┃   ┣{大连汇洋公司}                   dalianHuiyang
             *          ┃   ┗{大连地区管理部}                 dalianArea
             *          ┃
             *          ┗{商业地产事业部}                      bizLand
             *                ┗{远洋国际中心（北京）}          beijingSOI
             *                       ┣{北京远洋国际综合部}     beijingSOIC
             *                       ┃      ┣范晓君           fanxiaojun
             *                       ┃      ┗高根             gaogen
             *                       ┗胡瑞雪                   hurx
             *          
             * <应用授权>
             * ┣{办公门户}
             * ┗{机构人员管理}
             *      <角色>
             *    →┣【系统管理员】
             *    | ┃       ┣养老地产业务发展中心
             *    | ┃       ┣DemoGroup
             *    | ┃       ┗樊海云                           fanhy
             *    | ┃       
             *    | ┃
             *    | ┗【系统维护员】 ←┐
             *    | <权限>             |
             *    |  ┣《创建新机构》←┘
             *    → ┗《创建新用户》
             * 
             *		秘书:
             *		王立臣(fanhy)←{ 李业明(liyem),杨睿1(yangrui1) }
             *		陈科(chenke）←{ 李业明(liyem),王立臣(fanhy)}
             *		
             *              
             */
            MCS.Library.SOA.DataObjects.Security.Adapters.SchemaObjectAdapter.Instance.ClearAllData();

            GenerateOnly();
        }

        public static void GenerateOnly()
        {
            //Thread.Sleep(20);
            InitOrganizations();
            //Thread.Sleep(20);
            InitApplications();
            //Thread.Sleep(20);
            InitAppRoles();
            //Thread.Sleep(20);
            InitAppPermissions();
            //Thread.Sleep(20);
            InitAppPermissionsOfRoles();
            //Thread.Sleep(20);
            InitGroups();
            //Thread.Sleep(20);
            InitUsers();
            //Thread.Sleep(20);
            InitUsersInGroups();
            //Thread.Sleep(20);
            InitRoleMembers();
            //Thread.Sleep(20);
            InitSecretaries();
            Thread.Sleep(20);
        }

        class DemoUser : IUser, IUserPropertyAccessible
        {

            public OguObjectCollection<IUser> AllRelativeUserInfo
            {
                get { throw new NotImplementedException(); }
            }

            public UserAttributesType Attributes
            {
                get;
                set;
            }

            public string Email
            {
                get;
                set;
            }

            public bool IsSideline
            {
                get;
                set;
            }

            public string LogOnName
            {
                get;
                set;
            }

            public OguObjectCollection<IGroup> MemberOf
            {
                get { throw new NotImplementedException(); }
            }

            public string Occupation
            {
                get;
                set;
            }

            public UserPermissionCollection Permissions
            {
                get { throw new NotImplementedException(); }
            }

            public UserRankType Rank
            {
                get;
                set;
            }

            public UserRoleCollection Roles
            {
                get
                {
                    return new UserRoleCollection(this);
                }
            }

            public OguObjectCollection<IUser> Secretaries
            {
                get { throw new NotImplementedException(); }
            }

            public OguObjectCollection<IUser> SecretaryOf
            {
                get { throw new NotImplementedException(); }
            }

            public string Description
            {
                get;
                set;
            }

            public string DisplayName
            {
                get;
                set;
            }

            public string FullPath
            {
                get;
                set;
            }

            public string GlobalSortID
            {
                get;
                set;
            }

            public string ID
            {
                get;
                set;
            }

            public int Levels
            {
                get;
                set;
            }

            public string Name
            {
                get;
                set;
            }

            public SchemaType ObjectType
            {
                get;
                set;
            }

            public IOrganization Parent
            {
                get;
                set;
            }

            public System.Collections.IDictionary Properties
            {
                get { throw new NotImplementedException(); }
            }

            public string SortID
            {
                get;
                set;
            }

            public IOrganization TopOU
            {
                get;
                set;
            }


            public bool IsChildrenOf(IOrganization parent, bool includeSideline)
            {
                throw new NotImplementedException();
            }

            public bool IsInGroups(params IGroup[] groups)
            {
                throw new NotImplementedException();
            }


            public bool IsChildrenOf(IOrganization parent)
            {
                throw new NotImplementedException();
            }
        }

        internal static IUser CastUser(PC.SCUser obj)
        {
            IUserPropertyAccessible user = new DemoUser();

            user.Name = obj.Name;
            user.ObjectType = SchemaType.Users;
            user.DisplayName = obj.DisplayName;
            user.Email = obj.Properties.GetValue<string>("Mail", string.Empty);
            user.ID = obj.ID;
            user.LogOnName = obj.CodeName;
            user.Rank = UserRankType.FuKeji;

            return (IUser)user;

        }

        private static void InitRoleMembers()
        {

            var role = (PC.SCRole)(PC.Adapters.SchemaObjectAdapter.Instance.LoadByCodeName(c =>
            {
                SchemaStrict(c, "O.", PC.StandardObjectSchemaType.Roles);
            }, DateTime.MinValue, "系统管理员").First());


            var grp = (PC.SCGroup)(PC.Adapters.SchemaObjectAdapter.Instance.LoadByCodeName(c =>
            {
                SchemaStrict(c, "O.", PC.StandardObjectSchemaType.Groups);
            }, DateTime.MinValue, "DemoGroup").First());

            var user = (PC.SCUser)(PC.Adapters.SchemaObjectAdapter.Instance.LoadByCodeName(c =>
            {
                SchemaStrict(c, "O.", PC.StandardObjectSchemaType.Users);
            }, DateTime.MinValue, "fanhy").First());

            var org = (PC.SCOrganization)(PC.Adapters.SchemaObjectAdapter.Instance.LoadByCodeName(c =>
            {
                SchemaStrict(c, "O.", PC.StandardObjectSchemaType.Organizations);
            }, DateTime.MinValue, "养老地产业务发展中心").First());

            Facade.AddMemberToRole(grp, role);
            Facade.AddMemberToRole(user, role);
            Facade.AddMemberToRole(org, role);


        }

        private static void InitUsersInGroups()
        {
            var user = (PC.SCUser)(PC.Adapters.SchemaObjectAdapter.Instance.LoadByCodeName(c =>
            {
                SchemaStrict(c, "O.", PC.StandardObjectSchemaType.Users);
            }, DateTime.MinValue, "wanglch").First());

            var grp = (PC.SCGroup)(PC.Adapters.SchemaObjectAdapter.Instance.LoadByCodeName(c =>
            {
                SchemaStrict(c, "O.", PC.StandardObjectSchemaType.Groups);
            }, DateTime.MinValue, "DemoGroup").First());


            Facade.AddUserToGroup(user, grp);

            user = (PC.SCUser)(PC.Adapters.SchemaObjectAdapter.Instance.LoadByCodeName(c =>
            {
                SchemaStrict(c, "O.", PC.StandardObjectSchemaType.Users);
            }, DateTime.MinValue, "zhaolin1").First());

            grp = (PC.SCGroup)(PC.Adapters.SchemaObjectAdapter.Instance.LoadByCodeName(c =>
            {
                SchemaStrict(c, "O.", PC.StandardObjectSchemaType.Groups);
            }, DateTime.MinValue, "groupB").First());

            Facade.AddUserToGroup(user, grp);
        }

        private static void InitUsers()
        {
            PC.SCOrganization org = (PC.SCOrganization)PC.Adapters.SchemaObjectAdapter.Instance.LoadByCodeName(c =>
            {
                SchemaStrict(c, "O.", PC.StandardObjectSchemaType.Organizations);
            }, DateTime.MinValue, "流程管理部").First();

            PC.SCUser user1 = FillAdditionProperties(new PC.SCUser()
            {
                Name = "樊海云",
                DisplayName = "樊海云",
                CodeName = "fanhy",
                FirstName = "海云",
                LastName = "樊",
                Status = SchemaObjectStatus.Normal,
                ID = "3210EEA0-9E13-44E3-A321-D7693BD0911F",
                Creator = CastUser(Wangli),
            });

            PC.SCUser user2 = FillAdditionProperties(new PC.SCUser()
            {
                Name = "杨睿1",
                DisplayName = "杨睿(集团流程管理部)",
                CodeName = "yangrui1",
                FirstName = "睿1",
                LastName = "杨",
                Status = SchemaObjectStatus.Normal,
                ID = "766DFF51-6A29-43B2-95D9-D33456E536F9",
                Creator = CastUser(Wangli),

            });

            user2.Properties["Mail"].StringValue = "yangrui1@sinooceanland.com";

            PC.SCUser user3 = FillAdditionProperties(new PC.SCUser()
            {
                Name = "刘闽辉",
                DisplayName = "刘闽辉",
                CodeName = "liumh",
                FirstName = "闽辉",
                LastName = "刘",
                Status = SchemaObjectStatus.Normal,
                ID = "3729DAC3-80E0-476C-8C4A-264E0F67BBC2",
                Creator = CastUser(Wangli),
            });

            user3.Properties["Mail"].StringValue = "liumh@sinooceanland.com";

            Facade.AddUser(user1, org);
            Facade.AddUser(user2, org);
            Facade.AddUser(user3, org);

            PC.Adapters.UserPasswordAdapter.Instance.SetPassword(user3.ID, PC.Adapters.UserPasswordAdapter.GetPasswordType(), "password");

            PC.SCUser userOther = FillAdditionProperties(new PC.SCUser()
            {
                Name = "王立臣",
                DisplayName = "王立臣",
                CodeName = "wanglch",
                FirstName = "立臣",
                LastName = "王",
                Status = SchemaObjectStatus.Normal,
                ID = "587200E1-3EC4-43B2-8F3A-167E20F41D09",
                Creator = CastUser(Wangli),

            });

            userOther.Properties["Mail"].StringValue = "wanglch@sinooceanland.com";


            PC.SCOrganization orgOther = (PC.SCOrganization)PC.Adapters.SchemaObjectAdapter.Instance.LoadByCodeName(c =>
            {
                SchemaStrict(c, "O.", PC.StandardObjectSchemaType.Organizations);
            }, DateTime.MinValue, "costEngineer").First();

            Facade.AddUser(userOther, orgOther);

            orgOther = (PC.SCOrganization)PC.Adapters.SchemaObjectAdapter.Instance.LoadByCodeName(c =>
            {
                SchemaStrict(c, "O.", PC.StandardObjectSchemaType.Organizations);
            }, DateTime.MinValue, "beijingYuanlianExecutives").First();

            Facade.AddUserToOrganization(user3, orgOther);

            orgOther = (PC.SCOrganization)PC.Adapters.SchemaObjectAdapter.Instance.LoadByCodeName(c =>
            {
                SchemaStrict(c, "O.", PC.StandardObjectSchemaType.Organizations);
            }, DateTime.MinValue, "养老地产业务发展中心").First();

            userOther = FillAdditionProperties(new PC.SCUser()
            {
                Name = "赵林",
                DisplayName = "赵林",
                CodeName = "zhaolin1",
                FirstName = "林",
                LastName = "赵",
                Status = SchemaObjectStatus.Normal,
                ID = "D1C28431-DD5D-496E-865B-85C6D89ED3D6",
                Creator = CastUser(Wangli),

            });

            userOther.Properties["Mail"].StringValue = "zhaolin1@sinooceanland.com";

            Facade.AddUser(userOther, orgOther);

            userOther = FillAdditionProperties(new PC.SCUser()
            {
                Name = "李业明",
                DisplayName = "李业明",
                CodeName = "liyem",
                FirstName = "业明",
                LastName = "李",
                Status = SchemaObjectStatus.Normal,
                ID = "B36CC307-1D92-4D27-B670-A4301006BF0B",
                Creator = CastUser(Wangli),

            });

            userOther.Properties["Mail"].StringValue = "liyem@sinooceanland.com";

            Facade.AddUser(userOther, orgOther);

            userOther = FillAdditionProperties(new PC.SCUser()
            {
                Name = "张帅一",
                DisplayName = "张帅一",
                CodeName = "zhangshuaiy",
                FirstName = "帅一",
                LastName = "张",
                Status = SchemaObjectStatus.Normal,
                ID = "EF68CDD1-3FD3-418A-8D4D-98F2E02FB2CA",
                Creator = CastUser(Wangli),

            });

            userOther.Properties["Mail"].StringValue = "zhangshuaiy@sinooceanland.com";

            Facade.AddUser(userOther, org); //主职

            Facade.AddUserToOrganization(userOther, orgOther); //兼职

            Facade.SetUserDefaultOrganization(userOther, org); //确保主职


            userOther = FillAdditionProperties(new PC.SCUser()
            {
                Name = "陈科",
                DisplayName = "陈科",
                CodeName = "chenke",
                FirstName = "科",
                LastName = "陈",
                Status = SchemaObjectStatus.Normal,
                ID = "E8F71526-23A2-4A39-A8AC-F79A2D255D1A",
                Creator = CastUser(Wangli),

            });

            userOther.Properties["Mail"].StringValue = "chenke@sinooceanland.com";

            orgOther = (PC.SCOrganization)PC.Adapters.SchemaObjectAdapter.Instance.LoadByCodeName(c =>
            {
                SchemaStrict(c, "O.", PC.StandardObjectSchemaType.Organizations);
            }, DateTime.MinValue, "costEngineer").First();

            Facade.AddUser(userOther, orgOther);

            // 独立部门开始

            userOther = FillAdditionProperties(new PC.SCUser()
            {
                Name = "王发平",
                DisplayName = "王发平",
                CodeName = "wangfaping",
                FirstName = "发平",
                LastName = "王",
                Status = SchemaObjectStatus.Normal,
                ID = "2B67F7D0-9362-401F-977F-3E267E87298B",
                Creator = CastUser(Wangli),

            });

            userOther.Properties["Mail"].StringValue = "wangfaping@sinooceanland.com";

            orgOther = (PC.SCOrganization)PC.Adapters.SchemaObjectAdapter.Instance.LoadByCodeName(c =>
            {
                SchemaStrict(c, "O.", PC.StandardObjectSchemaType.Organizations);
            }, DateTime.MinValue, "yuanyangAobei").First();

            Facade.AddUser(userOther, orgOther);

            userOther = FillAdditionProperties(new PC.SCUser()
            {
                Name = "金戈",
                DisplayName = "金戈",
                CodeName = "jinge",
                FirstName = "戈",
                LastName = "金",
                Status = SchemaObjectStatus.Normal,
                ID = "1688EB81-0FFB-4072-B26B-6DB571DCC391",
                Creator = CastUser(Wangli),

            });

            userOther.Properties["Mail"].StringValue = "jinge@sinooceanland.com";

            Facade.AddUser(userOther, orgOther);

            userOther = FillAdditionProperties(new PC.SCUser()
            {
                Name = "范晓君",
                DisplayName = "范晓君",
                CodeName = "fanxiaojun",
                FirstName = "晓君",
                LastName = "范",
                Status = SchemaObjectStatus.Normal,
                ID = "165EE11A-76ED-4C91-BD02-4AB6703F8FFA",
                Creator = CastUser(Wangli),

            });

            userOther.Properties["Mail"].StringValue = "fanxiaojun@sinooceanland.com";

            orgOther = (PC.SCOrganization)PC.Adapters.SchemaObjectAdapter.Instance.LoadByCodeName(c =>
            {
                SchemaStrict(c, "O.", PC.StandardObjectSchemaType.Organizations);
            }, DateTime.MinValue, "beijingSOIC").First();

            Facade.AddUser(userOther, orgOther);

            userOther = FillAdditionProperties(new PC.SCUser()
            {
                Name = "高根",
                DisplayName = "高根",
                CodeName = "gaogen",
                FirstName = "根",
                LastName = "高",
                Status = SchemaObjectStatus.Normal,
                ID = "81987195-756E-4FDA-868E-E4177096B212",
                Creator = CastUser(Wangli),

            });

            userOther.Properties["Mail"].StringValue = "gaogen@sinooceanland.com";

            Facade.AddUser(userOther, orgOther);

            userOther = FillAdditionProperties(new PC.SCUser()
            {
                Name = "胡瑞雪",
                DisplayName = "胡瑞雪",
                CodeName = "hurx",
                FirstName = "瑞雪",
                LastName = "胡",
                Status = SchemaObjectStatus.Normal,
                ID = "4FD109E4-FFC5-487C-B99B-DB20B6A0B2B9",
                Creator = CastUser(Wangli),

            });

            userOther.Properties["Mail"].StringValue = "hurx@sinooceanland.com";

            orgOther = (PC.SCOrganization)PC.Adapters.SchemaObjectAdapter.Instance.LoadByCodeName(c =>
            {
                SchemaStrict(c, "O.", PC.StandardObjectSchemaType.Organizations);
            }, DateTime.MinValue, "beijingSOI").First();

            Facade.AddUser(userOther, orgOther);

        }

        private static void InitSecretaries()
        {
            // 秘书
            PC.SCUser wanglich = (PC.SCUser)PC.Adapters.SchemaObjectAdapter.Instance.LoadByCodeName("Users", "wanglch", DateTime.MinValue);

            PC.SCUser liyem = (PC.SCUser)PC.Adapters.SchemaObjectAdapter.Instance.LoadByCodeName("Users", "liyem", DateTime.MinValue);

            PC.SCUser yangrui1 = (PC.SCUser)PC.Adapters.SchemaObjectAdapter.Instance.LoadByCodeName("Users", "yangrui1", DateTime.MinValue);

            PC.SCUser chenke = (PC.SCUser)PC.Adapters.SchemaObjectAdapter.Instance.LoadByCodeName("Users", "chenke", DateTime.MinValue);

            PC.Executors.SCObjectOperations.Instance.AddSecretaryToUser(liyem, wanglich);

            PC.Executors.SCObjectOperations.Instance.AddSecretaryToUser(yangrui1, wanglich);

            PC.Executors.SCObjectOperations.Instance.AddSecretaryToUser(liyem, chenke);

            PC.Executors.SCObjectOperations.Instance.AddSecretaryToUser(wanglich, chenke);

        }


        private static void InitGroups()
        {
            PC.SCOrganization org = (PC.SCOrganization)PC.Adapters.SchemaObjectAdapter.Instance.LoadByCodeName(c =>
            {
                SchemaStrict(c, "O.", PC.StandardObjectSchemaType.Organizations);
            }, DateTime.MinValue, "流程管理部").First();


            PC.SCGroup grp1 = new PC.SCGroup()
            {
                Name = "DemoGroup",
                CodeName = "DemoGroup",
                DisplayName = "DemoGroup",
                Creator = CastUser(Wangli),
                ID = "BA4E965E-8A35-4986-B5EA-7631D9368FFF",
                Status = SchemaObjectStatus.Normal,
            };

            Facade.AddGroup(grp1, org);

            org = (PC.SCOrganization)PC.Adapters.SchemaObjectAdapter.Instance.LoadByCodeName(c =>
            {
                SchemaStrict(c, "O.", PC.StandardObjectSchemaType.Organizations);
            }, DateTime.MinValue, "beijingYuanlian").First();

            grp1 = new PC.SCGroup()
            {
                Name = "群组A",
                CodeName = "groupA",
                DisplayName = "群组A",
                Creator = CastUser(Wangli),
                ID = "CA093A1E-B207-48DB-B3B2-B085A81DA36A",
                Status = SchemaObjectStatus.Normal,
            };

            Facade.AddGroup(grp1, org);

            grp1 = new PC.SCGroup()
            {
                Name = "群组B",
                CodeName = "groupB",
                DisplayName = "群组B",
                Creator = CastUser(Wangli),
                ID = "A465FFC8-A742-41F3-A1B6-0D40FC5EA3D5",
                Status = SchemaObjectStatus.Normal,
            };

            org = (PC.SCOrganization)PC.Adapters.SchemaObjectAdapter.Instance.LoadByCodeName(c =>
            {
                SchemaStrict(c, "O.", PC.StandardObjectSchemaType.Organizations);
            }, DateTime.MinValue, "yuanyangAobei").First();

            Facade.AddGroup(grp1, org);

        }

        private static void InitAppPermissionsOfRoles()
        {
            PC.SCApplication app = (PC.SCApplication)PC.Adapters.SchemaObjectAdapter.Instance.LoadByCodeName(c => { }, DateTime.MinValue, "OGU_ADMIN").First();

            var permissions = app.CurrentPermissions;

            var roles = app.CurrentRoles;

            var justRole = (from r in roles where ((PC.SCRole)r).CodeName == "系统管理员" select ((PC.SCRole)r)).First();

            var justPermission = (from p in permissions where ((PC.SCPermission)p).CodeName == "创建新用户" select ((PC.SCPermission)p)).First();

            Facade.JoinRoleAndPermission(justRole, justPermission);

            justRole = (from r in roles where ((PC.SCRole)r).CodeName == "系统维护员" select ((PC.SCRole)r)).First();

            justPermission = (from p in permissions where ((PC.SCPermission)p).CodeName == "创建新机构" select ((PC.SCPermission)p)).First();

            Facade.JoinRoleAndPermission(justRole, justPermission);

        }

        private static void InitAppPermissions()
        {
            PC.SCApplication app = (PC.SCApplication)PC.Adapters.SchemaObjectAdapter.Instance.LoadByCodeName(c => { }, DateTime.MinValue, "OGU_ADMIN").First();

            PC.SCPermission fun1 = new PC.SCPermission()
            {
                Name = "创建新机构",
                CodeName = "创建新机构",
                DisplayName = "创建新机构",
                Creator = CastUser(Wangli),
                ID = "48DED987-42AA-4E53-A79D-270449CE6056",
                Status = SchemaObjectStatus.Normal,
            };

            PC.SCPermission fun2 = new PC.SCPermission()
            {
                Name = "创建新用户",
                CodeName = "创建新用户",
                DisplayName = "创建新用户",
                Creator = CastUser(Wangli),
                ID = "C04A7B64-8839-49DE-AA34-4EF28ACEEEEA",
                Status = SchemaObjectStatus.Normal,
            };

            Facade.AddPermission(fun1, app);
            Facade.AddPermission(fun2, app);

            Assert.IsTrue(app.CurrentPermissions.Count == 2);
        }

        private static void InitAppRoles()
        {
            PC.SCApplication app = (PC.SCApplication)PC.Adapters.SchemaObjectAdapter.Instance.LoadByCodeName(c => { }, DateTime.MinValue, "OGU_ADMIN").First();

            PC.SCRole role = new PC.SCRole()
            {
                Name = "系统管理员",
                CodeName = "系统管理员",
                DisplayName = "系统管理员",
                Creator = CastUser(Wangli),
                ID = "32491E78-BE9D-4159-8F29-1D6D56BC3166",
                Status = SchemaObjectStatus.Normal,
            };

            PC.SCRole role2 = new PC.SCRole()
            {
                Name = "系统维护员",
                CodeName = "系统维护员",
                DisplayName = "系统维护员",
                Creator = CastUser(Wangli),
                ID = "B7C4C54C-C241-4704-9079-230CE9F61B53",
                Status = SchemaObjectStatus.Normal,
            };

            Facade.AddRole(role, app);

            Facade.AddRole(role2, app);

            app = (PC.SCApplication)PC.Adapters.SchemaObjectAdapter.Instance.LoadByCodeName(c => { }, DateTime.MinValue, "APP_ADMIN").First();


            Facade.AddRole(new SCRole()
            {
                Name = "系统应用维护员",
                CodeName = "SYSTEM_APP_MODIFYER",
                DisplayName = "系统应用维护员",
                Creator = CastUser(Wangli),
                ID = "476442f3-5336-918b-4c57-f7b5cd519f6c",
                Status = SchemaObjectStatus.Normal,
            }, app);

            Facade.AddRole(new SCRole()
            {
                Name = "系统总管理员",
                CodeName = "SYSTEM_MAX_ADMINISTRATOR",
                DisplayName = "系统总管理员",
                Creator = CastUser(Wangli),
                ID = "cdcfc01f-316a-adc7-4bac-f4f111a6d270",
                Status = SchemaObjectStatus.Normal,
            }, app);
        }

        private static void InitApplications()
        {
            PC.SCApplication appGeneral = new PC.SCApplication()
            {

                CodeName = "OGU_ADMIN",
                Name = "机构人员管理",
                DisplayName = "机构人员管理",
                ID = "99bc9c59-d436-4156-88e4-53c1147de180",
                Status = SchemaObjectStatus.Normal,
            };

            PC.SCApplication officeHome = new PC.SCApplication()
            {

                CodeName = "OAPORTAL",
                Name = "办公门户",
                DisplayName = "办公门户",
                ID = "18f556b2-f047-43a1-af53-d228818682e3",
                Status = SchemaObjectStatus.Normal,
            };

            PC.SCApplication appAdmin = new SCApplication()
            {
                CodeName = "APP_ADMIN",
                Name = "通用授权",
                DisplayName = "通用授权",
                ID = "11111111-1111-1111-1111-111111111111",
                Status = SchemaObjectStatus.Normal
            };

            Facade.AddApplication(appGeneral);
            Facade.AddApplication(officeHome);
            Facade.AddApplication(appAdmin);
        }

        private static void InitOrganizations()
        {
            var rootOrg = (PC.SCOrganization)PC.Adapters.SchemaObjectAdapter.Instance.Load(PC.SCOrganization.RootOrganizationID);

            var smd = FillAdditionProperties(new PC.SCOrganization()
            {
                Name = "标准管理维度",
                CodeName = "smd",
                Creator = CastUser(Wangli),
                ID = "FA03243F-94C7-4BF4-A3C6-80AA4D52C9D1",
                DisplayName = "标准管理维度",
                Status = SchemaObjectStatus.Normal
            });


            var sinoocean = FillAdditionProperties(new PC.SCOrganization()
            {
                Name = "远洋地产",
                CodeName = "sinooceanland",
                Creator = CastUser(Wangli),
                ID = "C5D914AC-9C53-42DC-8A32-5E7652C4691C",
                DisplayName = "远洋地产",
                Status = SchemaObjectStatus.Normal,
            });

            var groupHQ = FillAdditionProperties(new PC.SCOrganization()
            {
                Name = "集团总部",
                CodeName = "groupHQ",
                Creator = CastUser(Wangli),
                ID = "D2649FD8-CD4A-4B06-9420-8108BA3976B4",
                DisplayName = "集团总部",
                Status = SchemaObjectStatus.Normal,
            });



            var costEngineer = FillAdditionProperties(new PC.SCOrganization()
            {
                Name = "成本工程部",
                CodeName = "costEngineer",
                Creator = CastUser(Wangli),
                ID = "5B5EF7DB-E83A-4752-9F75-D7EC6745E307",
                DisplayName = "成本工程部",
                Status = SchemaObjectStatus.Normal,
            });


            var processDept = FillAdditionProperties(new PC.SCOrganization()
            {
                Name = "流程管理部",
                CodeName = "流程管理部",
                Creator = CastUser(Wangli),
                ID = "A3546A55-3A8C-44EF-8CAC-BF1E4144D715",
                DisplayName = "流程管理部",
                Status = SchemaObjectStatus.Normal,
            });

            var yanglao = FillAdditionProperties(new PC.SCOrganization()
            {
                Name = "养老地产业务发展中心",
                CodeName = "养老地产业务发展中心",
                Creator = CastUser(Wangli),
                ID = "ED582E77-167D-4B87-95A8-82AC4B1017A8",
                DisplayName = "养老地产业务发展中心",
                Status = SchemaObjectStatus.Normal,
            });


            Facade.AddOrganization(smd, rootOrg);

            Facade.AddOrganization(sinoocean, smd);

            Facade.AddOrganization(groupHQ, sinoocean);

            Facade.AddOrganization(costEngineer, groupHQ);

            Facade.AddOrganization(processDept, groupHQ);

            Facade.AddOrganization(yanglao, groupHQ);

            var cityCompany = FillAdditionProperties(new PC.SCOrganization()
            {
                Name = "城市公司",
                CodeName = "cityCompany",
                Creator = CastUser(Wangli),
                ID = "91841971-44CB-4895-8B31-D9EA7432A74A",
                DisplayName = "城市公司",
                Status = SchemaObjectStatus.Normal,
            });

            Facade.AddOrganization(cityCompany, sinoocean);

            var beijingYuanlian = FillAdditionProperties(new PC.SCOrganization()
            {
                Name = "北京远联公司",
                CodeName = "beijingYuanlian",
                Creator = CastUser(Wangli),
                ID = "471F24D5-962E-46B9-849B-639D0AEB2B16",
                DisplayName = "北京远联公司",
                Status = SchemaObjectStatus.Normal,
            });

            Facade.AddOrganization(beijingYuanlian, cityCompany);

            var beijingYuanlianExecutives = FillAdditionProperties(new PC.SCOrganization()
            {
                Name = "北京远联高管",
                CodeName = "beijingYuanlianExecutives",
                Creator = CastUser(Wangli),
                ID = "066352AA-8349-4D21-B83F-C909BA5B8352",
                DisplayName = "北京远联高管",
                Status = SchemaObjectStatus.Normal,
            });

            Facade.AddOrganization(beijingYuanlianExecutives, beijingYuanlian);

            var yuanyangAobei = FillAdditionProperties(new PC.SCOrganization()
            {
                Name = "远洋傲北项目部",
                CodeName = "yuanyangAobei",
                Creator = CastUser(Wangli),
                ID = "2F28C437-BBF9-4969-9C07-639BD9716B1E",
                DisplayName = "远洋傲北项目部",
                Status = SchemaObjectStatus.Normal,
            });

            Facade.AddOrganization(yuanyangAobei, beijingYuanlian);

            var beijingZhonglian = FillAdditionProperties(new PC.SCOrganization()
            {
                Name = "北京中联公司",
                CodeName = "beijingZhonglian",
                Creator = CastUser(Wangli),
                ID = "16903BF9-74B5-4B58-9204-8BB20F341D88",
                DisplayName = "北京中联公司",
                Status = SchemaObjectStatus.Normal,
            });

            Facade.AddOrganization(beijingZhonglian, cityCompany);

            var beijingYuanhao = FillAdditionProperties(new PC.SCOrganization()
            {
                Name = "北京远豪公司",
                CodeName = "beijingYuanhao",
                Creator = CastUser(Wangli),
                ID = "D526FC07-E1EA-4AD4-9E24-C4645DF059D3",
                DisplayName = "北京远豪公司",
                Status = SchemaObjectStatus.Normal,
            });

            Facade.AddOrganization(beijingYuanhao, cityCompany);

            var beijingYuansheng = FillAdditionProperties(new PC.SCOrganization()
            {
                Name = "北京远胜公司",
                CodeName = "beijingYuansheng",
                Creator = CastUser(Wangli),
                ID = "78B35578-675F-460F-B7EE-18827F30FA05",
                DisplayName = "北京远胜公司",
                Status = SchemaObjectStatus.Normal,
            });

            Facade.AddOrganization(beijingYuansheng, cityCompany);

            var dalianMingyuan = FillAdditionProperties(new PC.SCOrganization()
            {
                Name = "大连明远公司",
                CodeName = "dalianMingyuan",
                Creator = CastUser(Wangli),
                ID = "6902377B-572E-4329-94D2-B63228A0F921",
                DisplayName = "大连明远公司",
                Status = SchemaObjectStatus.Normal,
            });

            Facade.AddOrganization(dalianMingyuan, cityCompany);

            var dalianGuangyu = FillAdditionProperties(new PC.SCOrganization()
            {
                Name = "大连广宇公司",
                CodeName = "dalianGuangyu",
                Creator = CastUser(Wangli),
                ID = "8E83B406-0DE1-41F3-BDF9-F8700F9A3C21",
                DisplayName = "大连广宇公司",
                Status = SchemaObjectStatus.Normal,
            });

            Facade.AddOrganization(dalianGuangyu, cityCompany);

            var dalianHuiyang = FillAdditionProperties(new PC.SCOrganization()
            {
                Name = "大连汇洋公司",
                CodeName = "dalianHuiyang",
                Creator = CastUser(Wangli),
                ID = "8BC92709-7430-42F5-8E7C-B6CDEE737B08",
                DisplayName = "大连汇洋公司",
                Status = SchemaObjectStatus.Normal,
            });

            Facade.AddOrganization(dalianHuiyang, cityCompany);

            var dalianArea = FillAdditionProperties(new PC.SCOrganization()
            {
                Name = "大连地区管理部",
                CodeName = "dalianArea",
                Creator = CastUser(Wangli),
                ID = "604927E3-75C4-4156-9D01-457F49C9B8F3",
                DisplayName = "大连地区管理部",
                Status = SchemaObjectStatus.Normal,
            });

            Facade.AddOrganization(dalianArea, cityCompany);

            var bizLand = FillAdditionProperties(new PC.SCOrganization()
            {
                Name = "商业地产事业部",
                CodeName = "bizLand",
                Creator = CastUser(Wangli),
                ID = "09355DE5-EC35-4B10-99B6-A8BF5050A43A",
                DisplayName = "商业地产事业部",
                Status = SchemaObjectStatus.Normal,
            });

            Facade.AddOrganization(bizLand, sinoocean);

            var beijingSOI = FillAdditionProperties(new PC.SCOrganization()
            {
                Name = "远洋国际中心（北京）",
                CodeName = "beijingSOI",
                Creator = CastUser(Wangli),
                ID = "4C257248-1C99-42AB-A5DA-313DABDFEAB8",
                DisplayName = "远洋国际中心（北京）",
                Status = SchemaObjectStatus.Normal,
            });

            Facade.AddOrganization(beijingSOI, bizLand);

            var beijingSOIC = FillAdditionProperties(new PC.SCOrganization()
            {
                Name = "北京远洋国际综合部",
                CodeName = "beijingSOIC",
                Creator = CastUser(Wangli),
                ID = "4C83FE72-032E-43C2-ADA2-B322BDE4A454",
                DisplayName = "北京远洋国际综合部",
                Status = SchemaObjectStatus.Normal,
            });

            Facade.AddOrganization(beijingSOIC, beijingSOI);

        }

        private static PC.SCOrganization FillAdditionProperties(PC.SCOrganization org)
        {
            //testOrg.Properties["DepartmentRank"].StringValue = ((int)DepartmentRankType.MinGanJiBie).ToString();
            //testOrg.Properties["DepartmentType"].StringValue = ((int)DepartmentTypeDefine.XuNiJiGou).ToString();
            //testOrg.Properties["DepartmentClass"].StringValue = ((int)DepartmentClassType.QiTaJiGou).ToString();
            //testOrg.Properties["CustomsCode"].StringValue = "Some more";

            return org;
        }


        private static PC.SCUser FillAdditionProperties(PC.SCUser user)
        {
            user.Properties["Mail"].StringValue = "fanhy@sinooceanland.com";
            user.Properties["Sip"].StringValue = "fanhy@sinooceanland.com";
            user.Properties["MP"].StringValue = "fanhy@sinooceanland.com";
            user.Properties["WP"].StringValue = "fanhy@sinooceanland.com";
            user.Properties["Address"].StringValue = "fanhy@sinooceanland.com";

            //user.Properties["CadreType"].StringValue = ((int)UserAttributesType.ShuGuanGanBu).ToString(); //干部属性
            //user.Properties["UserRank"].StringValue = ((int)UserRankType.FuBuJi).ToString(); //人员职级
            //user.Properties["Occupation"].StringValue = "test any"; //职位

            return user;
        }
    }
}
