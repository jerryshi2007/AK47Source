// ClientEnv.h : Declaration of the CClientEnv

#pragma once
#include "resource.h"       // main symbols



#include "DeluxeWebHelperControl_i.h"



#if defined(_WIN32_WCE) && !defined(_CE_DCOM) && !defined(_CE_ALLOW_SINGLE_THREADED_OBJECTS_IN_MTA)
#error "Single-threaded COM objects are not properly supported on Windows CE platform, such as the Windows Mobile platforms that do not include full DCOM support. Define _CE_ALLOW_SINGLE_THREADED_OBJECTS_IN_MTA to force ATL to support creating single-thread COM object's and allow use of it's single-threaded COM object implementations. The threading model in your rgs file was set to 'Free' as that is the only threading model supported in non DCOM Windows CE platforms."
#endif

using namespace ATL;


// CClientEnv

class ATL_NO_VTABLE CClientEnv :
	public CComObjectRootEx<CComSingleThreadModel>,
	public CComCoClass<CClientEnv, &CLSID_ClientEnv>,
	public IDispatchImpl < IClientEnv, &IID_IClientEnv, &LIBID_DeluxeWebHelperControlLib, /*wMajor =*/ 1, /*wMinor =*/ 0 >,
	public IObjectSafetyImpl<CClientEnv, INTERFACESAFE_FOR_UNTRUSTED_CALLER | INTERFACESAFE_FOR_UNTRUSTED_DATA>
{
public:
	CClientEnv()
	{
	}

	DECLARE_REGISTRY_RESOURCEID(IDR_CLIENTENV)


	BEGIN_COM_MAP(CClientEnv)
		COM_INTERFACE_ENTRY(IClientEnv)
		COM_INTERFACE_ENTRY(IDispatch)
	END_COM_MAP()



	DECLARE_PROTECT_FINAL_CONSTRUCT()

	HRESULT FinalConstruct()
	{
		return S_OK;
	}

	void FinalRelease()
	{
	}

public:
	STDMETHOD(GetMACAddressFromUuid)(BSTR* retVal);
	STDMETHOD(GetAllMACAddresses)(VARIANT* addresses);
	STDMETHOD(GetAllEncryptedMACAddresses)(BSTR encString, VARIANT* encAddresses);

private:
	void FormatMACAddress(byte address[], int len, BSTR result);
	void FormatEncryptedMACAddress(byte address[], int len, byte* encData, LONG encDataSize, BSTR result);
	int HexStringToByteArray(BSTR encString, byte encData[], int encByteLength);
	byte TCHARToByte(TCHAR ch);
};

OBJECT_ENTRY_AUTO(__uuidof(ClientEnv), CClientEnv)
