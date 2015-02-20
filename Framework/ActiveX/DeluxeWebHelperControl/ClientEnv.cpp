// ClientEnv.cpp : Implementation of CClientEnv

#include "stdafx.h"
#include "objsafe.h"
#include "Cderr.h"
#include <stdlib.h>
#include <comdef.h>
#include "AtlSafe.h"
#include <iphlpapi.h>
#include "ClientEnv.h"

#pragma comment( lib, "rpcrt4.lib" )
#pragma comment(lib, "Iphlpapi.lib")

// CClientEnv

STDMETHODIMP CClientEnv::GetMACAddressFromUuid(BSTR* retVal)
{
	UUID uuid;

	// Ask OS to create UUID
	UuidCreateSequential(&uuid);

	CComBSTR macData(12);

	TCHAR buf[3] = { 0 };

	// Bytes 2 through 7 inclusive are MAC address
	for (int i = 2; i < 8; i++)
	{
		int j = (i - 2) * 2;

		_stprintf_s(buf, _T("%02X"), uuid.Data4[i]);

		macData[j] = buf[0];
		macData[j + 1] = buf[1];
	}

	*retVal = macData.Detach();

	return S_OK;
}


STDMETHODIMP CClientEnv::GetAllMACAddresses(VARIANT* addresses)
{
	CComSafeArray<VARIANT> addrArray;

	IP_ADAPTER_INFO adapterInfo[32];

	DWORD dwBufLen = sizeof(adapterInfo);  // Save memory size of buffer

	// Call GetAdapterInfo
	DWORD dwStatus = ::GetAdaptersInfo(
		adapterInfo,	// [out] buffer to receive data
		&dwBufLen);

	PIP_ADAPTER_INFO pAdapterInfo = adapterInfo;

	do {
		CComBSTR formattedAddress = CComBSTR(pAdapterInfo->AddressLength * 2);

		for (int i = 0; i < (int)formattedAddress.Length(); i++)
			formattedAddress[i] = '\0';

		FormatMACAddress(pAdapterInfo->Address, pAdapterInfo->AddressLength, formattedAddress.m_str);

		addrArray.Add(CComVariant(formattedAddress));
		pAdapterInfo = pAdapterInfo->Next;    // Progress through 
	} while (pAdapterInfo);

	CComVariant var = CComVariant(addrArray);

	var.ChangeType(VT_ARRAY | VT_BSTR);
	var.Detach(addresses);

	return S_OK;
}

STDMETHODIMP CClientEnv::GetAllEncryptedMACAddresses(BSTR encString, VARIANT* encAddresses)
{
	byte encData[256];

	int encDataLength = HexStringToByteArray(encString, encData, 256);

	CComSafeArray<VARIANT> addrArray;

	IP_ADAPTER_INFO adapterInfo[32];

	DWORD dwBufLen = sizeof(adapterInfo);  // Save memory size of buffer

	// Call GetAdapterInfo
	DWORD dwStatus = ::GetAdaptersInfo(
		adapterInfo,	// [out] buffer to receive data
		&dwBufLen);

	PIP_ADAPTER_INFO pAdapterInfo = adapterInfo;

	do {
		CComBSTR formattedAddress = CComBSTR(pAdapterInfo->AddressLength * 2);

		for (int i = 0; i < (int)formattedAddress.Length(); i++)
			formattedAddress[i] = '\0';

		FormatEncryptedMACAddress(pAdapterInfo->Address, pAdapterInfo->AddressLength, encData, encDataLength, formattedAddress.m_str);

		addrArray.Add(CComVariant(formattedAddress));
		pAdapterInfo = pAdapterInfo->Next;    // Progress through

	} while (pAdapterInfo);

	CComVariant var = CComVariant(addrArray);

	var.ChangeType(VT_ARRAY | VT_BSTR);
	var.Detach(encAddresses);

	return S_OK;
}

void CClientEnv::FormatMACAddress(byte address[], int len, BSTR result)
{
	TCHAR buf[3] = { 0 };

	for (int i = 0; i < len; i++)
	{
		_stprintf_s(buf, _T("%02X"), address[i]);

		result[i * 2] = buf[0];
		result[i * 2 + 1] = buf[1];
	}
}

void CClientEnv::FormatEncryptedMACAddress(byte address[], int len, byte* encData, LONG encDataSize, BSTR result)
{
	TCHAR buf[3] = { 0 };

	for (int i = 0; i < len; i++)
	{
		int encIndex = i % encDataSize;
		byte encByte = encData[encIndex];

		byte encryptedByte = address[i] ^ encByte;

		_stprintf_s(buf, _T("%02X"), encryptedByte);

		result[i * 2] = buf[0];
		result[i * 2 + 1] = buf[1];
	}
}

int CClientEnv::HexStringToByteArray(BSTR encString, byte encData[], int encByteLength)
{
	bstr_t bstr = bstr_t(encString);

	UINT i = 0;
	int dataIndex = 0;

	while (i < bstr.length())
	{
		byte high = TCHARToByte(encString[i++]);
		byte low = '0';

		if (i < bstr.length())
			low = TCHARToByte(encString[i++]);

		encData[dataIndex++] = (high << 4) | low;
	}

	return dataIndex;
}

byte CClientEnv::TCHARToByte(TCHAR ch)
{
	byte result = 0;

	if (ch >= '0' && ch <= '9')
		result = ch - '0';
	else
		if (ch >= 'A' && ch <= 'F')
			result = 10 + ch - 'A';
		else
			if (ch >= 'a' && ch <= 'f')
				result = 10 + ch - 'a';

	return result;
}

