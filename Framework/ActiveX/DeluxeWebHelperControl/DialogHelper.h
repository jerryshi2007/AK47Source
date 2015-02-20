// DialogHelper.h : Declaration of the CDialogHelper

#pragma once
#include "resource.h"       // main symbols
#include "AtlSafe.h"


#include "DeluxeWebHelperControl_i.h"



#if defined(_WIN32_WCE) && !defined(_CE_DCOM) && !defined(_CE_ALLOW_SINGLE_THREADED_OBJECTS_IN_MTA)
#error "Single-threaded COM objects are not properly supported on Windows CE platform, such as the Windows Mobile platforms that do not include full DCOM support. Define _CE_ALLOW_SINGLE_THREADED_OBJECTS_IN_MTA to force ATL to support creating single-thread COM object's and allow use of it's single-threaded COM object implementations. The threading model in your rgs file was set to 'Free' as that is the only threading model supported in non DCOM Windows CE platforms."
#endif

using namespace ATL;


// CDialogHelper

class ATL_NO_VTABLE CDialogHelper :
	public CComObjectRootEx<CComSingleThreadModel>,
	public CComCoClass<CDialogHelper, &CLSID_DialogHelper>,
	public IDispatchImpl < IDialogHelper, &IID_IDialogHelper, &LIBID_DeluxeWebHelperControlLib, /*wMajor =*/ 1, /*wMinor =*/ 0 >,
	public IObjectSafetyImpl<CDialogHelper, INTERFACESAFE_FOR_UNTRUSTED_CALLER | INTERFACESAFE_FOR_UNTRUSTED_DATA>
{
public:
	CDialogHelper()
	{
	}

	DECLARE_REGISTRY_RESOURCEID(IDR_DIALOGHELPER)


	BEGIN_COM_MAP(CDialogHelper)
		COM_INTERFACE_ENTRY(IDialogHelper)
		COM_INTERFACE_ENTRY(IDispatch)
	END_COM_MAP()



	DECLARE_PROTECT_FINAL_CONSTRUCT()

	HRESULT FinalConstruct()
	{
		m_bstrFileName = "";
		m_bstrTitle = "Open Files";

		return S_OK;
	}

	void FinalRelease()
	{
	}

private:
	// 文件打开对话框的标题
	CComBSTR m_bstrTitle;

	//输入和选择的文件
	CComBSTR m_bstrFileName;
	//char m_bstrFileName[1024];
	//拓展名的限制串
	CComBSTR m_filter;

	//允许多选择
	VARIANT_BOOL m_MultiSelect = true;

	//文件名数组
	CComSafeArray<VARIANT> m_FileNames;

public:



	STDMETHOD(get_Title)(BSTR* pVal);
	STDMETHOD(put_Title)(BSTR newVal);
	STDMETHOD(get_FileName)(BSTR* pVal);
	STDMETHOD(put_FileName)(BSTR newVal);
	STDMETHOD(get_MultiSelect)(VARIANT_BOOL* pVal);
	STDMETHOD(put_MultiSelect)(VARIANT_BOOL newVal);
	STDMETHOD(get_FileNames)(VARIANT* pVal);
	STDMETHOD(get_Filter)(BSTR* pVal);
	STDMETHOD(put_Filter)(BSTR newVal);
	STDMETHOD(OpenDialog)(VARIANT_BOOL* retVal);

	HRESULT(checkError)();
};

OBJECT_ENTRY_AUTO(__uuidof(DialogHelper), CDialogHelper)
