using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DeluxeWebHelperControlLib;
using System.IO;
using System.Globalization;

namespace DeluxeWebHelperControlTest
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void buttonShowWindowsTemp_Click(object sender, EventArgs e)
        {
            IComponentHelper helper = new ComponentHelper();

            dynamic fso = helper.CreateObject("Scripting.FileSystemObject");

            MessageBox.Show(this, fso.GetSpecialFolder(2).Path);
        }

        private void buttonOpenWord_Click(object sender, EventArgs e)
        {
            IComponentHelper helper = new ComponentHelper();

            dynamic word = helper.CreateLocalServer("Word.Application");

            word.Visible = true;
        }

        private void buttonOpenDialog_Click(object sender, EventArgs e)
        {
            IDialogHelper helper = new DialogHelper();

            helper.MultiSelect = true;

            if (helper.OpenDialog())
            {
                StringBuilder strB = new StringBuilder();

                foreach (string fileName in helper.FileNames)
                {
                    if (strB.Length > 0)
                        strB.Append("\n");

                    strB.Append(fileName);
                }

                MessageBox.Show(this, strB.ToString());
            }
        }

        private void buttonGetAuthor_Click(object sender, EventArgs e)
        {
            IComponentHelper helper = new ComponentHelper();

            dynamic author = helper.GetAuthor();

            MessageBox.Show(this, author.Name);
        }

        private void buttonGetMACFromUuid_Click(object sender, EventArgs e)
        {
            IClientEnv env = new ClientEnv();

            MessageBox.Show(this, env.GetMACAddressFromUuid());
        }

        private void buttonGetAllMAC_Click(object sender, EventArgs e)
        {
            IClientEnv env = new ClientEnv();

            MessageBox.Show(this, string.Join("\n", env.GetAllMACAddresses()));
        }

        private static readonly byte[] EncData = new byte[] { 15, 25, 35, 45 };

        private static string EncDataToString(byte[] encData)
        {
            StringBuilder strB = new StringBuilder();

            for (int i = 0; i < encData.Length; i++)
                strB.AppendFormat("{0:X2}", encData[i]);

            return strB.ToString();
        }

        private void buttonGetAllEncryptedMACAddress_Click(object sender, EventArgs e)
        {
            IClientEnv env = new ClientEnv();

            MessageBox.Show(this, string.Join("\n", env.GetAllEncryptedMACAddresses((EncDataToString(EncData)))));
        }

        private void buttonGetAllDecryptedMACAddress_Click(object sender, EventArgs e)
        {
            IClientEnv env = new ClientEnv();

            dynamic dynEncryptedAddresses = env.GetAllEncryptedMACAddresses(EncDataToString(EncData));

            List<string> encryptedAddresses = new List<string>();

            foreach (string data in dynEncryptedAddresses)
                encryptedAddresses.Add(data);

            MessageBox.Show(this, string.Join("\n", DecryptMACAddresses(EncData, encryptedAddresses.ToArray())));
        }

        private static string[] DecryptMACAddresses(byte[] encData, string[] encryptedMACAddresses)
        {
            string[] result = new string[encryptedMACAddresses.Length];

            for (int i = 0; i < encryptedMACAddresses.Length; i++)
                result[i] = DecryptMACAddresses(encData, encryptedMACAddresses[i]);

            return result;
        }

        private static string DecryptMACAddresses(byte[] encData, string encryptedMACAddress)
        {
            byte[] encMacAddress = StringToByteArray(encryptedMACAddress);

            StringBuilder strB = new StringBuilder();

            for (int i = 0; i < encMacAddress.Length; i++)
            {
                int encIndex = i % encData.Length;

                strB.AppendFormat("{0:X2}", encMacAddress[i] ^ encData[encIndex]);
            }

            return strB.ToString();
        }

        private static byte[] StringToByteArray(string data)
        {
            List<byte> result = new List<byte>();

            if (string.IsNullOrEmpty(data) == false)
            {
                int i = 0;

                while (i < data.Length)
                {
                    byte high = 0;

                    int readCount = 2;

                    if (i + 1 >= data.Length)
                        readCount = 1;

                    string byteData = data.Substring(i, readCount);

                    if (readCount == 1)
                        byteData += "0";

                    result.Add(byte.Parse(byteData, NumberStyles.HexNumber));

                    i += readCount;
                }
            }

            return result.ToArray();
        }
    }
}
