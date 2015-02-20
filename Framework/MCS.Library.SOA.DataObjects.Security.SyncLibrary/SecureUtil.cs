using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Security.Permissions;
using System.Security;

namespace MCS.Library.SOA.DataObjects.Security.SyncLibrary
{
    class SecureUtil
    {
        private static volatile ReflectionPermission memberAccessPermission;
        private static volatile ReflectionPermission restrictedMemberAccessPermission;

        public static object SecureCreateInstance(Type type, object[] args, bool allowNonPublic)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }
            BindingFlags bindingAttr = BindingFlags.CreateInstance | BindingFlags.Public | BindingFlags.Instance;
            if (!type.IsVisible)
            {
                DemandReflectionAccess(type);
            }
            else if (allowNonPublic && !HasReflectionPermission(type))
            {
                allowNonPublic = false;
            }
            if (allowNonPublic)
            {
                bindingAttr |= BindingFlags.NonPublic;
            }
            return Activator.CreateInstance(type, bindingAttr, null, args, null);
        }

        private static bool HasReflectionPermission(Type type)
        {
            try
            {
                DemandReflectionAccess(type);
                return true;
            }
            catch (SecurityException)
            {
            }
            return false;
        }

        private static void DemandReflectionAccess(Type type)
        {
            try
            {
                MemberAccessPermission.Demand();
            }
            catch (SecurityException)
            {
                DemandGrantSet(type.Assembly);
            }
        }

        private static ReflectionPermission MemberAccessPermission
        {
            get
            {
                if (memberAccessPermission == null)
                {
                    memberAccessPermission = new ReflectionPermission(ReflectionPermissionFlag.MemberAccess);
                }
                return memberAccessPermission;
            }
        }

        private static ReflectionPermission RestrictedMemberAccessPermission
        {
            get
            {
                if (restrictedMemberAccessPermission == null)
                {
                    restrictedMemberAccessPermission = new ReflectionPermission(ReflectionPermissionFlag.RestrictedMemberAccess);
                }
                return restrictedMemberAccessPermission;
            }
        }


        [SecuritySafeCritical]
        private static void DemandGrantSet(Assembly assembly)
        {
            PermissionSet permissionSet = assembly.PermissionSet;
            permissionSet.AddPermission(RestrictedMemberAccessPermission);
            permissionSet.Demand();
        }


        internal static object GetIntValue(System.Collections.Specialized.NameValueCollection config, string p, int p_2, bool p_3, int p_4)
        {
            throw new NotImplementedException();
        }
    }
}
