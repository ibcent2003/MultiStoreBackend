using Project.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Project.Models
{
    public class CodeGenerator
    {
        private PROEntities db = new PROEntities();
        public CodeGenerator()
        {
            // this.db = new DBEntities(ConfigurationManager.ConnectionStrings["AppDb"].ConnectionString);
        }
        public string GenerateUniqueIDFromDate()
        {
            try
            {
                string UniqueID = "";
                DateTime TheDate = DateTime.Now;
                string StrDate = TheDate.ToString("dd-MM-yyyy");
                string[] DateSplit = StrDate.Split(new Char[] { '-' });
                string day = DateSplit[0];
                string month = DateSplit[1];
                string year = DateSplit[2];
                Dictionary<string, string> Months = new Dictionary<string, string>
            {
            {"01","25"},
            {"02","68"},
            {"03","69"},
            {"04","31"},
            {"05","42"},
            {"06","90"},
            {"07","38"},
            {"08","40"},
            {"09","56"},
            {"10","63"},
            {"11","45"},
            {"12","90"}
            };

                Dictionary<string, string> Days = new Dictionary<string, string>
            {
            {"01" , "50"},
            {"02" , "31"},
            {"03" , "23"},
            {"04" , "12"},
            {"05" , "54"},
            {"06" , "67"},
            {"07" , "87"},
            {"08" , "90"},
            {"09" , "11"},
            {"10" , "34"},
            {"11" , "22"},
            {"12" , "38"},
            {"13" , "88"},
            {"14" , "78"},
            {"15" , "33"},
            {"16" , "54"},
            {"17" , "67"},
            {"18" , "77"},
            {"19" , "29"},
            {"20" , "59"},
            {"21" , "17"},
            {"22" , "32"},
            {"23" , "44"},
            {"24" , "66"},
            {"25" , "00"},
            {"26" , "04"},
            {"27" , "05"},
            {"28" , "03"},
            {"29" , "08"},
            {"30" , "20"},
            {"31" , "45"}
            };

                Dictionary<string, string> Years = new Dictionary<string, string>
            {
            {"2013" , "33"},
            {"2014" , "44"},
            {"2015" , "55"},
            {"2016" , "66"},
            {"2017" , "77"},
            {"2018" , "88"},
            {"2019" , "99"},
            {"2020" , "31"},
            {"2021" , "52"},
            {"2022" , "14"},
            {"2023" , "24"},
            {"2024" , "57"},
            {"2025" , "68"},
            {"2026" , "30"},
            {"2027" , "70"},
            {"2028" , "73"},
            {"2029" , "87"},
            {"2030" , "62"},
            {"2031" , "91"},
            {"2032" , "83"},
            {"2033" , "34"},
            {"2034" , "45"},
            {"2035" , "48"}
            };
                string CodeDay = Days.First(x => x.Key == DateSplit[0]).Value;
                string CodeMonth = Months.First(x => x.Key == DateSplit[1]).Value;
                string CodeYear = Years.First(x => x.Key == DateSplit[2]).Value;
                string DateCode = CodeDay + CodeMonth + CodeYear;
                string nextid = GetNextID(DateCode).ToString("0000");
                UniqueID = DateCode + nextid;
                return UniqueID;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

        }

        public string GenerateUniqueIDFromDate(DateTime TheDate)
        {
            try
            {
                string UniqueID = "";
                string StrDate = TheDate.ToString("dd-MM-yyyy");
                string[] DateSplit = StrDate.Split(new Char[] { '-' });
                string day = DateSplit[0];
                string month = DateSplit[1];
                string year = DateSplit[2];
                Dictionary<string, string> Months = new Dictionary<string, string>
            {
            {"01","25"},
            {"02","68"},
            {"03","69"},
            {"04","31"},
            {"05","42"},
            {"06","90"},
            {"07","38"},
            {"08","40"},
            {"09","56"},
            {"10","63"},
            {"11","45"},
            {"12","90"}
            };

                Dictionary<string, string> Days = new Dictionary<string, string>
            {
            {"01" , "50"},
            {"02" , "31"},
            {"03" , "23"},
            {"04" , "12"},
            {"05" , "54"},
            {"06" , "67"},
            {"07" , "87"},
            {"08" , "90"},
            {"09" , "11"},
            {"10" , "34"},
            {"11" , "22"},
            {"12" , "38"},
            {"13" , "88"},
            {"14" , "78"},
            {"15" , "33"},
            {"16" , "54"},
            {"17" , "67"},
            {"18" , "77"},
            {"19" , "29"},
            {"20" , "59"},
            {"21" , "17"},
            {"22" , "32"},
            {"23" , "44"},
            {"24" , "66"},
            {"25" , "00"},
            {"26" , "04"},
            {"27" , "05"},
            {"28" , "03"},
            {"29" , "08"},
            {"30" , "20"},
            {"31" , "45"}
            };

                Dictionary<string, string> Years = new Dictionary<string, string>
            {
            {"2013" , "33"},
            {"2014" , "44"},
            {"2015" , "55"},
            {"2016" , "66"},
            {"2017" , "77"},
            {"2018" , "88"},
            {"2019" , "99"},
            {"2020" , "31"},
            {"2021" , "52"},
            {"2022" , "14"},
            {"2023" , "24"},
            {"2024" , "57"},
            {"2025" , "68"},
            {"2026" , "30"},
            {"2027" , "70"},
            {"2028" , "73"},
            {"2029" , "87"},
            {"2030" , "62"},
            {"2031" , "91"},
            {"2032" , "83"},
            {"2033" , "34"},
            {"2034" , "45"},
            {"2035" , "48"}
            };
                string CodeDay = Days.First(x => x.Key == DateSplit[0]).Value;
                string CodeMonth = Months.First(x => x.Key == DateSplit[1]).Value;
                string CodeYear = Years.First(x => x.Key == DateSplit[2]).Value;
                string DateCode = CodeDay + CodeMonth + CodeYear;
                string nextid = GetNextID(DateCode).ToString("0000");
                UniqueID = DateCode + nextid;
                return UniqueID;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

        }

        public int GetNextID(string TableName)
        {
            try
            {
                var Tab = (from t in db.DataGenerator where t.TableName == TableName select t).FirstOrDefault();
                if (Tab != null)
                {
                    Tab.TableID += 1;
                    db.DataGenerator.Context.SaveChanges();
                    return Tab.TableID;
                }
                else
                {
                    db.DataGenerator.AddObject(new DataGenerator { TableName = TableName, TableID = 1 });
                    db.DataGenerator.Context.SaveChanges();
                    return 1;
                }

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return 0;
            }

        }

        public string PadZeroes(int length, int number)
        {
            string format = "D" + length.ToString();
            string paddednumber = number.ToString(format);
            return paddednumber;
        }
    }
}