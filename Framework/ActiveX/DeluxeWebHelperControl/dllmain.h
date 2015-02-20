// dllmain.h : Declaration of module class.

class CDeluxeWebHelperControlModule : public ATL::CAtlDllModuleT< CDeluxeWebHelperControlModule >
{
public :
	DECLARE_LIBID(LIBID_DeluxeWebHelperControlLib)
	DECLARE_REGISTRY_APPID_RESOURCEID(IDR_DELUXEWEBHELPERCONTROL, "{94DE0A7D-F36D-459D-B106-3C4D71992864}")
};

extern class CDeluxeWebHelperControlModule _AtlModule;
