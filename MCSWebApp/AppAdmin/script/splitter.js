<!--
	var m_oStartOrg = null;
	var m_nLeftMargin = 30;
	var m_nRightMargin = 80;
	var m_splitter = null;

	function onSplitterMouseOver()
	{
		var obj = event.srcElement;

		if (obj.tagName == "DIV")
			obj.className = "activeElement";
	}

	function onSplitterMouseOut()
	{
		var obj = event.srcElement;

		if (obj.tagName == "DIV")
			obj.className = "";
	}

	function onSplitterMove()
	{
		if (event.button == 1)
		{
			if (m_oStartOrg == null)
			{
				var obj = event.srcElement;

				m_oStartOrg = new Object();

				m_oStartOrg.srcElement = obj;
				m_oStartOrg.xOffset = event.x - obj.style.pixelLeft;
				m_oStartOrg.yOffset = event.y - obj.style.pixelTop;
				m_oStartOrg.xLast = event.x;
				m_oStartOrg.yLast = event.y;

				obj.setCapture();
			}
			else
			{
				var obj = m_oStartOrg.srcElement;

				var nLeft = event.x - m_oStartOrg.xOffset;

				var oTD = obj.container;
				var oPrev = oTD.previousSibling;

				if (oPrev)
				{						
					if (nLeft <= m_nLeftMargin)
						nLeft = m_nLeftMargin;
					else
					if (nLeft > document.body.offsetWidth - m_nRightMargin)
						nLeft = document.body.offsetWidth - m_nRightMargin;

					obj.style.pixelLeft = nLeft;

					var nDelta = nLeft - (m_oStartOrg.xLast - m_oStartOrg.xOffset);

					if (oPrev.style.pixelWidth + nDelta > 0)
						oPrev.style.pixelWidth += nDelta;
				}
				m_oStartOrg.xLast = nLeft + m_oStartOrg.xOffset;
				m_oStartOrg.yLast = event.y;
			}
		}
	}

	function onSplitterUp()
	{
		var obj = event.srcElement;

		if (m_oStartOrg)
		{
			m_oStartOrg.srcElement.releaseCapture();
			m_oStartOrg = null;
		}
	}

	function onWindowResize()
	{
		initSplitter(splitterContainer);
	}

	function initSplitter(container)
	{
		if (!m_splitter)
		{
			m_splitter = document.createElement('<div style="DISPLAY:NONE;BORDER-RIGHT: black 1px solid;BORDER-LEFT: black 1px solid; CURSOR: w-resize; POSITION: absolute; HEIGHT: 0%"></div>');

			document.body.insertAdjacentElement("afterBegin", m_splitter);
			m_splitter.style.width = "4px";
		}

		with(container)
		{
			m_splitter.style.left = absLeft(container);
			m_splitter.style.top = absTop(container);
			m_splitter.style.height = container.offsetHeight;
			m_splitter.style.display = "inline";
			m_splitter.container = container;

			m_splitter.onmousemove = onSplitterMove;
			m_splitter.onmouseup = onSplitterUp;
			m_splitter.onmouseover = onSplitterMouseOver;
			m_splitter.onmouseout = onSplitterMouseOut;
		}
	}
//-->
