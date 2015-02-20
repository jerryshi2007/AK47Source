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


	//如果选择了多选方式，则使用多选标记填充Flags
	if (m_MultiSelect)
		ofn.Flags = OFN_EXPLORER | OFN_FILEMUSTEXIST | OFN_HIDEREADONLY | OFN_PATHMUSTEXIST | OFN_ALLOWMULTISELECT;
	else
		ofn.Flags = OFN_EXPLORER | OFN_FILEMUSTEXIST;

	*retVal = GetOpenFileName(&ofn);

	::SysFreeString(tFilter);

	bool atLast = false;

	//清空选择的文件名数组
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
		message = L"由OPENFILENAME结构的lpstrFile成员指向的缓冲区对由用户指定的文件名来说太小";
		break;
	case FNERR_INVALIDFILENAME:
		message = L"文件名无效";
		break;
	case FNERR_SUBCLASSFAILURE:
		message = L"由于没有足够内存，在对列表框分类时出错";
		break;
	case CDERR_DIALOGFAILURE:
		message = L"对话框不能创建";
		break;
	case CDERR_FINDRESFAILURE:
		message = L"公共对话框函数没能找到指定资源";
		break;
	case CDERR_INITIALIZATION:
		message = L"公共对话框函数在初始化过程中失败,没有足够内存";
		break;
	case CDERR_LOADRESFAILURE:
		message = L"公共对话框函数没能调出指定的资源";
		break;
	case CDERR_LOADSTRFAILURE:
		message = L"公共对话框函数没能调出指定的串";
		break;
	case CDERR_LOCKRESFAILURE:
		message = L"公共对话框函数没能销定指定的资源";
		break;
	case CDERR_MEMLOCKFAILURE:
		message = L"公共对话框函数不能为内部结构分配内存";
		break;
	case CDERR_NOHINSTANCE:
		message = L"在对应的公共对话框初始化结构Flags成员中设置ENABLETEMPLATE标志,但是在提供相应的事例句柄时出错";
		break;
	case CDERR_NOHOOK:
		message = L"在对应的公共对话框初始化结构Flags成员中设置ENABLEHOOK标志，但是在提供相应的挂钩程序指针时出错";
		break;
	case CDERR_NOTEMPLATE:
		message = L"在对应的公共对话框初始化结构Flag成员中设置ENABLETEMPLATE标志,但是在提供相应的模板时出错";
		break;
	case CDERR_REGISTERMSGFAIL:
		message = L"当RegisterWindowMessage函数被公共对话框函数调用时，该函数返回错误代码";
		break;
	case CDERR_STRUCTSIZE:
		message = L"当RegisterWindowMessage函数被公共对话框函数调用时，该函数返回错误代码";
		break;
	default:
		break;
	}

	if (errorCode == 0)
		return NULL;

	//errorCode 是DWORD类型 其范围是0---2^32-1，转换为字符串的话100个char宽度足以。
	TCHAR msg[200] = { 0 };

	_stprintf_s(msg, _T("ErrorCode: %d"), errorCode);

	message += msg;

	return Error(message);
}
