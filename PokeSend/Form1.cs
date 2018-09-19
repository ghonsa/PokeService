using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Net;
using System.IO;
using System.Security.Cryptography;

namespace FMBroadCast
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

       void ProcessRequest(object parameter)
        {
            try
            {
                HttpWebRequest request1 = (HttpWebRequest)parameter;
                request1.GetResponse();
                Thread.Sleep(1000);
                request1.Abort();
            }
            catch (WebException e)
            {
                
            }
        }
        internal static string Encrypt(string data)
        {
            byte[] originalData = Encoding.UTF8.GetBytes(data);
            byte[] originalBytes = { };
            byte[] key = Encoding.UTF8.GetBytes("shmegjef");

            MemoryStream memStream = new MemoryStream(originalBytes.Length);
            DESCryptoServiceProvider algor = new DESCryptoServiceProvider();
            algor.Padding = PaddingMode.PKCS7;
            ICryptoTransform encryptor = algor.CreateEncryptor(key, key);
            CryptoStream c = new CryptoStream(memStream, encryptor, CryptoStreamMode.Write);
            c.Write(originalData, 0, originalData.Length);
            c.FlushFinalBlock();
            algor.Clear();
            originalBytes = memStream.ToArray();
            string encoded_string = Convert.ToBase64String(originalBytes);
            return encoded_string;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            if (messageBox.Text != "")
            {
                // Message payload looks like
                // MachineName&message//
                string payload = Environment.MachineName + "&" + messageBox.Text;

                HttpWebRequest request = null;
                request = (HttpWebRequest)WebRequest.Create("http://" + ClientNameBox.Text + ":80/GCHPoke/" + Encrypt(payload));
                request.Proxy = null;
                ProcessRequest(request);
            }
        }
    }
}
