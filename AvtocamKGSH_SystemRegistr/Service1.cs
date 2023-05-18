using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.IO.Ports;
using OwenProtocol.IO;
using OwenProtocol;
using System.Threading;
using static System.Net.WebRequestMethods;

namespace AvtocamKGSH_SystemRegistr
{
    public partial class Service1 : ServiceBase
    {
        SerialPortAdapter sp = new SerialPortAdapter(3, 115200, 0, 8, System.IO.Ports.StopBits.One);

        public Service1()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
              try
                {
                   Task.Factory.StartNew(() =>
                   {
                       if (!sp.IsOpened) sp.Open();
                       doWhile();

                   });
                 System.IO.File.AppendAllText("d:\\" + "ошибка.txt", "OnStart1 " +  "\n");
 
                }
                catch (Exception ex)
                {
                    System.IO.File.AppendAllText("d:\\" + "ошибка.txt",  "OnStart - " + ex.Message + "\n");
                } 
        }

        public void doWhile()
        {
            byte[] res;
            float value;
            System.IO.File.AppendAllText("d:\\" + "ошибка.txt", " doWhile start" + "\n");
            OwenioNet.DataConverter.Converter.ConverterFloat conv = new OwenioNet.DataConverter.Converter.ConverterFloat(3);
            using (var OPM = OwenProtocolMaster.Create(sp))
            {
                try
                {
                while (true)
                    {//что то//что то еще
                        WorkWithData.ReadDevices();
                        if (WorkWithData.DevicesReader.HasRows)
                        {
                            while (WorkWithData.DevicesReader.Read())
                            {
                               Int16 adr =  Convert.ToInt16(WorkWithData.DevicesReader[0]);
                                try 
                                { 
                                    res = OPM.OwenRead(adr, OwenProtocol.Types.AddressLengthType.Bits8, "PV");
                                    value = conv.ConvertBack(res);
                                }
                                catch (Exception ex)
                                {   
                                    value = 0;
                                    WorkWithData.InsertValue(adr, value);
                                    System.IO.File.AppendAllText("d:\\" + "ошибка.txt", " ошибка чтения " + adr.ToString() + ex.Message +"\n");
                                    if (!sp.IsOpened) sp.Open();
                                    continue; 
                                }
                                WorkWithData.InsertValue(adr, value);
                                //Thread.Sleep(1000);
                            }
                        }
                        WorkWithData.DevicesReader.Close();
                        Thread.Sleep(5000);
                    }
                }
                catch (Exception ex)
                {
                    System.IO.File.AppendAllText("d:\\" + "ошибка.txt", " ошибка doWhile " + ex.Message + "\n");
                    if (!sp.IsOpened) sp.Open();
                    doWhile();
                }
            }
        }

        protected override void OnStop()
        {
           if (!WorkWithData.DevicesReader.IsClosed) WorkWithData.DevicesReader.Close();
           if (sp.IsOpened) sp.Close();
         }
    }
}
