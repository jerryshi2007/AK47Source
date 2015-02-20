// Author.h : Declaration of the CAuthor

#pragma once
#include "resource.h"       // main symbols



#include "DeluxeWebHelperControl_i.h"



#if defined(_WIN32_WCE) && !defined(_CE_DCOM) && !defined(_CE_ALLOW_SINGLE_THREADED_OBJECTS_IN_MTA)
#error "Single-threaded COM objects are not properly supported on Windows CE platform, such as the Windows Mobile platforms that do not include full DCOM support. Define _CE_ALLOW_SINGLE_THREADED_OBJECTS_IN_MTA to force ATL to support creating single-thread COM object's and allow use of it's single-threaded COM object implementations. The threading model in your rgs file was set to 'Free' as that is the only threading model supported in non DCOM Windows CE platforms."
#endif

using namespace ATL;


// CAuthor

class ATL_NO_VTABLE CAuthor :
	public CComObjectRootEx<CComSingleThreadModel>,
	public CComCoClass<CAuthor, &CLSID_Author>,
	public IDispatchImpl < IAuthor, &IID_IAuthor, &LIBID_DeluxeWebHelperControlLib, /*wMajor =*/ 1, /*wMinor =*/ 0 >,
	public IObjectSafetyImpl<CAuthor, INTERFACESAFE_FOR_UNTRUSTED_CALLER | INTERFACESAFE_FOR_UNTRUSTED_DATA>
{
public:
	CAuthor()
	{
	}

	DECLARE_REGISTRY_RESOURCEID(IDR_AUTHOR)


	BEGIN_COM_MAP(CAuthor)
		COM_INTERFACE_ENTRY(IAuthor)
		COM_INTERFACE_ENTRY(IDispatch)
	END_COM_MAP()


	CComBSTR m_bstrTitle = "Zheng Shen";

	DECLARE_PROTECT_FINAL_CONSTRUCT()

	HRESULT FinalConstruct()
	{
		return S_OK;
	}

	void FinalRelease()
	{
	}

public:



	STDMETHOD(get_Name)(BSTR* pVal);
	STDMETHOD(put_Name)(BSTR newVal);
};

OBJECT_ENTRY_AUTO(__uuidof(Author), CAuthor)
