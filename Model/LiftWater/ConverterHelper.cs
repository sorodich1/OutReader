using System;
using System.Collections.Generic;
using System.Linq;

namespace OutReader.Model.LiftWater
{
    public class ConverterHelper
    {
        public static float ByteToReal(byte[] data)
        {

            return Convert.ToSingle(Math.Round(BitConverter.ToSingle(ToCDAB4(data), 0), 4, MidpointRounding.AwayFromZero));
        }
        public static float ByteToReal(byte[] byte1, byte[] byte2)
        {
           return Convert.ToSingle(Math.Round(BitConverter.ToSingle(ToCDAB4(new[] { byte1[0], byte1[1], byte2[0], byte2[1] }), 0), 4, MidpointRounding.AwayFromZero));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="data">abcd</param>
        /// <returns></returns>
        public static int ByteToInt(byte[] data)
        {
            return BitConverter.ToInt32(ToCDAB4(data), 0);
        }
        public static int ByteToInt(byte[] byte1, byte[] byte2)
        {
            return BitConverter.ToInt32(ToCDAB4(new []{byte1[0],byte1[1],byte2[0],byte2[1]}), 0);
        }
        public static int ByteToInt16(byte[] data)
        {
            return BitConverter.ToInt16(data, 0);
        }
        //public static float UshortToReal(ushort[] data)
        //{
        //    var bytes = data.Select(x=>BitConverter.GetBytes(x));
        //    return 0.0f;
        //}
        public static List<bool> ByteToBools(byte[] data)
        {
            var res = new List<bool>();
            foreach (var b in data)
            {
                var array = Convert.ToString(b, 2).PadLeft(8, '0').ToArray();
                res.AddRange(array.Select(x => x.CompareTo('1') == 0).ToList());
            }
            res.Reverse();
            return res;
        }
        public static List<bool> UshortToBools(ushort data)
        {
            var array = Convert.ToString(data, 2).PadLeft(8, '0').ToArray();
            var res = array.Select(x=>x.CompareTo('1')==0).ToList();
            for (int i = res.Count(); i < 16; i++)
            {
                res.Add(false);
            }
            return res;
        }

        public static byte[] ToCDAB4(byte[] data)
        {
            return new byte[]{data[2],data[3],data[0],data[1]};
        }
        
    }
}