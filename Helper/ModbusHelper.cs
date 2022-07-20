using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using Modbus.Device;
using OutReader.Model;
using OutReader.Properties;

namespace OutReader.Helper
{
    public static class ModbusHelper
    {
        public static string IP = Config.Default.ServerIp;
        public static int BRICK_PORT = 5128;
        public static int STREAM_LUX_PORT = 5130;

        public static Tn ReadTn(ModbusIpMaster master, byte slaveId)
        {
            Tn tn = null;
            try
            {
                ushort[] registers = master.ReadInputRegisters(slaveId, 0, 12);
                var res = registers.Select(x => BitConverter.GetBytes(x)).ToList();
                tn = new Tn(master.ReadInputs(slaveId, 0, 6).ToList(), res);
            }
            catch (Exception ex) { }
            return tn;
        }
        public static M ReadM(ModbusIpMaster master, byte slaveId)
        {
            M m = null;
            try
            {	
                ushort[] registers = master.ReadInputRegisters(slaveId, 0, 8);
                var res = registers.Select(x => BitConverter.GetBytes(x)).ToList();
                m = new M(master.ReadInputs(slaveId, 0, 8).ToList(), res);
            }
            catch (Exception ex)
            {
                var fsd = ex;
                var gg = fsd;
            }
            return m;
        }

