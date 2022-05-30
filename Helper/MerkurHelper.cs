//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Net.Sockets;
//using System.Text;
//using System.Threading;
//using Modbus.Device;
//using OutReader.Model;
//using OutReader.Properties;

//namespace OutReader.Helper
//{
//    public class MerkurHelper
//    {
//        public static bool IsExists(TcpClient client, int id)
//        {
//            var val = false;
            
//                try
//                {
//                    //Connect to the server
//                    var vt = new byte[] { (byte)id, 0 };
//                    AddCrc(ref vt);
//                    //Sending the byte array to the server
//                    client.Client.Send(vt);
//                    //Get the network stream
//                    NetworkStream stream = client.GetStream();
//                    byte[] buffer = new byte[10];
//                    int bytesRead = stream.Read(buffer, 0, buffer.Length);
//                    val = buffer[0] == vt[0] && buffer[1] == vt[1];
//                }
//                catch (Exception ex)
//                {
//                    var gg = ex;
//                    var hj = gg;
//                }
            
//            return val;
//        }

//        public static bool Open(TcpClient client, int id)
//        {
//            var val = false;
            
//                try
//                {
//                    //pass 111111
//                    var vt = new byte[] { (byte)id, 1, 1, 1, 1, 1, 1, 1, 1 };
//                    AddCrc(ref vt);
//                    //Sending the byte array to the server
//                    client.Client.Send(vt);
//                    //Get the network stream
//                    NetworkStream stream = client.GetStream();
//                    byte[] buffer = new byte[10];
//                    int bytesRead = stream.Read(buffer, 0, buffer.Length);
//                    val = buffer[0] == vt[0] && buffer[1] == 0x00;
//                }
//                catch (Exception ex)
//                {
//                    var gg = ex;
//                    var hj = gg;
//                }
           
//            return val;
//        }
//        public static bool Close(TcpClient client, int id)
//        {
//            var val = false;
            
//                try
//                {
//                    //pass 111111
//                    var vt = new byte[] { (byte)id, 0x02 };
//                    AddCrc(ref vt);
//                    //Sending the byte array to the server
//                    client.Client.Send(vt);
//                    //Get the network stream
//                    NetworkStream stream = client.GetStream();
//                    byte[] buffer = new byte[10];
//                    int bytesRead = stream.Read(buffer, 0, buffer.Length);
//                    val = buffer[0] == vt[0] && buffer[1] == 0x00;
//                }
//                catch (Exception ex)
//                {
//                    var gg = ex;
//                    var hj = gg;
//                }
            
//            return val;
//        }
//        //Прочитать напряжения по 1-ой фазе для счетчика с сетевым адресом 128 (используем
//        //запрос с номером 11h).
//        //Запрос: 80 08 11 (11) | 12 | 13 (CRC)
//        //Ток 
//        //Запрос: 80 08 11 (21) | 22 | 23 (CRC)



//        public static List<decimal> I(TcpClient client, int id)
//        {
//            var val = new List<decimal>();
           
//                try
//                {
//                    for (var i = 0; i < 3; i++)
//                    {
//                        var vt = new byte[] { (byte)id, 0x08, 0x11, (byte)(33+1*i) };
//                        AddCrc(ref vt);
//                        client.Client.Send(vt);
//                        //Get the network stream
//                        NetworkStream stream = client.GetStream();
//                        byte[] buffer = new byte[10];
//                        int bytesRead = stream.Read(buffer, 0, buffer.Length);
//                        val.Add(ConverterHelper.ByteToInt(new byte[] { 0x00, buffer[1], buffer[2], buffer[3] }) / 1000.0m);
//                        Thread.Sleep(300);
//                    }
//                }
//                catch (Exception ex)
//                {
//                    var gg = ex;
//                    val = null;
//                }
           
//            return val;
//        }
//        public static List<decimal> U(TcpClient client, int id)
//        {
//            var val = new List<decimal>();
//                try
//                {
//                    for (var i = 0; i < 3; i++)
//                    {
//                        var vt = new byte[] { (byte)id, 8, 0x11, (byte)(17 + 1 * i) };
//                        AddCrc(ref vt);
//                        client.Client.Send(vt);
//                        //Get the network stream
//                        NetworkStream stream = client.GetStream();
//                        byte[] buffer = new byte[50];
//                        int bytesRead = stream.Read(buffer, 0, buffer.Length);
//                        var v = ConverterHelper.ByteToInt(new byte[] {0x00, buffer[1], buffer[2], buffer[3]});
//                        val.Add(v > 0 ? v/100.0m : 0);
//                        Thread.Sleep(300);
//                    }

//                }
//                catch (Exception ex)
//                {
//                    var gg = ex;
//                    val = null;
//                }
            
