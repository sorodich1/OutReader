using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using Modbus.Device;

namespace OutReader.Model.LiftWater
{
    internal class ModbusHelper
    {
        public static string IP = "192.168.22.100";
        public static string IPOWEN = "192.168.22.150";
        //public static void ModbusTcpMasterReadInputs()
        //{
        //    using (TcpClient client = new TcpClient(IP, 502))
        //    {
        //        ModbusIpMaster master = ModbusIpMaster.CreateIp(client);

        //        // read five input values
        //        ushort startAddress = 1;
        //        ushort numInputs = 20;
        //        bool[] inputs = master.ReadCoils(startAddress, numInputs);

        //        for (int i = 0; i < numInputs; i++)
        //            Console.WriteLine("Input {0}={1}", startAddress + i, inputs[i] ? 1 : 0);
        //    }

        //    // output: 
        //    // Input 100=0
        //    // Input 101=0
        //    // Input 102=0
        //    // Input 103=0
        //    // Input 104=0
        //}
        public static List<byte[]> ReadRegisters(ushort startAddress = 100, ushort count = 5)
        {
            var res = new List<byte[]>();
            //try
            //{
            using (TcpClient client = new TcpClient(IP, 502))
            {
                ModbusIpMaster master = ModbusIpMaster.CreateIp(client);
                
                // read five registers		
                ushort[] registers = master.ReadHoldingRegisters(1, startAddress, count);
                res = registers.Select(x => BitConverter.GetBytes(x)).ToList();
            }
            //}
            //catch (Exception ex) { }
            return res;
        }
        public static List<byte[]> ReadInputRegisters(ushort startAddress = 0, ushort count = 20)
        {
            var res = new List<byte[]>();
            try
            {
                using (TcpClient client = new TcpClient(IPOWEN, 502))
                {
                    client.SendTimeout = 3000;
                    client.ReceiveTimeout = 3000;
                    ModbusIpMaster master = ModbusIpMaster.CreateIp(client);
                
                    ushort[] registers = master.ReadInputRegisters(100, startAddress, count);
                    res = registers.Select(x => BitConverter.GetBytes(x)).ToList();
                }
            }
            catch (Exception ex) { 
            }
            return res;
        }