        public static MB16D ReadM16D(TcpClient client, int modbusId=2)
        {
            MB16D mb = null;
            try
            {
                var vt = new byte[] { (byte)modbusId, 0x03, 0x00, 0x33, 0x00, 0x01, 0, 0 };
                var crc = BitConverter.GetBytes(ModRTU_CRC(vt, 6));
                vt[6] = crc[0];
                vt[7] = crc[1];
                //Sending the byte array to the server
                client.Client.Send(vt);
                //Get the network stream
                NetworkStream stream = client.GetStream();
                byte[] buffer = new byte[8];
                int bytesRead = stream.Read(buffer, 0, buffer.Length);
                var res = BitConverter.ToInt16(
                        new byte[]
                        {
                            buffer[4], buffer[3]
                        }, 0);

                vt = new byte[] { (byte)modbusId, 0x03, 0x00, 0x40, 0x00, 0x02, 0, 0 };
                crc = BitConverter.GetBytes(ModRTU_CRC(vt, 6));
                vt[6] = crc[0];
                vt[7] = crc[1];
                //Sending the byte array to the server
                client.Client.Send(vt);
                //Get the network stream
                stream = client.GetStream();
                buffer = new byte[20];
                bytesRead = stream.Read(buffer, 0, buffer.Length);
                var counter1 = BitConverter.ToInt16(
                    new byte[]
                    {
                        buffer[4], buffer[3]
                    }, 0);
                var counter2 = BitConverter.ToInt16(
                    new byte[]
                    {
                        buffer[6], buffer[5]
                    }, 0);
                mb = new MB16D(res, counter1, counter2);
             }
            catch (Exception ex) {
                var zz = ex;
            }
            return mb;
        }
        public static OBEH ReadOBEH(TcpClient client, int modbusId = 1)
        {
            OBEH mb = null;
            try
            {
                var vt = new byte[] { (byte)modbusId, 0x03, 0x00, 0x00, 0x00, 0x01, 0, 0 };
                var crc = BitConverter.GetBytes(ModRTU_CRC(vt, 6));
                vt[6] = crc[0];
                vt[7] = crc[1];
                //Sending the byte array to the server
                client.Client.Send(vt);
                //Get the network stream
                NetworkStream stream = client.GetStream();
                byte[] buffer = new byte[8];
                int bytesRead = stream.Read(buffer, 0, buffer.Length);
                var res = BitConverter.ToInt16(
                        new byte[]
                        {
                            buffer[4], buffer[3]
                        }, 0);

                mb = new OBEH(res);
            }
            catch (Exception ex)
            {
                var zz = ex;
            }
            return mb;
        }
        public static OBEH_2 ReadOBEH_2(TcpClient client, int modbusId = 1)
        {
            OBEH_2 mb = null;
            try
            {
                var vt = new byte[] { (byte)modbusId, 0x03, 0x00, 0x01, 0x00, 0x01, 0, 0 };
                var crc = BitConverter.GetBytes(ModRTU_CRC(vt, 6));
                vt[6] = crc[0];
                vt[7] = crc[1];
                //Sending the byte array to the server
                client.Client.Send(vt);
                //Get the network stream
                NetworkStream stream = client.GetStream();
                byte[] buffer = new byte[8];
                int bytesRead = stream.Read(buffer, 0, buffer.Length);
                var res = BitConverter.ToInt16(
                        new byte[]
                        {
                            buffer[4], buffer[3]
                        }, 0);

                mb = new OBEH_2(res);
            }
            catch (Exception ex)
            {
                var zz = ex;
            }
            return mb;
        }
        public static OBEH_3 ReadOBEH_3(TcpClient client, int modbusId = 1)
        {
            OBEH_3 mb = null;
            try
            {
                var vt = new byte[] { (byte)modbusId, 0x03, 0x00, 0x02, 0x00, 0x01, 0, 0 };
                var crc = BitConverter.GetBytes(ModRTU_CRC(vt, 6));
                vt[6] = crc[0];
                vt[7] = crc[1];
                //Sending the byte array to the server
                client.Client.Send(vt);
                //Get the network stream
                NetworkStream stream = client.GetStream();
                byte[] buffer = new byte[8];
                int bytesRead = stream.Read(buffer, 0, buffer.Length);
                var res = BitConverter.ToInt16(
                        new byte[]
                        {
                            buffer[4], buffer[3]
                        }, 0);

                mb = new OBEH_3(res);
            }
            catch (Exception ex)
            {
                var zz = ex;
            }
            return mb;
        }
        public static OBEH_VRU ReadOBEH_VRU(TcpClient client, int modbusId = 1)
        {
            OBEH_VRU mb = null;
            try
            {
                var vt = new byte[] { (byte)modbusId, 0x03, 0x00, 0x08, 0x00, 0x01, 0, 0 };
                var crc = BitConverter.GetBytes(ModRTU_CRC(vt, 6));
                vt[6] = crc[0];
                vt[7] = crc[1];
                //Sending the byte array to the server
                client.Client.Send(vt);
                //Get the network stream
                NetworkStream stream = client.GetStream();
                byte[] buffer = new byte[8];
                int bytesRead = stream.Read(buffer, 0, buffer.Length);
                var res = BitConverter.ToInt16(
                        new byte[]
                        {
                            buffer[4], buffer[3]
                        }, 0);

                mb = new OBEH_VRU(res);
            }
            catch (Exception ex)
            {
                var zz = ex;
            }
            return mb;
        }
        public static OBEH_Alarm ReadOBEH_Alarm(TcpClient client, int modbusId = 1)
        {
            OBEH_Alarm mb = null;
            try
            {
                var vt = new byte[] { (byte)modbusId, 0x03, 0x00, 0x09, 0x00, 0x01, 0, 0 };
                var crc = BitConverter.GetBytes(ModRTU_CRC(vt, 6));
                vt[6] = crc[0];
                vt[7] = crc[1];
                //Sending the byte array to the server
                client.Client.Send(vt);
                //Get the network stream
                NetworkStream stream = client.GetStream();
                byte[] buffer = new byte[8];
                int bytesRead = stream.Read(buffer, 0, buffer.Length);
                var res = BitConverter.ToInt16(
                        new byte[]
                        {
                            buffer[4], buffer[3]
                        }, 0);

                mb = new OBEH_Alarm(res);
            }
            catch (Exception ex)
            {
                var zz = ex;
            }
            return mb;
        }
        public static OBEH_level ReadOBEH_level(TcpClient client, int modbusId = 1)
        {
            OBEH_level mb = null;
            try
            {
                var vt = new byte[] { (byte)modbusId, 0x03, 0x00, 0x07, 0x00, 0x01, 0, 0 };
                var crc = BitConverter.GetBytes(ModRTU_CRC(vt, 6));
                vt[6] = crc[0];
                vt[7] = crc[1];
                //Sending the byte array to the server
                client.Client.Send(vt);
                //Get the network stream
                NetworkStream stream = client.GetStream();
                byte[] buffer = new byte[8];
                int bytesRead = stream.Read(buffer, 0, buffer.Length);
                var res = BitConverter.ToInt16(
                        new byte[]
                        {
                            buffer[4], buffer[3]
                        }, 0);

                mb = new OBEH_level(res);
            }
            catch (Exception ex)
            {
                var zz = ex;
            }
            return mb;
        }
        public static decimal ReadMass(TcpClient client)
        {
            try
            {
                //var vt = new byte[] { 0x45 };
                    
                ////Sending the byte array to the server
                //client.Client.Send(vt);
                ////Get the network stream
                //NetworkStream stream = client.GetStream();
                //byte[] buffer = new byte[8];
                //int bytesRead = stream.Read(buffer, 0, buffer.Length);
                //var res = BitConverter.ToInt16(
                //    new byte[]
                //    {
                //        buffer[0], buffer[1]
                //    }, 0);

                var vt = new byte[] { 0xF8, 0x55, 0xCE, 0x01, 0x00, 0x23, 0x23, 0x00 };

                //Sending the byte array to the server
                client.Client.Send(vt);
                //Get the network stream
                NetworkStream stream = client.GetStream();
                byte[] buffer = new byte[22];
                int bytesRead = stream.Read(buffer, 0, buffer.Length);
                var mass1 = BitConverter.ToInt32(
                    new byte[]
                    {
                        buffer[6], buffer[7], buffer[8], buffer[9]
                    }, 0);

                var mass2 = BitConverter.ToInt32(
                    new byte[]
                    {
                        buffer[14], buffer[15], buffer[16], buffer[17]
                    }, 0);

                return (mass1 + mass2)*0.01m;
            }
            catch (Exception ex)
            {
                var zz = ex;
            }

            return -1;
        }

