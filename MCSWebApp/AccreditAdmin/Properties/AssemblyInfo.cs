using System.Reflection;
using System.Runtime.CompilerServices;

//
// �йس��򼯵ĳ�����Ϣ��ͨ������
// ���Լ����Ƶġ�������Щ����ֵ���޸������
// ��������Ϣ��
//
[assembly: AssemblyTitle("������Ա����ϵͳ")]
//[assembly: AssemblyDescription("������Ա�������")]
[assembly: AssemblyConfiguration("")]
//[assembly: AssemblyCompany("China Customs")]
[assembly: AssemblyProduct("������Ա����ϵͳ")]
//[assembly: AssemblyCopyright("cgac\\yuan_yong")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]		

//
// ���򼯵İ汾��Ϣ������ 4 ��ֵ���:
//
//      ���汾
//      �ΰ汾 
//      �ڲ��汾��
//      �޶���
//
// ������ָ��������Щֵ��Ҳ����ʹ�á��޶��š��͡��ڲ��汾�š���Ĭ��ֵ�������ǰ�
// ������ʾʹ�� '*':

//[assembly: AssemblyVersion("1.0.0.0")]
[assembly: System.Runtime.InteropServices.Guid("3d5900ae-111a-45be-3456-d9e4606ca793")]

//
// Ҫ�Գ��򼯽���ǩ��������ָ��Ҫʹ�õ���Կ���йس���ǩ���ĸ�����Ϣ����ο�
// Microsoft .NET Framework �ĵ���
//
// ʹ����������Կ�������ǩ������Կ��
//
// ע��: 
//   (*) ���δָ����Կ������򼯲��ᱻǩ����
//   (*) KeyName ��ָ�Ѿ���װ��
//       ������ϵļ��ܷ����ṩ����(CSP)�е���Կ��KeyFile ��ָ����
//       ��Կ���ļ���
//   (*) ��� KeyFile �� KeyName ֵ����ָ������
//       ��������Ĵ���: 
//       (1) ����� CSP �п����ҵ� KeyName����ʹ�ø���Կ��
//       (2) ��� KeyName �����ڶ� KeyFile ���ڣ��� 
//           KeyFile �е���Կ��װ�� CSP �в���ʹ�ø���Կ��
//   (*) Ҫ���� KeyFile������ʹ�� sn.exe(ǿ����)ʵ�ù��ߡ�
//        ��ָ�� KeyFile ʱ��KeyFile ��λ��Ӧ��
//        ����ڡ���Ŀ���Ŀ¼������Ŀ���
//        Ŀ¼��λ��ȡ����������ʹ�ñ�����Ŀ���� Web ��Ŀ��
//        ���ڱ�����Ŀ����Ŀ���Ŀ¼����Ϊ
//       <Project Directory>\obj\<Configuration>�����磬��� KeyFile λ�ڸ�
//       ��ĿĿ¼�У�Ӧ�� AssemblyKeyFile 
//       ����ָ��Ϊ [assembly: AssemblyKeyFile("..\\..\\mykey.snk")]
//        ���� Web ��Ŀ����Ŀ���Ŀ¼����Ϊ
//       %HOMEPATH%\VSWebCache\<Machine Name>\<Project Directory>\obj\<Configuration>��
//   (*) ���ӳ�ǩ������һ���߼�ѡ�� - �й����ĸ�����Ϣ������� Microsoft .NET Framework
//       �ĵ���
//
[assembly: AssemblyDelaySign(false)]
//[assembly: AssemblyKeyFile("")]
//[assembly: AssemblyKeyName("")]
[assembly: AssemblyVersion("1.4.0.0")]
[assembly: AssemblyFileVersion("1.4.0.0")]
[assembly: AssemblyCopyright("@China Customs Information Center (CCIC) 2008")]
[assembly: AssemblyCompany("China Customs Information Center (CCIC)")]
[assembly: AssemblyDescription(@"Version 1.4 [2009.1.5] : 1��ͳһ�������ݻ������������µ����ݻ���lock���⣬ͬʱ��Ĭ�ϻ�����е���;")]
