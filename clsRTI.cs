using System;
using System.Threading; 
using System.Net.Sockets;
using System.Data ;
using System.Text ;
using System.Net;
using System.Collections;
using System.Data.Odbc;
using System.IO;


namespace UDP_RTI_NET
{
    public class SRTIMessage
    {
        public const ushort MAX_RTI_MESSAGE_LENGTH = 250;
        public ushort usLength;
        public byte[] chrMessage;
        public SRTIMessage()
        {
            usLength = 0;
            chrMessage = new byte[MAX_RTI_MESSAGE_LENGTH];
        }
    }

    public class clsRTI
	{
        private volatile bool bStop = false;
        public const ushort RTI_BUFFER_SIZE = 4000;
        public UdpClient udpSocket;

        private ushort usBusNo;
        private System.Net.IPEndPoint ipSendPoint;

        private SRTIMessage[] stcRTIBuffers = new SRTIMessage[RTI_BUFFER_SIZE];
        private ushort usRTIBufferStart = 0, usRTIBufferEnd = 0;
        private byte[] chrStoredMessages = new byte[10 * SRTIMessage.MAX_RTI_MESSAGE_LENGTH];
        private ushort usStoredMessageNumber = 0;

        private System.Threading.Thread hThreadReceiver = null;

        public clsRTI(int port,ushort usReceiveBusNo)
		{
            this.usBusNo = usReceiveBusNo;
            udpSocket = new UdpClient(port);
            ipSendPoint = new IPEndPoint(IPAddress.Any, 0);
            for (ushort i = 0; i < RTI_BUFFER_SIZE; i++)
                stcRTIBuffers[i] = new SRTIMessage();      
		}

        ~clsRTI()
        {
            if (this.hThreadReceiver != null)
            {

            }
            this.udpSocket.Close();
        }
        private void ReceiveRTIMessage()
        {
            ushort usLength = 0;
	        ushort usEnd = 0;
            byte[] byteNewMessage;
	        ushort usStart = 0;
	        byte ucSouceFederaNo = 0;
            ushort i = 0;
            ushort usCurrentBusNo = 0;

            while ( ! bStop )
            {
                try
                {
                    byteNewMessage = this.udpSocket.Receive(ref ipSendPoint);
                    usEnd = (ushort)byteNewMessage.Length;
                    if (usEnd + usStoredMessageNumber > 10 * SRTIMessage.MAX_RTI_MESSAGE_LENGTH)
                    {
                        usStoredMessageNumber = 0;
                        continue;
                    }
                    byteNewMessage.CopyTo(chrStoredMessages, usStoredMessageNumber);
                    usStoredMessageNumber += usEnd;
                    if (usStoredMessageNumber < 5)
                        continue;
                    usStart = 0;
/*
                    ucSouceFederaNo = chrStoredMessages[usStart];
                    usCurrentBusNo = System.BitConverter.ToUInt16(chrStoredMessages, usStart + 1);
                    usLength = System.BitConverter.ToUInt16(chrStoredMessages, usStart + 3);

                    while (usLength <= usStoredMessageNumber - usStart - 5)
                    {
                        if (usCurrentBusNo == this.usBusNo)
                        {
                            stcRTIBuffers[usRTIBufferEnd].usLength = usLength;
                            for (i = 0; i < usLength; i++) stcRTIBuffers[usRTIBufferEnd].chrMessage[i] = chrStoredMessages[usStart + 5 + i];
                            usRTIBufferEnd++; if (usRTIBufferEnd >= RTI_BUFFER_SIZE) usRTIBufferEnd = 0;
                        }
                        usStart += (ushort) (usLength + 5);
                        if (usStart <= usStoredMessageNumber-5)
                        {
                            ucSouceFederaNo = chrStoredMessages[usStart];
                            usBusNo = System.BitConverter.ToUInt16(chrStoredMessages, usStart + 1);
                            usLength = System.BitConverter.ToUInt16(chrStoredMessages, usStart + 3);
                        }
                        else
                            break;
                    }
    */
                    do 
                    {
                        ucSouceFederaNo = chrStoredMessages[usStart];
                        usCurrentBusNo = System.BitConverter.ToUInt16(chrStoredMessages, usStart + 1);
                        usLength = System.BitConverter.ToUInt16(chrStoredMessages, usStart + 3);
                        if (usLength > usStoredMessageNumber - usStart - 5) break;
                        if (usCurrentBusNo == this.usBusNo )
                        {
                            stcRTIBuffers[usRTIBufferEnd].usLength = usLength;
                            for (i = 0; i < usLength; i++) stcRTIBuffers[usRTIBufferEnd].chrMessage[i] = chrStoredMessages[usStart + 5 + i];
                            usRTIBufferEnd++; if (usRTIBufferEnd >= RTI_BUFFER_SIZE) usRTIBufferEnd = 0;
                        }
                        usStart += (ushort)(usLength + 5);
                    } while (usStart <= usStoredMessageNumber - 5);
                    usLength = (ushort)(usStoredMessageNumber - usStart);
                    for (i = 0; i < usLength; i++)  chrStoredMessages[i] = chrStoredMessages[usStart + i];
                    chrStoredMessages[usLength] = 0;
                    usStoredMessageNumber = usLength;
                }
                catch (Exception err)
                {
                    string strMsg = err.ToString();
                }
            }
        }