        public static bool SetMY8ForMass(TcpClient client, int modbusId, byte value=0)
        {
            try
            {
                var vt = new byte[] { (byte)modbusId, 0x10, 0x00, 0x32, 0x00, 0x01, 0x02, 0, value, 0, 0 };
                var crc = BitConverter.GetBytes(ModRTU_CRC(vt, 9));
                vt[9] = crc[0];
                vt[10] = crc[1];

                //Sending the byte array to the server
                client.Client.Send(vt);
                return true;
            }
            catch (Exception ex)
            {
                var zz = ex;
            }

            return false;
        }

        public static MB8A ReadM8A(TcpClient client, int modbusId = 3)
        {
            MB8A mb = null;
            try
            {
                mb = new MB8A();
                mb.A1 = MBAnalog(client, modbusId, new byte[] {0, 4});
                mb.A2 = MBAnalog(client, modbusId, new byte[] { 0, 10 });
                mb.A3 = MBAnalog(client, modbusId, new byte[] { 0, 16 });
                mb.A4 = MBAnalog(client, modbusId, new byte[] { 0, 22 });
                mb.A5 = MBAnalog(client, modbusId, new byte[] { 0, 28 });
                mb.A6 = MBAnalog(client, modbusId, new byte[] { 0, 34 });
                mb.A7 = MBAnalog(client, modbusId, new byte[] { 0, 40 });
                mb.A8 = MBAnalog(client, modbusId, new byte[] { 0, 46 });
            }
            catch (Exception ex)
            {
                var zz = ex;
            }
            return mb;
        }
        public static MB8A_OBEH ReadMB8A_OBEH(TcpClient client, int modbusId = 1)
        {
            MB8A_OBEH mb = null;
            try
            {
                mb = new MB8A_OBEH();
                mb.A1 = MBAnalog(client, modbusId, new byte[] { 0, 4 });
                mb.A2 = MBAnalog(client, modbusId, new byte[] { 0, 10 });
                mb.A3 = MBAnalog(client, modbusId, new byte[] { 0, 16 });
                mb.A4 = MBAnalog(client, modbusId, new byte[] { 0, 22 });
                mb.A5 = MBAnalog(client, modbusId, new byte[] { 0, 28 });
                mb.A6 = MBAnalog(client, modbusId, new byte[] { 0, 34 });
                mb.A7 = MBAnalog(client, modbusId, new byte[] { 0, 40 });
                mb.A8 = MBAnalog(client, modbusId, new byte[] { 0, 46 });
            }
            catch (Exception ex)
            {
                var zz = ex;
            }
            return mb;
        }
        public static TER ReadTER(TcpClient client, int modbusId = 10)
        {
            TER ter = null;
            try
            {
                
                    // Q 0x801E текущий расход л/мин
                    var vt = new byte[] { (byte)modbusId, 0x04, 0x80, 0x1E, 0x00, 0x02, 0, 0 };
                    var crc = BitConverter.GetBytes(ModRTU_CRC(vt, 6));
                    vt[6] = crc[0];
                    vt[7] = crc[1];
                    //Sending the byte array to the server
                    client.Client.Send(vt);
                    //Get the network stream
                    NetworkStream stream = client.GetStream();
                    byte[] buffer = new byte[12];
                    int bytesRead = stream.Read(buffer, 0, buffer.Length);
                    var q = BitConverter.ToSingle(
                        new byte[]
                        {
                            buffer[6], buffer[5], buffer[4], buffer[3]
                        }, 0);
                
                    //V целая часть 0x8020
                    vt = new byte[] { (byte)modbusId, 0x04, 0x80, 0x20, 0x00, 0x02, 0, 0 };
                    crc = BitConverter.GetBytes(ModRTU_CRC(vt, 6));
                    vt[6] = crc[0];
                    vt[7] = crc[1];
                    //Sending the byte array to the server
                    client.Client.Send(vt);
                    //Get the network stream
                    stream = client.GetStream();
                    buffer = new byte[12];
                    bytesRead = stream.Read(buffer, 0, buffer.Length);
                    var v = BitConverter.ToInt32(
                        new byte[]
                        {
                            buffer[6], buffer[5], buffer[4], buffer[3]
                        }, 0);
                    //V дробная часть 0x8022


                    ter = new TER((decimal)((q > -1 ? q : 0) * 0.06), (decimal)(v > -1 ? v : 0));
            }
            catch (Exception ex)
            {
                var zz = ex;
            }
            return ter;
        }