        public static List<byte[]> ReadInputRegisters2(ushort startAddress = 0, ushort count = 20)
        {
            var res = new List<byte[]>();
            //try
            //{
            //    using (TcpClient client = new TcpClient(IPOWEN, 502))
            //    {
            //        client.SendTimeout = 30;
            //        client.ReceiveTimeout = 30;
            //        ModbusIpMaster master = ModbusIpMaster.CreateIp(client);
            //        ushort[] registers = master.ReadInputRegisters(100, startAddress, count);
            //        res = registers.Select(x => BitConverter.GetBytes(x)).ToList();
            //    }
            //}
            //catch (Exception ex)
            //{
            //}

            List<byte[]> bytes = new List<byte[]>();
            try
            {
                using (TcpClient client = new TcpClient(IPOWEN, 502))
                {
                    var a = BitConverter.GetBytes(startAddress);
                var c = BitConverter.GetBytes(count);
                var vt = new byte[] { (byte)100, 4, a[1], a[0], c[1], c[0], 0, 0 };
                //var crc = BitConverter.GetBytes(ModRTU_CRC(vt, 6));
                //vt[6] = crc[0];
                //vt[7] = crc[1];
                client.Client.Send(vt);
                NetworkStream stream = client.GetStream();
                stream.WriteTimeout = 15000; //  <------- 15 second timeout
                stream.ReadTimeout = 15000;
                var buffer = new byte[6 + count * 2];
                int bytesRead = stream.Read(buffer, 0, buffer.Length);
                for (var i = 3; i < buffer.Length - 2; i += 2)
                    bytes.Add(new byte[]
                    {
                        buffer[i + 1], buffer[i]
                    });
                }
            }
            catch (Exception ex)
            {
                var zz = ex;
            }
            return res;
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

        /// <summary>
        /// 0-27
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static List<Well> GetWells(List<byte[]> bytes, DateTime dt)
        {
            var wells = new List<Well>();
            var well1 = new Well() { Id = 1, Title = "7_1" };
            var well2 = new Well() { Id = 2, Title = "7_2" };

            //40001-40028
            //var bytes = ReadRegisters(0, 28);
            //if (!bytes.Any()) return null;
            var isDIs = ConverterHelper.ByteToBools(bytes[0]);
            var isPumps = ConverterHelper.ByteToBools(bytes[1]);
            well1.WellDatas = new List<WellData>();
            var a1 = ConverterHelper.ByteToInt16(bytes[9]);
            var a2 = ConverterHelper.ByteToInt16(bytes[23]);
            var al1 = "";
            var al2 = "";
            if (a1 > 0) al1 = PumpsAlarms(a1);
            if (a2 > 0) al2 = PumpsAlarms(a2);
            if (isDIs.Any())
            {
                well1.WellDatas.Add(new WellData()
                {
                    IsUps = isDIs[0], //1-работает
                    IsDoorOpen = !isDIs[1], //1-дверь закрыта
                    IsAuto = !isDIs[2], //1-дистанционная работа
                    IsLevel = isDIs[3],
                    IsPumpLink = !isDIs[8], //1-нет связи
                    IsFlowWaterLink = !isDIs[9], //1-нет связи
                    IsWellLink = !isDIs[14], //1-нет связи
                    FlowWater =
                        ConverterHelper.ByteToReal(bytes[10], bytes[11]),
                    FlowWaterTotal =
                        ConverterHelper.ByteToReal(bytes[12], bytes[13]),
                    Level = ConverterHelper.ByteToInt16(bytes[6]),
                    PressCamera = ConverterHelper.ByteToInt16(bytes[7]) * 0.01, //0.01 Bar
                    Press = (ConverterHelper.ByteToInt16(bytes[8]) * 0.01 * 1.6-1.3) / 1.47, //0.01 Bar
                    Sysdate = dt,
                    WellId = 1
                });
            }
            if (isPumps.Any())
            {
                well1.WellPumps.Add(new WellPump()
                {
                    Title = "1",
                    WellPumpDatas = new List<WellPumpData>()
                    {
                        new WellPumpData()
                        {
                            IsAuto = isPumps[0],
                            IsDist = isPumps[1],
                            IsActive = isPumps[2],
                            IsAlarm = isPumps[3],
                            Speed = ConverterHelper.ByteToInt16(bytes[2]), //ГЦ
                            Current = ConverterHelper.ByteToInt16(bytes[3]), //A
                            OperationTime =
                                ConverterHelper.ByteToInt(new byte[]
                                {bytes[4][0], bytes[4][1], bytes[5][0], bytes[5][1]}),
                            Sysdate = dt, 
                            WellPumpId = 1,
                            Alarm=al1
                        }
                    }
                });
            }
            
            //well2
            var isDIs2 = ConverterHelper.ByteToBools(bytes[14]);
            var isPumps2 = ConverterHelper.ByteToBools(bytes[15]);
            well2.WellDatas = new List<WellData>();
            if (isDIs2.Any())
            {
                well2.WellDatas.Add(new WellData()
                {
                    IsUps = isDIs2[0], //1-работает
                    IsDoorOpen = !isDIs2[1], //1-дверь закрыта
                    IsAuto = !isDIs2[2], //1-дистанционная работа
                    IsLevel = isDIs2[3],
                    IsPumpLink = !isDIs2[8], //1-нет связи
                    IsFlowWaterLink = !isDIs2[9], //1-нет связи
                    IsWellLink = !isDIs2[14], //1-нет связи
                    FlowWater =
                        ConverterHelper.ByteToReal(new byte[] { bytes[24][0], bytes[24][1], bytes[25][0], bytes[25][1] }),
                    FlowWaterTotal =
                        ConverterHelper.ByteToReal(new byte[] { bytes[26][0], bytes[26][1], bytes[27][0], bytes[27][1] }),
                    Level = ConverterHelper.ByteToInt16(bytes[20]),
                    PressCamera = ConverterHelper.ByteToInt16(bytes[21]) * 0.01, //0.01 Bar
                    Press =  (ConverterHelper.ByteToInt16(bytes[22]) * 0.01* 1.6-1.3) / 1.47, //0.01 Bar
                    Sysdate = dt,
                    WellId = 2
                });
            }
            if (isPumps2.Any())
            {
                well2.WellPumps.Add(new WellPump()
                {
                    Title = "1",
                    WellPumpDatas = new List<WellPumpData>()
                    {
                        new WellPumpData()
                        {
                            IsAuto = isPumps2[0],
                            IsDist = isPumps2[1],
                            IsActive = isPumps2[2],
                            IsAlarm = isPumps[3],
                            Speed = ConverterHelper.ByteToInt16(bytes[16]), //ГЦ
                            Current = ConverterHelper.ByteToInt16(bytes[17]), //A
                            OperationTime =
                                ConverterHelper.ByteToInt(new byte[]
                                {bytes[18][0], bytes[18][1], bytes[19][0], bytes[19][1]}),
                            Sysdate = dt,
                            WellPumpId = 2,
                            Alarm=al2
                        }
                    }
                });
            }
            wells.Add(well1);
            wells.Add(well2);
            return wells;
        }

        /// <summary>
        /// 28-31
        /// </summary>
        /// <returns></returns>
        public static List<ReactionChamber> GetReactionChambers(List<byte[]> bytes, DateTime dt)
        {
            var chambers = new List<ReactionChamber>();
            //var bytes = ReadRegisters(28, 4);
            var is203 = ConverterHelper.ByteToBools(bytes[0]);
            var is204 = ConverterHelper.ByteToBools(bytes[2]);
            var chamber_203 = new ReactionChamber
            {
                Id = 1,
                Title = "20.3",
                ReactionChamberDatas = new List<ReactionChamberData>()
                {
                    new ReactionChamberData()
                    {
                        IsBobber = is203[5],
                        IsLevelH = is203[1],
                        IsLevelHH = is203[0],
                        IsLevelL = is203[2],
                        IsLevelLL = is203[3],
                        IsSA = is203[4],
                        Level = ConverterHelper.ByteToInt16(bytes[1])*0.01,
                        Sysdate = dt,
                        ReactionChamberId = 1
                    }
                }
            };
            var chamber_204 = new ReactionChamber
            {
                Id = 2,
                Title = "20.4",
                ReactionChamberDatas = new List<ReactionChamberData>()
                {
                    new ReactionChamberData()
                    {
                        IsBobber = is204[5],
                        IsLevelH = is204[1],
                        IsLevelHH = is204[0],
                        IsLevelL = is204[2],
                        IsLevelLL = is204[3],
                        IsSA = is204[4],
                        Level = ConverterHelper.ByteToInt16(bytes[3])*0.01,
                        Sysdate = dt,
                        ReactionChamberId = 2
                    }
                }
            };
            chambers.Add(chamber_203);
            chambers.Add(chamber_204);
            return chambers;
        }
        public static void GetReactionChambersLevels(List<byte[]> bytes, List<ReactionChamber> rc)
        {
            rc[0].ReactionChamberDatas.FirstOrDefault().LL = Convert.ToDecimal(ConverterHelper.ByteToInt16(bytes[0]) * 0.01);
            rc[0].ReactionChamberDatas.FirstOrDefault().L = Convert.ToDecimal(ConverterHelper.ByteToInt16(bytes[1]) * 0.01);
            rc[0].ReactionChamberDatas.FirstOrDefault().H = Convert.ToDecimal(ConverterHelper.ByteToInt16(bytes[2]) * 0.01);
            rc[0].ReactionChamberDatas.FirstOrDefault().HH = Convert.ToDecimal(ConverterHelper.ByteToInt16(bytes[3]) * 0.01);
            rc[1].ReactionChamberDatas.FirstOrDefault().LL = Convert.ToDecimal(ConverterHelper.ByteToInt16(bytes[4]) * 0.01);
            rc[1].ReactionChamberDatas.FirstOrDefault().L = Convert.ToDecimal(ConverterHelper.ByteToInt16(bytes[5]) * 0.01);
            rc[1].ReactionChamberDatas.FirstOrDefault().H = Convert.ToDecimal(ConverterHelper.ByteToInt16(bytes[6]) * 0.01);
            rc[1].ReactionChamberDatas.FirstOrDefault().HH = Convert.ToDecimal(ConverterHelper.ByteToInt16(bytes[7]) * 0.01);
        }
        /// <summary>
        /// 32-33
        /// </summary>
        /// <returns></returns>
        public static Tank GetTank(List<byte[]> bytes, DateTime dt)
        {
            //var bytes = ReadRegisters(32, 2);
            var is206 = ConverterHelper.ByteToBools(bytes[0]);

            var tank = new Tank()
            {
                Id = 1,
                Title = "20.6",
                TankDatas = new List<TankData>()
                {
                    new TankData()
                    {
                        IsBobberMax = is206[5],
                        IsBobberMin = is206[6],
                        IsBobberAlarm = is206[7],
                        IsLevelH = is206[1],
                        IsLevelHH = is206[0],
                        IsLevelL = is206[2],
                        IsLevelLL = is206[3],
                        IsSA = is206[4],
                        Level = ConverterHelper.ByteToInt16(bytes[1])*0.01,
                        Sysdate = dt, TankId = 1
                    }
                }
            };

            return tank;
        }
        public static void GetTankLevels(List<byte[]> bytes, Tank tank)
        {
            tank.TankDatas.FirstOrDefault().LL = Convert.ToDecimal(ConverterHelper.ByteToInt16(bytes[0]) * 0.01);
            tank.TankDatas.FirstOrDefault().L = Convert.ToDecimal(ConverterHelper.ByteToInt16(bytes[1]) * 0.01);
            tank.TankDatas.FirstOrDefault().H = Convert.ToDecimal(ConverterHelper.ByteToInt16(bytes[2]) * 0.01);
            tank.TankDatas.FirstOrDefault().HH = Convert.ToDecimal(ConverterHelper.ByteToInt16(bytes[3]) * 0.01);
        }
        /// <summary>
        /// 33-34
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static List<FlowMeter> GetDrainPumps(List<byte[]> bytes)
        {
            //throw NotImplementedException;
            return null;
        }

        /// <summary>
        /// 35-51
        /// </summary>
        /// <returns></returns>
        public static List<FlowMeter> GetFlowMeters(List<byte[]> bytes, DateTime dt)
        {
            var flowMeters = new List<FlowMeter>();
            var isState = ConverterHelper.ByteToBools(bytes[0]);
            flowMeters.Add(new FlowMeter()
            {
                Id = 1,
                Title = "19.24",
                FlowMeterDatas = new List<FlowMeterData>()
                {
                    new FlowMeterData()
                    {
                        Intake =
                            Convert.ToInt32(ConverterHelper.ByteToReal(new byte[] {bytes[1][0], bytes[1][1], bytes[2][0], bytes[2][1]})),
                        IsConnected = !isState[0],
                        Sysdate = dt,
                        Total =
                            Convert.ToInt32(ConverterHelper.ByteToReal(new byte[] {bytes[3][0], bytes[3][1], bytes[4][0], bytes[4][1]})),
                            FlowMeterId = 1
                    }
                }
            });
            flowMeters.Add(new FlowMeter()
            {
                Id = 2,
                Title = "19.25",
                FlowMeterDatas = new List<FlowMeterData>()
                {
                    new FlowMeterData()
                    {
                        Intake =
                            Convert.ToInt32(ConverterHelper.ByteToReal(new byte[] {bytes[5][0], bytes[5][1], bytes[6][0], bytes[6][1]})),
                        IsConnected = !isState[1],
                        Sysdate = dt,
                        Total =
                            Convert.ToInt32(ConverterHelper.ByteToReal(new byte[] {bytes[7][0], bytes[7][1], bytes[8][0], bytes[8][1]})),
                            FlowMeterId = 2
                    }
                }
            });
            flowMeters.Add(new FlowMeter()
            {
                Id = 3,
                Title = "19.40",
                FlowMeterDatas = new List<FlowMeterData>()
                {
                    new FlowMeterData()
                    {
                        Intake =
                            Convert.ToInt32(ConverterHelper.ByteToReal(new byte[] {bytes[9][0], bytes[9][1], bytes[10][0], bytes[10][1]})),
                        IsConnected = !isState[2],
                        Sysdate = dt,
                        Total =
                            Convert.ToInt32(ConverterHelper.ByteToReal(new byte[]
                            {bytes[11][0], bytes[11][1], bytes[12][0], bytes[12][1]})),
                            FlowMeterId = 3
                    }
                }
            });
            flowMeters.Add(new FlowMeter()
            {
                Id = 4,
                Title = "19.41",
                FlowMeterDatas = new List<FlowMeterData>()
                {
                    new FlowMeterData()
                    {
                        Intake =
                            Convert.ToInt32(ConverterHelper.ByteToReal(new byte[]
                            {bytes[13][0], bytes[13][1], bytes[14][0], bytes[14][1]})),
                        IsConnected = !isState[3],
                        Sysdate = dt,
                        Total =
                            Convert.ToInt32(ConverterHelper.ByteToReal(new byte[]
                            {bytes[15][0], bytes[15][1], bytes[16][0], bytes[16][1]})),
                            FlowMeterId = 4
                    }
                }
            });
            return flowMeters;
        }

        /// <summary>
        /// 52, count=5
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static LiftWaterStatu GetLiftWaterStatus(List<byte[]> bytes, DateTime dt)
        {
            var isState = ConverterHelper.ByteToBools(bytes[0]);
            var lws = new LiftWaterStatu()
            {
                IsAccessMode = isState[0],
                IsActive = isState[1],
                IsAlarm = isState[2],
                IsWarning = isState[3],
                IsPumpAct = isState[4],
                IsSpeedMax = isState[5],
                IsSpeedMin = isState[7],
                IsStandBy = isState[6],
                IsResetAlarmAck = isState[11],
                IsSetpointAct = isState[12],
                IsPowerMax = isState[13],
                IsRotation = isState[14],
                IsDirection = isState[15],
                Feedback = ConverterHelper.ByteToInt16(bytes[1]),
                ControlMode = new byte[] { bytes[2][0] },
                OperationMode = new byte[] { bytes[2][1] },
                Press = ConverterHelper.ByteToReal(bytes[3], bytes[4]),
                Sysdate = dt

            };
            return lws;
        }

        public static void GetChlorine(LiftWaterStatu lift, List<byte[]> bytes)
        {
            lift.Chlorine = ConverterHelper.ByteToInt16(bytes[0]) * 0.01;
            lift.Temp = ConverterHelper.ByteToInt16(bytes[1]) * 0.1;
        }
        /// <summary>
        /// 40181 x100
        /// </summary>
        /// <returns></returns>
        public static void GetLiftWaterPressSetPoint(byte[] bytes, LiftWaterStatu st)
        {
            st.PressSetPoint = ConverterHelper.ByteToInt16(bytes) * 0.01;
        }

        /// <summary>
        /// 57, count=16
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static List<LiftWaterPump> GetLiftWaterPumps(List<byte[]> bytes, DateTime dt)
        {
            var lwps = new List<LiftWaterPump>();
            var is1221 = ConverterHelper.ByteToBools(bytes[0]);
            var is1222 = ConverterHelper.ByteToBools(bytes[4]);
            var is1223 = ConverterHelper.ByteToBools(bytes[8]);
            var is1224 = ConverterHelper.ByteToBools(bytes[12]);

            var speed1f = 0.0;
            var speed1 = ConverterHelper.ByteToInt16(bytes[3]);
            if (speed1 == -1) speed1f = -1;
            else if (speed1 == 0) speed1f = 0;
            else speed1f = speed1 * 0.01 / 2;
            var time1 = ConverterHelper.ByteToReal(bytes[1], bytes[2]);
            if (float.IsNaN(time1)) time1 = 0;

            var speed2f = 0.0;
            var speed2 = ConverterHelper.ByteToInt16(bytes[7]);
            if (speed2 == -1) speed2f = -1;
            else if (speed2 == 0) speed2f = 0;
            else speed2f = speed2 * 0.01 / 2;
            var time2 = ConverterHelper.ByteToReal(bytes[5], bytes[6]);
            if (float.IsNaN(time2)) time2 = 0;

            //var speed3f = 0.0;
            //var speed3 = ConverterHelper.ByteToInt16(bytes[11]);
            //if (speed3 == -1) speed3f = -1;
            //else if (speed3 == 0) speed3f = 0;
            //else speed3f = speed3 * 0.01 / 2;
            //var time3 = ConverterHelper.ByteToReal(bytes[9], bytes[10]);
            //if (float.IsNaN(time3)) time3 = 0;

            //var speed4f = 0.0;
            //var speed4 = ConverterHelper.ByteToInt16(bytes[15]);
            //if (speed4 == -1) speed4f = -1;
            //else if (speed4 == 0) speed4f = 0;
            //else speed4f = speed4 * 0.01 / 2;
            //var time4 = ConverterHelper.ByteToReal(bytes[13], bytes[14]);
            //if (float.IsNaN(time4)) time4 = 0;

            lwps.Add(new LiftWaterPump()
            {
                Id = 1,
                Title = "12.2.1",
                LiftWaterPumpDatas = new List<LiftWaterPumpData>()
                {
                    new LiftWaterPumpData()
                    {
                        IsAccessMode = is1221[0],
                        IsActive = is1221[1],
                        IsPumpAlarm = is1221[2],
                        AlarmCode = new byte[] {bytes[0][1]},
                        OperationTime =Convert.ToDecimal(time1),
                        Speed = Convert.ToInt32(speed1f),
                        Sysdate = dt, 
                        PumpId = 1,
                        IsCommFault = is1221[3]
                    }
                }
            });
            lwps.Add(new LiftWaterPump()
            {
                Id = 3,
                Title = "12.2.2",
                LiftWaterPumpDatas = new List<LiftWaterPumpData>()
                {
                    new LiftWaterPumpData()
                    {
                        IsAccessMode = is1222[0],
                        IsActive = is1222[1],
                        IsPumpAlarm = is1222[2],
                        AlarmCode = new byte[] {bytes[4][1]},
                        OperationTime =Convert.ToDecimal(time2),
                        Speed =  Convert.ToInt32(speed2f),
                        Sysdate = dt,
                        PumpId = 3,
                        IsCommFault = is1222[3]
                    }
                }
            });
            //lwps.Add(new LiftWaterPump()
            //{
            //    Id = 4,
            //    Title = "12.2.3",
            //    LiftWaterPumpDatas = new List<LiftWaterPumpData>()
            //    {
            //        new LiftWaterPumpData()
            //        {
            //            IsAccessMode = is1223[0],
            //            IsActive = is1223[1],
            //            IsPumpAlarm = is1223[2],
            //            AlarmCode = new byte[] {bytes[8][1]},
            //            OperationTime =Convert.ToDecimal(time3),
            //            Speed =  Convert.ToInt32(speed3f),
            //            Sysdate = dt, 
            //            PumpId = 4,
            //            IsCommFault = is1223[3]
            //        }
            //    }
            //});
            //lwps.Add(new LiftWaterPump()
            //{
            //    Id = 5,
            //    Title = "12.2.4",
            //    LiftWaterPumpDatas = new List<LiftWaterPumpData>()
            //    {
            //        new LiftWaterPumpData()
            //        {
            //            IsAccessMode = is1224[0],
            //            IsActive = is1224[1],
            //            IsPumpAlarm = is1224[2],
            //            AlarmCode = new byte[] {bytes[12][1]},
            //            OperationTime =Convert.ToDecimal(time4),
            //            Speed =  Convert.ToInt32(speed4f),
            //            Sysdate = dt, 
            //            PumpId = 5,
            //            IsCommFault = is1224[3]
            //        }
            //    }
            //});
            return lwps;
        }

        public static List<LiftWaterPump> GetLiftWaterPumpsPromEnergo(List<byte[]> bData, List<byte[]> bState, List<byte[]> bAlarm, List<byte[]> bWarning, DateTime dt)
        {
            var lwps = new List<LiftWaterPump>();
            var is1222 = ConverterHelper.ByteToInt16(bState[0]);
            //var is1223 = ConverterHelper.ByteToBools(bytes[8]);
            //var is1224 = ConverterHelper.ByteToBools(bytes[12]);

            //var speed1f = 0.0;
            //var speed1 = ConverterHelper.ByteToInt16(bytes[3]);
            //if (speed1 == -1) speed1f = -1;
            //else if (speed1 == 0) speed1f = 0;
            //else speed1f = speed1 * 0.01 / 2;
            //var time1 = ConverterHelper.ByteToReal(bytes[1], bytes[2]);
            //if (float.IsNaN(time1)) time1 = 0;

            var speed2f = 0.0;
            var speed2 = ConverterHelper.ByteToReal(bData[5], bData[4]);
            if (speed2 == -1) speed2f = -1;
            else if (speed2 == 0) speed2f = 0;
            else speed2f = speed2;
            var time2 = ConverterHelper.ByteToInt(bState[3], bState[2]);
            //if (float.IsNaN(time2)) time2 = 0;
            var press = ConverterHelper.ByteToReal(bData[1], bData[0]);


            lwps.Add(new LiftWaterPump()
            {
                Id = 3,
                Title = "12.2.2",
                LiftWaterPumpDatas = new List<LiftWaterPumpData>()
                {
                    new LiftWaterPumpData()
                    {
                        IsAccessMode = is1222 == 3 || is1222 == 4,
                        IsActive = is1222 == 4 || is1222 == 8,
                        IsPumpAlarm = is1222 == 1 || is1222 == 2,
                        AlarmCode = new byte[] {bAlarm[0][0],bAlarm[0][1],bAlarm[1][0],bAlarm[1][1],bAlarm[2][0],bAlarm[2][1]},
                        OperationTime =time2,
                        Speed =  Convert.ToInt32(speed2f),
                        Sysdate = dt, 
                        PumpId = 3,
                        IsCommFault = is1222 == 0, 
                        Press = Convert.ToDecimal(press), 
                        StateCode = bState[0], WarningCode = new byte[] {bWarning[0][0], bWarning[0][1], bWarning[1][0], bWarning[1][1]}
                    }
                }
            });
            //lwps.Add(new LiftWaterPump()
            //{
            //    Id = 4,
            //    Title = "12.2.3",
            //    LiftWaterPumpDatas = new List<LiftWaterPumpData>()
            //    {
            //        new LiftWaterPumpData()
            //        {
            //            IsAccessMode = is1223[0],
            //            IsActive = is1223[1],
            //            IsPumpAlarm = is1223[2],
            //            AlarmCode = new byte[] {bytes[8][1]},
            //            OperationTime =Convert.ToDecimal(time3),
            //            Speed =  Convert.ToInt32(speed3f),
            //            Sysdate = dt, 
            //            PumpId = 4,
            //            IsCommFault = is1223[3]
            //        }
            //    }
            //});
            //lwps.Add(new LiftWaterPump()
            //{
            //    Id = 5,
            //    Title = "12.2.4",
            //    LiftWaterPumpDatas = new List<LiftWaterPumpData>()
            //    {
            //        new LiftWaterPumpData()
            //        {
            //            IsAccessMode = is1224[0],
            //            IsActive = is1224[1],
            //            IsPumpAlarm = is1224[2],
            //            AlarmCode = new byte[] {bytes[12][1]},
            //            OperationTime =Convert.ToDecimal(time4),
            //            Speed =  Convert.ToInt32(speed4f),
            //            Sysdate = dt, 
            //            PumpId = 5,
            //            IsCommFault = is1224[3]
            //        }
            //    }
            //});
            return lwps;
        }

        /// <summary>
        /// 73, count=3
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static KNSData GetKNS(List<byte[]> bytes, DateTime dt)
        {
            var isKnsD = ConverterHelper.ByteToBools(bytes[0]);
            var isKnsStatus = ConverterHelper.ByteToBools(bytes[2]);
            var kns = new KNSData()
            {
                IsLevelHH = isKnsD[0],
                IsLevelH = isKnsD[1],
                IsLevelL = isKnsD[2],
                IsLevelLL = isKnsD[3],
                IsRemoteAccess = isKnsStatus[0],
                IsResetAlarmAck = isKnsStatus[1],
                IsAutoPitAck = isKnsStatus[2],
                IsInterlockPitAck = isKnsStatus[3],
                IsCustomRelayPulseAck = isKnsStatus[4],
                IsPitMode = isKnsStatus[5],
                Sysdate = dt,
                OperationMode = new[] { bytes[2][1] },
                Level = ConverterHelper.ByteToInt16(bytes[1]) * 0.01
            };

            return kns;
        }

        public static void GetKNSLevels(List<byte[]> bytes, KNSData kns)
        {
            kns.LL = Convert.ToDecimal(ConverterHelper.ByteToInt16(bytes[0]) * 0.01);
            kns.L = Convert.ToDecimal(ConverterHelper.ByteToInt16(bytes[1]) * 0.01);
            kns.H = Convert.ToDecimal(ConverterHelper.ByteToInt16(bytes[2]) * 0.01);
            kns.HH = Convert.ToDecimal(ConverterHelper.ByteToInt16(bytes[3]) * 0.01);
        }
        /// <summary>
        /// 73, count=15
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static List<KNSPump> GetKNSPumps(List<byte[]> bytes, DateTime dt)
        {
            var is291 = ConverterHelper.ByteToBools(bytes[0]);
            var is292 = ConverterHelper.ByteToBools(bytes[5]);
            var is293 = ConverterHelper.ByteToBools(bytes[10]);
            var runTime1 = ConverterHelper.ByteToInt(bytes[3], bytes[4]);
            var runTime2 = ConverterHelper.ByteToInt(bytes[8], bytes[9]);
            var runTime3 = ConverterHelper.ByteToInt(bytes[13], bytes[14]);
            var knsPumps = new List<KNSPump>();
            knsPumps.Add(new KNSPump()
            {
                Id = 1,
                Title = "29.1",
                KNSPumpDatas = new List<KNSPumpData>()
                {
                    new KNSPumpData()
                    {
                        IsPresent = is291[0],
                        IsRunning = is291[1],
                        IsAlarm = is291[2],
                        IsCommFault = is291[3],
                        OperatingMode = new[] {bytes[0][1]},
                        Sysdate = dt,
                        Current = ConverterHelper.ByteToInt16(bytes[1]),
                        Frequency = ConverterHelper.ByteToInt16(bytes[2]),
                        RunTime = runTime1 > 0 ? Convert.ToDecimal(runTime1/(60*1.0f)) : Convert.ToDecimal(runTime1),
                        KNSPumpId = 1
                    }
                }
            });
            knsPumps.Add(new KNSPump()
            {
                Id = 2,
                Title = "29.2",
                KNSPumpDatas = new List<KNSPumpData>()
                {
                    new KNSPumpData()
                    {
                        IsPresent = is292[0],
                        IsRunning = is292[1],
                        IsAlarm = is292[2],
                        IsCommFault = is292[3],
                        OperatingMode = new[] {bytes[5][1]},
                        Sysdate = dt,
                        Current = ConverterHelper.ByteToInt16(bytes[6]),
                        Frequency = ConverterHelper.ByteToInt16(bytes[7]),
                        RunTime = runTime2 > 0 ? Convert.ToDecimal(runTime2/(60*1.0f)) : Convert.ToDecimal(runTime2),
                        KNSPumpId = 2
                    }
                }
            });
            knsPumps.Add(new KNSPump()
            {
                Id = 3,
                Title = "29.3",
                KNSPumpDatas = new List<KNSPumpData>()
                {
                    new KNSPumpData()
                    {
                        IsPresent = is293[0],
                        IsRunning = is293[1],
                        IsAlarm = is293[2],
                        IsCommFault = is293[3],
                        OperatingMode = new[] {bytes[10][1]},
                        Sysdate = dt,
                        Current = ConverterHelper.ByteToInt16(bytes[11]),
                        Frequency = ConverterHelper.ByteToInt16(bytes[12]),
                        RunTime = runTime3 > 0 ? Convert.ToDecimal(runTime3/(60*1.0f)) : Convert.ToDecimal(runTime3),
                        KNSPumpId = 3
                    }
                }
            });
            return knsPumps;
        }

        /// <summary>
        /// 91, count=10
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static List<Valve> GetValves(List<byte[]> bytes, DateTime dt)
        {
            var v1 = ConverterHelper.ByteToBools(bytes[0]);
            var v2 = ConverterHelper.ByteToBools(bytes[1]);
            var v3 = ConverterHelper.ByteToBools(bytes[2]);
            var v4 = ConverterHelper.ByteToBools(bytes[3]);
            var v5 = ConverterHelper.ByteToBools(bytes[4]);
            var v6 = ConverterHelper.ByteToBools(bytes[5]);
            var v7 = ConverterHelper.ByteToBools(bytes[6]);
            var v8 = ConverterHelper.ByteToBools(bytes[7]);
            var v9 = ConverterHelper.ByteToBools(bytes[8]);
            var v10 = ConverterHelper.ByteToBools(bytes[9]);
            var vs = new List<Valve>();
            vs.Add(new Valve()
            {
                Id = 1,
                Title = "17.3.1",
                ValveDatas = new List<ValveData>()
                {
                    new ValveData()
                    {
                        IsClosed = v1[0],
                        IsOpen = v1[1],
                        IsAlarm = v1[4],
                        IsAuto = v1[5],
                        IsNotOpenAlarm = v1[6],
                        IsNotClosedAlarm = v1[7], Sysdate = dt, 
                        ValveId = 1
                    }
                }
            });
            vs.Add(new Valve()
            {
                Id = 2,
                Title = "17.3.2",
                ValveDatas = new List<ValveData>()
                {
                    new ValveData()
                    {
                        IsClosed = v2[0],
                        IsOpen = v2[1],
                        IsAlarm = v2[4],
                        IsAuto = v2[5],
                        IsNotOpenAlarm = v2[6],
                        IsNotClosedAlarm = v2[7], Sysdate = dt, 
                        ValveId = 2
                    }
                }
            });
            vs.Add(new Valve()
            {
                Id = 3,
                Title = "17.3.3",
                ValveDatas = new List<ValveData>()
                {
                    new ValveData()
                    {
                        IsClosed = v3[0],
                        IsOpen = v3[1],
                        IsAlarm = v3[4],
                        IsAuto = v3[5],
                        IsNotOpenAlarm = v3[6],
                        IsNotClosedAlarm = v3[7], Sysdate = dt, 
                        ValveId = 3
                    }
                }
            });
            vs.Add(new Valve()
            {
                Id = 4,
                Title = "17.3.4",
                ValveDatas = new List<ValveData>()
                {
                    new ValveData()
                    {
                        IsClosed = v4[0],
                        IsOpen = v4[1],
                        IsAlarm = v4[4],
                        IsAuto = v4[5],
                        IsNotOpenAlarm = v4[6],
                        IsNotClosedAlarm = v4[7], Sysdate = dt, 
                        ValveId = 4
                    }
                }
            });
            vs.Add(new Valve()
            {
                Id = 5,
                Title = "17.3.5",
                ValveDatas = new List<ValveData>()
                {
                    new ValveData()
                    {
                        IsClosed = v5[0],
                        IsOpen = v5[1],
                        IsAlarm = v5[4],
                        IsAuto = v5[5],
                        IsNotOpenAlarm = v5[6],
                        IsNotClosedAlarm = v5[7], Sysdate = dt, 
                        ValveId = 5
                    }
                }
            });
            vs.Add(new Valve()
            {
                Id = 6,
                Title = "17.3.6",
                ValveDatas = new List<ValveData>()
                {
                    new ValveData()
                    {
                        IsClosed = v6[0],
                        IsOpen = v6[1],
                        IsAlarm = v6[4],
                        IsAuto = v6[5],
                        IsNotOpenAlarm = v6[6],
                        IsNotClosedAlarm = v6[7], Sysdate = dt, 
                        ValveId = 6
                    }
                }
            });
            vs.Add(new Valve()
            {
                Id = 7,
                Title = "18.1",
                ValveDatas = new List<ValveData>()
                {
                    new ValveData()
                    {
                        IsClosed = v7[0],
                        IsOpen = v7[1],
                        IsAlarm = v7[4],
                        IsAuto = v7[5],
                        IsNotOpenAlarm = v7[6],
                        IsNotClosedAlarm = v7[7], Sysdate = dt, 
                        ValveId = 7
                    }
                }
            });
            vs.Add(new Valve()
            {
                Id = 8,
                Title = "18.2",
                ValveDatas = new List<ValveData>()
                {
                    new ValveData()
                    {
                        IsClosed = v8[0],
                        IsOpen = v8[1],
                        IsAlarm = v8[4],
                        IsAuto = v8[5],
                        IsNotOpenAlarm = v8[6],
                        IsNotClosedAlarm = v8[7], Sysdate = dt, 
                        ValveId = 8
                    }
                }
            });
            vs.Add(new Valve()
            {
                Id = 9,
                Title = "18.6",
                ValveDatas = new List<ValveData>()
                {
                    new ValveData()
                    {
                        IsClosed = v9[0],
                        IsOpen = v9[1],
                        IsAlarm = v9[4],
                        IsAuto = v9[5],
                        IsNotOpenAlarm = v9[6],
                        IsNotClosedAlarm = v9[7], Sysdate = dt, 
                        ValveId = 9
                    }
                }
            });
            vs.Add(new Valve()
            {
                Id = 10,
                Title = "18.7",
                ValveDatas = new List<ValveData>()
                {
                    new ValveData()
                    {
                        IsClosed = v10[0],
                        IsOpen = v10[1],
                        IsAlarm = v10[4],
                        IsAuto = v10[5],
                        IsNotOpenAlarm = v10[6],
                        IsNotClosedAlarm = v10[7], Sysdate = dt, 
                        ValveId = 10
                    }
                }
            });
            return vs;
        }

        /// <summary>
        /// 101, count=6
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static List<PSHU> GetPSHUs(List<byte[]> bytes, DateTime dt)
        {
            var p1 = ConverterHelper.ByteToBools(bytes[0]);
            var p2 = ConverterHelper.ByteToBools(bytes[1]);
            var p3 = ConverterHelper.ByteToBools(bytes[2]);
            var p4 = ConverterHelper.ByteToBools(bytes[3]);
            var p5 = ConverterHelper.ByteToBools(bytes[4]);
            var p6 = ConverterHelper.ByteToBools(bytes[5]);

            var ps = new List<PSHU>();
            ps.Add(new PSHU()
            {
                Id = 1,
                Title = "6.1",
                PSHUDatas = new List<PSHUData>()
                {
                    new PSHUData()
                    {
                        IsClosed = p1[0],
                        IsOpen = p1[1],
                        IsPress = p1[2],
                        IsAuto = p1[5], Sysdate = dt,
                        PSHUId = 1
                    }
                }
            });
            ps.Add(new PSHU()
            {
                Id = 2,
                Title = "6.2",
                PSHUDatas = new List<PSHUData>()
                {
                    new PSHUData()
                    {
                        IsClosed = p2[0],
                        IsOpen = p2[1],
                        IsPress = p2[2],
                        IsAuto = p2[5], Sysdate = dt,
                        PSHUId = 2
                    }
                }
            });
            ps.Add(new PSHU()
            {
                Id = 3,
                Title = "6.3",
                PSHUDatas = new List<PSHUData>()
                {
                    new PSHUData()
                    {
                        IsClosed = p3[0],
                        IsOpen = p3[1],
                        IsPress = p3[2],
                        IsAuto = p3[5], Sysdate = dt,
                        PSHUId = 3
                    }
                }
            });
            ps.Add(new PSHU()
            {
                Id = 4,
                Title = "6.4",
                PSHUDatas = new List<PSHUData>()
                {
                    new PSHUData()
                    {
                        IsClosed = p4[0],
                        IsOpen = p4[1],
                        IsPress = p4[2],
                        IsAuto = p4[5], Sysdate = dt,
                        PSHUId = 4
                    }
                }
            });
            ps.Add(new PSHU()
            {
                Id = 5,
                Title = "6.5",
                PSHUDatas = new List<PSHUData>()
                {
                    new PSHUData()
                    {
                        IsClosed = p5[0],
                        IsOpen = p5[1],
                        IsPress = p5[2],
                        IsAuto = p5[5], Sysdate = dt,
                        PSHUId = 5
                    }
                }
            });
            ps.Add(new PSHU()
            {
                Id = 6,
                Title = "6.6",
                PSHUDatas = new List<PSHUData>()
                {
                    new PSHUData()
                    {
                        IsClosed = p6[0],
                        IsOpen = p6[1],
                        IsPress = p6[2],
                        IsAuto = p6[5], Sysdate = dt,
                        PSHUId = 6
                    }
                }
            });
            return ps;
        }
        /// <summary>
        /// 113, count=1
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static DoseStatu GetDoseStatus(List<byte[]> bytes, DateTime dt)
        {
            var d = ConverterHelper.ByteToBools(bytes[0]);
            return new DoseStatu()
            {
                IsActive = d[0],
                IsAlarm = d[1],
                IsMode1 = d[2],
                IsMode2 = d[3],
                IsOpen289 = d[4],
                IsOpen2811 = d[5],
                Sysdate = dt,
                IsLevelNaOCl1 = d[6],
                IsLevelWater1 = d[7],
                IsLevelNaOCl2 = d[8],
                IsLevelwater2 = d[9]
            };
        }
        /// <summary>
        /// 114, count=40
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static List<Dos> GetDoses(List<byte[]> bytes, DateTime dt)
        {
            var d1 = ConverterHelper.ByteToBools(bytes[0]);
            var d1_1 = ConverterHelper.ByteToBools(bytes[1]);
            var d2 = ConverterHelper.ByteToBools(bytes[10]);
            var d2_1 = ConverterHelper.ByteToBools(bytes[11]);
            var d3 = ConverterHelper.ByteToBools(bytes[20]);
            var d3_1 = ConverterHelper.ByteToBools(bytes[21]);
            var d4 = ConverterHelper.ByteToBools(bytes[30]);
            var d4_1 = ConverterHelper.ByteToBools(bytes[31]);
            var ds = new List<Dos>();
            ds.Add(new Dos()
            {
                Id = 1,
                Title = "10.1.4.1",
                DoseDatas = new List<DoseData>(){new DoseData()
            {
                IsActive = d1[0],IsDist = d1[1], IsAlarm = d1[2], IsAuto = d1[3],IsHC = d1[4], IsOff = d1[9], IsCF = d1[15],
                IsMembrF = d1_1[0], IsLL = d1_1[1], IsLLL = d1_1[2], IsHolo = d1_1[3], 
                Deb = ConverterHelper.ByteToReal(bytes[2],bytes[3]),
                DebSP = ConverterHelper.ByteToReal(bytes[4],bytes[5]),
                DebSum = ConverterHelper.ByteToReal(bytes[6],bytes[7]),
                OperationTime = ConverterHelper.ByteToInt(bytes[8],bytes[9]), IsPmax = false, IsPmax5 = false, Sysdate = dt,
                DoseId = 1
            }}
            });
            ds.Add(new Dos()
            {
                Id = 2,
                Title = "10.1.4.2",
                DoseDatas = new List<DoseData>(){new DoseData()
            {
                IsActive = d2[0],IsDist = d2[1], IsAlarm = d2[2], IsAuto = d2[3],IsHC = d2[4], IsOff = d2[9], IsCF = d2[15],
                IsMembrF = d2_1[0], IsLL = d2_1[1], IsLLL = d2_1[2], IsHolo = d2_1[3], 
                Deb = ConverterHelper.ByteToReal(bytes[12],bytes[13]),
                DebSP = ConverterHelper.ByteToReal(bytes[14],bytes[15]),
                DebSum = ConverterHelper.ByteToReal(bytes[16],bytes[17]),
                OperationTime = ConverterHelper.ByteToInt(bytes[18],bytes[19]), IsPmax = false, IsPmax5 = false, Sysdate = dt,
                DoseId = 2
            }}
            });
            ds.Add(new Dos()
            {
                Id = 3,
                Title = "10.1.7.1",
                DoseDatas = new List<DoseData>(){new DoseData()
            {
                IsActive = d3[0],IsDist = d3[1], IsAlarm = d3[2], IsAuto = d3[3],IsHC = d3[4], IsOff = d3[9], IsCF = d3[15],
                IsMembrF = d3_1[0], IsLL = d3_1[1], IsLLL = d3_1[2], IsHolo = d3_1[3], IsPmax = d3_1[14], IsPmax5 = d3_1[15],
                Deb = ConverterHelper.ByteToReal(bytes[22],bytes[23]),
                DebSP = ConverterHelper.ByteToReal(bytes[24],bytes[25]),
                DebSum = ConverterHelper.ByteToReal(bytes[26],bytes[27]),
                OperationTime = ConverterHelper.ByteToInt(bytes[28],bytes[29]), Sysdate = dt,
                DoseId = 3
            }}
            });
            ds.Add(new Dos()
            {
                Id = 4,
                Title = "10.1.7.2",
                DoseDatas = new List<DoseData>(){new DoseData()
            {
               IsActive = d4[0],IsDist = d4[1], IsAlarm = d4[2], IsAuto = d4[3],IsHC = d4[4], IsOff = d4[9], IsCF = d4[15],
                IsMembrF = d4_1[0], IsLL = d4_1[1], IsLLL = d4_1[2], IsHolo = d4_1[3], IsPmax = d4_1[14], IsPmax5 = d4_1[15],
                Deb = ConverterHelper.ByteToReal(bytes[32],bytes[33]),
                DebSP = ConverterHelper.ByteToReal(bytes[34],bytes[35]),
                DebSum = ConverterHelper.ByteToReal(bytes[36],bytes[37]),
                OperationTime = ConverterHelper.ByteToInt(bytes[38],bytes[39]),  Sysdate = dt,
                DoseId = 4
            }}
            });
            return ds;
        }

        public static void GetDoseJobM3(List<byte[]> bytes, DoseStatu ds)
        {
            var d7 = ConverterHelper.ByteToInt16(bytes[0]);
            var d4 = ConverterHelper.ByteToInt16(bytes[1]);
            ds.Job1014 = Convert.ToDecimal(d4 * 0.001);
            ds.Job1017 = Convert.ToDecimal(d7 * 0.001);
        }
        public static List<Alarm> GetAlarms(List<byte[]> bytes, DateTime dt)
        {
            var alarms = new List<Alarm>();
            var a1 = ConverterHelper.ByteToBools(bytes[0]);
            var a2 = ConverterHelper.ByteToBools(bytes[1]);
            alarms.Add(new Alarm() { AlarmTypeId = 1, IsConfirmed = false, Sysdate = dt, IsState = a1[0] });
            alarms.Add(new Alarm() { AlarmTypeId = 2, IsConfirmed = false, Sysdate = dt, IsState = a1[1] });
            alarms.Add(new Alarm() { AlarmTypeId = 3, IsConfirmed = false, Sysdate = dt, IsState = a1[2] });
            alarms.Add(new Alarm() { AlarmTypeId = 4, IsConfirmed = false, Sysdate = dt, IsState = a1[3] });
            alarms.Add(new Alarm() { AlarmTypeId = 21, IsConfirmed = false, Sysdate = dt, IsState = a1[4] });
            alarms.Add(new Alarm() { AlarmTypeId = 5, IsConfirmed = false, Sysdate = dt, IsState = a1[5] });
            alarms.Add(new Alarm() { AlarmTypeId = 7, IsConfirmed = false, Sysdate = dt, IsState = a1[6] });
            alarms.Add(new Alarm() { AlarmTypeId = 20, IsConfirmed = false, Sysdate = dt, IsState = a1[7] });
            alarms.Add(new Alarm() { AlarmTypeId = 8, IsConfirmed = false, Sysdate = dt, IsState = a1[8] });
            alarms.Add(new Alarm() { AlarmTypeId = 9, IsConfirmed = false, Sysdate = dt, IsState = a1[9] });
            alarms.Add(new Alarm() { AlarmTypeId = 10, IsConfirmed = false, Sysdate = dt, IsState = a1[10] });
            alarms.Add(new Alarm() { AlarmTypeId = 11, IsConfirmed = false, Sysdate = dt, IsState = a1[11] });
            alarms.Add(new Alarm() { AlarmTypeId = 12, IsConfirmed = false, Sysdate = dt, IsState = a1[12] });
            alarms.Add(new Alarm() { AlarmTypeId = 13, IsConfirmed = false, Sysdate = dt, IsState = a1[13] });
            alarms.Add(new Alarm() { AlarmTypeId = 14, IsConfirmed = false, Sysdate = dt, IsState = a1[14] });
            alarms.Add(new Alarm() { AlarmTypeId = 15, IsConfirmed = false, Sysdate = dt, IsState = a1[15] });
            alarms.Add(new Alarm() { AlarmTypeId = 16, IsConfirmed = false, Sysdate = dt, IsState = a2[0] });
            alarms.Add(new Alarm() { AlarmTypeId = 17, IsConfirmed = false, Sysdate = dt, IsState = a2[1] });
            alarms.Add(new Alarm() { AlarmTypeId = 18, IsConfirmed = false, Sysdate = dt, IsState = a2[2] });
            alarms.Add(new Alarm() { AlarmTypeId = 19, IsConfirmed = false, Sysdate = dt, IsState = a2[3] });
            alarms.Add(new Alarm() { AlarmTypeId = 22, IsConfirmed = false, Sysdate = dt, IsState = a2[4] });
            alarms.Add(new Alarm() { AlarmTypeId = 23, IsConfirmed = false, Sysdate = dt, IsState = a2[5] });
            alarms.Add(new Alarm() { AlarmTypeId = 24, IsConfirmed = false, Sysdate = dt, IsState = a2[6] });
            alarms.Add(new Alarm() { AlarmTypeId = 25, IsConfirmed = false, Sysdate = dt, IsState = a2[7] });
            alarms.Add(new Alarm() { AlarmTypeId = 26, IsConfirmed = false, Sysdate = dt, IsState = a2[8] });
            alarms.Add(new Alarm() { AlarmTypeId = 27, IsConfirmed = false, Sysdate = dt, IsState = a2[9] });
            alarms.Add(new Alarm() { AlarmTypeId = 28, IsConfirmed = false, Sysdate = dt, IsState = a2[10] });
            alarms.Add(new Alarm() { AlarmTypeId = 29, IsConfirmed = false, Sysdate = dt, IsState = a2[11] });
            alarms.Add(new Alarm() { AlarmTypeId = 30, IsConfirmed = false, Sysdate = dt, IsState = a2[12] });
            alarms.Add(new Alarm() { AlarmTypeId = 31, IsConfirmed = false, Sysdate = dt, IsState = a2[13] });
            alarms.Add(new Alarm() { AlarmTypeId = 32, IsConfirmed = false, Sysdate = dt, IsState = a2[14] });
            alarms.Add(new Alarm() { AlarmTypeId = 33, IsConfirmed = false, Sysdate = dt, IsState = a2[15] });
            return alarms;
        }
        public static List<Alarm> GetAlarms2(List<byte[]> bytes, DateTime dt)
        {
            var alarms = new List<Alarm>();
            var a1 = ConverterHelper.ByteToBools(bytes[0]);
            alarms.Add(new Alarm() { AlarmTypeId = 34, IsConfirmed = false, Sysdate = dt, IsState = a1[0] });
            alarms.Add(new Alarm() { AlarmTypeId = 35, IsConfirmed = false, Sysdate = dt, IsState = a1[1] });
            alarms.Add(new Alarm() { AlarmTypeId = 36, IsConfirmed = false, Sysdate = dt, IsState = a1[2] });
            alarms.Add(new Alarm() { AlarmTypeId = 37, IsConfirmed = false, Sysdate = dt, IsState = a1[3] });
            alarms.Add(new Alarm() { AlarmTypeId = 38, IsConfirmed = false, Sysdate = dt, IsState = a1[4] });
            alarms.Add(new Alarm() { AlarmTypeId = 39, IsConfirmed = false, Sysdate = dt, IsState = a1[5] });
            alarms.Add(new Alarm() { AlarmTypeId = 40, IsConfirmed = false, Sysdate = dt, IsState = a1[6] });
            alarms.Add(new Alarm() { AlarmTypeId = 41, IsConfirmed = false, Sysdate = dt, IsState = a1[7] });
            return alarms;
        }
        public static CompressorStatu GetCompressorStatus(List<byte[]> bytes, DateTime dt)
        {
            return new CompressorStatu()
            {
                Sysdate = dt,
                Press = Convert.ToDecimal(ConverterHelper.ByteToInt16(bytes[0]) * 0.01),
                PressSetPoint = Convert.ToDecimal(ConverterHelper.ByteToInt16(bytes[1]) * 0.01)
            };
        }

        public static void GetAlarmsKNSPumps(List<KNSPump> pumps, List<byte[]> bytes)
        {
            try
            {
                var a = ConverterHelper.ByteToBools(bytes[0]);
                a.AddRange(ConverterHelper.ByteToBools(bytes[1]));
                a.AddRange(ConverterHelper.ByteToBools(bytes[2]));
                a.AddRange(ConverterHelper.ByteToBools(bytes[3]));
                pumps[0].KNSPumpDatas.FirstOrDefault().Alarm = KNSPumpsAlarm(a);
                var a5 = ConverterHelper.ByteToBools(bytes[4]);
                a5.AddRange(ConverterHelper.ByteToBools(bytes[5]));
                a5.AddRange(ConverterHelper.ByteToBools(bytes[6]));
                a5.AddRange(ConverterHelper.ByteToBools(bytes[7]));
                pumps[1].KNSPumpDatas.FirstOrDefault().Alarm = KNSPumpsAlarm(a5);
                var a9 = ConverterHelper.ByteToBools(bytes[8]);
                a9.AddRange(ConverterHelper.ByteToBools(bytes[9]));
                a9.AddRange(ConverterHelper.ByteToBools(bytes[10]));
                a9.AddRange(ConverterHelper.ByteToBools(bytes[11]));
                pumps[2].KNSPumpDatas.FirstOrDefault().Alarm = KNSPumpsAlarm(a9);
            }
            catch (Exception ex) { }
        }

        public static void GetAlarmsLiftWaterPumps ( List<LiftWaterPump> pumps, List<byte []> bytes )
        {
            try
            {
                var a1 = ConverterHelper.ByteToInt16 ( bytes [0] );
                var a2 = ConverterHelper.ByteToInt16 ( bytes [1] );
                var a3 = ConverterHelper.ByteToInt16 ( bytes [2] );
                var a4 = ConverterHelper.ByteToInt16 ( bytes [3] );
                if (a1 > 0) pumps [0].LiftWaterPumpDatas.FirstOrDefault ().Alarm = PumpsAlarms ( a1 );
                if (a2 > 0) pumps [1].LiftWaterPumpDatas.FirstOrDefault ().Alarm = PumpsAlarms ( a2 );
                if (a3 > 0) pumps [2].LiftWaterPumpDatas.FirstOrDefault ().Alarm = PumpsAlarms ( a3 );
                if (a4 > 0) pumps [3].LiftWaterPumpDatas.FirstOrDefault ().Alarm = PumpsAlarms ( a4 );
            }
            catch (Exception ex) { }
        }

        public static void GetTempWellPumpData(List<Well> wells, List<byte[]> bytes)
        {
            try
            {
                var a1 = ConverterHelper.ByteToInt16(bytes[0]);
                var a2 = ConverterHelper.ByteToInt16(bytes[1]);
                var a3 = ConverterHelper.ByteToInt16(bytes[2]);
                var a4 = ConverterHelper.ByteToInt16(bytes[4]);
                var a5 = ConverterHelper.ByteToInt16(bytes[5]);
                var a6 = ConverterHelper.ByteToInt16(bytes[6]);
                wells[0].WellPumps.First().WellPumpDatas.First().Temp = Convert.ToDecimal(a1*0.01);
                wells[0].WellPumps.First().WellPumpDatas.First().Humidity = Convert.ToDecimal(a2 *0.01);
                wells[0].WellPumps.First().WellPumpDatas.First().DewPoint = Convert.ToDecimal(a3*0.01);
                wells[1].WellPumps.First().WellPumpDatas.First().Temp = Convert.ToDecimal(a4*0.01);
                wells[1].WellPumps.First().WellPumpDatas.First().Humidity = Convert.ToDecimal(a5*0.01);
                wells[1].WellPumps.First().WellPumpDatas.First().DewPoint = Convert.ToDecimal(a6 * 0.01);
            }
            catch (Exception ex) { }
        }

        public static OwenDose GetOwenDose(List<byte[]> bytes, DateTime dt,List<byte[]> bytes2=null)
        {
            
            try
            {
                var dose = new OwenDose();
                dose.Sysdate = dt;
                dose.CanFillV1 = Convert.ToDecimal(ConverterHelper.ByteToReal(bytes[1], bytes[0]));
                dose.CanFillV2 = Convert.ToDecimal(ConverterHelper.ByteToReal(bytes[3], bytes[2]));
                dose.Flow1 = Convert.ToDecimal(ConverterHelper.ByteToReal(bytes[5], bytes[4]));
                dose.Flow2 = Convert.ToDecimal(ConverterHelper.ByteToReal(bytes[7], bytes[6]));
                dose.V1Cur = Convert.ToDecimal(ConverterHelper.ByteToReal(bytes[9], bytes[8]));
                dose.V2Cur = Convert.ToDecimal(ConverterHelper.ByteToReal(bytes[11], bytes[10]));
                dose.H1Cur = Convert.ToDecimal(ConverterHelper.ByteToReal(bytes[13], bytes[12]));
                dose.H2Cur = Convert.ToDecimal(ConverterHelper.ByteToReal(bytes[15], bytes[14]));
                dose.FillTime1 = ConverterHelper.ByteToInt16(bytes[16]);
                dose.FillTime2 = ConverterHelper.ByteToInt16(bytes[17]);
                dose.MY8 = ConverterHelper.ByteToInt16(bytes[18]);
                dose.MB16 = ConverterHelper.ByteToInt16(bytes[19]);
                dose.V1CurNew = Convert.ToDecimal(ConverterHelper.ByteToReal(bytes2[1], bytes2[0]));
                dose.V2CurNew = Convert.ToDecimal(ConverterHelper.ByteToReal(bytes2[3], bytes2[2]));
                return dose;
            }
            catch (Exception ex)
            {
                return null;
            }

        }

        public static string KNSPumpsAlarm(List<bool> bools)
        {
            var res = "";
            if (bools[0]) res += " MotorTemperatureAlarmPTC1 (69)";
            if (bools[1]) res += " MotorTemperature alarm PTC2 (70)";
            if (bools[2]) res += " MotorStatorTemp1High (64)";
            if (bools[3]) res += " MotorStatorTemp2High (71)";
            if (bools[4]) res += " MotorSupportBearingTempHigh (145)";
            if (bools[5]) res += " MotorMainBearingTempHigh (146)";
            if (bools[6]) res += " MotorInsulationResistanceLow (20)";
            if (bools[7]) res += " MotorLowVoltage (40)";
            if (bools[8]) res += " MotorHighVoltage (32)";
            if (bools[9]) res += " MotorPhaseSequenceReversal (9)";
            if (bools[10]) res += " MotorOverload, max current (48)";
            if (bools[11]) res += " MotorUnderload, min current (56)";
            if (bools[12]) res += " MotorProtectorActivated (27)";
            if (bools[13]) res += " MotorMissingPhase (2)";
            if (bools[14]) res += " MotorCurrenAsymmetry (111)";
            if (bools[15]) res += " LoadContinuesDespiteSwitchOff (26)";
            if (bools[16]) res += " MotorProtectorCommandedTrip (18)";
            if (bools[17]) res += " MotorPhaseFailure (241)";
            if (bools[18]) res += " MotorMoistureSwitch (22)";
            if (bools[19]) res += " PumpVibrationHigh (24)";
            if (bools[20]) res += " MotorOilWaterContentHigh (11)";
            if (bools[21]) res += " FaultInMainsSupply (6)";
            if (bools[22]) res += " ContactorFeedbackFault (220)";
            if (bools[23]) res += " StartsPerHourLimitExceeded (21)";
            if (bools[24]) res += " TimeForMotorService (12)";
            if (bools[25]) res += " AutoRestartsPer24HoursTooHigh (4)";
            if (bools[26]) res += " PumpLowFlow (58)";
            if (bools[27]) res += " PumpMaxContinousRunTime (245)";
            if (bools[28]) res += " MotorCosPhiHigh (112)";
            if (bools[29]) res += " MotorCosPhiLow (113)";

            if (bools[32]) res += " MalfuncDueToAuxDevice (224)";
            if (bools[33]) res += " PumpModuleCommFault (225)";
            if (bools[34]) res += " PumpRelayManuallyOperated (243)";
            if (bools[35]) res += " SignalFaultPT100Sensor (175)";
            if (bools[36]) res += " SignalFaultPTCSensor  (181)";
            if (bools[37]) res += " SignalFaultWIOSensor (170)";
            if (bools[38]) res += " SupportBearingSensorSigFault (179)";
            if (bools[39]) res += " MainBearingSensorSignalFault (180)";
            if (bools[40]) res += " PumpPowerLineCommFault (10)";
            if (bools[41]) res += " SetupConflict (25)";
            if (bools[42]) res += " GeneralHardwareFault (72)";
            if (bools[43]) res += " MotorCurrentSensorSignalFault (88)";
            if (bools[44]) res += " InrushFault (155)";
            if (bools[45]) res += " SignalFaultSensor 2 (93)";
            if (bools[46]) res += " MotorBearingTempHighDriveEnd (148)";
            if (bools[47]) res += " MotorBearingTempHighNonDriveEnd (149)";
            if (bools[48]) res += " LeakageCurrent (1)";
            if (bools[49]) res += " SignalFaultTemp3Sensor (176)";
            if (bools[50]) res += " SignalFaultFeedbackSensor (89)";
            if (bools[51]) res += " OverCurrent (49)";
            if (bools[52]) res += " CurrentProtectionActivated (55)";
            if (bools[53]) res += " ServiceInfoChangeBearings (30)";
            if (bools[54]) res += " ServiceInfoLubricateBearings (240)";
            if (bools[55]) res += " MotorModelRecognitionFailed (242)";
            if (bools[56]) res += " CommunicationFaultTwinHeadPump (77)";
            if (bools[57]) res += " SignalFaultTemp1Sensor (91)";
            if (bools[58]) res += " DryRunning (57)";
            if (bools[59]) res += " VFDnotReady (213)";
            if (bools[60]) res += " Other (16)";
            if (bools[61]) res += " PumpOrMotorBlocked (51)";
            if (bools[62]) res += " SignalFaultMotorPowerSensor (186)";

            return res;
        }
        public static string PumpsAlarms(int code)
        {
            if (code == 10) return "";
           var dict = new Dictionary<int, string>()
            {
                {1, "1: Ток утечки"},
                {35, "35: Воздух в насосе, проблемадеаэрирования"},
                {76, "76: Внутренняя ошибка связи"},
                {2, "2: Отсутствие одной фазы"},
                {36, "36: Утечка в напорной арматуре"},
                {77, "77: Сбой канала связи, сдвоенныйнасос"},
                {3, "3: Внешняя авария"},
                {37, "37: Утечка во всасывающем клапане"},
                {78, "78: Неисправность, ошибка скорости"},
                {4, "4: Слишком много повторныхвключений"},
                {38, "38: Неисправность клапана вентиляционного"},
                {79, "79: Функциональная модуль расширения неисправность,"},
                {5, "5: Рекуперативное торможение"},
                {40, "40: Пониженное напряжение"},
                {80, "80: Аппаратный сбой типа 2"},
                {6, "6: Сбои в подаче питания"},
                {41, "41: Пониженное переходноенапряжение"},
                {81, "81: Ошибка проверки, область данных(ОЗУ)"},
                {7, "7: Слишком частое отключениеоборудования"},
                {42, "42: Сбой при включении (dV/dt)"},
                {82, "82: Ошибка (ПЗУ, флэш проверки -память , область ) данных"},
                {8, "8: Пониженная частота коммутацииШИМ"},
                {45, "45: Асимметрия напряжения"},
                {83, "83: Ошибка параметра проверки FE (EEPROM) , область"},
                {9, "9: Изменение последовательностифаз"},
                {48, "48: Перегрузка"},
                {84, "84: Ошибка доступа к памяти"},
                {10, "10: Ошибка связи, насос"},
                {49, "49: Перегрузка по току(i_line, i_dc, i_mo)"},
                {85, "85: Ошибка параметра проверки BE (EEPROM) , область"},
                {11, "11: Неисправность из-за воды в масле(масло электродвигателя)"},
                {50, "50: Функция общее выключение защиты электродвигателя (MPF) ,"},
                {88, "88: Неисправность датчика"},
                {12, "12: Время техобслуживания (сведенияоб общем техобслуживании)"},
                {51, "51: Двигатель/насос заблокирован"},
                {89, "89: Ошибка (обратная сигнала связь) датчика 1"},
                {13, "13: Аналоговый аварийный сигнал оповышенной влажности"},
                {52, "52: Повышенное электродвигателя проскальзывание"},
                {90, "90: Ошибка сигнала датчика скорости"},
                {14, "14: Активирована электронная защитазвена пост. тока (ERP)"},
                {53, "53: Двигатель работает импульсами"},
                {91, "91: Ошибка температуры сигнала 1 датчика"},
                {15, "15: Сбой канала связи, основнаясистема (SCADA)"},
                {54, "54: Функция лимит 3 сек защиты . электродвигателя,"},
                {92, "92: Ошибка обратной калибровки связи датчика"},
                {16, "16: Прочее"},
                {55, "55: Активирована защита по токудвигателя (MCP)"},
                {93, "93: Ошибка сигнала датчика 2"},
                {17, "17: Несоответствие требованиюпроизводительности"},
                {56, "56: Неполная нагрузка"},
                {94, "94: Превышение предельного значения,датчик 1"},
                {18, "18: Передается команда аварийногосигнала в дежурном режиме(отключение)"},
                {57, "57: \"Сухой\"ход"},
                {95, "95: Превышение предельного значения,датчик 2"},
                {19, "19: Разрыв мембраны(дозирующий насос)"},
                {58, "58: Низкий расход"},
                {96, "96: Сигнал вне диапазона установленного значения"},
                {20, "20: Низкое сопротивление изоляции"},
                {59, "59: Расхода нет"},
                {97, "97: Сигнал неисправности, входустановленного значения"},
                {21, "21: Превышение количества пусков вчас"},
                {64, "64: Перегрев"},
                {98, "98: Сигнал неисправности, вход длявлияния на установленное значение"},
                {22, "22: Аварийный датчик влажности,цифровой"},
                {65, "65: Температура (t_m, или t_mo, двигателя или t_mo1) 1"},
                {99, "99: Сигнал аналогового неисправности установленного , вход длязначения"},
                {23, "23: Аварийный сигналмикропроцессорного датчикарегулируемого зазора"},
                {66, "66: Температура, электронная системауправления (t_e)"},
                {104, "104: Программное отключение"},
                {24, "24: Вибрация 67Слишком высокая температура,внутренний модульпреобразователя частоты (t_m)"},
                {105, "105: Активирована электронная защитавыпрямителя (ERP)"},
                {25, "25: Конфликт настроек"},
                {68, "68: Температура окружающейсреды/температура воды (t_w)"},
                {106, "106: Активирована инвертора (EIP) электронная защита"},
                {26, "26: Нагрузка остаётся даже послеотключения двигателя"},
                {69, "69: Термореле 1 в двигателе(например, Klixon)"},
                {110, "110: Сдвиг электрическая фазы нагрузки асимметрия ,"},
                {27, "27: Активирована внешняя защитаэлектродвигателя (напр., MP 204)"},
                {70, "70: Термореле (например, 2 термистор в двигателе )"},
                {111, "111: Асимметрия тока"},
                {28, "28: Низкое напряжение аккумулятора"},
                {71, "71: Температура двигателя 2(Pt100, t_mo2)"},
                {112, "112: Слишком мощностибольшой коэффициент"},
                {29, "29: Турбинный режим работы (рабочиеколёса вращаются потокомжидкости в обратном направлении)"},
                {72, "72: Аппаратный сбой типа 1"},
                {113, "113: Слишком низкий коэффициентмощности"},
                {30, "30: Замена подшипников(конкретные сведения отехобслуживании)"},
                {73, "73: Аппаратное отключение (HSD) 120Неисправность вспомогательнойобмотки(однофазный электродвигатель)"},
                {31, "31: Замена варистора(-ов)(конкретные сведения отехобслуживании)"},
                {74, "74: Слишком высокое внутреннеенапряжение питания"},
                {121, "121: Слишком высокий токвспомогательной обмотки(однофазный электродвигатель)"},
                {32, "32: Скачок напряжения;"},
                {75, "75: Слишком низкое напряжениевнутреннего источника питания"},
                {122, "122: Слишком низкий токвспомогательной обмотки(однофазный электродвигатель)"},
                {123, "123: Пусковой конденсатор, низкаяемкость (однофазный двигатель)"},
                {183, "183: Ошибка датчика температуры сигнала дополнительного"},
                {215, "215: Отключение увеличения давления по времени плавного"},
                {124, "124: Рабочий конденсатор, низкаяёмкость (однофазные двигатели)"},
                {184, "184: Ошибка назначения сигнала датчика общего"},
                {216, "216: Аварийный насоса сигнал дежурного"},
                {144, "144: Температура двигателя 3(Pt100, t_mo3)"},
                {185, "185: Неизвестный тип датчика"},
                {217, "217: Аварийный значение общего сигнал датчика , высокое"},
                {145, "145: Высокая температураподшипников (Pt100), в общем иливерхнего подшипника"},
                {186, "186: Сигнал неисправности ваттметра"},
                {218, "218: Аварийный сигнал, низкоезначение общего датчика"},
                {146, "146: Высокая температура подшипника(Pt100), средний подшипник"},
                {187, "187: Сигнал электрического неисправности счётчика"},
                {219, "219: Ненадлежащий сброс давления"},
                {147, "147: Высокая температура подшипника(Pt100), нижний подшипник"},
                {188, "188: Сигнал пользовательского неисправности датчика"},
                {220, "220: Неисправность контактора двигателя , обратная связь"},
                {148, "148: Высокая температура подшипникадвигателя (Pt100) на приводнойстороне (DE)"},
                {189, "189: Сигнал неисправности датчикауровня"},
                {221, "221: Неисправность, обратная связьконтактора мешалки"},
                {149, "149: Высокая температура подшипникадвигателя (Pt100) на неприводнойстороне (NDE)"},
                {190, "190: Превышение порога 1 датчика(например, аварийный уровень прииспользовании в WW)"},
                {222, "222: Время техобслуживания, мешалка"},
                {152, "152: Неисправность связи,дополнительный модуль 191Превышение порога 2 датчика(например, высокий уровень прииспользовании в WW)"
                },
                {223, "223: Превышение максимальногоколичества запусков мешалки в час"},
                {153, "153: Неисправность, аналоговый выход 192Превышение порога 3 датчика(например, перелив прииспользовании в WW)"
                },
                {224, "224: Неисправность насоса(из-за дополнительного компонентаили общей неисправности)"},
                {154, "154: Обрыв связи с дисплеем"},
                {193, "193: Превышения порога 4 датчика"},
                {225, "225: Обрыв связи с модулем насоса"},
                {155, "155: Пусковой бросок тока"},
                {194, "194: Превышение порога 5 датчика"},
                {226, "226: Обрыв связи с модулемввода/вывода"},
                {156, "156: Обрыв связи с внутренниммодулем преобразователя частоты"},
                {195, "195: Превышение порога 6 датчика"},
                {227, "227: Комбинированное событие"},
                {157, "157: Неисправны часы реальноговремени"},
                {196, "196: Работа при пониженнойпроизводительности"},
                {228, "228: Не используется"},
                {158, "158: Сбой при измерении контураоборудования"},
                {197, "197: Работа с пониженным давлением"},
                {229, "229: Не используется"},
                {159, "159: Неисправность CIM(модуль передачи данных)"},
                {198, "198: Работа потребляемой при повышенной мощности"},
                {230, "230: Аварийный сигнал сети"},
                {160, "160: Неисправность SIM-картыGSM- модема"},
                {199, "199: Процесс оценка/расчет вне диапазона /управление (контроль ) /"},
                {231, "231: Ethernet: сервера DHCP Отсутствует IP-адрес с"},
                {168, "168: Ошибка сигнала датчика давления"},
                {200, "200: Авария программы 232Ethernet: Автоматическаяблокировка из-за неправильногоприменения"},
                {169, "169: Ошибка сигнала датчика расхода"},
                {201, "201: Высокий уровень на входевнешнего датчика"},
                {233, "233: Ethernet: Конфликт IP-адресов"},
                {170, "170: Ошибка сигнала датчика воды вмасле"},
                {202, "202: Низкий уровень на входе внешнегодатчика"},
                {236, "236: Неисправность насоса 1"},
                {171, "171: Ошибка сигнала датчикавлажности"},
                {203, "203: Аварийный сигнал, все насосы"},
                {237, "237: Неисправность насоса 2"},
                {172, "172: Ошибка сигнала датчикаатмосферного давления"},
                {204, "204: Рассогласование датчиков"},
                {238, "238: Неисправность насоса 3"},
                {173, "173: Ошибка сигнала датчикаположения ротора (датчика Холла) 205Рассогласованиепоследовательности поплавковыхуровнемеров"
                },
                {239, "239: Неисправность насоса 4"},
                {174, "174: Ошибка сигнала датчика нулевогоположения ротора"},
                {206, "206: Нехватка воды, уровень 1"},
                {240, "240: Смазать подшипники (особыесведения о техобслуживании)"},
                {175, "175: Ошибка сигнала датчикатемпературы 2 (t_mo2)"},
                {207, "207: Утечка воды"},
                {241, "241: Неисправность фаз двигателя"},
                {176, "176: Ошибка сигнала датчикатемпературы 3 (t_mo3)"},
                {208, "208: Кавитации"},
                {242, "242: Сбой распознавания автоматического модели двигателя"},
                {177, "177: Ошибка сигналамикропроцессорного датчикарегулируемого зазора"},
                {209, "209: Неисправность обратного клапана 243Принудительное переключениереле двигателя(в ручном управлении/по команде)"
                }
            };



            if (dict.ContainsKey(code))
                return dict[code];
            else
                return "Not code:" + code.ToString();
        }

        internal static List<CompressorData> GetCompressoDatas(List<byte[]> bytes, DateTime dt)
        {
            var zero = new byte[] {0, 0};
            var c1 = new CompressorData
            {
                CompressorId = 1,
                Press = ConverterHelper.ByteToInt16(bytes[0])*0.001m,
                TempOutput = ConverterHelper.ByteToInt16(bytes[1])*0.1m,
                TempDew = ConverterHelper.ByteToInt16(bytes[2])*0.1m,
                Temp = ConverterHelper.ByteToInt16(bytes[3])*0.1m,
                IsLinearStarter = bytes[8][0] == 0x00,
                IsStarStarter = bytes[9][0] == 0x00,
                IsTriangleStarter = bytes[10][0] == 0x00,
                IsLoading = bytes[11][0] == 0x00,
                IsAlarmButton = bytes[12][0] == 0x00,
                IsDehumidifierMotor = bytes[13][0] == 0x00,
                MMS = ConverterHelper.ByteToInt16(bytes[20]),
                MCM = ConverterHelper.ByteToInt16(bytes[21]),
                GS = ConverterHelper.ByteToInt16(bytes[22]),
                IsAlarmDI = bytes[26][0] == 0x00,
                IsOverloadDI = bytes[27][0] == 0x00,
                IsDrainageDI = bytes[28][0] == 0x00,
                OperationTime = (ConverterHelper.ByteToInt(zero, bytes[32]) + ConverterHelper.ByteToInt(zero, bytes[33]) * 65636) / 3600,
                OperationTimeLoad = (ConverterHelper.ByteToInt(zero, bytes[34]) + ConverterHelper.ByteToInt(zero, bytes[35]) * 65636) / 3600,
                StartCount = ConverterHelper.ByteToInt(zero, bytes[36]) + ConverterHelper.ByteToInt(zero, bytes[37]) * 65636,
                LoadRelay = ConverterHelper.ByteToInt(zero, bytes[38]) + ConverterHelper.ByteToInt(zero, bytes[39]) * 65636,
                StartDehumidifier = ConverterHelper.ByteToInt(zero, bytes[40]) + ConverterHelper.ByteToInt(zero, bytes[41]) * 65636,
                HoursControl = ConverterHelper.ByteToInt(zero, bytes[42]) + ConverterHelper.ByteToInt(zero, bytes[43]) * 65636,
                Sysdate = dt,
                IsRotationProtection = bytes[44][0] == 0x01,
                IsDehumidifierProtaction = bytes[45][0] == 0x01,
                IsAlarmPress = bytes[46][0] == 0x01,
                Coils = ConverterHelper.ByteToInt16(bytes[50]),
                IsActive = Convert.ToString(bytes[50][0], 2).Last() == '1'
            };
            var c2 = new CompressorData
            {
                CompressorId = 2,
                Press = ConverterHelper.ByteToInt16(bytes[4]) * 0.001m,
                TempOutput = ConverterHelper.ByteToInt16(bytes[5]) * 0.1m,
                TempDew = ConverterHelper.ByteToInt16(bytes[6]) * 0.1m,
                Temp = ConverterHelper.ByteToInt16(bytes[7]) * 0.1m,
                IsLinearStarter = bytes[14][0] == 0x00,
                IsStarStarter = bytes[15][0] == 0x00,
                IsTriangleStarter = bytes[16][0] == 0x00,
                IsLoading = bytes[17][0] == 0x00,
                IsAlarmButton = bytes[18][0] == 0x00,
                IsDehumidifierMotor = bytes[19][0] == 0x00,
                MMS = ConverterHelper.ByteToInt16(bytes[23]),
                MCM = ConverterHelper.ByteToInt16(bytes[24]),
                GS = ConverterHelper.ByteToInt16(bytes[25]),
                IsAlarmDI = bytes[29][0] == 0x00,
                IsOverloadDI = bytes[30][0] == 0x00,
                IsDrainageDI = bytes[31][0] == 0x00,
                OperationTime = (ConverterHelper.ByteToInt(zero, bytes[52]) + ConverterHelper.ByteToInt(zero, bytes[53]) * 65636) / 3600,
                OperationTimeLoad = (ConverterHelper.ByteToInt(zero, bytes[54]) + ConverterHelper.ByteToInt(zero, bytes[55]) * 65636) / 3600,
                StartCount = ConverterHelper.ByteToInt(zero, bytes[56]) + ConverterHelper.ByteToInt(zero, bytes[57]) * 65636,
                LoadRelay = ConverterHelper.ByteToInt(zero, bytes[58]) + ConverterHelper.ByteToInt(zero, bytes[59]) * 65636,
                StartDehumidifier = ConverterHelper.ByteToInt(zero, bytes[60]) + ConverterHelper.ByteToInt(zero, bytes[61]) * 65636,
                HoursControl = ConverterHelper.ByteToInt(zero, bytes[62]) + ConverterHelper.ByteToInt(zero, bytes[63]) * 65636,
                Sysdate = dt,
                IsRotationProtection = bytes[47][0] == 0x01,
                IsDehumidifierProtaction = bytes[48][0] == 0x01,
                IsAlarmPress = bytes[49][0] == 0x01,
                Coils = ConverterHelper.ByteToInt16(bytes[51]),
                IsActive = Convert.ToString(bytes[51][0], 2).Last() == '1'
            };

            return new List<CompressorData>() {c1, c2};
        }
    }
}
