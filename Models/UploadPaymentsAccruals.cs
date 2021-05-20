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
        public string createNewRecording(string nameServer)
        {
            // Получить список идентификаторов приложений с указанного центрального ИнМета-сервера.
            string[] idAppCentralServer = DataApplication.GetAppIds(nameServer);

            //DataApplication app = new DataApplication(idAppCentralServer[0], nameServer);
            DataApplication app;
            if (idAppCentralServer.Contains("EstateOrenburg"))
            {
                app = new DataApplication("estateorenburg");
            }
            else { app = null; return "Create application failure"; }

            // Создание сессии (подключение к БД)
            DataSession session = app.CreateSession("AD\\lucenkovlse");
            //session.Db.BeginTransaction("","");

            // Прописываем свойства, которые нам необходимо прогрузить при первом запросе
            string loadPlan = "<Executor/>";

            DateTime dateTime = new DateTime(2021, 03, 31);

            // Получение списка объектов
            DataObjectList rentApartContrProc;

            //DataStorage dataStorage = Session["Constr/RentPrivatizationApartmentContractProcess"];
            //rentApartContrProc = dataStorage.Query(loadPlan, "OID=?", "0009D8090EAE"); // 000859F5EC33

            if (session != null)
            {
                DataStorage dataStorage = session["Constr/RentPrivatizationApartmentContractProcess"];
                rentApartContrProc = dataStorage.Query(loadPlan, "OID=?", "0009D8090EAE"); // 000859F5EC33
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
                    if (employer.GetString("StatusSubject").Equals("Наниматель"))
                    {
                        searchEmployer = employer;
                        break;
                    }
                }

                if (searchEmployer != null)
                {
                    accrualsPaymentsSocialContract.SetLink("Subject", searchEmployer);
                }
                accrualsPaymentsSocialContract.SetString("PersonalAccount", "0000000002");
                accrualsPaymentsSocialContract.SetString("IdAccrual", "123456");
                accrualsPaymentsSocialContract.SetDateTime("DateAccrual", dateTime);
                accrualsPaymentsSocialContract.SetDouble("Accrual", 2000);
            }
            //Сохранение объектов в БД
            session.Commit();
            //Session.Commit();
            return "Upload successful";
        }

        public void searchAccruals()
        {
            DataApplication app = new DataApplication("estateorenburg");

            // Создание сессии (подключение к БД)
            DataSession session = app.CreateSession();

            // Прописываем свойства, которые нам необходимо прогрузить при первом запросе
            string loadPlan = "<Executor/>";

            DateTime dateTime = new DateTime(2021, 01, 02);

            // Получение списка объектов
            DataStorage dataStorage = session["Constr/RentPrivatizationApartmentContractProcess"];
            DataObjectList rentApartContrProc = dataStorage.Query(loadPlan, "OID=?", "0009B470855A");
        }

    }
}