        public static TER ReadMR(TcpClient client, int modbusId = 1)
        {
            TER ter = null;
            try
            {


                // Q 0x801E текущий расход л/мин
                var vt = new byte[] { (byte)modbusId, 0x04, 0x81, 0x5A, 0x00, 0x02, 0, 0 };
                var crc = BitConverter.GetBytes(ModRTU_CRC(vt, 6));
                vt[6] = crc[0];
                vt[7] = crc[1];
                //Sending the byte array to the server
                client.Client.Send(vt);
                //Get the network stream
                NetworkStream stream = client.GetStream();
                byte[] buffer = new byte[12];
                int bytesRead = stream.Read(buffer, 0, buffer.Length);
                var q = BitConverter.ToSingle(
                    new byte[]
                        {
                            buffer[6], buffer[5], buffer[4], buffer[3]
                        }, 0);

                //V целая часть 0x8020
                vt = new byte[] { (byte)modbusId, 0x04, 0x80, 0x22, 0x00, 0x04, 0, 0 };
                crc = BitConverter.GetBytes(ModRTU_CRC(vt, 6));
                vt[6] = crc[0];
                vt[7] = crc[1];
                //Sending the byte array to the server
                client.Client.Send(vt);
                //Get the network stream
                stream = client.GetStream();
                buffer = new byte[15];
                bytesRead = stream.Read(buffer, 0, buffer.Length);
                var v = BitConverter.ToInt32(
                    new byte[]
                        {
                            buffer[6], buffer[5], buffer[4], buffer[3]
                        }, 0);
                //V дробная часть 0x8022


                ter = new TER((decimal)((q > -1 ? q : 0) * 0.06), (decimal)(v > -1 ? v : 0));
            }
            catch (Exception ex)
            {
                var zz = ex;
            }
            return ter;
        }

        private static decimal MBAnalog(TcpClient client, int modbusId, byte[] address)
        {
            try
            {
                var vt = new byte[] {(byte) modbusId, 0x03, address[0], address[1], 0x00, 0x02, 0, 0};
                var crc = BitConverter.GetBytes(ModRTU_CRC(vt, 6));
                vt[6] = crc[0];
                vt[7] = crc[1];
                //Sending the byte array to the server
                client.Client.Send(vt);
                //Get the network stream
                NetworkStream stream = client.GetStream();
                byte[] buffer = new byte[12];
                int bytesRead = stream.Read(buffer, 0, buffer.Length);
                var ai = BitConverter.ToSingle(new byte[]
                {
                    buffer[6], buffer[5], buffer[4], buffer[3]
                }, 0);
                if (ai < -1) ai = 0;
                return Decimal.Round(Convert.ToDecimal(ai),2);
            }
            catch (Exception ex)
            {
                return -1;
            }
        }

        public static PR200 ReadPR200(TcpClient client, int modbusId)
        {
            PR200 pr = null;
            try
            {
                //DI1-DI8 Входы 0x0100
                var vt = new byte[] { (byte)modbusId, 0x03, 0x01, 0, 0, 0x01, 0, 0 };
                var crc = BitConverter.GetBytes(ModRTU_CRC(vt, 6));
                vt[6] = crc[0];
                vt[7] = crc[1];
                //Sending the byte array to the server
                client.Client.Send(vt);
                //Get the network stream
                NetworkStream stream = client.GetStream();
                byte[] buffer = new byte[12];
                int bytesRead = stream.Read(buffer, 0, buffer.Length);
                var di = BitConverter.ToInt16(new byte[]
                {
                    buffer[4], buffer[3]
                }, 0);
                //Q1-Q8
                vt = new byte[] { (byte)modbusId, 0x03, 0, 0, 0, 0x01, 0, 0 };
                crc = BitConverter.GetBytes(ModRTU_CRC(vt, 6));
                vt[6] = crc[0];
                vt[7] = crc[1];
                //Sending the byte array to the server
                client.Client.Send(vt);
                //Get the network stream
                stream = client.GetStream();
                buffer = new byte[12];
                bytesRead = stream.Read(buffer, 0, buffer.Length);
                var q = BitConverter.ToInt16(new byte[]
                {
                    buffer[4], buffer[3]
                }, 0);
                //AI1-AI4
                vt = new byte[] { (byte)modbusId, 0x03, 0x0b, 0, 0, 0x08, 0, 0 };
                crc = BitConverter.GetBytes(ModRTU_CRC(vt, 6));
                vt[6] = crc[0];
                vt[7] = crc[1];
                //Sending the byte array to the server
                client.Client.Send(vt);
                //Get the network stream
                stream = client.GetStream();
                buffer = new byte[12];
                bytesRead = stream.Read(buffer, 0, buffer.Length);
                var ai1 = BitConverter.ToSingle(new byte[]
                {
                    buffer[6], buffer[5], buffer[4], buffer[3]
                }, 0);
                var ai2 = BitConverter.ToSingle(new byte[]
                {
                    buffer[10], buffer[9], buffer[8], buffer[7]
                }, 0);
                var ai3 = BitConverter.ToSingle(new byte[]
                {
                    buffer[10], buffer[9], buffer[8], buffer[7]
                }, 0);
                var ai4 = BitConverter.ToSingle(new byte[]
                {
                    buffer[10], buffer[9], buffer[8], buffer[7]
                }, 0);
                pr = new PR200(di, q, (decimal)ai1, (decimal)ai2, (decimal)ai3, (decimal)ai4);
            }
            catch (Exception ex)
            {
            }
            return pr;
        }

