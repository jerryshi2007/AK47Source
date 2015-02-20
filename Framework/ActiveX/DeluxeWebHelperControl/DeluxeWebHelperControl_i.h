

/* this ALWAYS GENERATED file contains the definitions for the interfaces */


 /* File created by MIDL compiler version 8.00.0603 */
/* at Wed Feb 11 12:59:51 2015
 */
/* Compiler settings for DeluxeWebHelperControl.idl:
    Oicf, W1, Zp8, env=Win32 (32b run), target_arch=X86 8.00.0603 
    protocol : dce , ms_ext, c_ext, robust
    error checks: allocation ref bounds_check enum stub_data 
    VC __declspec() decoration level: 
         __declspec(uuid()), __declspec(selectany), __declspec(novtable)
         DECLSPEC_UUID(), MIDL_INTERFACE()
*/
/* @@MIDL_FILE_HEADING(  ) */

#pragma warning( disable: 4049 )  /* more than 64k source lines */


/* verify that the <rpcndr.h> version is high enough to compile this file*/
#ifndef __REQUIRED_RPCNDR_H_VERSION__
#define __REQUIRED_RPCNDR_H_VERSION__ 475
#endif

#include "rpc.h"
#include "rpcndr.h"

#ifndef __RPCNDR_H_VERSION__
#error this stub requires an updated version of <rpcndr.h>
#endif // __RPCNDR_H_VERSION__

#ifndef COM_NO_WINDOWS_H
#include "windows.h"
#include "ole2.h"
#endif /*COM_NO_WINDOWS_H*/

#ifndef __DeluxeWebHelperControl_i_h__
#define __DeluxeWebHelperControl_i_h__

#if defined(_MSC_VER) && (_MSC_VER >= 1020)
#pragma once
#endif

/* Forward Declarations */ 

#ifndef __IComponentHelper_FWD_DEFINED__
#define __IComponentHelper_FWD_DEFINED__
typedef interface IComponentHelper IComponentHelper;

#endif 	/* __IComponentHelper_FWD_DEFINED__ */


#ifndef __IDialogHelper_FWD_DEFINED__
#define __IDialogHelper_FWD_DEFINED__
typedef interface IDialogHelper IDialogHelper;

#endif 	/* __IDialogHelper_FWD_DEFINED__ */


#ifndef __IAuthor_FWD_DEFINED__
#define __IAuthor_FWD_DEFINED__
typedef interface IAuthor IAuthor;

#endif 	/* __IAuthor_FWD_DEFINED__ */


#ifndef __IClientEnv_FWD_DEFINED__
#define __IClientEnv_FWD_DEFINED__
typedef interface IClientEnv IClientEnv;

#endif 	/* __IClientEnv_FWD_DEFINED__ */


#ifndef __ComponentHelper_FWD_DEFINED__
#define __ComponentHelper_FWD_DEFINED__

#ifdef __cplusplus
typedef class ComponentHelper ComponentHelper;
#else
typedef struct ComponentHelper ComponentHelper;
#endif /* __cplusplus */

#endif 	/* __ComponentHelper_FWD_DEFINED__ */


#ifndef __DialogHelper_FWD_DEFINED__
#define __DialogHelper_FWD_DEFINED__

#ifdef __cplusplus
typedef class DialogHelper DialogHelper;
#else
typedef struct DialogHelper DialogHelper;
#endif /* __cplusplus */

#endif 	/* __DialogHelper_FWD_DEFINED__ */


#ifndef __Author_FWD_DEFINED__
#define __Author_FWD_DEFINED__

#ifdef __cplusplus
typedef class Author Author;
#else
typedef struct Author Author;
#endif /* __cplusplus */

#endif 	/* __Author_FWD_DEFINED__ */


#ifndef __ClientEnv_FWD_DEFINED__
#define __ClientEnv_FWD_DEFINED__

#ifdef __cplusplus
typedef class ClientEnv ClientEnv;
#else
typedef struct ClientEnv ClientEnv;
#endif /* __cplusplus */

#endif 	/* __ClientEnv_FWD_DEFINED__ */


/* header files for imported files */
#include "oaidl.h"
#include "ocidl.h"
#include "shobjidl.h"

