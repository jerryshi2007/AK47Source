using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Globalization;

namespace MCS.Library.Office.OpenXml.Excel
{
    public class BorderXmlWrapper : StyleXmlBaseWrapper
    {
        internal BorderXmlWrapper()
        {
        }

        //internal BorderXmlWrapper(XmlNamespaceManager nsm, XmlNode topNode) :
        //    base(nsm, topNode)
        //{
        //    this._Left = new BorderItemXmlWrapper(nsm, topNode.SelectSingleNode(leftPath, nsm));
        //    this._Right = new BorderItemXmlWrapper(nsm, topNode.SelectSingleNode(rightPath, nsm));
        //    this._Top = new BorderItemXmlWrapper(nsm, topNode.SelectSingleNode(topPath, nsm));
        //    this._Bottom = new BorderItemXmlWrapper(nsm, topNode.SelectSingleNode(bottomPath, nsm));
        //    this._Diagonal = new BorderItemXmlWrapper(nsm, topNode.SelectSingleNode(diagonalPath, nsm));
        //}

        internal override string Id
        {
            get
            {
                return Left.Id + "|" + Right.Id + "|" + Top.Id + "|" + Bottom.Id + "|" + Diagonal.Id + "|" + DiagonalUp.ToString() + "|" + DiagonalDown.ToString();
            }
        }

        private const string leftPath = "d:left";
        private BorderItemXmlWrapper _Left;
        public BorderItemXmlWrapper Left
        {
            get
            {
                if (this._Left == null)
                {
                    this._Left = new BorderItemXmlWrapper();
                }

                return this._Left;
            }
            internal set
            {
                this._Left = value;
            }
        }

        private const string rightPath = "d:right";
        private BorderItemXmlWrapper _Right;
        public BorderItemXmlWrapper Right
        {
            get
            {
                if (this._Right == null)
                {
                    this._Right = new BorderItemXmlWrapper();
                }
                return this._Right;
            }
            internal set
            {
                this._Right = value;
            }
        }

        private const string topPath = "d:top";
        private BorderItemXmlWrapper _Top;
        public BorderItemXmlWrapper Top
        {
            get
            {
                if (this._Top == null)
                {
                    this._Top = new BorderItemXmlWrapper();
                }
                return this._Top;
            }
            internal set
            {
                this._Top = value;
            }
        }

        private const string bottomPath = "d:bottom";
        private BorderItemXmlWrapper _Bottom;
        public BorderItemXmlWrapper Bottom
        {
            get
            {
                if (this._Bottom == null)
                {
                    this._Bottom = new BorderItemXmlWrapper();
                }
                return this._Bottom;
            }
            internal set
            {
                this._Bottom = value;
            }
        }

        private const string diagonalPath = "d:diagonal";
        private BorderItemXmlWrapper _Diagonal = null;
        public BorderItemXmlWrapper Diagonal
        {
            get
            {
                if (this._Diagonal == null)
                {
                    this._Diagonal = new BorderItemXmlWrapper();
                }
                return this._Diagonal;
            }
            internal set
            {
                this._Diagonal = value;
            }
        }

        private const string diagonalUpPath = "@diagonalUp";
        private bool _DiagonalUp = false;
        public bool DiagonalUp
        {
            get
            {
                return this._DiagonalUp;
            }
            internal set
            {
                this._DiagonalUp = value;
            }
        }

        private const string diagonalDownPath = "@diagonalDown";
        private bool _DiagonalDown = false;
        public bool DiagonalDown
        {
            get
            {
                return this._DiagonalDown;
            }
            internal set
            {
                this._DiagonalDown = value;
            }
        }

        internal BorderXmlWrapper Copy()
        {
            BorderXmlWrapper newBorder = new BorderXmlWrapper();

            newBorder.Bottom = this._Bottom.Copy();
            newBorder.Diagonal = this._Diagonal.Copy();
            newBorder.Left = this._Left.Copy();
            newBorder.Right = this._Right.Copy();
            newBorder.Top = this._Top.Copy();
            newBorder.DiagonalUp = this._DiagonalUp;
            newBorder.DiagonalDown = this._DiagonalDown;

            return newBorder;

        }
    }
}