        public static double TRM200(TcpClient client, int modbusId, double flowWater)
        {
            try
            {
                var vt = new byte[] { (byte)modbusId, 0x03, 0, 1, 0, 1, 0, 0 };
                var crc = BitConverter.GetBytes(ModRTU_CRC(vt, 6));
                vt[6] = crc[0];
                vt[7] = crc[1];
                //Sending the byte array to the server
                client.Client.Send(vt);
                //Get the network stream
                NetworkStream stream = client.GetStream();
                byte[] buffer = new byte[12];
                int bytesRead = stream.Read(buffer, 0, buffer.Length);
                var level = BitConverter.ToInt16(new byte[]
                {
                    buffer[4], buffer[3]
                }, 0);

                if (flowWater > 0)
                {
                    byte[] v = BitConverter.GetBytes((int) flowWater);
                    vt = new byte[] {(byte) modbusId, 0x10, 0x02, 0x0E, 0, 1, 2, v[1], v[0], 0, 0};
                    crc = BitConverter.GetBytes(ModRTU_CRC(vt, 9));
                    vt[9] = crc[0];
                    vt[10] = crc[1];
                    //Sending the byte array to the server
                    client.Client.Send(vt);
                    ////Get the network stream
                    stream = client.GetStream();
                    buffer = new byte[15];
                    bytesRead = stream.Read(buffer, 0, buffer.Length);
                }

                return level;
            }
            catch (Exception ex)
            {
                
            }

            return -1;
        }

        public static ROC ReadROC(TcpClient client, int modbusId)
        {
            ROC roc= null;
            try
            {
                //DI1-DI8 Входы
                var vt = new byte[] { (byte)modbusId, 0x04, 0x00, 0, 0, 0x09, 0, 0 };
                var crc = BitConverter.GetBytes(ModRTU_CRC(vt, 6));
                vt[6] = crc[0];
                vt[7] = crc[1];
                //Sending the byte array to the server
                client.Client.Send(vt);
                //Get the network stream
                NetworkStream stream = client.GetStream();
                byte[] buffer = new byte[26];
                int bytesRead = stream.Read(buffer, 0, buffer.Length);
                var s1 = BitConverter.ToSingle(new byte[]
                {
                    buffer[6], buffer[5], buffer[4], buffer[3]
                }, 0);
                var n1 = BitConverter.ToInt16(new byte[]
                {
                    buffer[8], buffer[7]
                }, 0);
                var s2 = BitConverter.ToSingle(new byte[]
                {
                    buffer[12], buffer[11], buffer[10], buffer[9]
                }, 0);
                var n2 = BitConverter.ToInt16(new byte[]
                {
                    buffer[14], buffer[13]
                }, 0);
                var t = BitConverter.ToSingle(new byte[]
                {
                    buffer[18], buffer[17], buffer[16], buffer[15]
                }, 0);
                var n3 = BitConverter.ToInt16(new byte[]
                {
                    buffer[20], buffer[19]
                }, 0);
                var di = BitConverter.ToInt16(new byte[]
                {
                    buffer[22], buffer[21]
                }, 0);
                //Q1-Q8
                var q = BitConverter.ToInt16(new byte[]
                {
                    buffer[24], buffer[23]
                }, 0);

                roc = new ROC(di, q, (decimal)s1, (decimal)s2, (decimal)t);
            }
            catch (Exception ex)
            {
            }
            return roc;
        }

        public static SI30 ReadSI30(TcpClient client, int modbusId)
        {
            SI30 si = null;
            try
            {
                var vt = new byte[] { (byte)modbusId, 0x04, 0x00, 0x02, 0, 0x04, 0, 0 };
                var crc = BitConverter.GetBytes(ModRTU_CRC(vt, 6));
                vt[6] = crc[0];
                vt[7] = crc[1];
                //Sending the byte array to the server
                client.Client.Send(vt);
                //Get the network stream
                NetworkStream stream = client.GetStream();
                byte[] buffer = new byte[13];
                int bytesRead = stream.Read(buffer, 0, buffer.Length);
                var v = BitConverter.ToInt32(new byte[]
                {
                    buffer[6], buffer[5], buffer[4], buffer[3]
                }, 0);

                //ushort[] registers = master.ReadInputRegisters(slaveId, 0, 8);
                //var res = registers.Select(x => BitConverter.GetBytes(x)).ToList();
                //m = new M(master.ReadInputs(slaveId, 0, 8).ToList(), res);

                si = new SI30((decimal)v);
            }
            catch (Exception ex)
            {
            }
            return si;
        }

        public static M ReadM(TcpClient client, byte slaveId)
        {
            List<byte[]> bytes = new List<byte[]>();
            try
            {
                var a = BitConverter.GetBytes(0);
                var c = BitConverter.GetBytes(8);
                var vt = new byte[] { slaveId, 4, a[1], a[0], c[1], c[0], 0, 0 };
                var crc = BitConverter.GetBytes(ModRTU_CRC(vt, 6));
                vt[6] = crc[0];
                vt[7] = crc[1];
                client.Client.Send(vt);
                NetworkStream stream = client.GetStream();
                stream.WriteTimeout = 15000; //  <------- 15 second timeout
                stream.ReadTimeout = 15000;
                var buffer = new byte[22];
                int bytesRead = stream.Read(buffer, 0, buffer.Length);
                for (var i = 3; i < buffer.Length - 2; i += 2)
                    bytes.Add(new byte[] { buffer[i + 1], buffer[i] });
                
                var m = new M(bytes);

                vt = new byte[] { slaveId, 2, a[1], a[0], c[1], c[0], 0, 0 };
                crc = BitConverter.GetBytes(ModRTU_CRC(vt, 6));
                vt[6] = crc[0];
                vt[7] = crc[1];
                client.Client.Send(vt);
                stream = client.GetStream();
                stream.WriteTimeout = 15000; //  <------- 15 second timeout
                stream.ReadTimeout = 15000;
                buffer = new byte[8];
                bytesRead = stream.Read(buffer, 0, buffer.Length);
                var di = ConverterHelper.ByteToBools(new byte[]{buffer[2], buffer[3]});
                m.DI = di;
                return m;
            }
            catch (Exception ex)
            {
                var zz = ex;
            }
            return null;
        }

