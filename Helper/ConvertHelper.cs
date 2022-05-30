using System;
using System.Collections.Generic;
using System.Linq;

namespace OutReader.Helper
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
        public static decimal ByteToDecimal(byte[] byte1, byte[] byte2)
        {
            return Decimal.Round(Convert.ToDecimal(BitConverter.ToSingle(ToCDAB4(new[] { byte1[0], byte1[1], byte2[0], byte2[1] }), 0)), 4);
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

        public static int ByteToInt2(byte[] data)
        {
            return BitConverter.ToInt32(data, 0);
        }

        public static int ByteToInt(byte[] byte1, byte[] byte2)
        {
            return BitConverter.ToInt32(ToCDAB4(new[] { byte1[0], byte1[1], byte2[0], byte2[1] }), 0);
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
            var res = array.Select(x => x.CompareTo('1') == 0).ToList();
            for (int i = res.Count(); i < 16; i++)
            {
                res.Add(false);
            }
            return res;
        }

        public static byte[] ToCDAB4(byte[] data)
        {
            return new byte[] { data[2], data[3], data[0], data[1] };
        }

        public static double ToPress6(decimal current)
        {
            var res = current > 0 ? Convert.ToDouble((current - 4)) * 0.375 : 0;
            if (res < 0) res = 0;
            return res;
        }
        public static decimal ToPress10(decimal current)
        {
            var res = current > 0 ? Convert.ToDecimal((current - 4)) * 0.625m : 0;
            if (res < 0) res = 0;
            return res;
        }
        public static string Reverse(string s)
        {
            char[] charArray = s.ToCharArray();
            Array.Reverse(charArray);
            return new string(charArray);
        }
        public static string ToBitString(int val, bool reverse = true)
        {
            return reverse ? Reverse(Convert.ToString(val, 2)) : Convert.ToString(val, 2);
        }
    }
}
