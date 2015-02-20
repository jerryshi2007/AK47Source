using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using MCS.Library.SOA.DocServiceContract;

namespace MCS.Library.SOA.DocServiceContract
{
    /// <summary>
    /// 文件存储服务
    /// </summary>
    [ServiceContract]
    [ServiceKnownType(typeof(DCTGroup))]
    [ServiceKnownType(typeof(DCTUser))]
    [ServiceKnownType(typeof(DCTFolder))]
    [ServiceKnownType(typeof(DCTFile))]
    public interface IDCSStorageService
    {
        /// <summary>
        /// 获取根文件夹
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        DCTFolder DCMGetRootFolder();

        /// <summary>
        /// 根据Id获取文件夹
        /// </summary>
        /// <param name="id">文件夹id</param>
        /// <exception cref="TargetNotFoundException">目标不存在</exception>
        /// <returns></returns>
        [OperationContract]
        DCTFolder DCMGetFolderById(int id);

        /// <summary>
        /// 根据Uri打开文件夹
        /// </summary>
        /// <param name="uri">文件夹uri</param>
        /// <exception cref="TargetNotFoundException">目标不存在</exception>
        /// <returns></returns>
        [OperationContract]
        DCTFolder DCMGetFolderByUri(string uri);

        /// <summary>
        /// 获取上一级文件夹
        /// </summary>
        /// <param name="storageObject">文件或文件夹</param>
        /// <exception cref="TargetNotFoundException">目标不存在</exception>
        /// <returns></returns>
        [OperationContract]
        DCTFolder DCMGetParentFolder(DCTStorageObject storageObject);

        /// <summary>
        /// 获取子对象
        /// </summary>
        /// <param name="folder">文件夹</param>
        /// <param name="contentType">获取文件、文件夹还是全部</param>
        /// <exception cref="TargetNotFoundException">目标不存在</exception>
        /// <returns></returns>
        [OperationContract]
        BaseCollection<DCTStorageObject> DCMGetChildren(DCTFolder folder, DCTContentType contentType);

        /// <summary>
        /// 创建文件夹
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="parentFolder">父文件夹</param>
        /// <param name="raiseErrorWhenExist">当文件夹存在时是否抛出错误</param>
        /// <exception cref="TargetNotFoundException">目标不存在</exception>
        /// <exception cref="FolderAlreadyExistException">文件夹已存在</exception>
        /// <returns></returns>
        [OperationContract]
        DCTFolder DCMCreateFolder(string name, DCTFolder parentFolder);

        /// <summary>
        /// 更新文件夹
        /// </summary>
        /// <param name="folder">文件夹信息</param>
        /// <exception cref="TargetNotFoundException">目标不存在</exception>
        /// <returns></returns>
        [OperationContract]
        void UpdateFolder(DCTFolder folder);

        /// <summary>
        /// 删除文件或文件夹(将在回收站中不存在)
        /// </summary>
        /// <param name="storageObject">要删除的对象</param>
        /// <exception cref="TargetNotFoundException">目标不存在</exception>
        [OperationContract]
        void DCMDelete(DCTStorageObject storageObject);

        /// <summary>
        /// 删除文件或文件夹(将在回收站中不存在)
        /// </summary>
        /// <param name="storageObject">要删除的对象</param>
        /// <exception cref="TargetNotFoundException">目标不存在</exception>
        [OperationContract]
        void DCMRemove(DCTStorageObject storageObject);

        /// <summary>
        /// 创建角色
        /// </summary>
        /// <param name="rolename">角色的名称</param>
        /// <param name="descriptioin">角色的描述</param>
        /// <exception cref="MCS.Library.SOA.DocServiceContract.Exceptions.RoleAlreadyExistException">角色已经存在</exception>
        /// <returns></returns>
        [OperationContract]
        DCTRoleDefinition DCMCreateRole(string rolename, string descriptioin);

        /// <summary>
        /// 获取角色的权限
        /// </summary>
        /// <param name="role">角色</param>
        /// <exception cref="TargetNotFoundException">目标角色不存在</exception>
        /// <returns></returns>
        [OperationContract]
        BaseCollection<DCTPermission> DCMGetRolePermissions(DCTRoleDefinition role);