        public static Tn ReadTn(TcpClient client, byte slaveId)
        {
            List<byte[]> bytes = new List<byte[]>();
            try
            {
                
                var vt = new byte[] { slaveId, 2, 0, 0, 0, 6, 0, 0 };
                var crc = BitConverter.GetBytes(ModRTU_CRC(vt, 6));
                vt[6] = crc[0];
                vt[7] = crc[1];
                client.Client.Send(vt);
                var stream = client.GetStream();
                stream.WriteTimeout = 15000; //  <------- 15 second timeout
                stream.ReadTimeout = 15000;
                var buffer = new byte[22];
                var bytesRead = stream.Read(buffer, 0, buffer.Length);
                var di = ConverterHelper.ByteToBools(new byte[] { buffer[2], buffer[3] });

                var a = BitConverter.GetBytes(0);
                var c = BitConverter.GetBytes(12);
                vt = new byte[] { slaveId, 4, a[1], a[0], c[1], c[0], 0, 0 };
                crc = BitConverter.GetBytes(ModRTU_CRC(vt, 6));
                vt[6] = crc[0];
                vt[7] = crc[1];
                client.Client.Send(vt);
                stream = client.GetStream();
                stream.WriteTimeout = 15000; //  <------- 15 second timeout
                stream.ReadTimeout = 15000;
                buffer = new byte[30];
                bytesRead = stream.Read(buffer, 0, buffer.Length);
                for (var i = 3; i < buffer.Length - 2; i += 2)
                    bytes.Add(new byte[] { buffer[i + 1], buffer[i] });
                var m = new Tn(di, bytes);
                return m;
            }
            catch (Exception ex)
            {
                var zz = ex;
            }
            return null;
        }
        public static List<byte[]> Read(TcpClient client, byte id, byte function, ushort address, ushort count)
        {
            List<byte[]> bytes = new List<byte[]>();
            try
            {
                var a = BitConverter.GetBytes(address);
                var c = BitConverter.GetBytes(count);
                var vt = new byte[] { id, function, a[1], a[0], c[1], c[0], 0, 0 };
                var crc = BitConverter.GetBytes(ModRTU_CRC(vt, 6));
                vt[6] = crc[0];
                vt[7] = crc[1];
                client.Client.Send(vt);
                NetworkStream stream = client.GetStream();
                stream.WriteTimeout = 15000; //  <------- 15 second timeout
                stream.ReadTimeout = 15000;
                var buffer = new byte[6+count*2];
                int bytesRead = stream.Read(buffer, 0, buffer.Length);
                for (var i = 3; i < buffer.Length-2; i+=2)
                    bytes.Add(new byte[] { buffer[i + 1], buffer[i] });
            }
            catch (Exception ex)
            {
                var zz = ex;
            }
            return bytes;
        }


        public static Pulsar ReadPulsar(int port, bool brick=false)
        {
            Pulsar pulsar = null;
            using (TcpClient client = new TcpClient()){
                client.SendTimeout = 10000;
                client.ReceiveTimeout = 10000;
                try
                {
                    //Connect to the server
                    client.Connect(IP, port);
                    var vt = new byte[]
                        {0x00, 0x46, 0x18, 0x54, 0x01, 0x0E, 0x03, 0x00, 0x00, 0x00, 0xA4, 0xD5, 0xF4, 0x97};
                        //{0xF0, 0x0F, 0x0F, 0xF0, 0x00, 0x00, 0x00, 0x00, 0x00, 0xA5, 0x44};
                    if(brick)
                        vt = new byte[] { 0x00, 0x46, 0x18, 0x57, 0x01, 0x0E, 0x03, 0x00, 0x00, 0x00, 0x4E, 0xD4, 0x6E, 0xC7 };
                    //Sending the byte array to the server
                    client.Client.Send(vt);
                    //Get the network stream
                    NetworkStream stream = client.GetStream();
                    byte[] buffer = new byte[50];
                    int bytesRead = stream.Read(buffer, 0, buffer.Length);
                    var flow0 = BitConverter.ToDouble(
                            new byte[]
                            {
                                buffer[0], buffer[1], buffer[2], buffer[3], buffer[4], buffer[5], buffer[6],
                                buffer[7]
                            }, 0);
                    var flow1 = BitConverter.ToDouble(
                            new byte[]
                            {
                                buffer[6], buffer[7], buffer[8], buffer[9], buffer[10], buffer[11], buffer[12],
                                buffer[13]
                            }, 0);
                    var flow2 = BitConverter.ToDouble(
                            new byte[]
                            {
                                buffer[14], buffer[15], buffer[16], buffer[17], buffer[18], buffer[19], buffer[20],
                                buffer[21]
                            }, 0);
                    pulsar = new Pulsar() {Flow1 = flow1, Flow2 = flow2};
                }
                catch (Exception ex)
                {
                    var gg = ex;
                    var hj = gg;
                }
            }
            return pulsar;
        }

