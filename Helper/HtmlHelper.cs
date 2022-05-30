using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using OutReader.Model;

namespace OutReader.Helper
{
    public class HtmlHelper
    {
        public static List<Tag> GetTags(string errors)
        {
            var tags = new List<Tag>();
            var urls = new List<string>()
            {
                "http://192.168.25.1/Portal/Portal.mwsl?PriNav=Varstate&v1=Vvod%201&t1=BOOL&v2=emergency_stop&t2=BOOL&v3=PS1&t3=BOOL&v4=PS2&t4=BOOL&v5=PS3&t5=BOOL&v6=PS4&t6=BOOL&v7=\"2.1.1_ready\"&t7=BOOL&v8=\"2.1.2_ready\"&t8=BOOL&v9=\"2.2.1_ready\"&t9=BOOL&v10=\"2.2.2_ready\"&t10=BOOL&v11=\"6.1_ready\"&t11=BOOL&v12=\"6.2_ready\"&t12=BOOL&v13=\"7.1_ready\"&t13=BOOL&v14=\"7.2_ready\"&t14=BOOL&v15=Vvod%202&t15=BOOL&v16=Avar_2podiom&t16=BOOL&v17=\"Pump_2.1.1\"&t17=BOOL&v18=\"Pump_2.1.2\"&t18=BOOL&v19=\"Pump_2.2.1\"&t19=BOOL&v20=\"Pump_2.2.2\"&t20=BOOL&v21=\"Pump_6.1\"&t21=BOOL&v22=\"Pump_6.2\"&t22=BOOL&v23=\"Pump_7.1\"&t23=BOOL&v24=\"Pump_7.2\"&t24=BOOL&v25=Vacuum&t25=BOOL&v26=UF_small&t26=BOOL&v27=UF_big&t27=BOOL&v28=Kislota&t28=BOOL&v29=Cheloch&t29=BOOL&v30=AirFlow&t30=BOOL&v31=Avariya&t31=BOOL&v32=Avar_SMS&t32=BOOL&v33=Sukhoi_hod_2_podiom&t33=BOOL&v34=Avar_2podiom_sms&t34=BOOL&v35=LE1_scaled&t35=FLOATING_POINT&v36=Auto_ON&t36=BOOL",
                "http://192.168.25.1/Portal/Portal.mwsl?PriNav=Varstate&v1=Vkl_posle_sukhogo&t1=FLOATING_POINT&v2=LE_ish_max&t2=FLOATING_POINT&v3=LE_ish_min&t3=FLOATING_POINT&v4=LE3_max&t4=FLOATING_POINT&v5=LE3_min&t5=FLOATING_POINT&v6=\"2.1.1_fault\"&t6=BOOL&v7=\"2.1.2_fault\"&t7=BOOL&v8=\"2.2.1_fault\"&t8=BOOL&v9=\"2.2.2_fault\"&t9=BOOL&v10=\"6.1_fault\"&t10=BOOL&v11=\"6.2_fault\"&t11=BOOL&v12=\"7.1_fault\"&t12=BOOL&v13=\"7.2_fault\"&t13=BOOL&v14=Reset_faults&t14=BOOL&v15=Vacuum_time&t15=DEC&v16=Kolvo-promivok Plan&t16=DEC&v17=Kolvo-promivok Tekuch&t17=DEC&v18=Reset_Narabotka&t18=BOOL&v19=Datchik_chist&t19=DEC&v20=Datchik_ish&t20=DEC&v21=Promyvka on&t21=BOOL&v22=Time_fault&t22=DEC&v23=Time_on_filter2&t23=TIME_OF_DAY&v24=Day_Filter_1&t24=DEC_UNSIGNED&v25=Day_Filter_2&t25=DEC_UNSIGNED&v26=Pereliv_LE3&t26=FLOATING_POINT&v27=Sukhoi_hod_2&t27=FLOATING_POINT",
                "http://192.168.25.1/Portal/Portal.mwsl?PriNav=Varstate&v1=LE2_scaled&t1=FLOATING_POINT&v2=LE3_scaled&t2=FLOATING_POINT&v3=LE4_scaled&t3=FLOATING_POINT&v4=LE5_scaled&t4=FLOATING_POINT&v5=LE6_scaled&t5=FLOATING_POINT&v6=LE7_scaled&t6=FLOATING_POINT&v7=D_LE1&t7=FLOATING_POINT&v8=L_LE1&t8=FLOATING_POINT&v9=D_LE2&t9=FLOATING_POINT&v10=L_LE2&t10=FLOATING_POINT&v11=D_LE3&t11=FLOATING_POINT&v12=L_LE3&t12=FLOATING_POINT&v13=D_LE4&t13=FLOATING_POINT&v14=L_LE4&t14=FLOATING_POINT&v15=D_LE5&t15=FLOATING_POINT&v16=L_LE5&t16=FLOATING_POINT&v17=D_LE6&t17=FLOATING_POINT&v18=L_LE6&t18=FLOATING_POINT&v19=D_LE7&t19=FLOATING_POINT&v20=L_LE7&t20=FLOATING_POINT&v21=\"2.1.1_man\"&t21=BOOL&v22=\"2.1.2_man\"&t22=BOOL&v23=\"2.2.1_man\"&t23=BOOL&v24=\"2.2.2_man\"&t24=BOOL&v25=\"6.1_man\"&t25=BOOL&v26=\"6.2_man\"&t26=BOOL&v27=\"7.1_man\"&t27=BOOL&v28=\"7.2_man\"&t28=BOOL&v29=UDPR1_man&t29=BOOL&v30=UDPR2_man&t30=BOOL&v31=Vacuum_man&t31=BOOL&v32=Airflow_man&t32=BOOL&v33=UF2_man&t33=BOOL&v34=UF10_man&t34=BOOL&v35=Vacuum2_man&t35=BOOL&v36=Promyvka2&t36=BOOL",
                "http://192.168.25.1/Portal/Portal.mwsl?PriNav=Varstate&v1=Promyvka&t1=BOOL&v2=Rezhim_2&t2=BOOL&v3=Rezhim_10&t3=BOOL&v4=LE_ish&t4=BOOL&v5=LE_chist&t5=BOOL&v6=LE3_avar_max&t6=FLOATING_POINT&v7=LE3_avar_min&t7=FLOATING_POINT&v8=LE_chist_max&t8=FLOATING_POINT&v9=LE_chist_min&t9=FLOATING_POINT&v10=LE1_avar_min&t10=FLOATING_POINT&v11=LE1_avar_max&t11=FLOATING_POINT&v12=LE2_avar_min&t12=FLOATING_POINT&v13=LE2_avar_max&t13=FLOATING_POINT&v14=LE4_avar_min&t14=FLOATING_POINT&v15=LE4_avar_max&t15=FLOATING_POINT&v16=LE5_avar_min&t16=FLOATING_POINT&v17=LE5_avar_max&t17=FLOATING_POINT&v18=LE6_avar_min&t18=FLOATING_POINT&v19=LE6_avar_max&t19=FLOATING_POINT&v20=LE7_avar_min&t20=FLOATING_POINT&v21=LE7_avar_max&t21=FLOATING_POINT&v22=Time_on_filter1&t22=TIME_OF_DAY&v23=Interval&t23=DEC"
            };

            using (WebClient client = new WebClient())
            {
                var z = 0;
                foreach (var url in urls)
                {
                    try
                    {
                        string resource = client.DownloadString(url);
                        HtmlDocument html = new HtmlDocument();
                        html.LoadHtml(resource);

                        foreach (var tr in html.DocumentNode.SelectNodes("//*[contains(@class, 'vartableRow')]"))
                        {
                            var tag = new Tag();
                            var i = 0;
                            foreach (var td in tr.SelectNodes("td"))
                            {
                                if (i == 0)
                                {
                                    var input =
                                        td.SelectSingleNode("input")
                                            .GetAttributeValue("value", "")
                                            .Replace("&#x20;", " ").Replace(".", "-").Replace("\"", "").Replace("&quot;", "");
                                    tag.Name = input;
                                    var option = td.SelectSingleNode("select/option[@selected='']");
                                    if (option != null)
                                        tag.TypeClass = option.GetAttributeValue("value", "");
                                }
                                else if (i == 1)
                                    tag.Value = td.InnerText.Replace("&nbsp;", "").Trim();
                                i++;
                            }
                            if (!string.IsNullOrEmpty(tag.Value) && !string.IsNullOrEmpty(tag.TypeClass))
                                tags.Add(tag);
                        }
                    }
                    catch (Exception ex)
                    {
                        errors += string.Format("Html Num:{0} Error:{1} \r\n", z, ex.Message);
                    }
                    z++;
                }
            }

            return tags;
        }