        /// <summary>
        /// 删除角色
        /// </summary>
        /// <param name="rolename">角色名称</param>
        [OperationContract]
        void DCMRemoveRole(string rolename);
        /// <summary>
        /// 向角色添加权限
        /// </summary>
        /// <param name="role">角色</param>
        /// <param name="permissions">权限</param>
        /// <exception cref="TargetNotFoundException">目标角色不存在</exception>
        [OperationContract]
        void DCMSetRolePermissions(DCTRoleDefinition role, BaseCollection<DCTPermission> permissions);


        /// <summary>
        /// 获取某一对象的角色用户关联关系集合
        /// </summary>
        /// <param name="storageObject">文件或文件夹</param>
        /// <exception cref="TargetNotFoundException">目标对象不存在</exception>
        /// <returns></returns>
        [OperationContract]
        BaseCollection<DCTRoleAssignment> DCMGetRoleAssignments(int storageObjID);

        /// <summary>
        /// 添加角色用户关联关系
        /// </summary>
        /// <param name="storageObject">文件或文件夹</param>
        /// <param name="roleAssignments">角色用户关联关系集合</param>
        /// <exception cref="TargetNotFoundException">目标对象不存在</exception>
        /// <param name="?"></param>
        [OperationContract]
        void DCMSetRoleAssignments(int storageObjID, BaseCollection<DCTRoleAssignment> roleAssignments);

        /// <summary>
        /// 获取用户的角色
        /// </summary>
        /// <param name="storageObjID">文件或文件夹</param>
        /// <param name="userAccount">用户账号</param>
        /// <returns></returns>
        [OperationContract]
        BaseCollection<string> DCMGetUserRoles(int storageObjID, string userAccount);

        /// <summary>
        /// 添加用户的角色
        /// </summary>
        /// <param name="storageObjID">文件或文件夹</param>
        /// <param name="userAccount">用户账号</param>
        /// <param name="rolename">角色名称</param>
        [OperationContract]
        void DCMAddUserRole(int storageObjID, string userAccount, string rolename);

        /// <summary>
        /// 删除用户角色
        /// </summary>
        /// <param name="storageObjID">文件或文件夹</param>
        /// <param name="userAccount">用户账号</param>
        /// <param name="rolesToRemove">角色名称</param>
        [OperationContract]
        void DCMRemoveUserRoles(int storageObjID, string userAccount, BaseCollection<string> rolesToRemove);

        /// <summary>
        /// 为文档库创建一个字段
        /// </summary>
        /// <param name="field">字段</param>
        /// <param name="raiseErrorWhenExist">如果存在是否抛出异常</param>
        /// <exception cref="FieldAlreadyExistException">字段已经存在</exception>
        [OperationContract]
        DCTFieldInfo DCMAddField(DCTFieldInfo field, bool raiseErrorWhenExist);

        /// <summary>
        /// 获取当前文档库已经存在的所有字段
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        BaseCollection<DCTFieldInfo> DCMGetFields();

        /// <summary>
        /// 从文档库删除字段
        /// </summary>
        /// <param name="field">字段</param>
        /// <exception cref="TargetNotFoundException">目标字段不存在</exception>
        [OperationContract]
        void DCMDeleteField(DCTFieldInfo field);

        /// <summary>
        /// 保存文件
        /// </summary>
        /// <param name="folder">文件夹</param>
        /// <param name="fileData">文件数据</param>
        /// <param name="filename">文件名</param>
        /// <param name="overwrite">如果文件存在，是否覆盖</param>
        /// <exception cref="TargetNotFoundException">文件夹不存在</exception>
        /// <exception cref="FileContentIsEmptyException">文件内容为空</exception>
        /// <exception cref="InvalidFilenameException">文件名无效</exception>
        /// <exception cref="FileCheckedOutException">文件无法编辑，因为被签出</exception>
        /// <returns></returns>
        [OperationContract]
        DCTFile DCMSave(DCTFolder folder, byte[] fileData, string filename, bool overwrite);