#ifdef __cplusplus
extern "C"{
#endif 


#ifndef __IComponentHelper_INTERFACE_DEFINED__
#define __IComponentHelper_INTERFACE_DEFINED__

/* interface IComponentHelper */
/* [unique][nonextensible][dual][uuid][object] */ 


EXTERN_C const IID IID_IComponentHelper;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("A5310BE0-3441-4D43-8B16-64A677D7C889")
    IComponentHelper : public IDispatch
    {
    public:
        virtual /* [id] */ HRESULT STDMETHODCALLTYPE CreateObject( 
            /* [in] */ BSTR progID,
            /* [retval][out] */ IDispatch **result) = 0;
        
        virtual /* [id] */ HRESULT STDMETHODCALLTYPE CreateLocalServer( 
            BSTR progID,
            /* [retval][out] */ IDispatch **result) = 0;
        
        virtual /* [id] */ HRESULT STDMETHODCALLTYPE GetAuthor( 
            /* [retval][out] */ IDispatch **author) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IComponentHelperVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IComponentHelper * This,
            /* [in] */ REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IComponentHelper * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IComponentHelper * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfoCount )( 
            IComponentHelper * This,
            /* [out] */ UINT *pctinfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfo )( 
            IComponentHelper * This,
            /* [in] */ UINT iTInfo,
            /* [in] */ LCID lcid,
            /* [out] */ ITypeInfo **ppTInfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetIDsOfNames )( 
            IComponentHelper * This,
            /* [in] */ REFIID riid,
            /* [size_is][in] */ LPOLESTR *rgszNames,
            /* [range][in] */ UINT cNames,
            /* [in] */ LCID lcid,
            /* [size_is][out] */ DISPID *rgDispId);
        
        /* [local] */ HRESULT ( STDMETHODCALLTYPE *Invoke )( 
            IComponentHelper * This,
            /* [annotation][in] */ 
            _In_  DISPID dispIdMember,
            /* [annotation][in] */ 
            _In_  REFIID riid,
            /* [annotation][in] */ 
            _In_  LCID lcid,
            /* [annotation][in] */ 
            _In_  WORD wFlags,
            /* [annotation][out][in] */ 
            _In_  DISPPARAMS *pDispParams,
            /* [annotation][out] */ 
            _Out_opt_  VARIANT *pVarResult,
            /* [annotation][out] */ 
            _Out_opt_  EXCEPINFO *pExcepInfo,
            /* [annotation][out] */ 
            _Out_opt_  UINT *puArgErr);
        
        /* [id] */ HRESULT ( STDMETHODCALLTYPE *CreateObject )( 
            IComponentHelper * This,
            /* [in] */ BSTR progID,
            /* [retval][out] */ IDispatch **result);
        
        /* [id] */ HRESULT ( STDMETHODCALLTYPE *CreateLocalServer )( 
            IComponentHelper * This,
            BSTR progID,
            /* [retval][out] */ IDispatch **result);
        
        /* [id] */ HRESULT ( STDMETHODCALLTYPE *GetAuthor )( 
            IComponentHelper * This,
            /* [retval][out] */ IDispatch **author);
        
        END_INTERFACE
    } IComponentHelperVtbl;

    interface IComponentHelper
    {
        CONST_VTBL struct IComponentHelperVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IComponentHelper_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IComponentHelper_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IComponentHelper_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IComponentHelper_GetTypeInfoCount(This,pctinfo)	\
    ( (This)->lpVtbl -> GetTypeInfoCount(This,pctinfo) ) 

#define IComponentHelper_GetTypeInfo(This,iTInfo,lcid,ppTInfo)	\
    ( (This)->lpVtbl -> GetTypeInfo(This,iTInfo,lcid,ppTInfo) ) 

#define IComponentHelper_GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId)	\
    ( (This)->lpVtbl -> GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId) ) 

#define IComponentHelper_Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr)	\
    ( (This)->lpVtbl -> Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr) ) 


#define IComponentHelper_CreateObject(This,progID,result)	\
    ( (This)->lpVtbl -> CreateObject(This,progID,result) ) 

#define IComponentHelper_CreateLocalServer(This,progID,result)	\
    ( (This)->lpVtbl -> CreateLocalServer(This,progID,result) ) 