        #region Teploset
        /// <summary>
        /// Вытаскивает список точек теплосетей из csv
        /// http://188.130.243.74:81/?user=mpgvk&password=cPy4x2&menu=mpgvk&action=download&mode=1
        /// </summary>
        /// <returns></returns>
        //public static void GetTeplosets(Teplosets ts, string errors)
        //{
        //    var link = "http://188.130.243.74:81/?user=mpgvk&password=cPy4x2&menu=mpgvk&action=download&mode=1";
        //    var res = new List<Teploset>();
        //    try
        //    {
        //        string fileList = GetCSV(link);
        //        var tempStr = fileList.Split(new char[] {'\r', '\n'}).ToList();

        //        var num_col = new Dictionary<string, int>();
        //        var tt = tempStr[0].Split(';');
        //        for (var i = 0; i < tt.Count(); i++)
        //        {
        //            switch (tt[i])
        //            {
        //                case "Pхв":
        //                    num_col["Press"] = i;
        //                    break;
        //                case "Mпд":
        //                    num_col["Flow"] = i;
        //                    break;
        //                case "Время":
        //                    num_col["Time"] = i;
        //                    break;
        //            }
        //        }

        //        if (!num_col.Any())
        //        {
        //            errors += "CSV не удалось прочитать список домов \r\n";
        //            return;
        //        }
        //        foreach (string item in tempStr.GetRange(2, tempStr.Count - 3))
        //        {
        //            try
        //            {
        //                var tepl = new Teploset();
        //                var t = item.Split(';');
        //                if (!t.Any()) break;
        //                tepl.Name = t[0]; //.Replace("'", "\"");
        //                tepl.Press = ConvertToDecimal(t[num_col["Press"]]);
        //                tepl.Flow = ConvertToDecimal(t[num_col["Flow"]]);
        //                tepl.Date = ConvertToDateTime(t[num_col["Time"]]);

