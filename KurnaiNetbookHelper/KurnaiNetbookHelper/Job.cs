using System;

namespace KurnaiNetbookHelper
{
    public class Job
    {
        private Constants _Constants = new Constants();

        public string JobID { get; private set; }
        public string StudentID { get; private set; }
        public string StudentName { get; private set; }
        public byte AllowCollect { get; set; }

        public Job(string jobID, string studentID, string studentName, string allowCollect)
        {
            JobID = jobID;
            StudentID = studentID;
            StudentName = studentName;

            // Set "Allow Collect" state:
            // 0 = deny; 1 = allow; 2 = unknown
            if (allowCollect == _Constants.PAYMENTS_VALUE_DENY.ToString())
                AllowCollect = _Constants.PAYMENTS_VALUE_DENY;
            else if (allowCollect == _Constants.PAYMENTS_VALUE_ALLOW.ToString())
                AllowCollect = _Constants.PAYMENTS_VALUE_ALLOW;
            else
                AllowCollect = _Constants.PAYMENTS_VALUE_UNKNOWN;
        }

        public override string ToString()
        {
            return String.Format("[Ticket #{0}] {1} ({2})", JobID, StudentName, StudentID);
        }
    }
}
