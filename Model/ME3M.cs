﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using OutReader.Helper;
using System.Threading.Tasks;

namespace OutReader.Model
    {
    public class ME3M
        {
        public ME3M() { }
        public ME3M(decimal au, decimal bu, decimal cu, decimal ai, decimal bi, decimal ci)
            {
            Au = au;
            Bu = bu;
            Cu = cu;
            Ai = ai;
            Bi = bi;
            Ci = ci;
            }
        public decimal Ai { get; set; }
        public decimal Bi { get; set; }
        public decimal Ci { get; set; }
        public decimal Au { get; set; }
        public decimal Bu { get; set; }
        public decimal Cu { get; set; }

        public override string ToString()
            {
            return string.Format("Ai:{0:F},Bi:{1:F},Ci:{2:F},Au:{3:F},Bu:{4:F},Cu:{5:F}", Ai, Bi, Ci, Au, Bu, Cu);
            }

        public string MeQuery(string knsId, DateTime dt)
            {
            return string.Format("IF NOT EXISTS(SELECT * FROM TeleskopTns tt WHERE tt.TeleskopId=(SELECT TOP 1 Id FROM Teleskops WHERE Name='{1}') AND A={2} AND B={3} AND C={4}) " +
                "Insert TeleskopTns (Sysdate, A, B, C, TeleskopId) VALUES ('{0}', {2}, {3}, {4}, (SELECT TOP 1 Id FROM Teleskops WHERE Name='{1}'));", dt, knsId, Au.ToString(CultureInfo.InvariantCulture), Bu.ToString(CultureInfo.InvariantCulture), Cu.ToString(CultureInfo.InvariantCulture));
            }
        }
    }