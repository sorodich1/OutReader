﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Threading;
using OutReader.Model;

namespace OutReader.Helper
{
    public class DbHelper
    {
        private const string connect =
            "Password=eU4nR!~h413h;Persist Security Info=True;User ID=Telemetry;Initial Catalog=Pskov;Data Source=192.168.8.25;";

        private static DataSet GetDataSet(string request, string connectString = "")
        {
            try
            {
                SqlDataAdapter adapter = new SqlDataAdapter(request, string.IsNullOrEmpty(connectString) ? connect : connectString);
                adapter.SelectCommand.CommandTimeout = 0;
                DataSet ds = new DataSet();
                adapter.Fill(ds);
                adapter.Dispose();
                return ds;
            }
            catch (Exception ex)
            {
                //log
                throw ex;
            }
        }

        public static void SetLog(string text, string objectName = "Error", bool isError = true, string objectName2 = "")
        {

            try
            {
                GetDataSet(
                    string.Format(
                        "INSERT OutReaderLogsNew (Log, ObjectName, IsError, Sysdate, ObjectName2) VALUES (N'{0}', '{1}', {2}, SYSDATETIME(), '{3}');",
                        text, objectName, isError ? 1 : 0, objectName2));
            }
            catch (Exception ex)
            {
            }
        }

        public static void SetStreamLuxLog(StreamLux o)
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("en-GB");
            GetDataSet(string.Format(
                "INSERT StreamLuxs (FlowWaterDay, FlowWaterHour, FlowWaterMinute, FlowWaterNet, FlowWaterPos, FlowWaterNeg, SignalQuality, ErrorCode, Sysdate, FlowWaterCurrent) " +
                "VALUES ({0:0.00}, {1:0.00}, {2:0.00}, {3:0.00}, {4:0.00}, {5:0.00}, N'{6}', N'{7}', '{8:yyyyMMdd HH:mm:ss}', {9});",
                o.FlowWaterDay, o.FlowWaterHour, o.FlowWaterMinute, o.FlowWaterNet, o.FlowWaterPos, o.FlowWaterNeg,
                o.SignalQuality, o.ErrorCode, o.LastUpdate, o.FlowWaterCurrent));
        }

        public static void SetQueryLiftWater(string query)
        {
            GetDataSet(query, "Password=g7eq7bRzv2lr;Persist Security Info=True;User ID=LiftWater;Initial Catalog=LiftWater;Data Source=192.168.8.25;");
        }