#define IComponentHelper_GetAuthor(This,author)	\
    ( (This)->lpVtbl -> GetAuthor(This,author) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IComponentHelper_INTERFACE_DEFINED__ */


#ifndef __IDialogHelper_INTERFACE_DEFINED__
#define __IDialogHelper_INTERFACE_DEFINED__

/* interface IDialogHelper */
/* [unique][nonextensible][dual][uuid][object] */ 


EXTERN_C const IID IID_IDialogHelper;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("CE5489C3-E5FE-4B07-8A05-7C468FE54849")
    IDialogHelper : public IDispatch
    {
    public:
        virtual /* [id][propget] */ HRESULT STDMETHODCALLTYPE get_Title( 
            /* [retval][out] */ BSTR *pVal) = 0;
        
        virtual /* [id][propput] */ HRESULT STDMETHODCALLTYPE put_Title( 
            /* [in] */ BSTR newVal) = 0;
        
        virtual /* [id][propget] */ HRESULT STDMETHODCALLTYPE get_FileName( 
            /* [retval][out] */ BSTR *pVal) = 0;
        
        virtual /* [id][propput] */ HRESULT STDMETHODCALLTYPE put_FileName( 
            /* [in] */ BSTR newVal) = 0;
        
        virtual /* [id][propget] */ HRESULT STDMETHODCALLTYPE get_MultiSelect( 
            /* [retval][out] */ VARIANT_BOOL *pVal) = 0;
        
        virtual /* [id][propput] */ HRESULT STDMETHODCALLTYPE put_MultiSelect( 
            /* [in] */ VARIANT_BOOL newVal) = 0;
        
        virtual /* [id][propget] */ HRESULT STDMETHODCALLTYPE get_FileNames( 
            /* [retval][out] */ VARIANT *pVal) = 0;
        
        virtual /* [id][propget] */ HRESULT STDMETHODCALLTYPE get_Filter( 
            /* [retval][out] */ BSTR *pVal) = 0;
        
        virtual /* [id][propput] */ HRESULT STDMETHODCALLTYPE put_Filter( 
            /* [in] */ BSTR newVal) = 0;
        
        virtual /* [id] */ HRESULT STDMETHODCALLTYPE OpenDialog( 
            /* [retval][out] */ VARIANT_BOOL *retVal) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IDialogHelperVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IDialogHelper * This,
            /* [in] */ REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IDialogHelper * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IDialogHelper * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfoCount )( 
            IDialogHelper * This,
            /* [out] */ UINT *pctinfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfo )( 
            IDialogHelper * This,
            /* [in] */ UINT iTInfo,
            /* [in] */ LCID lcid,
            /* [out] */ ITypeInfo **ppTInfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetIDsOfNames )( 
            IDialogHelper * This,
            /* [in] */ REFIID riid,
            /* [size_is][in] */ LPOLESTR *rgszNames,
            /* [range][in] */ UINT cNames,
            /* [in] */ LCID lcid,
            /* [size_is][out] */ DISPID *rgDispId);
        
        /* [local] */ HRESULT ( STDMETHODCALLTYPE *Invoke )( 
            IDialogHelper * This,
            /* [annotation][in] */ 
            _In_  DISPID dispIdMember,
            /* [annotation][in] */ 
            _In_  REFIID riid,
            /* [annotation][in] */ 
            _In_  LCID lcid,
            /* [annotation][in] */ 
            _In_  WORD wFlags,
            /* [annotation][out][in] */ 
            _In_  DISPPARAMS *pDispParams,
            /* [annotation][out] */ 
            _Out_opt_  VARIANT *pVarResult,
            /* [annotation][out] */ 
            _Out_opt_  EXCEPINFO *pExcepInfo,
            /* [annotation][out] */ 
            _Out_opt_  UINT *puArgErr);
        
        /* [id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_Title )( 
            IDialogHelper * This,
            /* [retval][out] */ BSTR *pVal);
        
        /* [id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_Title )( 
            IDialogHelper * This,
            /* [in] */ BSTR newVal);
        
        /* [id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_FileName )( 
            IDialogHelper * This,
            /* [retval][out] */ BSTR *pVal);
        
        /* [id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_FileName )( 
            IDialogHelper * This,
            /* [in] */ BSTR newVal);
        
        /* [id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_MultiSelect )( 
            IDialogHelper * This,
            /* [retval][out] */ VARIANT_BOOL *pVal);
        
        /* [id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_MultiSelect )( 
            IDialogHelper * This,
            /* [in] */ VARIANT_BOOL newVal);
        
        /* [id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_FileNames )( 
            IDialogHelper * This,
            /* [retval][out] */ VARIANT *pVal);
        
        /* [id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_Filter )( 
            IDialogHelper * This,
            /* [retval][out] */ BSTR *pVal);
        
        /* [id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_Filter )( 
            IDialogHelper * This,
            /* [in] */ BSTR newVal);
        
        /* [id] */ HRESULT ( STDMETHODCALLTYPE *OpenDialog )( 
            IDialogHelper * This,
            /* [retval][out] */ VARIANT_BOOL *retVal);
        
        END_INTERFACE
    } IDialogHelperVtbl;

    interface IDialogHelper
    {
        CONST_VTBL struct IDialogHelperVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IDialogHelper_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IDialogHelper_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IDialogHelper_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IDialogHelper_GetTypeInfoCount(This,pctinfo)	\
    ( (This)->lpVtbl -> GetTypeInfoCount(This,pctinfo) ) 

#define IDialogHelper_GetTypeInfo(This,iTInfo,lcid,ppTInfo)	\
    ( (This)->lpVtbl -> GetTypeInfo(This,iTInfo,lcid,ppTInfo) ) 

#define IDialogHelper_GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId)	\
    ( (This)->lpVtbl -> GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId) ) 

