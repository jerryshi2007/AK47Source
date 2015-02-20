// ComponentHelper.cpp : Implementation of CComponentHelper

#include "stdafx.h"
#include "ComponentHelper.h"


// CComponentHelper



STDMETHODIMP CComponentHelper::CreateObject(BSTR progID, IDispatch** result)
{
	CComPtr<IDispatch> dispatch;

	dispatch.CoCreateInstance(progID);

	*result = dispatch.Detach();

	return S_OK;
}


STDMETHODIMP CComponentHelper::CreateLocalServer(BSTR progID, IDispatch** result)
{
	CComPtr<IDispatch> dispatch;

	dispatch.CoCreateInstance(progID, NULL, CLSCTX_LOCAL_SERVER);

	*result = dispatch.Detach();

	return S_OK;
}


STDMETHODIMP CComponentHelper::GetAuthor(IDispatch** author)
{
	CComPtr<IDispatch> dispatch;

	dispatch.CoCreateInstance(CLSID_Author);

	dispatch.PutPropertyByName(L"Name", &CComVariant("…Ú·ø"));

	*author = dispatch.Detach();

	return S_OK;
}
