using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace winViewerSender
{
    public class clsParameterSender
    {
        public UDP_RTI_NET.clsRTISender udpRTISender;
        public void SendValue(ushort iCode, double Value)
        {
            byte[] byteSend = new byte[11];
            byteSend[0] = 1;
            byte[] byteiCode = BitConverter.GetBytes(iCode);
            byteiCode.CopyTo(byteSend, 1);
            byte[] byteValue = BitConverter.GetBytes(Value);
            byteValue.CopyTo(byteSend, 3);
            this.udpRTISender.SendRTIMessage(byteSend, 11);
        }
    }
}
