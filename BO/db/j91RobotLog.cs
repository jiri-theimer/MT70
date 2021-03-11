﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BO
{
    public enum j91RobotTaskFlag
    {
        Start = 0,
        p40 = 1,
        MailQueue = 2,
        ImapImport = 3,
        ScheduledReports = 4,
        SqlTasks = 5,
        CnbKurzy = 6,
        CentralPing = 7,
        DbBackup = 8,
        RecurrenceP41 = 9,
        RecurrenceP56 = 10,
        ClearTemp = 11,
        AutoWorkflowSteps = 12,
        p67 = 13,
        x46Notify = 14
    }

    public class j91RobotLog:BaseBO
    {
        public int j91ID { get; set; }
        public DateTime j91Date { get; set; }
        public string j91BatchGuid { get; set; }
        public j91RobotTaskFlag j91TaskFlag { get; set; }
        public string j91InfoMessage { get; set; }
        public string j91ErrorMessage { get; set; }
        public string j91Account { get; set; }
    }
}
