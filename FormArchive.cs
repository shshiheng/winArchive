using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace winArchive
{
    public partial class FormArchive : Form
    {
        private UDP_RTI_NET.clsRTI clsRTIReceiver;
        private clsArchive clsArchiver = new clsArchive();
        public FormArchive()
        {
            InitializeComponent();
        }
        private void timerReceiver_Tick(object sender, EventArgs e)
        {
            UDP_RTI_NET.SRTIMessage[] stcRTIBuffers = new UDP_RTI_NET.SRTIMessage[UDP_RTI_NET.clsRTI.RTI_BUFFER_SIZE];
            ushort usLength = this.clsRTIReceiver.CollectRTIMessages(stcRTIBuffers);
            if (usLength == 0)
            {
                if (this.labelLight.BackColor == Color.Red) System.Threading.Thread.Sleep(200); 
                this.labelLight.BackColor = Color.Gray;
                return;
            }
            for (ushort i = 0; i < usLength; i++)
            {
                this.clsArchiver.CopeArchiveMessages(ref stcRTIBuffers[i].chrMessage, stcRTIBuffers[i].usLength);
            }
            if (this.labelLight.BackColor ==Color.Lime)
                this.labelLight.BackColor = Color.Gray;
            else
                this.labelLight.BackColor = Color.Lime;
        }

        private void FormArchive_Load(object sender, EventArgs e)
        {
            string strConnectionString = "Driver={Microsoft Access Driver (*.mdb, *.accdb)};DBQ=C:\\DevelopBase\\Failure\\dbArchive.accdb";
            try
            {
                using (StreamReader sr = new StreamReader("Archive.INI"))
                {
                    strConnectionString = sr.ReadLine();
                    this.clsArchiver.Setup(strConnectionString);
                    this.clsArchiver.dbConnection.Open();
                    this.clsArchiver.ReadDataBase();
                    string[] strParameters = sr.ReadLine().Split('=');
                    int iPortNumber = System.Int32.Parse(strParameters[1]);
                    strParameters = sr.ReadLine().Split('=');
                    ushort usBusNo = System.UInt16.Parse(strParameters[1]);
                    this.clsRTIReceiver = new UDP_RTI_NET.clsRTI(iPortNumber,usBusNo);
                    this.clsRTIReceiver.OpenRTIReceiver();

                    strParameters = sr.ReadLine().Split('=');
                    byte byteFederalNo = System.Byte.Parse(strParameters[1]);
                    strParameters = sr.ReadLine().Split('=');
                    iPortNumber = System.Int32.Parse(strParameters[1]);
                    strParameters = sr.ReadLine().Split('=');
                    usBusNo = System.UInt16.Parse(strParameters[1]);
                    this.clsArchiver.clsDisplaySender.udpRTISender = new UDP_RTI_NET.clsRTISender(byteFederalNo, usBusNo, iPortNumber);
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.ToString());
                return;
            }
            this.timerReceiver.Enabled = true;
        }

        private void FormArchive_DoubleClick(object sender, EventArgs e)
        {
            string strClear;
            this.labelLight.BackColor = Color.Red;
            try
            {
                using (StreamReader sr = new StreamReader("Archive.INI"))
                {
                    do
                    {
                        strClear = sr.ReadLine();
                    } while (strClear != "DATABASE COMMANDS FIELD");
                    while ((strClear = sr.ReadLine()) != "END DATABASE COMMANDS FIELD")
                    {
                        this.clsArchiver.ExecuteNonQueryCommand(strClear);
                    }
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.ToString());
                return;
            } 
        }

        private void FormArchive_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.timerReceiver.Stop();
            this.clsRTIReceiver.CloseRTIReceiver();
            this.clsArchiver.clsDisplaySender.udpRTISender.udpSendSocket.Close();
            this.clsArchiver.dbConnection.Close();
        }

    }
}
