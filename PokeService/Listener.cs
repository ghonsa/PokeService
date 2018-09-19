using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Security.Cryptography;
using System.IO;
using System.Threading;

namespace PokeService
{
    internal class Listener
    {
        internal static HttpListener broadcastListener;

        internal void DoBroadcast()
        {
            bool broadcastListening = true;
            // Create a listener.
            broadcastListener = new HttpListener();
            string url = "http://" + Environment.MachineName + ":80/GCHPoke/";
            broadcastListener.Prefixes.Add(url);
            try
            {
                while (broadcastListening)
                {
                    if (!broadcastListener.IsListening)
                    {
                        broadcastListener.Start();
                    }
                    string sourceMachine = "";
                    HttpListenerContext context = broadcastListener.GetContext();
                    HttpListenerRequest request = context.Request;
                    HttpListenerResponse broadcastResponse = context.Response;

                    string encryptedData = request.RawUrl.Remove(0, 9);  // remove "/GCHPoke/"
                    string decryptedBroadcastData = Decrypt(encryptedData);
                    char[] trimChar = { '&' };
                    string[] parameterData = decryptedBroadcastData.Split(trimChar);

                    if (parameterData[0].Length != 0)
                        sourceMachine = parameterData[0];
                    if (parameterData[1].Length != 0)
                    {
                        int rslt =  DeskSession.DisplayMessage(parameterData[1], "Poke Message", 4);
                        if (rslt == 6) // YES
                        {


                        }


                    }
                    byte[] buffer = System.Text.Encoding.UTF8.GetBytes("Sent");
                    // Get a response stream and write the response to it.
                    Stream output = broadcastResponse.OutputStream;
                    output.Write(buffer, 0, buffer.Length);
                    // You must close the output stream.
                    output.Close();
                    broadcastResponse.Close();
                }
            }
            catch (HttpListenerException e)
            {
                
            }
            catch (ThreadAbortException)
            {
            }
        }
        internal static string Decrypt(string data)
        {
            char[] trimChars = { '\\', '0' };
            byte[] key = Encoding.UTF8.GetBytes("shmegjef");
            byte[] encryptedStrAsBytes = Convert.FromBase64String(data);
            byte[] initialText = new Byte[encryptedStrAsBytes.Length];
            DESCryptoServiceProvider algor = new DESCryptoServiceProvider();
            MemoryStream memStream = new MemoryStream(encryptedStrAsBytes);
            ICryptoTransform transform = algor.CreateDecryptor(key, key);
            CryptoStream cryt = new CryptoStream(memStream, transform, CryptoStreamMode.Read);
            cryt.Read(initialText, 0, initialText.Length);
            string decryptedStr = Encoding.UTF8.GetString(initialText);
            return (decryptedStr);
        }
    }
}
