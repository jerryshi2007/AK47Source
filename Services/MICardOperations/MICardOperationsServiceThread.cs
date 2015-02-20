using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Services;
using SinoOcean.Seagull2.Framework.MasterData;

namespace MICardOperations
{
	public class MICardOperationsServiceThread : ThreadTaskBase
	{
		public override void OnThreadTaskStart()
		{
			GenerateDataXML();
		}
		/// <summary>
		/// 生成XML数据
		/// </summary>
		/// <returns></returns>
		private void GenerateDataXML()
		{
			const string SWIPESPOT_XMLPATH = @"E:\\SwipeSpot.xml";
			const string CARDSINFO_XMLPATH = @"E:\\CardsInfo.xml";

			//生成刷卡地点XML文档
			SwipeSpotAdapter.Instance.CopyToXmlDoc().Save(SWIPESPOT_XMLPATH);
			//生成CardsInfo的XML文档
			CardsInfoAdapter.Instance.CopyToXmlDoc().Save(CARDSINFO_XMLPATH);
		}
	}
}
