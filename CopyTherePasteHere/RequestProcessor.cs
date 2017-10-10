using System;
using System.Drawing;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace CopyTherePasteHere
{
    public class RequestProcessor
    {
        private string done;

        public void Proccess(TcpClient client)
        {
            byte[] buf = new byte[client.ReceiveBufferSize];
            client.GetStream().Read(buf, 0, buf.Length);
            string type = Encoding.UTF8.GetString(buf).Split()[0];
            byte[] answer = Encoding.UTF8.GetBytes("ready\n");
            client.GetStream().Write(answer, 0, answer.Length);
            client.GetStream().Flush();
            done = "ok";
            if (type.Equals("text"))
            {
                buf = new byte[client.ReceiveBufferSize];
                client.GetStream().Read(buf, 0, buf.Length);
                string txt = Encoding.UTF8.GetString(buf);
                Thread thread = new Thread(() => SetTextAndPaste(txt));
                thread.SetApartmentState(ApartmentState.STA); //Set the thread to STA
                thread.Start();
                thread.Join();
            }
            else if (type.Equals("image"))
            {
                buf = new byte[client.ReceiveBufferSize];
                client.GetStream().Read(buf, 0, buf.Length);
                string txt = Encoding.UTF8.GetString(buf);
                System.Diagnostics.Debug.WriteLine(txt);
                int length = Int32.Parse(txt);
                client.GetStream().Flush();
                // send ready
                client.GetStream().Write(answer, 0, answer.Length);
                client.GetStream().Flush();
                //read bytes to buffer                
                NetworkStream inBytes = client.GetStream(); // client is a TcpClient
                BinaryReader br = new BinaryReader(inBytes); // since we receive a binary
                byte[] buff = br.ReadBytes(length);
                Image img = byteArrayToImage(buff);
                Thread thread = new Thread(() => SetImageAndPaste(img));
                thread.SetApartmentState(ApartmentState.STA); //Set the thread to STA
                thread.Start();
                thread.Join();
            }
            answer = Encoding.UTF8.GetBytes("ok\n");
            client.GetStream().Write(answer, 0, answer.Length);
            client.GetStream().Flush();
        }

        private void SetTextAndPaste(string txt)
        {
            try
            {
                Clipboard.SetText(txt);
                SendKeys.SendWait("^{v}");
            }
            catch (Exception e)
            {
                done = e.Message;
            }
        }

        private void SetImageAndPaste(Image img)
        {
            try
            {
                Clipboard.SetImage(img);
                SendKeys.SendWait("^{v}");
            }
            catch (Exception e)
            {
                done = e.Message;
            }
        }

        public Image byteArrayToImage(byte[] bytesArr)
        {
            MemoryStream memstr = new MemoryStream(bytesArr);
            Image img = Image.FromStream(memstr);
            return img;
        }
    }
}