#define IDialogHelper_Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr)	\
    ( (This)->lpVtbl -> Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr) ) 


#define IDialogHelper_get_Title(This,pVal)	\
    ( (This)->lpVtbl -> get_Title(This,pVal) ) 

#define IDialogHelper_put_Title(This,newVal)	\
    ( (This)->lpVtbl -> put_Title(This,newVal) ) 

#define IDialogHelper_get_FileName(This,pVal)	\
    ( (This)->lpVtbl -> get_FileName(This,pVal) ) 

#define IDialogHelper_put_FileName(This,newVal)	\
    ( (This)->lpVtbl -> put_FileName(This,newVal) ) 

#define IDialogHelper_get_MultiSelect(This,pVal)	\
    ( (This)->lpVtbl -> get_MultiSelect(This,pVal) ) 

#define IDialogHelper_put_MultiSelect(This,newVal)	\
    ( (This)->lpVtbl -> put_MultiSelect(This,newVal) ) 

#define IDialogHelper_get_FileNames(This,pVal)	\
    ( (This)->lpVtbl -> get_FileNames(This,pVal) ) 

#define IDialogHelper_get_Filter(This,pVal)	\
    ( (This)->lpVtbl -> get_Filter(This,pVal) ) 

#define IDialogHelper_put_Filter(This,newVal)	\
    ( (This)->lpVtbl -> put_Filter(This,newVal) ) 

#define IDialogHelper_OpenDialog(This,retVal)	\
    ( (This)->lpVtbl -> OpenDialog(This,retVal) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IDialogHelper_INTERFACE_DEFINED__ */


#ifndef __IAuthor_INTERFACE_DEFINED__
#define __IAuthor_INTERFACE_DEFINED__

/* interface IAuthor */
/* [unique][nonextensible][dual][uuid][object] */ 


EXTERN_C const IID IID_IAuthor;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("0DACA1F2-4D58-4E1F-98C1-B1CAEB9B48D4")
    IAuthor : public IDispatch
    {
    public:
        virtual /* [id][propget] */ HRESULT STDMETHODCALLTYPE get_Name( 
            /* [retval][out] */ BSTR *pVal) = 0;
        
        virtual /* [id][propput] */ HRESULT STDMETHODCALLTYPE put_Name( 
            /* [in] */ BSTR newVal) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IAuthorVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IAuthor * This,
            /* [in] */ REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IAuthor * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IAuthor * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfoCount )( 
            IAuthor * This,
            /* [out] */ UINT *pctinfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfo )( 
            IAuthor * This,
            /* [in] */ UINT iTInfo,
            /* [in] */ LCID lcid,
            /* [out] */ ITypeInfo **ppTInfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetIDsOfNames )( 
            IAuthor * This,
            /* [in] */ REFIID riid,
            /* [size_is][in] */ LPOLESTR *rgszNames,
            /* [range][in] */ UINT cNames,
            /* [in] */ LCID lcid,
            /* [size_is][out] */ DISPID *rgDispId);
        
        /* [local] */ HRESULT ( STDMETHODCALLTYPE *Invoke )( 
            IAuthor * This,
            /* [annotation][in] */ 
            _In_  DISPID dispIdMember,
            /* [annotation][in] */ 
            _In_  REFIID riid,
            /* [annotation][in] */ 
            _In_  LCID lcid,
            /* [annotation][in] */ 
            _In_  WORD wFlags,
            /* [annotation][out][in] */ 
            _In_  DISPPARAMS *pDispParams,
            /* [annotation][out] */ 
            _Out_opt_  VARIANT *pVarResult,
            /* [annotation][out] */ 
            _Out_opt_  EXCEPINFO *pExcepInfo,
            /* [annotation][out] */ 
            _Out_opt_  UINT *puArgErr);
        
        /* [id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_Name )( 
            IAuthor * This,
            /* [retval][out] */ BSTR *pVal);
        
        /* [id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_Name )( 
            IAuthor * This,
            /* [in] */ BSTR newVal);
        
        END_INTERFACE
    } IAuthorVtbl;

    interface IAuthor
    {
        CONST_VTBL struct IAuthorVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IAuthor_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IAuthor_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IAuthor_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IAuthor_GetTypeInfoCount(This,pctinfo)	\
    ( (This)->lpVtbl -> GetTypeInfoCount(This,pctinfo) ) 

#define IAuthor_GetTypeInfo(This,iTInfo,lcid,ppTInfo)	\
    ( (This)->lpVtbl -> GetTypeInfo(This,iTInfo,lcid,ppTInfo) ) 

#define IAuthor_GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId)	\
    ( (This)->lpVtbl -> GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId) ) 