        //                var find = ts.Kots.FirstOrDefault(x => x.Name == tepl.Name);
        //                if (find == null)
        //                {
        //                    var title = Regex.Matches(tepl.Name, @"[А-Яа-яёЁ0-9-]+")
        //                        .Cast<Match>()
        //                        .Aggregate("", (current, str) => current + str.Value);
        //                    var num = Regex.Match(title, @"([0-9]+-)*[0-9]+").Value;
        //                    var str2 = title.Substring(0, title.IndexOf(num) + num.Length);
        //                    find = ts.Kots.FirstOrDefault(x => str2 == x.ShortName);
        //                    if (find != null)
        //                    {
        //                        find.ReplaceName = true;
        //                        find.NewName = tepl.Name;
        //                        errors += string.Format("Изменено название объекта с ( {0} ) на ( {1} )", find.Name, tepl.Name);
        //                    }
        //                }
        //                if (find == null)
        //                {
        //                    tepl.IsNew = true;
        //                    errors += "На сайте теплосетей добавлен новый объект " + tepl.Name;
        //                    ts.Kots.Add(tepl);
        //                }
        //                else
        //                {
        //                    //if (find.Flow != tepl.Flow || find.Press != tepl.Press || tepl.IsEdit)
        //                    //{
        //                    find.Date = tepl.Date;
        //                    find.Flow = tepl.Flow;
        //                    find.Press = tepl.Press;
        //                    find.IsEdit = true;
        //                    //}
        //                }
        //            }
        //            catch (Exception ex)
        //            {
        //                errors += string.Format("CSVLine {0} Error:{1} \r\n", item, ex.Message);
        //            }
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        errors += string.Format("CSV Error:{1} \r\n", e.Message);
        //        return;
        //    }
        //}
        private static string GetCSV(string url)
        {
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            req.Timeout = 120000;
            req.ContentType = @"text/xml;charset=""windows-1251""";
            HttpWebResponse resp = (HttpWebResponse)req.GetResponse();

            StreamReader sr = new StreamReader(resp.GetResponseStream(), Encoding.GetEncoding("windows-1251"));
            string results = sr.ReadToEnd();
            sr.Close();

            return results;
        }

        private static decimal ConvertToDecimal(string value)
        {
            var res = 0.0m;
            if (!string.IsNullOrEmpty(value))
                res = Convert.ToDecimal(value);
            res = Math.Round(res, 2);
            return res;
        }
        private static DateTime ConvertToDateTime(string value)
        {
            var res = DateTime.MinValue;
            if (!string.IsNullOrEmpty(value))
                res = Convert.ToDateTime(value);
            return res;
        }
        #endregion
    }
}
