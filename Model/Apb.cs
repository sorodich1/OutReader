using System;
using System.Collections.Generic;
using System.Linq;
using OutReader.Helper;

namespace OutReader.Model
{
    /// <summary>
    /// APB PLC Псковкирпич
    /// </summary>
    public class Apb
    {
        public Apb()
        {
            Is = new List<bool>();
            Qs = new List<bool>();
            Ms = new List<bool>();
            DWs = new List<int>();
        }
        public Apb(List<bool> ii, List<bool> qs, List<bool> ms, List<byte[]> dws)
        {
            List<byte[]> states = dws.GetRange(0, 6);
            StateWashingPump = ConverterHelper.ByteToInt(states[0], states[1]);
            StateInput = ConverterHelper.ByteToInt(states[2], states[3]);
            StateFilters = Convert.ToString(ConverterHelper.ByteToInt(states[4], states[5]), 2).Select(s => s.Equals('1')).ToList() ??
                           new List<bool>();
            if (StateFilters.Count < 6)
            {
                for (int i = 0; i < 6 - StateFilters.Count; i++)
                {
                    StateFilters.Add(false);
                }
            }

            Is = ii;
            Qs = qs;
            Ms = ms;
            DWs = new List<int>();
            for (int j = 0; j < dws.Count; j += 2)
            {
                DWs.Add(ConverterHelper.ByteToInt(dws[j], dws[j + 1]));
            }
            //14
            FWInput = DWs[15];
            FWWashing = DWs[20];
            FWOutput = DWs[31];
            FlowWaterInput = (DWs[15] + 630313) * 0.1;
            FlowWaterWashing = (DWs[20] + 5936) * 0.1;
            FlowWaterOutput = (DWs[31] + 1510489) * 0.1;
        }
        public Apb(List<byte[]> states, List<byte[]> flow)
        {
            StateWashingPump = ConverterHelper.ByteToInt(states[0], states[1]);
            StateInput = ConverterHelper.ByteToInt(states[2], states[3]);
            StateFilters = Convert.ToString(ConverterHelper.ByteToInt(states[4], states[5]), 2).Select(s => s.Equals('1')).ToList();
            if(StateFilters.Count < 6)
                for (int i = 0; i < 6 - StateFilters.Count; i++)
                {
                    StateFilters.Add(false);
                }
            FlowWaterInput = ConverterHelper.ByteToReal(flow[0], flow[1]);
            FlowWaterWashing = ConverterHelper.ByteToReal(flow[2], flow[3]);
        }

        /// <summary>
        /// 0 - выкл
        /// 1 - вкл
        /// 2 - авария
        /// </summary>
        public int StateWashingPump { get; set; }

        /// <summary>
        /// 0 - нижний уровень РЧВ
        /// 1 - средний уровень РЧВ
        /// 2 - верхний уровень РЧВ
        /// </summary>
        public int StateInput { get; set; }

        /// <summary>
        /// 0 - Блокировка
        /// 1 - работа
        /// 2 - сброс давления
        /// 3 - промывка
        /// </summary>
        public List<bool> StateFilters { get; set; }
        public List<bool> Is { get; set; }
        public List<bool> Qs { get; set; }
        public List<bool> Ms { get; set; }
        public List<int> DWs { get; set; }
        public List<byte[]> Ds { get; set; }

        public double FlowWaterInput { get; set; }
        public double FlowWaterWashing { get; set; }
        public double FlowWaterOutput { get; set; }

        public int FWInput { get; set; }
        public int FWWashing { get; set; }
        public int FWOutput { get; set; }
        

        public string Filter1ToString
        {
            get { return string.Join("", StateFilters.GetRange(0, 2).Select(x => x ? "1" : "0")); }
        }
        public string Filter2ToString
        {
            get { return string.Join("", StateFilters.GetRange(2, 2).Select(x => x ? "1" : "0")); }
        }
        public string Filter3ToString
        {
            get { return string.Join("", StateFilters.GetRange(4, 2).Select(x => x ? "1" : "0")); }
        }
        public string AllToString()
        {
            var res = string.Format("Apb: {0}, I={1}, Q={2}, M={3}, DW={4}", DateTime.Now,
                    string.Join("", Is.Select(Convert.ToInt32)), string.Join("", Qs.Select(Convert.ToInt32)), string.Join("", Ms.Select(Convert.ToInt32)),string.Join(",", DWs));
                return res;
           
        }
    }
}