#define IAuthor_Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr)	\
    ( (This)->lpVtbl -> Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr) ) 


#define IAuthor_get_Name(This,pVal)	\
    ( (This)->lpVtbl -> get_Name(This,pVal) ) 

#define IAuthor_put_Name(This,newVal)	\
    ( (This)->lpVtbl -> put_Name(This,newVal) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IAuthor_INTERFACE_DEFINED__ */


#ifndef __IClientEnv_INTERFACE_DEFINED__
#define __IClientEnv_INTERFACE_DEFINED__

/* interface IClientEnv */
/* [unique][nonextensible][dual][uuid][object] */ 


EXTERN_C const IID IID_IClientEnv;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("D6D17CF3-BB5A-4B80-ABB0-080DE96AC4ED")
    IClientEnv : public IDispatch
    {
    public:
        virtual /* [id] */ HRESULT STDMETHODCALLTYPE GetMACAddressFromUuid( 
            /* [retval][out] */ BSTR *retVal) = 0;
        
        virtual /* [id] */ HRESULT STDMETHODCALLTYPE GetAllMACAddresses( 
            /* [retval][out] */ VARIANT *addresses) = 0;
        
        virtual /* [id] */ HRESULT STDMETHODCALLTYPE GetAllEncryptedMACAddresses( 
            /* [in] */ BSTR encString,
            /* [retval][out] */ VARIANT *encAddresses) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IClientEnvVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IClientEnv * This,
            /* [in] */ REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IClientEnv * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IClientEnv * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfoCount )( 
            IClientEnv * This,
            /* [out] */ UINT *pctinfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfo )( 
            IClientEnv * This,
            /* [in] */ UINT iTInfo,
            /* [in] */ LCID lcid,
            /* [out] */ ITypeInfo **ppTInfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetIDsOfNames )( 
            IClientEnv * This,
            /* [in] */ REFIID riid,
            /* [size_is][in] */ LPOLESTR *rgszNames,
            /* [range][in] */ UINT cNames,
            /* [in] */ LCID lcid,
            /* [size_is][out] */ DISPID *rgDispId);
        
        /* [local] */ HRESULT ( STDMETHODCALLTYPE *Invoke )( 
            IClientEnv * This,
            /* [annotation][in] */ 
            _In_  DISPID dispIdMember,
            /* [annotation][in] */ 
            _In_  REFIID riid,
            /* [annotation][in] */ 
            _In_  LCID lcid,
            /* [annotation][in] */ 
            _In_  WORD wFlags,
            /* [annotation][out][in] */ 
            _In_  DISPPARAMS *pDispParams,
            /* [annotation][out] */ 
            _Out_opt_  VARIANT *pVarResult,
            /* [annotation][out] */ 
            _Out_opt_  EXCEPINFO *pExcepInfo,
            /* [annotation][out] */ 
            _Out_opt_  UINT *puArgErr);
        
        /* [id] */ HRESULT ( STDMETHODCALLTYPE *GetMACAddressFromUuid )( 
            IClientEnv * This,
            /* [retval][out] */ BSTR *retVal);
        
        /* [id] */ HRESULT ( STDMETHODCALLTYPE *GetAllMACAddresses )( 
            IClientEnv * This,
            /* [retval][out] */ VARIANT *addresses);
        
        /* [id] */ HRESULT ( STDMETHODCALLTYPE *GetAllEncryptedMACAddresses )( 
            IClientEnv * This,
            /* [in] */ BSTR encString,
            /* [retval][out] */ VARIANT *encAddresses);
        
        END_INTERFACE
    } IClientEnvVtbl;

    interface IClientEnv
    {
        CONST_VTBL struct IClientEnvVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IClientEnv_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IClientEnv_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IClientEnv_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IClientEnv_GetTypeInfoCount(This,pctinfo)	\
    ( (This)->lpVtbl -> GetTypeInfoCount(This,pctinfo) ) 

#define IClientEnv_GetTypeInfo(This,iTInfo,lcid,ppTInfo)	\
    ( (This)->lpVtbl -> GetTypeInfo(This,iTInfo,lcid,ppTInfo) ) 

#define IClientEnv_GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId)	\
    ( (This)->lpVtbl -> GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId) ) 

