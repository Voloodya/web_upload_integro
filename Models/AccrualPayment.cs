using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AploadPaymentsAccruals.Models
{
    public class AccrualPayment
    {
        public string OIDobject { get; set; }
        public string PersonalAccount { get; set; }
        public string IdAccrual { get; set; }
        public string Period { get; set; }
        public string SummAccrual { get; set; }
        public string SummPayment { get; set; }

        public AccrualPayment(string oidObject, string personalAccount, string idAccrual, string period, string summAccrual, string summPayment)
        {
            OIDobject = oidObject;
            PersonalAccount = personalAccount;
            IdAccrual = idAccrual;
            Period = period;
            SummAccrual = summAccrual;
            SummPayment = summPayment;
        }
    }

}