        /// <summary>
        /// 根据Id获取文件信息
        /// </summary>
        /// <param name="fileId">文件Id</param>
        /// <exception cref="TargetNotFoundException">文件不存在</exception>
        /// <returns></returns>
        [OperationContract]
        DCTFile DCMGetFileById(int fileId);

        /// <summary>
        /// 根据uri获取文件
        /// </summary>
        /// <param name="uri">uri</param>
        /// <returns></returns>
        [OperationContract]
        DCTFile DCMGetFileByUri(string uri);

        /// <summary>
        /// 获取某文件夹下的文件
        /// </summary>
        /// <param name="folder">文件夹</param>
        /// <param name="filename">文件名</param>
        /// <returns></returns>
        [OperationContract]
        DCTFile DCMGetFileInFolder(DCTFolder folder, string filename);

        /// <summary>
        /// 根据Id获取文件的内容
        /// </summary>
        /// <param name="fileId">文件Id</param>
        /// <exception cref="TargetNotFoundException">文件不存在</exception>
        /// <returns></returns>
        [OperationContract]
        byte[] DCMGetFileContent(int fileId);

        /// <summary>
        /// 设置文件的字段值
        /// </summary>
        /// <param name="fileId">文件的Id</param>
        /// <param name="fileFields">文件的字段值</param>
        /// <exception cref="TargetNotFoundException">文件不存在</exception>
        /// <exception cref="InvalidFilenameException">文件名无效</exception>
        /// <exception cref="FileCheckedOutException">文件无法编辑，因为被签出</exception>
        [OperationContract]
        void DCMSetFileFields(int fileId, BaseCollection<DCTFileField> fileFields);

        /// <summary>
        /// 获取文件的全部字段值
        /// </summary>
        /// <param name="fileId">文件Id</param>
        /// <exception cref="TargetNotFoundException">文件不存在</exception>
        /// <returns></returns>
        [OperationContract]
        BaseCollection<DCTFileField> DCMGetAllFileFields(int fileId);

        /// <summary>
        /// 获取特定的文件字段值
        /// </summary>
        /// <param name="fileId">文件Id</param>
        /// <param name="fields">字段</param>
        /// <exception cref="TargetNotFoundException">文件不存在</exception>
        /// <returns></returns>
        [OperationContract]
        BaseCollection<DCTFileField> DCMGetFileFields(int fileId, BaseCollection<DCTFieldInfo> fields);

        /// <summary>
        /// 签入
        /// </summary>
        /// <param name="fileId">文件的Id</param>
        /// <param name="comment">版本说明</param>
        /// <param name="checkInType">签入的类型</param>
        /// <exception cref="TargetNotFoundException">文件不存在</exception>
        /// <exception cref="FileIsNotCheckedOutException">文件没有被签出/exception>
        [OperationContract]
        void DCMCheckIn(int fileId, string comment, DCTCheckinType checkInType);

        /// <summary>
        /// 签出
        /// </summary>
        /// <param name="fileId">文件的Id</param>
        /// <exception cref="TargetNotFoundException">文件不存在</exception>
        /// <exception cref="FileCheckedOutException">文件无法签出，因为已被签出</exception>
        [OperationContract]
        void DCMCheckOut(int fileId);

        /// <summary>
        /// 撤销签出
        /// </summary>
        /// <param name="fileId">文件的Id</param>
        /// <exception cref="TargetNotFoundException">文件不存在</exception>
        /// <exception cref="FileIsNotCheckedOutException">文件没有被签出/exception>
        [OperationContract]
        void DCMUndoCheckOut(int fileId);

        /// <summary>
        /// 获取版本
        /// </summary>
        /// <param name="fileId">文件的Id</param>
        /// <exception cref="TargetNotFoundException">文件不存在</exception>
        /// <returns></returns>
        [OperationContract]
        BaseCollection<DCTFileVersion> DCMGetVersions(int fileId);

        /// <summary>
        /// 获取版本内容
        /// </summary>
        /// <param name="fileId">文件Id</param>
        /// <param name="versionId">版本Id</param>
        /// <exception cref="TargetNotFoundException">版本不存在</exception>
        /// <returns></returns>
        [OperationContract]
        byte[] DCMGetVersionFileContent(int fileId, int versionId);

    }
}