#define IClientEnv_Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr)	\
    ( (This)->lpVtbl -> Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr) ) 


#define IClientEnv_GetMACAddressFromUuid(This,retVal)	\
    ( (This)->lpVtbl -> GetMACAddressFromUuid(This,retVal) ) 

#define IClientEnv_GetAllMACAddresses(This,addresses)	\
    ( (This)->lpVtbl -> GetAllMACAddresses(This,addresses) ) 

#define IClientEnv_GetAllEncryptedMACAddresses(This,encString,encAddresses)	\
    ( (This)->lpVtbl -> GetAllEncryptedMACAddresses(This,encString,encAddresses) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IClientEnv_INTERFACE_DEFINED__ */



#ifndef __DeluxeWebHelperControlLib_LIBRARY_DEFINED__
#define __DeluxeWebHelperControlLib_LIBRARY_DEFINED__

/* library DeluxeWebHelperControlLib */
/* [version][uuid] */ 


EXTERN_C const IID LIBID_DeluxeWebHelperControlLib;

EXTERN_C const CLSID CLSID_ComponentHelper;

#ifdef __cplusplus

class DECLSPEC_UUID("A4C46B85-9E6E-4B38-8911-2EFC1F2984C3")
ComponentHelper;
#endif

EXTERN_C const CLSID CLSID_DialogHelper;

#ifdef __cplusplus

class DECLSPEC_UUID("ED8B69F0-6563-472C-8177-C9ACDC26B2DC")
DialogHelper;
#endif

EXTERN_C const CLSID CLSID_Author;

#ifdef __cplusplus

class DECLSPEC_UUID("ED85DDEC-691E-4807-9243-C5E9ECFFE2B3")
Author;
#endif

EXTERN_C const CLSID CLSID_ClientEnv;

#ifdef __cplusplus

class DECLSPEC_UUID("44C5B98D-3B18-4821-8CA5-2139E95EE689")
ClientEnv;
#endif
#endif /* __DeluxeWebHelperControlLib_LIBRARY_DEFINED__ */

/* Additional Prototypes for ALL interfaces */

unsigned long             __RPC_USER  BSTR_UserSize(     unsigned long *, unsigned long            , BSTR * ); 
unsigned char * __RPC_USER  BSTR_UserMarshal(  unsigned long *, unsigned char *, BSTR * ); 
unsigned char * __RPC_USER  BSTR_UserUnmarshal(unsigned long *, unsigned char *, BSTR * ); 
void                      __RPC_USER  BSTR_UserFree(     unsigned long *, BSTR * ); 

unsigned long             __RPC_USER  VARIANT_UserSize(     unsigned long *, unsigned long            , VARIANT * ); 
unsigned char * __RPC_USER  VARIANT_UserMarshal(  unsigned long *, unsigned char *, VARIANT * ); 
unsigned char * __RPC_USER  VARIANT_UserUnmarshal(unsigned long *, unsigned char *, VARIANT * ); 
void                      __RPC_USER  VARIANT_UserFree(     unsigned long *, VARIANT * ); 

/* end of Additional Prototypes */

#ifdef __cplusplus
}
#endif

#endif


