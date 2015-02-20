// DialogHelper.cpp : Implementation of CDialogHelper

#include "stdafx.h"
#include "DialogHelper.h"
#include "objsafe.h"
#include "Cderr.h"
#include <stdlib.h>
#include <comdef.h>

using namespace std;

// CDialogHelper
STDMETHODIMP CDialogHelper::get_Title(BSTR* pVal)
{
	*pVal = m_bstrTitle.Copy();

	return S_OK;
}


STDMETHODIMP CDialogHelper::put_Title(BSTR newVal)
{
	m_bstrTitle = newVal;

	return S_OK;
}


STDMETHODIMP CDialogHelper::get_FileName(BSTR* pVal)
{
	*pVal = m_bstrFileName.Copy();

	return S_OK;
}


STDMETHODIMP CDialogHelper::put_FileName(BSTR newVal)
{
	m_bstrFileName = newVal;

	return S_OK;
}


STDMETHODIMP CDialogHelper::get_MultiSelect(VARIANT_BOOL* pVal)
{
	*pVal = m_MultiSelect;

	return S_OK;
}


STDMETHODIMP CDialogHelper::put_MultiSelect(VARIANT_BOOL newVal)
{
	m_MultiSelect = newVal;

	return S_OK;
}


STDMETHODIMP CDialogHelper::get_FileNames(VARIANT* pVal)
{
	CComVariant var(m_FileNames);

	var.ChangeType(VT_ARRAY | VT_BSTR);
	var.Detach(pVal);

	return S_OK;
}


STDMETHODIMP CDialogHelper::get_Filter(BSTR* filter)
{
	*filter = m_filter.Copy();

	return S_OK;
}


STDMETHODIMP CDialogHelper::put_Filter(BSTR filter)
{
	m_filter = filter;

	return S_OK;
}


STDMETHODIMP CDialogHelper::OpenDialog(VARIANT_BOOL* retVal)
{
	OPENFILENAME ofn;

	::ZeroMemory(&ofn, sizeof(ofn));

	ofn.lStructSize = sizeof(OPENFILENAME);
	ofn.hwndOwner = GetForegroundWindow();

	UINT filterLen = m_filter.Length();

	BSTR tFilter = ::SysAllocStringLen(m_filter, filterLen + 2);

	tFilter[filterLen] = _T('\0');
	tFilter[filterLen + 1] = _T('\0');

	if (m_filter != "")
	{
		for (int i = 0; tFilter[i] != '\0'; i++)
		{
			if (tFilter[i] == '|')
				tFilter[i] = '\0';
		}
	}

	TCHAR fileNameBuffer[1024];

	int nMaxLength = 1023;

	if ((int)m_bstrFileName.Length() < nMaxLength)
		nMaxLength = (int)m_bstrFileName.Length();

	lstrcpyn(fileNameBuffer, (LPCTSTR)m_bstrFileName, nMaxLength);
	fileNameBuffer[nMaxLength] = 0;

	ofn.lpstrFilter = tFilter;
	ofn.nFilterIndex = 1;
	ofn.lpstrFile = fileNameBuffer;
	ofn.nMaxFile = 1024;
	ofn.lpstrTitle = m_bstrTitle;
	ofn.lpstrInitialDir = NULL;


	//���ѡ���˶�ѡ��ʽ����ʹ�ö�ѡ������Flags
	if (m_MultiSelect)
		ofn.Flags = OFN_EXPLORER | OFN_FILEMUSTEXIST | OFN_HIDEREADONLY | OFN_PATHMUSTEXIST | OFN_ALLOWMULTISELECT;
	else
		ofn.Flags = OFN_EXPLORER | OFN_FILEMUSTEXIST;

	*retVal = GetOpenFileName(&ofn);

	::SysFreeString(tFilter);

	bool atLast = false;

	//���ѡ����ļ�������
	m_FileNames.Destroy();

	if (*retVal)
	{
		LPTSTR fname = (LPTSTR)ofn.lpstrFile;
		LPTSTR tmpString;
		_bstr_t allString = (LPTSTR)ofn.lpstrFile;
		_bstr_t path = ofn.lpstrFile;

		if (m_MultiSelect)
		{
			while (atLast == false)
			{
				if (*fname == NULL)
				{
					*fname++;

					if (*fname != NULL)
					{
						tmpString = fname;

						allString += L"|";
						allString += tmpString;

						m_FileNames.Add(CComVariant((BSTR)(path + "\\" + tmpString)));
					}
					else
						atLast = true;
				}

				*fname++;
			}
		}
		
		if (m_FileNames.m_psa == NULL)
			m_FileNames.Add(CComVariant(allString.GetBSTR()));

		//m_bstrFileName = allString.GetBSTR();
	}
	else
	{
		return checkError();
	}

	return S_OK;
}

