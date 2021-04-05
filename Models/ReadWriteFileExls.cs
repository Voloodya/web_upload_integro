using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Web;

namespace AploadPaymentsAccruals.Models
{
    public class ReadWriteFileExls
    {
        string fileDirectory;
        string fileName;
        string fileType;
        List<Dictionary<string, string>> dataExcelFile; //Данные из Excel файла
        char[] charsToTrim = { ' ', '\n', '\r', '\'', '\'' };

        private OleDbConnectionStringBuilder  CreateConnectExcelStrBuilder(string way, string filename, string typeFile, string HDR, bool hdr, bool IMEX)
        {
            string path = way + @"\" + filename + "." + typeFile;
           
            OleDbConnectionStringBuilder connectStrbuilder = new OleDbConnectionStringBuilder();
            connectStrbuilder.Provider = "Microsoft.ACE.OLEDB.12.0";
            connectStrbuilder.DataSource = path;
            if (hdr && IMEX)
                connectStrbuilder.Add("Extended Properties", String.Format("Excel 12.0 Xml;HDR={0}; IMEX=1", HDR));
            else if (hdr)
                connectStrbuilder.Add("Extended Properties", String.Format("Excel 12.0 Xml;HDR={0};", HDR));
            else
                connectStrbuilder.Add("Extended Properties", String.Format("Excel 12.0 Xml;"));

            return connectStrbuilder;
        }

        private void RedNameListXls(string way, string filename)
        {

            OleDbConnectionStringBuilder DataTableConnect = CreateConnectExcelStrBuilder(way, filename, "xls", "YES", true, true);

            using (OleDbConnection connection = new OleDbConnection(DataTableConnect.ConnectionString))
            {
                connection.Open();

                // Получаем списко листов в файле
                DataTable schemaBook = connection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables,
                            new object[] { null, null, null, "TABLE" });

                connection.Close();
            }
        }

        private void Redxls(string way, string filename, string nameList, string startColumn, string finishColumn, string countRows, string nameColumnDefault)
        {
            var DataTableConnect = CreateConnectExcelStrBuilder(way, filename, "xls", "NO", true, true);

            using (OleDbConnection connection = new OleDbConnection(DataTableConnect.ConnectionString))
            {
                connection.Open();

                //Вывод заданного диапазон в 1-ый DataGridView

                //Получаем конкретный диапазон
                string selectSql = String.Format(@"SELECT * FROM [{0}{1}0:{2}{3}]", nameList, startColumn, finishColumn, countRows);

                OleDbCommand excelDbCommand = new OleDbCommand(selectSql, connection);

                DataTable dataTable = new DataTable();
                using (OleDbDataAdapter adapter = new OleDbDataAdapter(excelDbCommand))
                {
                    adapter.Fill(dataTable);
                }

                //Вывод всех значений 1-ой строки (заголовок таблицы) в столбец 2-го DataGridView
                List<string> nameColumns = new List<string>();
                OleDbDataReader reader = excelDbCommand.ExecuteReader();
                reader.Read();

                //Считывание ячеек строки с заголовками
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    nameColumns.Add(reader[i].ToString());
                }
                //Считывание оставшихся строк с содержимым
                dataExcelFile = new List<Dictionary<string, string>>();
                while (reader.Read())
                {
                    Dictionary<string, string> dict = new Dictionary<string, string>();

                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        dict.Add(nameColumns[i], reader[i].ToString());
                    }
                    dataExcelFile.Add(dict);
                }

                reader.Close();
                connection.Close();
            }
        }

        public List<Dictionary<string, string>> ClearExcelData(List<Dictionary<string, string>> dataExcelFile)
        {
            foreach (Dictionary<string, string> strExcelFile in dataExcelFile)
            {
                foreach (string key in strExcelFile.Keys)
                {
                    strExcelFile[key] = strExcelFile[key].Trim(charsToTrim);
                }
            }
            return dataExcelFile;
        }
    }
}