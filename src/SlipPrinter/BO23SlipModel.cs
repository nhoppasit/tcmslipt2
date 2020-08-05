using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SlipPrinter
{
    public class BO23SlipModel
    {
        public string Branch { get; set; }
        public string BranchNbr { get; set; }
        public string Telephone { get; set; }

        public string DateText { get; set; }
        public string TimeText { get; set; }
        public int CopyNbr { get; set; }

        public string CarTag { get; set; }
        public string BO23 { get; set; }
        public int TwoTonecount { get; set; }
        public int SriThaiCount { get; set; }

        public string TicketNbr { get; set; }

        public BO23SlipModel(string branch, string branchNbr, string telephone,
            string dateText, string timeText, int copyNbr,
            string carTag, string bo23, int twoToneCount, int sriThaiCount, string ticketNbr)
        {
            this.Branch = branch;
            this.BranchNbr = branchNbr;
            this.Telephone = telephone;
            this.DateText = dateText;
            this.TimeText = timeText;
            this.CopyNbr = copyNbr;
            this.CarTag = carTag;
            this.BO23 = bo23;
            this.TwoTonecount = twoToneCount;
            this.SriThaiCount = sriThaiCount;
            this.TicketNbr = ticketNbr;
        }

        public override string ToString()
        {
            return "Printing Info: " +DateText + "," + TimeText + "," + CarTag + "," + BO23 + "," + TwoTonecount.ToString() + "," + SriThaiCount.ToString() + "," + TicketNbr;
        }
    }
}