HRESULT CDialogHelper::checkError()
{
	DWORD errorCode = CommDlgExtendedError();

	CComBSTR message;

	switch (errorCode)
	{
	case FNERR_BUFFERTOOSMALL:
		message = L"��OPENFILENAME�ṹ��lpstrFile��Աָ��Ļ����������û�ָ�����ļ�����˵̫С";
		break;
	case FNERR_INVALIDFILENAME:
		message = L"�ļ�����Ч";
		break;
	case FNERR_SUBCLASSFAILURE:
		message = L"����û���㹻�ڴ棬�ڶ��б�����ʱ����";
		break;
	case CDERR_DIALOGFAILURE:
		message = L"�Ի����ܴ���";
		break;
	case CDERR_FINDRESFAILURE:
		message = L"�����Ի�����û���ҵ�ָ����Դ";
		break;
	case CDERR_INITIALIZATION:
		message = L"�����Ի������ڳ�ʼ��������ʧ��,û���㹻�ڴ�";
		break;
	case CDERR_LOADRESFAILURE:
		message = L"�����Ի�����û�ܵ���ָ������Դ";
		break;
	case CDERR_LOADSTRFAILURE:
		message = L"�����Ի�����û�ܵ���ָ���Ĵ�";
		break;
	case CDERR_LOCKRESFAILURE:
		message = L"�����Ի�����û������ָ������Դ";
		break;
	case CDERR_MEMLOCKFAILURE:
		message = L"�����Ի���������Ϊ�ڲ��ṹ�����ڴ�";
		break;
	case CDERR_NOHINSTANCE:
		message = L"�ڶ�Ӧ�Ĺ����Ի����ʼ���ṹFlags��Ա������ENABLETEMPLATE��־,�������ṩ��Ӧ���������ʱ����";
		break;
	case CDERR_NOHOOK:
		message = L"�ڶ�Ӧ�Ĺ����Ի����ʼ���ṹFlags��Ա������ENABLEHOOK��־���������ṩ��Ӧ�Ĺҹ�����ָ��ʱ����";
		break;
	case CDERR_NOTEMPLATE:
		message = L"�ڶ�Ӧ�Ĺ����Ի����ʼ���ṹFlag��Ա������ENABLETEMPLATE��־,�������ṩ��Ӧ��ģ��ʱ����";
		break;
	case CDERR_REGISTERMSGFAIL:
		message = L"��RegisterWindowMessage�����������Ի���������ʱ���ú������ش������";
		break;
	case CDERR_STRUCTSIZE:
		message = L"��RegisterWindowMessage�����������Ի���������ʱ���ú������ش������";
		break;
	default:
		break;
	}

	if (errorCode == 0)
		return NULL;

	//errorCode ��DWORD���� �䷶Χ��0---2^32-1��ת��Ϊ�ַ����Ļ�100��char������ԡ�
	TCHAR msg[200] = { 0 };

	_stprintf_s(msg, _T("ErrorCode: %d"), errorCode);

	message += msg;

	return Error(message);
}