//            return val;
//        }
//        //Запрос чтения количества зафиксированной энергии по сумме тарифов для счётчика с ад-
//        //ресом 128.
//        //Запрос: 80 08 14 (F0) (CRC)
//        //Ответ: 80 (00 00 2C 36) a+ (FF FF FF FF)a- (00 00 2F 07)r+ 00 00 00 00 (CRC)r-
//        public static List<int> Energies(TcpClient client, int id)
//        {
//            var val = new List<int>();
            
//                try
//                {
//                    var vt = new byte[] { (byte)id, 0x05, 0x01, 0x00 };
//                    AddCrc(ref vt);
//                    client.Client.Send(vt);
//                    //Get the network stream
//                    var stream = client.GetStream();
//                    var buffer = new byte[20];
//                    var bytesRead = stream.Read(buffer, 0, buffer.Length);
//                    val.Add(ConverterHelper.ByteToInt(new byte[] { buffer[1], buffer[2], buffer[3], buffer[4] }));
//                    //val.Add(ConverterHelper.ByteToInt(new byte[] { buffer[5], buffer[6], buffer[7], buffer[8] }));
//                    val.Add(ConverterHelper.ByteToInt(new byte[] { buffer[9], buffer[10], buffer[11], buffer[12] }));
//                    val.Add(ConverterHelper.ByteToInt(new byte[] { buffer[13], buffer[14], buffer[15], buffer[16] }));
//                }
//                catch (Exception ex)
//                {
//                    var gg = ex;
//                    val = null;
//                }
            
//            return val;
//        }
//        //Прочитать мгновенную полную мощность по сумме фаз для счетчика с сетевым адресом
//        //128 (используем запрос с номером 14h).
//        //Запрос: 80 08 14 08 (CRC)
//        //Ответ: 80 00 40 E7 29 00 40 E7 29 00 00 00 00 00 00 00 00 (CRC)
//        public static List<decimal> P(TcpClient client, int id)
//        {
//            var val = new List<decimal>();
            
//                try
//                {
//                    var vt = new byte[] { (byte)id, 0x08, 0x14, 0x08 };
//                    AddCrc(ref vt);
//                    client.Client.Send(vt);
//                    //Get the network stream
//                    NetworkStream stream = client.GetStream();
//                    byte[] buffer = new byte[50];
//                    int bytesRead = stream.Read(buffer, 0, buffer.Length);
//                    var v = ConverterHelper.ByteToInt(new byte[] {buffer[1], buffer[2], buffer[3], buffer[4]});
//                    val.Add(v > 0 ? v / 100.0m : 0);
//                    v = ConverterHelper.ByteToInt(new byte[] {buffer[5], buffer[6], buffer[7], buffer[8]});
//                    val.Add(v > 0 ? v / 100.0m : 0);
//                    v = ConverterHelper.ByteToInt(new byte[] {buffer[9], buffer[10], buffer[11], buffer[12]});
//                    val.Add(v > 0 ? v / 100.0m : 0);
//                    v = ConverterHelper.ByteToInt(new byte[] {buffer[13], buffer[14], buffer[15], buffer[16]});
//                    val.Add(v > 0 ? v / 100.0m : 0);
//                }
//                catch (Exception ex)
//                {
//                    var gg = ex;
//                    val = null;
//                }
            
//            return val;
//        }
//        //Прочитать частоту сети для счетчика с сетевым адресом 128 (используем запрос с номе-
//        //ром 11h).
//        //Запрос: 80 08 11 40 (CRC)
//        //Ответ: 80 00 87 13(CRC)
//        public static decimal Fq(TcpClient client, int id)
//        {
//            var val = 0m;
            
//                try
//                {
//                    var vt = new byte[] { (byte)id, 0x08, 0x11, 0x40 };
//                    AddCrc(ref vt);
//                    client.Client.Send(vt);
//                    //Get the network stream
//                    NetworkStream stream = client.GetStream();
//                    byte[] buffer = new byte[50];
//                    int bytesRead = stream.Read(buffer, 0, buffer.Length);
//                    var v = ConverterHelper.ByteToInt(new byte[] { 0x00, buffer[1], buffer[2], buffer[3] });
//                    val = v > 0 ? v / 100.0m : 0;
//                }
//                catch (Exception ex)
//                {
//                    var gg = ex;
//                }
            
//            return val;
//        }
//        private static void AddCrc(ref byte[] arr)
//        {
//            var crc = Crc.ComputeCrc(arr);

//            var a = arr.ToList();
//            a.AddRange(crc);
//            //var bs = crc.ComputeHash(a.ToArray());
//            //a.Add(bs[0]);
//            //a.Add(bs[1]);
//            arr = a.ToArray();
//        }
        
//    }
//}