        public static Apb ReadApb(ModbusIpMaster master, byte slaveId=1)
        {
            var apb = new Apb();
            try
            {
                ushort[] registers = master.ReadHoldingRegisters(slaveId, 18460, 10);
                var states = registers.Select(x => BitConverter.GetBytes(x)).ToList();
                registers = master.ReadHoldingRegisters(slaveId, 18630, 4);
                var flows = registers.Select(x => BitConverter.GetBytes(x)).ToList();
                var registers2 = master.ReadHoldingRegisters(slaveId, 18632, 2);
                var fl2 = registers.Select(x => BitConverter.GetBytes(x)).ToList();
                apb = new Apb(states, flows);
                //apb.FlowWaterWashing2 = ConverterHelper.ByteToReal(fl2[0], fl2[1]);
                
            }
            catch (Exception ex) { }
            return apb;
        }

        public static Apb ReadApbAll(ModbusIpMaster master, byte slaveId = 1)
        {
            Apb apb = null;
            try
            {
                var d = new List<byte[]>();
                var i = master.ReadCoils(slaveId, 256, 22).ToList();
                var q = master.ReadCoils(slaveId, 512, 20).ToList();
                var m = master.ReadCoils(slaveId, 9728, 100).ToList();
                //DW0(18432)...DW16
                var dws = master.ReadHoldingRegisters(slaveId, 18460, 6).Select(x => BitConverter.GetBytes(x)).ToList();
                //DW102..113 
                var dws2 = master.ReadHoldingRegisters(slaveId, 18634, 22).Select(x => BitConverter.GetBytes(x))
                    .ToList();
                //DW191.... DW210
                var dws4 = master.ReadHoldingRegisters(slaveId, 18812, 42).Select(x => BitConverter.GetBytes(x))
                    .ToList();
                m.RemoveRange(22, 75);
                m.RemoveRange(0, 5);
                d.AddRange(dws);
                d.AddRange(dws2);
                d.AddRange(dws4);
                apb = new Apb(i, q, m, d);
            }
            catch (Exception ex)
            {

            }
            return apb;
        }

        public static Vzlet ReadVzlet(string nameCOMPort, byte slaveId = 7)
        {
            //var v = new Vzlet();
            //try
            //{
            //    using (ModbusSerialMaster master = ModbusSerialMaster.CreateRtu(new SerialPort(nameCOMPort)))
            //    {
            //        //var r1 = master.ReadInputRegisters(slaveId, 49179, 2); //1460
            //        //var q1 = r1.Select(x => BitConverter.GetBytes(x)).ToList();
            //        //r1 = master.ReadInputRegisters(slaveId, 32823, 2); //1470
            //        //var v1 = r1.Select(x => BitConverter.GetBytes(x)).ToList();
            //        //r1 = master.ReadInputRegisters(slaveId, 49209, 2); //1500
            //        //var q2 = r1.Select(x => BitConverter.GetBytes(x)).ToList();
            //        //r1 = master.ReadInputRegisters(slaveId, 32855, 2); //1510
            //        //var v2 = r1.Select(x => BitConverter.GetBytes(x)).ToList();
            //        //v = new Vzlet(q1,q2,v1,v2);
            //        //v.FlowWaterWashing2 = ConverterHelper.ByteToReal(fl2[0], fl2[1]);
            //    }
            //}
            //catch (Exception ex) { }
            //return v;
            return null;
        }

        public static void Ascii()
        {
            using (TcpClient client = new TcpClient(ModbusHelper.IP, 5130))
                {
                    client.SendTimeout = 4000;
                    client.ReceiveTimeout = 8000;
                    var master = ModbusSerialMaster.CreateAscii(client);

                    //master.
                }
            //using (SerialPort port = new SerialPort("COM1"))
            //{
            //    // configure serial port
            //    port.BaudRate = 9600;
            //    port.DataBits = 8;
            //    port.Parity = Parity.None;
            //    port.StopBits = StopBits.One;
            //    port.Open();

            //    var adapter = new SerialPortAdapter(port);

            //    var factory = new ModbusFactory();

            //    // create modbus master
            //    IModbusSerialMaster master = factory.CreateAsciiMaster(adapter);

            //    byte slaveId = 1;
            //    ushort startAddress = 1;
            //    ushort numRegisters = 5;

            //    // read five registers		
            //    ushort[] registers = master.ReadHoldingRegisters(slaveId, startAddress, numRegisters);

            //    for (int i = 0; i < numRegisters; i++)
            //    {
            //        Console.WriteLine($"Register {startAddress + i}={registers[i]}");
            //    }
            //}
        }

