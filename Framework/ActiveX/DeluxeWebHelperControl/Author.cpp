// Author.cpp : Implementation of CAuthor

#include "stdafx.h"
#include "Author.h"


// CAuthor



STDMETHODIMP CAuthor::get_Name(BSTR* pVal)
{
	*pVal = m_bstrTitle.Copy();

	return S_OK;
}

STDMETHODIMP CAuthor::put_Name(BSTR newVal)
{
	m_bstrTitle = newVal;

	return S_OK;
}
