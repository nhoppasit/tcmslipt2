using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace BO23_GUI_idea.Classes
{
    public static class OnlineService
    {
        public static bool SearchCarTag(string carTag)
        {
            try
            {
                string result;
                ServiceReference1.BKM00720ServiceClient svc = new ServiceReference1.BKM00720ServiceClient();
                result = svc.Truck_Validate(carTag);
                if (result.Equals("")) { return true; }
                else { return false; }
            }
            catch (Exception ex) { throw ex; }
        }

        public static bool SearchBO23(string orgCode, string carTag, out ServiceReference1.SearchModel result)
        {
            try
            {
                ServiceReference1.BKM00720ServiceClient svc = new ServiceReference1.BKM00720ServiceClient();
                result = svc.Search(orgCode, carTag);
                if (result.Result.Equals("")) { return true; }
                else { return false; }
            }
            catch (Exception ex) { throw ex; }
        }

        public static bool SearchTicketList(string orgCode, string carTag, out ServiceReference1.TicketModel result)
        {
            try
            {
                ServiceReference1.BKM00720ServiceClient svc = new ServiceReference1.BKM00720ServiceClient();
                result = svc.GetTicketLst(orgCode, carTag);
                if (result.Result.Equals("")) { return true; }
                else { return false; }
            }
            catch (Exception ex) { throw ex; }
        }

        public static bool AddTicket(string basketCode, string docNumber, string orgCode, int quantity, string TicketNo, string user, out string resDesc)
        {
            try
            {
                ServiceReference1.ResultModel item = new ServiceReference1.ResultModel();
                ServiceReference1.BKM00720ServiceClient svc = new ServiceReference1.BKM00720ServiceClient();

                item.BasketCode = basketCode;
                item.DocNumber = docNumber;
                item.OrgCode = orgCode;
                item.Quantity = quantity;
                item.TicketNo = TicketNo;
                item.User = user;

                string result = svc.AddTicket(item);

                if (result.Equals("")) { resDesc = result; return true; }
                else { resDesc = result; return false; }
            }
            catch (Exception ex) { throw ex; }
        }

    }
}
