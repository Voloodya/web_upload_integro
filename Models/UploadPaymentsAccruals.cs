using Integro.InMeta.Runtime;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Web;


namespace AploadPaymentsAccruals.Models
{
    public class UploadPaymentsAccruals
    {
        public string createNewRecording(string nameServer, string user, AccrualPayment[] accrualsPayments)
        {
            // Получить список идентификаторов приложений с указанного центрального ИнМета-сервера.
            string[] idAppCentralServer = DataApplication.GetAppIds(nameServer);

            DataApplication app;
            if (idAppCentralServer.Contains("EstateOrenburg"))
            {
                app = new DataApplication(idAppCentralServer[0], nameServer);
            }
            else { app = null; return "Create application failure"; }


            // Создание сессии (подключение к БД)
            DataSession session = app.CreateSession(user);
            //DataSession session = app.CreateSession();
            //session.Db.BeginTransaction("","");

            // Прописываем свойства, которые нам необходимо прогрузить при первом запросе
            string loadPlan = "<Executor/>";

            Dictionary<string, List<string>> dictMappingOIDwithPersonalAccount = getMappingOIDwithPersonalAccount();


            for (int i = 0; i < accrualsPayments.Length; i++)
            {
                if (dictMappingOIDwithPersonalAccount.ContainsKey(accrualsPayments[i].PersonalAccount))
                {
                    accrualsPayments[i].OIDobject = dictMappingOIDwithPersonalAccount[accrualsPayments[i].PersonalAccount][0];


                    // Получение списка объектов
                    DataObjectList rentApartContrProc;

                    if (session != null)
                    {
                        DataStorage dataStorage = session["Constr/RentPrivatizationApartmentContractProcess"];
                        rentApartContrProc = dataStorage.Query(loadPlan, "OID=?", accrualsPayments[i].OIDobject); // 000859F5EC33
                    }
                    else
                    {
                        return "Create session failure";
                    }

                    // Создание нового объекта
                    //session["Constr/AccrualsPaymentsSocialContract"].AddNew();

                    foreach (Integro.InMeta.Runtime.DataObject racp in rentApartContrProc)
                    {
                        //// Чтение свойств объекта
                        //racp.GetString("ContractNo", "No date");
                        //racp.GetDateTime("ContractDate");

                        ////Запись свойств объекта
                        //racp.SetString("ContractNo", "xxx");
                        //racp.SetDateTime("ContractDate", DateTime.Today);

                        // Добавлению нового дочернего объекта
                        DataObject accrualsPaymentsSocialContract = racp.GetChilds("Constr/AccrualsPaymentsSocialContract").AddNew();
                        DataObjectChildList employers = racp.GetChilds("Constr/Employers");
                        DataObject searchEmployer = null;

                        foreach (DataObject employer in employers)
                        {
                            if (employer.GetString("PersonalAccount").Equals(accrualsPayments[i].PersonalAccount))
                            {
                                searchEmployer = employer;
                                break;
                            }
                        }

                        if (searchEmployer != null)
                        {
                            accrualsPaymentsSocialContract.SetLink("Subject", searchEmployer);
                        }

                        // var dateTime2 = DateTime.ParseExact(accrualsPayments[i].DateAccrual, "dd.M.yyyy", null);

                        string[] periodStr = accrualsPayments[i].Period.Split('.');
                        DateTime period = new DateTime(Convert.ToInt32(periodStr[0]), Convert.ToInt32(periodStr[1]), Convert.ToInt32(periodStr[2]));

                        accrualsPaymentsSocialContract.SetString("PersonalAccount", accrualsPayments[i].PersonalAccount ?? String.Empty);
                        accrualsPaymentsSocialContract.SetString("IdAccrual", accrualsPayments[i].IdAccrual ?? String.Empty);
                        accrualsPaymentsSocialContract.SetDateTime("Period", period);
                        accrualsPaymentsSocialContract.SetDouble("Accrual", Convert.ToDouble(accrualsPayments[i].SummAccrual ?? String.Empty));
                        accrualsPaymentsSocialContract.SetDouble("SummPayment", Convert.ToDouble(accrualsPayments[i].SummPayment ?? String.Empty));
                    }
                }
                else
                {
                    accrualsPayments[i].OIDobject = "Not Found";
                }
            }
            //Сохранение объектов в БД
            session.Commit();

            //Запись не найденых ЛС
            writeInFile(accrualsPayments);

            return "Upload successful";
        }

        public void searchAccruals()
        {
            DataApplication app = new DataApplication("estateorenburg");

            // Создание сессии (подключение к БД)
            DataSession session = app.CreateSession();

            // Прописываем свойства, которые нам необходимо прогрузить при первом запросе
            string loadPlan = "<Executor/>";

            // Получение списка объектов
        }

        public Dictionary<string,List<string>> getMappingOIDwithPersonalAccount()
        {
            // Physical Path of Root Folder
            string rootPath = System.Web.HttpContext.Current.Server.MapPath("~/UploadedFiles");

            Dictionary<string, List<string>> dictMappingOIDwithPersonalAccount = new Dictionary<string, List<string>>(); 

            List<string> contentFile = ReadWriteFileTxt.readFile(rootPath + "\\MappingOIDwithPersonalAccount.txt");

            for(int i = 0; i < contentFile.Count; i++)
            {
                string[] oidAccount = contentFile[i].Split(' ');

                if (dictMappingOIDwithPersonalAccount.ContainsKey(oidAccount[0]))
                {
                    dictMappingOIDwithPersonalAccount[oidAccount[0]].Add(oidAccount[1]);
                }
                else if(oidAccount.Length>1)
                {
                    List<string> listPersAccount = new List<string>();
                    listPersAccount.Add(oidAccount[1]);
                    dictMappingOIDwithPersonalAccount.Add(oidAccount[0], listPersAccount);
                }
            }

            return dictMappingOIDwithPersonalAccount;
        }

        public void writeInFile(AccrualPayment[] accrualPayments)
        {
            // Physical Path of Root Folder
            string rootPath = System.Web.HttpContext.Current.Server.MapPath("~/UploadedFiles");

            List<string> personalAccount = new List<string>();

            for(int i = 0; i < accrualPayments.Length; i++)
            {
                if (accrualPayments[i].OIDobject.Equals("Not Found"))
                {
                    personalAccount.Add(accrualPayments[i].PersonalAccount);
                }
            }

            ReadWriteFileTxt.writeFile(personalAccount,rootPath,"NotFoundPersonalAccount"+DateTime.UtcNow.ToString().Replace('.','_'),"txt");

        }

    }
}