        public void OpenRTIReceiver()
        {
            this.hThreadReceiver = new System.Threading.Thread(this.ReceiveRTIMessage);
            this.hThreadReceiver.Start();
        }
        public void CloseRTIReceiver()
        {
            this.bStop = true;
            this.hThreadReceiver.Join(1000);
            this.udpSocket.Close();
        }

        public ushort CollectRTIMessages(SRTIMessage[] stcBufferOutput)
        {
	        ushort usLength=0;
	        ushort usEnd = usRTIBufferEnd;
	        ushort usStart = usRTIBufferStart;
            ushort i = 0;

	        if (usEnd == usStart) 
		        return(0);
	        else if (usEnd > usStart)
	        {
                for (i = 0; i < (usEnd - usStart); i++)      stcBufferOutput[i] = stcRTIBuffers[usStart + i];
                usLength = (ushort)(usEnd-usStart);
	        }
	        else
	        {
                for (i = 0; i < RTI_BUFFER_SIZE - usStart; i++) stcBufferOutput[i] = stcRTIBuffers[usStart + i];
                if (usEnd != 0)
                {
                    for (i = 0; i < usEnd; i++) stcBufferOutput[i + RTI_BUFFER_SIZE - usStart] = stcRTIBuffers[i];
                }
                usLength = (ushort)(RTI_BUFFER_SIZE + usEnd - usStart);
	        }
	        usRTIBufferStart = usEnd;
	        return usLength;
        }

    }

    public class clsRTISender
    {
        public UdpClient udpSendSocket;
        public System.Net.IPEndPoint ipSendPoint;
        private byte ucFederalNo;
        private ushort usBusNo;

        public clsRTISender(byte ucFederalNo,ushort usSendBusNo, int iPort)
        {
            this.ucFederalNo = ucFederalNo;
            this.usBusNo = usSendBusNo;
            this.udpSendSocket = new UdpClient();
            //this.ipSendPoint = new IPEndPoint(IPAddress.Broadcast, iPort);
            this.ipSendPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), iPort);
        }

        ~clsRTISender()
        {
            this.udpSendSocket.Close();
        }

        public void SendRTIMessage(byte[] byteMessages,ushort usLength)
        {
            byte[] byteSendMessages = new byte[SRTIMessage.MAX_RTI_MESSAGE_LENGTH];            
            if (usLength+5 > SRTIMessage.MAX_RTI_MESSAGE_LENGTH) return;
            byte[] byteFederal = BitConverter.GetBytes(this.ucFederalNo);
            byteFederal.CopyTo(byteSendMessages,0);
            byte[] byteBusNo = BitConverter.GetBytes(this.usBusNo);
            byteBusNo.CopyTo(byteSendMessages, 1);
            byte[] byteLength = BitConverter.GetBytes(usLength);
            byteLength.CopyTo(byteSendMessages,3);
            byteMessages.CopyTo(byteSendMessages,5);
            try
            {
                this.udpSendSocket.Send(byteSendMessages, System.Convert.ToInt32(usLength+5), this.ipSendPoint);
            }
            catch (System.Exception err)
            {
                string strMsg = err.ToString();
            }
        }
    }
}