        public static void SetKns(Kns kns)
        {
            var query = "";
            if (kns.KNSId == "KNS20")
            {
                //Алёхина 30 (альфа)
                query += getQueryKNS(kns.LastUpdate, kns.KNSId, 27, kns.MB16Ds[0].DI[0]); //поплавок Низкий Н
                query += getQueryKNS(kns.LastUpdate, kns.KNSId, 28, kns.MB16Ds[0].DI[1]); //поплавок Средний С
                query += getQueryKNS(kns.LastUpdate, kns.KNSId, 29, kns.MB16Ds[0].DI[2]); //поплавок Высокий В
                query += getQueryKNS(kns.LastUpdate, kns.KNSId, 30, kns.MB16Ds[0].DI[3]); //поплавок Авария А
                query += getQueryKNS(kns.LastUpdate, kns.KNSId, 26, kns.MB16Ds[0].DI[4]); //Сухой ход НН
                query += getQueryKNS(kns.LastUpdate, kns.KNSId, 19, !kns.MB16Ds[0].DI[5]); //авария насос 1
                query += getQueryKNS(kns.LastUpdate, kns.KNSId, 20, !kns.MB16Ds[0].DI[6]); //авария насос 2
                query += getQueryKNS(kns.LastUpdate, kns.KNSId, 24, kns.MB16Ds[0].DI[7]); //авария ввод 1
                query += getQueryKNS(kns.LastUpdate, kns.KNSId, 25, kns.MB16Ds[0].DI[8]); //авария ввод 2
                query += getQueryKNS(kns.LastUpdate, kns.KNSId, 62, kns.MB16Ds[0].DI[9]); //защита насос 1
                query += getQueryKNS(kns.LastUpdate, kns.KNSId, 63, kns.MB16Ds[0].DI[10]); //защита насос 2 
                query += getQueryKNS(kns.LastUpdate, kns.KNSId, 64, kns.MB16Ds[0].DI[11]); //переполнение
                query += getQueryKNS(kns.LastUpdate, kns.KNSId, 22, !kns.MB16Ds[0].DI[12]); //Дверь
                query += getQueryKNS(kns.LastUpdate, kns.KNSId, 1, kns.MB16Ds[0].DI[13]); //работа насос 1
                query += getQueryKNS(kns.LastUpdate, kns.KNSId, 2, kns.MB16Ds[0].DI[14]); //работа насос 2
            }

            /////////////////////////////
            else
            {
                if (kns.MB16Ds.Count > 0 || kns.OBEHs.Count > 0 || kns.GKNSs.Count > 0)
                {
                    if (kns.KNSId == "KNS06") //Prigorod
                    {
                        query += getQueryKNS(kns.LastUpdate, kns.KNSId, 22, !kns.MB16Ds[0].DI[0]); //Дверь
                        query += getQueryKNS(kns.LastUpdate, kns.KNSId, 27, kns.MB16Ds[0].DI[4]); //Поплавок, низкий уровень
                        query += getQueryKNS(kns.LastUpdate, kns.KNSId, 28, kns.MB16Ds[0].DI[5]); //Поплавок, средний уровень
                        query += getQueryKNS(kns.LastUpdate, kns.KNSId, 29, kns.MB16Ds[0].DI[6]); //Поплавок,  высокий уровень
                        query += getQueryKNS(kns.LastUpdate, kns.KNSId, 30, kns.MB16Ds[0].DI[7]);//Поплавок,аварийно высокий уровень
                    }
                    //if (kns.KNSId == "KNS14") //выключение защиты двигателей на Алёхина 20
                    //{
                    //    query += getQueryKNS(kns.LastUpdate, kns.KNSId, 62, !kns.MB16Ds[0].DI[0]); //защита насос 1
                    //    query += getQueryKNS(kns.LastUpdate, kns.KNSId, 63, !kns.MB16Ds[0].DI[0]); //защита насос 2

                    //}

                    else if (kns.KNSId == "KNS19") //ОСК
                    {
                        query += getQueryKNS(kns.LastUpdate, kns.KNSId, 22, !kns.MB16Ds[0].DI[0]); //Дверь
                        query += getQueryKNS(kns.LastUpdate, kns.KNSId, 26, kns.MB16Ds[0].DI[3]); //Сухой ход НН
                        query += getQueryKNS(kns.LastUpdate, kns.KNSId, 27, kns.MB16Ds[0].DI[4]); //поплавок Низкий Н
                        query += getQueryKNS(kns.LastUpdate, kns.KNSId, 28, kns.MB16Ds[0].DI[5]); //поплавок Средний С
                        query += getQueryKNS(kns.LastUpdate, kns.KNSId, 30, kns.MB16Ds[0].DI[7]); //поплавок Авария А
                        query += getQueryKNS(kns.LastUpdate, kns.KNSId, 29, kns.MB16Ds[0].DI[6]); //поплавок Высокий В
                    }
                    else if (kns.KNSId == "KNS02") // ЗЗК1
                    {
                        query += getQueryKNS(kns.LastUpdate, kns.KNSId, 10, !kns.MB16Ds[0].DI[1]); //Работа дренажного насоса
                        query += getQueryKNS(kns.LastUpdate, kns.KNSId, 40, kns.MB16Ds[0].DI[12]); //Аварийный уровень дренажного насоса
                        query += getQueryKNS(kns.LastUpdate, kns.KNSId, 3, !kns.MB16Ds[0].DI[11]); //Работа насоса 3
                        query += getQueryKNS(kns.LastUpdate, kns.KNSId, 26, !kns.MB16Ds[0].DI[3]); //Сухой ход НН
                        query += getQueryKNS(kns.LastUpdate, kns.KNSId, 27, !kns.MB16Ds[0].DI[4]); //поплавок Низкий Н
                        query += getQueryKNS(kns.LastUpdate, kns.KNSId, 28, !kns.MB16Ds[0].DI[5]); //поплавок Средний С
                        query += getQueryKNS(kns.LastUpdate, kns.KNSId, 29, !kns.MB16Ds[0].DI[6]); //поплавок Средний В
                        query += getQueryKNS(kns.LastUpdate, kns.KNSId, 30, !kns.MB16Ds[0].DI[7]); //поплавок Средний А
                    }
                    if (kns.KNSId == "KNS21") //Кампус
                    {
                        query += getQueryKNS(kns.LastUpdate, kns.KNSId, 22, kns.MB16Ds[0].DI[0]); //Дверь
                        //query += getQueryKNS(kns.LastUpdate, kns.KNSId, 24, kns.MB16Ds[0].DI[1]); //ввод 1
                        //query += getQueryKNS(kns.LastUpdate, kns.KNSId, 25, kns.MB16Ds[0].DI[2]); //ввод 2
                        query += getQueryKNS(kns.LastUpdate, kns.KNSId, 26, kns.MB16Ds[0].DI[3]); //Сухой ход НН
                        query += getQueryKNS(kns.LastUpdate, kns.KNSId, 27, kns.MB16Ds[0].DI[4]); //поплавок Низкий Н
                        query += getQueryKNS(kns.LastUpdate, kns.KNSId, 28, kns.MB16Ds[0].DI[5]); //поплавок Средний С
                        query += getQueryKNS(kns.LastUpdate, kns.KNSId, 29, kns.MB16Ds[0].DI[6]); //поплавок Высокий В
                        query += getQueryKNS(kns.LastUpdate, kns.KNSId, 30, kns.MB16Ds[0].DI[7]); //поплавок Авария А
                    }
                    //Орлецовская 
                    else if (kns.KNSId == "KNS18")
                    {
                        query += getQueryKNS(kns.LastUpdate, kns.KNSId, 19, kns.OBEHs[0].DI[1]); // авария насос 1
                        query += getQueryKNS(kns.LastUpdate, kns.KNSId, 1, kns.OBEHs[0].DI[2]); //работа насос 1
                        query += getQueryKNS(kns.LastUpdate, kns.KNSId, 20, kns.OBEH_2s[0].DI[1]); // авария насос 2
                        query += getQueryKNS(kns.LastUpdate, kns.KNSId, 2, kns.OBEH_2s[0].DI[2]); //работа насос 2
                        query += getQueryKNS(kns.LastUpdate, kns.KNSId, 21, kns.OBEH_3s[0].DI[1]); // авария насос 3
                        query += getQueryKNS(kns.LastUpdate, kns.KNSId, 3, kns.OBEH_3s[0].DI[2]); //работа насос 3
                        query += getQueryKNS(kns.LastUpdate, kns.KNSId, 24, !kns.OBEH_VRUs[0].DI[2]); //Ввод 1
                        query += getQueryKNS(kns.LastUpdate, kns.KNSId, 25, !kns.OBEH_VRUs[0].DI[3]); //Ввод 2
                        query += getQueryKNS(kns.LastUpdate, kns.KNSId, 71, kns.OBEH_Alarms[0].DI[0]); // авария ИБП
                        query += getQueryKNS(kns.LastUpdate, kns.KNSId, 58, kns.OBEH_Alarms[0].DI[1]); // Авария пожара
                        query += getQueryKNS(kns.LastUpdate, kns.KNSId, 22, kns.OBEH_Alarms[0].DI[2]); // Тревога ОС (Дверь)
                        query += getQueryKNS(kns.LastUpdate, kns.KNSId, 75, kns.OBEH_Alarms[0].DI[3]); // Тревога ГА
                        query += getQueryKNS(kns.LastUpdate, kns.KNSId, 56, kns.OBEH_Alarms[0].DI[4]); // Общая авария КНС
                        query += getQueryKNS(kns.LastUpdate, kns.KNSId, 27, kns.OBEH_levels[0].DI[1]); //поплавок Н
                        query += getQueryKNS(kns.LastUpdate, kns.KNSId, 28, kns.OBEH_levels[0].DI[2]); //поплавок С
                        query += getQueryKNS(kns.LastUpdate, kns.KNSId, 29, kns.OBEH_levels[0].DI[3]); //поплавок В
                        query += getQueryKNS(kns.LastUpdate, kns.KNSId, 30, kns.OBEH_levels[0].DI[4]); //поплавок А
                        query += MB8A_OBEHsQuery(kns.LastUpdate, kns.KNSId, kns.MB8A_OBEHs);
                    }
                    //Псковкирпич
                    else if (kns.KNSId == "KNS16")
                    {
                        query += getQueryKNS(kns.LastUpdate, kns.KNSId, 22, !kns.MB16Ds[0].DI[0]); //Дверь
                        query += getQueryKNS(kns.LastUpdate, kns.KNSId, 27, kns.MB16Ds[0].DI[4]); //поплавок Низкий Н
                        query += getQueryKNS(kns.LastUpdate, kns.KNSId, 29, !kns.MB16Ds[0].DI[7]); //поплавок Высокий В
                        query += getQueryKNS(kns.LastUpdate, kns.KNSId, 28, kns.MB16Ds[0].DI[8]); //поплавок Высокий А
                        query += getQueryKNS(kns.LastUpdate, kns.KNSId, 10, kns.MB16Ds[0].DI[12]); //Работа дренажного насоса
                    }
                   

                    //ГКНС
                    else if (kns.KNSId == "GKNS")
                    {
                        query += getQueryKNS(kns.LastUpdate, kns.KNSId, 1, kns.GKNSs[0].DI[1]); //Работа насоса 1
                        query += getQueryKNS(kns.LastUpdate, kns.KNSId, 2, kns.GKNS_2s[0].DI[1]); //Работа насоса 2
                        query += getQueryKNS(kns.LastUpdate, kns.KNSId, 3, kns.GKNS_3s[0].DI[1]); //Работа насоса 3
                        query += getQueryKNS(kns.LastUpdate, kns.KNSId, 4, kns.GKNS_4s[0].DI[1]); //Работа насоса 4


                    }

                    else if (kns.KNSId != "KNS23" || kns.KNSId != "RNS" || kns.KNSId != "KNS20" || kns.KNSId != "KNS02" || kns.KNSId != "KNS03")
                    {
                        //query += getQueryKNS ( kns.LastUpdate, kns.KNSId, 22, !kns.MB16Ds [0].DI [0] ); //Дверь
                        if (kns.KNSId == "KNS11")
                        {
                            query += getQueryKNS(kns.LastUpdate, kns.KNSId, 24, !kns.MB16Ds[0].DI[1]); //ввод 1
                            query += getQueryKNS(kns.LastUpdate, kns.KNSId, 25, !kns.MB16Ds[0].DI[2]); //ввод 2
                        }
                        else
                        {
                            query += getQueryKNS(kns.LastUpdate, kns.KNSId, 24, kns.MB16Ds[0].DI[1]); //ввод 1
                            query += getQueryKNS(kns.LastUpdate, kns.KNSId, 25, kns.MB16Ds[0].DI[2]); //ввод 2
                        }
                        if (kns.KNSId == "KNS17")
                        {
                            query += getQueryKNS(kns.LastUpdate, kns.KNSId, 27, kns.MB16Ds[0].DI[4]); //поплавок Низкий Н
                            query += getQueryKNS(kns.LastUpdate, kns.KNSId, 28, kns.MB16Ds[0].DI[5]); //поплавок Средний С
                            query += getQueryKNS(kns.LastUpdate, kns.KNSId, 30, kns.MB16Ds[0].DI[11]); //поплавок Авария А
                            query += getQueryKNS(kns.LastUpdate, kns.KNSId, 29, kns.MB16Ds[0].DI[6]); //поплавок Высокий В
                        }
                        else if (kns.KNSId == "KNS12")
                        {
                            query += getQueryKNS(kns.LastUpdate, kns.KNSId, 30, kns.MB16Ds[0].DI[11]); //поплавок Авария А
                            query += getQueryKNS(kns.LastUpdate, kns.KNSId, 29, kns.MB16Ds[0].DI[6]); //поплавок Высокий В
                        }
                        else if (kns.KNSId == "Unit_5249")
                        {
                            query += getQueryKNS(kns.LastUpdate, kns.KNSId, 27, kns.MB16Ds[0].DI[3]); //поплавок Низкий Н
                            query += getQueryKNS(kns.LastUpdate, kns.KNSId, 28, kns.MB16Ds[0].DI[4]); //поплавок Средний С
                            query += getQueryKNS(kns.LastUpdate, kns.KNSId, 29, kns.MB16Ds[0].DI[5]); //поплавок Высокий В
                            query += getQueryKNS(kns.LastUpdate, kns.KNSId, 30, kns.MB16Ds[0].DI[6]); //поплавок Авария А                            
                        }
                        else if (kns.KNSId == "KNS15")
                        {
                            query += getQueryKNS(kns.LastUpdate, kns.KNSId, 26, kns.MB16Ds[0].DI[3]); //Сухой ход НН
                            query += getQueryKNS(kns.LastUpdate, kns.KNSId, 27, !kns.MB16Ds[0].DI[4]); //поплавок Низкий Н
                            query += getQueryKNS(kns.LastUpdate, kns.KNSId, 28, !kns.MB16Ds[0].DI[5]); //поплавок Средний С
                            query += getQueryKNS(kns.LastUpdate, kns.KNSId, 30, !kns.MB16Ds[0].DI[7]); //поплавок Авария А
                            query += getQueryKNS(kns.LastUpdate, kns.KNSId, 29, !kns.MB16Ds[0].DI[7]); //поплавок Высокий В
                            query += getQueryKNS(kns.LastUpdate, kns.KNSId, 1, kns.MB16Ds[0].DI[2]); //работа насос 3
                        }
                        else if (kns.KNSId == "KNS07")
                        {
                            query += getQueryKNS(kns.LastUpdate, kns.KNSId, 26, kns.MB16Ds[0].DI[3]); //Сухой ход НН
                            query += getQueryKNS(kns.LastUpdate, kns.KNSId, 27, kns.MB16Ds[0].DI[4]); //поплавок Низкий Н
                            query += getQueryKNS(kns.LastUpdate, kns.KNSId, 28, kns.MB16Ds[0].DI[5]); //поплавок Средний С
                            query += getQueryKNS(kns.LastUpdate, kns.KNSId, 30, kns.MB16Ds[0].DI[7]); //поплавок Авария А
                            query += getQueryKNS(kns.LastUpdate, kns.KNSId, 29, kns.MB16Ds[0].DI[7]); //поплавок Высокий В
                        }
                        else if (kns.KNSId == "KNS14")
                        {
                            query += getQueryKNS(kns.LastUpdate, kns.KNSId, 27, !kns.MB16Ds[0].DI[3]); //поплавок Низкий Н
                            query += getQueryKNS(kns.LastUpdate, kns.KNSId, 28, !kns.MB16Ds[0].DI[4]); //поплавок Высокий С
                            query += getQueryKNS(kns.LastUpdate, kns.KNSId, 29, !kns.MB16Ds[0].DI[5]); //поплавок Высокий В
                            query += getQueryKNS(kns.LastUpdate, kns.KNSId, 30, !kns.MB16Ds[0].DI[5]); //поплавок Аварийный А
                        }

                        /////////////////////////////
                        ///////////////Маргелова//////////////
                        else if (kns.KNSId == "KNS24")
                        {
                            query += getQueryKNS(kns.LastUpdate, kns.KNSId, 22, !kns.MB16Ds[0].DI[0]);//Дверь
                            query += getQueryKNS(kns.LastUpdate, kns.KNSId, 27, kns.MB16Ds[0].DI[3]);//поплавок Низкий Н
                            query += getQueryKNS(kns.LastUpdate, kns.KNSId, 28, kns.MB16Ds[0].DI[4]); //поплавок средний С
                            query += getQueryKNS(kns.LastUpdate, kns.KNSId, 29, kns.MB16Ds[0].DI[5]); //поплавок высокий В
                            query += getQueryKNS(kns.LastUpdate, kns.KNSId, 30, kns.MB16Ds[0].DI[6]); // поплавок аварийный А
                        }
                        /////////////////////////////
                        //////////Кресты///////////
                        else if (kns.KNSId == "KNS04")
                        {
                            query += getQueryKNS(kns.LastUpdate, kns.KNSId, 22, kns.MB16Ds[0].DI[0]);//Дверь
                            //query += getQueryKNS(kns.LastUpdate, kns.KNSId, 27, kns.MB16Ds[0].DI[4]);//поплавок Ниже-нижнего НН
                            //query += getQueryKNS(kns.LastUpdate, kns.KNSId, 28, kns.MB16Ds[0].DI[5]); //поплавок Низкий Н
                            //query += getQueryKNS(kns.LastUpdate, kns.KNSId, 29, kns.MB16Ds[0].DI[6]); //поплавок средний С
                            //query += getQueryKNS(kns.LastUpdate, kns.KNSId, 30, kns.MB16Ds[0].DI[7]); //поплавок высокий В
                        }
                        /////////////////////////////
                        query += getQueryKNS(kns.LastUpdate, kns.KNSId, 1, kns.MB16Ds[0].DI[8]); //работа насос 1
                        query += getQueryKNS(kns.LastUpdate, kns.KNSId, 2, kns.MB16Ds[0].DI[9]); //работа насос 2
                        query += getQueryKNS(kns.LastUpdate, kns.KNSId, 3, kns.MB16Ds[0].DI[10]); //работа насос 3
                    }

                    if (kns.KNSId == "KNS21") //Кампус
                    {
                        query += getQueryKNS(kns.LastUpdate, kns.KNSId, 3, kns.MB16Ds[0].DI[10]); //работа насос 3
                        if (kns.SBIs != null)
                        {
                            for (var i = 0; i < kns.SBIs.Count; i++)
                            {
                                query += getQueryKNS(kns.LastUpdate, kns.KNSId, 19 + i,
                                    kns.SBIs[i].IsError); //авария насос 1,2,3
                            }
                        }
                        query += getQueryKNS(kns.LastUpdate, kns.KNSId, 67, !kns.MB16Ds[0].DI[12]); //Дверь 2
                        query += getQueryKNS(kns.LastUpdate, kns.KNSId, 68, kns.MB16Ds[0].DI[13]); //Загазованность
                        query += getQueryKNS(kns.LastUpdate, kns.KNSId, 54, kns.MB16Ds[0].DI[14]); //Измельчитель 1
                        query += getQueryKNS(kns.LastUpdate, kns.KNSId, 55, kns.MB16Ds[0].DI[15]); //Авария измельчитель 1
                    }
                    else if (kns.KNSId == "KNS11") //Н.Васильева
                    {
                        query += getQueryKNS(kns.LastUpdate, kns.KNSId, 59, kns.MB16Ds[0].DI[10]); //отопление
                        query += getQueryKNS(kns.LastUpdate, kns.KNSId, 10, kns.MB16Ds[0].DI[11]); //работа дренажного насоса
                        query += getQueryKNS(kns.LastUpdate, kns.KNSId, 40, !kns.MB16Ds[0].DI[13]); //аварийный уровень дренажного приямка
                    }
                    else if (kns.KNSId == "Unit_5249")
                    {
                        query += getQueryKNS(kns.LastUpdate, kns.KNSId, 54, kns.MB16Ds[0].DI[7]); //Измельчитель 1
                        query += getQueryKNS(kns.LastUpdate, kns.KNSId, 3, kns.MB16Ds[0].DI[10]); //работа насос 3
                        query += getQueryKNS(kns.LastUpdate, kns.KNSId, 19, !kns.MB16Ds[0].DI[11]); //авария насос 1
                        query += getQueryKNS(kns.LastUpdate, kns.KNSId, 20, !kns.MB16Ds[0].DI[12]); //авария насос 2
                        query += getQueryKNS(kns.LastUpdate, kns.KNSId, 21, !kns.MB16Ds[0].DI[13]); //авария насос 3
                    }
                    else if (kns.KNSId == "KNS04") //Труда
                    {
                        query += getQueryKNS(kns.LastUpdate, kns.KNSId, 3, kns.MB16Ds[0].DI[10]); //работа насос 3
                        query += getQueryKNS(kns.LastUpdate, kns.KNSId, 42, !kns.MB16Ds[0].DI[11]); //низкий уровень дренажного приямка
                        query += getQueryKNS(kns.LastUpdate, kns.KNSId, 40, !kns.MB16Ds[0].DI[12]); //аварийный уровень дренажного приямка
                    }
                    else if (kns.KNSId == "KNS15")
                    {
                        query += getQueryKNS(kns.LastUpdate, kns.KNSId, 19, !kns.MB16Ds[0].DI[12]);
                        query += getQueryKNS(kns.LastUpdate, kns.KNSId, 3, kns.MB16Ds[0].DI[10]); //работа насос 3
                        query += getQueryKNS(kns.LastUpdate, kns.KNSId, 10, kns.MB16Ds[0].DI[11]); //работа дренажного насоса
                        query += getQueryKNS(kns.LastUpdate, kns.KNSId, 40, kns.MB16Ds[0].DI[12]); //работа дрен level
                        query += getQueryKNS(kns.LastUpdate, kns.KNSId, 59, kns.MB16Ds[0].DI[13]); //отопление
                    }
                    if (kns.KNSId == "KNS15" /*|| kns.KNSId == "KNS07"*/)
                    {
                        query += getQueryKNS(kns.LastUpdate, kns.KNSId, 72, kns.MB16Ds[0].DI[14]); //Сеть 220В восстановлена
                        query += getQueryKNS(kns.LastUpdate, kns.KNSId, 71, kns.MB16Ds[0].DI[15]); //Работает ИБП
                    }
                    if (kns.KNSId == "KNS07")
                    {
                        query += getQueryKNS(kns.LastUpdate, kns.KNSId, 22, !kns.MB16Ds[0].DI[0]);//Дверь
                        query += getQueryKNS(kns.LastUpdate, kns.KNSId, 72, kns.MB16Ds[0].DI[15]); //Сеть 220В восстановлена
                        query += getQueryKNS(kns.LastUpdate, kns.KNSId, 71, kns.MB16Ds[0].DI[14]); //Работает ИБП
                    }
                    if (kns.KNSId == "VOZ")
                    {
                        query += getQueryKNS(kns.LastUpdate, kns.KNSId, 27, !kns.MB16Ds[0].DI[1]); //Низкий уровень VOZ
                        query += getQueryKNS(kns.LastUpdate, kns.KNSId, 28, !kns.MB16Ds[0].DI[2]); //Высокий уровень VOZ
                    }
                    if (kns.KNSId == "KNS02" || kns.KNSId == "KNS03" || kns.KNSId == "RNS" || kns.KNSId == "KNS23")
                    {
                        query += getQueryKNS(kns.LastUpdate, kns.KNSId, 22, false); //Отсутствие двери
                    }

                }

                query += MB8AsQuery(kns.LastUpdate, kns.KNSId, kns.MB8As);

                query += TERsQuery(kns.LastUpdate, kns.KNSId, kns.TERs);
                query += MB8A_KRESTYsQuery(kns.LastUpdate, kns.KNSId, kns.MB8A_KRESTYs);
                query += MB8A_GKNSsQuery(kns.LastUpdate, kns.KNSId, kns.MB8A_GKNSs);

                query += MB8A_OBEHsQuery(kns.LastUpdate, kns.KNSId, kns.MB8A_OBEHs);

                if (kns.SBIs != null && kns.SBIs.Count > 0)
                {
                    foreach (var knsSbI in kns.SBIs)
                    {
                        query += knsSbI.GetQueryDb();
                    }
                }

                if (kns.Tn != null)
                {
                    query += kns.Tn.InsertQuery(kns.KNSId, kns.LastUpdate);
                }
                if (kns.ME3M != null)
                {
                    query += kns.ME3M.MeQuery(kns.KNSId, kns.LastUpdate);
                    GetDataSet(query);
                }
            }

            query += getQueryKNS(kns.LastUpdate, kns.KNSId, 65, false);//нет связи
            GetDataSet(query);
        }
        public static void SetKNSAlehinaAlarm(string knsId)
        {
            var query = getQueryKNS(DateTime.Now, knsId, 65, true);//нет связи
            GetDataSet(query);
        }