        private static ushort ModRTU_CRC(byte[] buf, int len)
        {
            UInt16 crc = 0xFFFF;

            for (int pos = 0; pos < len; pos++)
            {
                crc ^= (UInt16)buf[pos];          // XOR byte into least sig. byte of crc

                for (int i = 8; i != 0; i--)
                {    // Loop over each bit
                    if ((crc & 0x0001) != 0)
                    {      // If the LSB is set
                        crc >>= 1;                    // Shift right and XOR 0xA001
                        crc ^= 0xA001;
                    }
                    else                            // Else LSB is not set
                        crc >>= 1;                    // Just shift right
                }
            }
            // Note, this number has low and high bytes swapped, so use it accordingly (or swap bytes)
            return crc;
        }

        internal static SBI ReadSBI(TcpClient client, int modbusId, string knsId, DateTime dt)
        {
            SBI sbi = null;
            try
            {
                // 1001h Регистр состояния
                var vt = new byte[] { (byte)modbusId, 0x03, 0x10, 0x01, 0x00, 0x01, 0, 0 };
                var crc = BitConverter.GetBytes(ModRTU_CRC(vt, 6));
                vt[6] = crc[0];
                vt[7] = crc[1];
                //Sending the byte array to the server
                client.Client.Send(vt);
                //Get the network stream
                NetworkStream stream = client.GetStream();
                byte[] buffer = new byte[12];
                int bytesRead = stream.Read(buffer, 0, buffer.Length);
                var state = BitConverter.ToInt16(new byte[] { buffer[4], buffer[3] }, 0);

                // 3000h Регистр сетевого напряжения
                vt = new byte[] { (byte)modbusId, 0x03, 0x30, 0x00, 0x00, 0x02, 0, 0 };
                crc = BitConverter.GetBytes(ModRTU_CRC(vt, 6));
                vt[6] = crc[0];
                vt[7] = crc[1];
                //Sending the byte array to the server
                client.Client.Send(vt);
                //Get the network stream
                stream = client.GetStream();
                buffer = new byte[12];
                bytesRead = stream.Read(buffer, 0, buffer.Length);
                var u = BitConverter.ToInt16(new byte[]{buffer[4], buffer[3]}, 0);
                // 3001h Регистр тока нагрузки
                var i= BitConverter.ToInt16(new byte[] { buffer[6], buffer[5] }, 0);

                // 5000h Регистр ошибки
                vt = new byte[] { (byte)modbusId, 0x03, 0x50, 0x00, 0x00, 0x02, 0, 0 };
                crc = BitConverter.GetBytes(ModRTU_CRC(vt, 6));
                vt[6] = crc[0];
                vt[7] = crc[1];
                //Sending the byte array to the server
                client.Client.Send(vt);
                //Get the network stream
                stream = client.GetStream();
                buffer = new byte[12];
                bytesRead = stream.Read(buffer, 0, buffer.Length);
                var e = BitConverter.ToInt16(new byte[] { buffer[4], buffer[3] }, 0);
                // 5001h Регистр ошибки связи
                var e_c = BitConverter.ToInt16(new byte[] { buffer[6], buffer[5] }, 0);
                sbi = new SBI(u, i, state, e.ToString("X"), e_c.ToString("X"), modbusId, knsId, dt);
            }
            catch (Exception ex)
            {
                var zz = ex;
            }
            return sbi;
        }

        public static ME3M ReadME3M(TcpClient client, int modbusId)
            {
            ME3M me = null;
            try
                {
                me = new ME3M();
                me.Au = ReadME(client, modbusId, new byte[] { 0, 80 });
                me.Bu = ReadME(client, modbusId, new byte[] { 0, 82 });
                me.Cu = ReadME(client, modbusId, new byte[] { 0, 84 });
                me.Ai = ReadME(client, modbusId, new byte[] { 0, 86 });
                me.Bi = ReadME(client, modbusId, new byte[] { 0, 88 });
                me.Ci = ReadME(client, modbusId, new byte[] { 0, 90 });
                }
            catch (Exception ex)
                {
                var zz = ex;
                }
            return me;
            }

        private static decimal ReadME(TcpClient client, int modbusId, byte[] address)
            {
            //List<byte[]> bytes = new List<byte[]>();
            try
                {
                var vt = new byte[] { (byte)modbusId, 0x03, address[0], address[1], 0x00, 0x02, 0, 0 };
                var crc = BitConverter.GetBytes(ModRTU_CRC(vt, 6));
                vt[6] = crc[0];
                vt[7] = crc[1];
                client.Client.Send(vt);
                NetworkStream stream = client.GetStream();
                var buffer = new byte[50];
                var bytesRead = stream.Read(buffer, 0, buffer.Length);

                var di = BitConverter.ToSingle(new byte[] { buffer[6], buffer[5], buffer[4], buffer[3] }, 0);

                if (di < -1) di = 0;
                return Decimal.Round(Convert.ToDecimal(di), 2);
                }
            catch (Exception ex)
                {
                return -1;
                }
            }

    }
}