        public static string TnQuery(Tn tn, string id, DateTime dt)
        {
            return string.Format("IF NOT EXISTS(SELECT * FROM TeleskopTns tt WHERE tt.TeleskopId=(SELECT TOP 1 Id FROM Teleskops WHERE Name='{1}') AND A={2} AND B={3} AND C={4}) " +
                "Insert TeleskopTns (Sysdate, A, B, C, TeleskopId) VALUES ('{0}', {2}, {3}, {4}, (SELECT TOP 1 Id FROM Teleskops WHERE Name='{1}'));", dt, id, tn.Au1.ToString(CultureInfo.InvariantCulture), tn.Au2, tn.Au3);
        }

        public static List<Kns> GetKns()
        {
            var list = new List<Kns>();
            var ds = GetDataSet(string.Format("SELECT t.Name, t.Title, t.IP, t.Port, t.Id FROM Teleskops t WHERE t.Is3g=1;"));
            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    var kns = new Kns(Convert.ToInt32(row["Port"]), row["IP"].ToString(), row["Name"].ToString(), row["Title"].ToString());
                    list.Add(kns);
                    var ds2 = GetDataSet(string.Format("SELECT ModbusId, Name FROM TeleskopDevices WHERE TeleskopId={0};", row["Id"]));
                    if (ds2.Tables.Count > 0 && ds2.Tables[0].Rows.Count > 0)
                    {
                        foreach (DataRow r in ds2.Tables[0].Rows)
                        {
                            kns.Devices.Add(new KnsDevice(Convert.ToInt32(r["ModbusId"]), r["Name"].ToString()));
                        }
                    }
                }
            }
            return list;
        }
        private static string MB8A_OBEHsQuery(DateTime lastUpdate, string knsId, List<MB8A_OBEH> mb8A_obehs)
        {
            var query = "";
            if (mb8A_obehs != null && mb8A_obehs.Count > 0)
            {
                query += getQueryAnalogKNS_OBEH(lastUpdate, knsId, 1, mb8A_obehs[0].A1); //Ток насоса 1
                query += getQueryAnalogKNS_OBEH(lastUpdate, knsId, 2, mb8A_obehs[0].A2); //Ток насоса 2
                query += getQueryAnalogKNS_OBEH(lastUpdate, knsId, 3, mb8A_obehs[0].A3); //Ток насоса 3
                query += getQueryAnalogKNS(lastUpdate, knsId, 4, mb8A_obehs[0].A4); //Температура
                query += getQueryAnalogKNS(lastUpdate, knsId, 5, mb8A_obehs[0].A5); //Давление чистой воды
                query += getQueryAnalogKNS(lastUpdate, knsId, 40, mb8A_obehs[0].A6); //накопленный расход 1
                query += getQueryAnalogKNS(lastUpdate, knsId, 41, mb8A_obehs[0].A7); //накопленный расход 2
            }

            return query;
        }
        private static string MB8A_KRESTYsQuery(DateTime lastUpdate, string knsId, List<MB8A_KRESTY> mb8A_krestys)
        {
            var query = "";
            if (mb8A_krestys != null && mb8A_krestys.Count > 0)
            {
                query += getQueryAnalogKNS(lastUpdate, knsId, 6, mb8A_krestys[0].A6); // Аналоговые уровни 
            }

            return query;
        }
        private static string MB8A_GKNSsQuery(DateTime lastUpdate, string knsId, List<MB8A_GKNS> mb8A_gknss)
        {
            var query = "";
            if (mb8A_gknss != null && mb8A_gknss.Count > 0)
            {
                query += getQueryAnalogKNS(lastUpdate, knsId, 1, mb8A_gknss[0].A1); //Ток насоса 1
                query += getQueryAnalogKNS(lastUpdate, knsId, 2, mb8A_gknss[0].A2); //Ток насоса 2
                query += getQueryAnalogKNS(lastUpdate, knsId, 3, mb8A_gknss[0].A3); //Ток насоса 3
                query += getQueryAnalogKNS(lastUpdate, knsId, 7, mb8A_gknss[0].A4); //Ток насоса 4
                query += getQueryAnalogKNS(lastUpdate, knsId, 6, mb8A_gknss[0].A6); // Аналоговые уровни 
                query += getQueryAnalogKNS(lastUpdate, knsId, 42, mb8A_gknss[0].A7); //Моментальный расход 1
                query += getQueryAnalogKNS(lastUpdate, knsId, 43, mb8A_gknss[0].A8); //Моментальный расход 2
            }

            return query;
        }

        private static string MB8AsQuery(DateTime lastUpdate, string knsId, List<MB8A> mb8As)
        {
            var query = "";
            if (mb8As != null && mb8As.Count > 0)
            {
                query += getQueryAnalogKNS(lastUpdate, knsId, 1, mb8As[0].A1); //Ток насоса 1
                query += getQueryAnalogKNS(lastUpdate, knsId, 2, mb8As[0].A2); //Ток насоса 2
                query += getQueryAnalogKNS(lastUpdate, knsId, 3, mb8As[0].A3); //Ток насоса 3
                query += getQueryAnalogKNS(lastUpdate, knsId, 4, mb8As[0].A4); //Температура
                query += getQueryAnalogKNS(lastUpdate, knsId, 5, mb8As[0].A5); //Давление чистой воды
                if (knsId == "KNS08")
                {
                    query += getQueryAnalogKNS(lastUpdate, knsId, 6, mb8As[0].A6); // Аналоговые уровни
                }
               

                if (knsId == "KNS06")
                {
                    query += getQueryAnalogKNS(lastUpdate, knsId, 42, mb8As[0].A6); //Моментальный расход 1
                    query += getQueryAnalogKNS(lastUpdate, knsId, 43, mb8As[0].A7); //Моментальный расход 2
                }
            }

            return query;
        }

        private static string TERsQuery(DateTime lastUpdate, string knsId, List<TER> ters)
        {
            var query = "";
            if (ters != null && ters.Count > 0)
            {
                query += getQueryAnalogKNS(lastUpdate, knsId, 40, ters[0].V); //Накопленный расход 1
                query += getQueryAnalogKNS(lastUpdate, knsId, 42, ters[0].Q); //Моментальный расход 1
                if (ters.Count > 1)
                {
                    query += getQueryAnalogKNS(lastUpdate, knsId, 42, ters[1].V); //Накопленный расход 2
                    query += getQueryAnalogKNS(lastUpdate, knsId, 43, ters[1].Q); //Моментальный расход 2
                }
            }

            return query;
        }

        private static string getQueryKNS(DateTime dt, string id, int alarm_id, bool value)
        {
            return string.Format("IF NOT EXISTS(SELECT * FROM teleskop_di WHERE ID IN(SELECT MAX(Id) FROM teleskop_di WHERE TeleskopId = '{1}' AND AlarmId = {2}) AND IsEnabled={3}) " +
                    "INSERT INTO teleskop_di (Sysdate, TeleskopId, AlarmId, IsEnabled) VALUES('{0}','{1}', {2}, {3});\r\n", dt, id, alarm_id, value ? 1 : 0);
        }

        private static string getQueryAnalogKNS(DateTime dt, string id, int alarm_id, decimal value)
        {
            return string.Format("IF NOT EXISTS(SELECT * FROM TeleskopAnalogs WHERE ID IN(SELECT MAX(Id) FROM TeleskopAnalogs WHERE TeleskopId = (SELECT TOP 1 Id FROM Teleskops WHERE Name='{1}') " +
                                 "AND TeleskopAnalogTitleId = {2}) AND Value={3})  INSERT INTO TeleskopAnalogs (TeleskopId, Value, Sysdate, TeleskopAnalogTitleId) " +
                                 "SELECT TOP 1 Id,{3}, '{0}', {2} FROM Teleskops WHERE Name='{1}';\r\n",
                dt, id, alarm_id, value.ToString(CultureInfo.InvariantCulture));
        }
        private static string getQueryAnalogKNS_OBEH(DateTime dt, string id, int alarm_id, int value)
        {
            return string.Format("IF NOT EXISTS(SELECT * FROM TeleskopAnalogs WHERE ID IN(SELECT MAX(Id) FROM TeleskopAnalogs WHERE TeleskopId = (SELECT TOP 1 Id FROM Teleskops WHERE Name='{1}') " +
                                 "AND TeleskopAnalogTitleId = {2}) AND Value={3})  INSERT INTO TeleskopAnalogs (TeleskopId, Value, Sysdate, TeleskopAnalogTitleId) " +
                                 "SELECT TOP 1 Id,{3}, '{0}', {2} FROM Teleskops WHERE Name='{1}';\r\n",
                dt, id, alarm_id, value.ToString(CultureInfo.InvariantCulture));
        }
        
    }
}
