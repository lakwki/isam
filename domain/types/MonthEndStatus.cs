using System;
using System.Collections;
using com.next.common.domain;
using com.next.common.domain.types;

namespace com.next.isam.domain.types
{
    [Serializable()]
    public class MonthEndStatus : DomainData
    {
        public static MonthEndStatus NOTREADY = new MonthEndStatus(Code.NotReady);
        public static MonthEndStatus READY = new MonthEndStatus(Code.Ready);
        public static MonthEndStatus START = new MonthEndStatus(Code.Start);
        public static MonthEndStatus VERIFY = new MonthEndStatus(Code.Verify);
        public static MonthEndStatus READYTOCAPTURE = new MonthEndStatus(Code.ReadyToCapture);
        public static MonthEndStatus CAPTURED = new MonthEndStatus(Code.Captured);
        public static MonthEndStatus COMPLETED = new MonthEndStatus(Code.Completed);
        public static MonthEndStatus FAILED = new MonthEndStatus(Code.Failed);

        private Code _code;

        private enum Code
        {
            NotReady = 0,
            Ready = 1,
            Start = 2,
            Verify = 3,
            ReadyToCapture = 4,
            Captured = 5,
            Failed = 6,
            Completed = -1
        }

        private MonthEndStatus(Code code)
        {
            this._code = code;
        }

        public int Id
        {
            get
            {
                return Convert.ToInt16(_code.GetHashCode());
            }
        }

        public string Description
        {
            get
            {
                switch (_code)
                {
                    case Code.NotReady:
                        return "Not Ready for Month-End Closing";
                    case Code.Ready:
                        return "Ready to Month-End Closing";
                    case Code.Start:
                        return "Start to Month-End Closing";
                    case Code.Verify:
                        return "Verifying Sales Figure";
                    case Code.ReadyToCapture:
                        return "Ready to Capture Sales Figure";
                    case Code.Captured:
                        return "Sales Figure Captured";
                    case Code.Completed:
                        return "Month-End Closing Completed";
                    case Code.Failed:
                        return "Month-End Closing Failed";
                    default:
                        return "UNDEFINED";
                }
            }
        }

        public string ShortName
        {
            get
            {
                switch (_code)
                {
                    case Code.NotReady:
                        return "NOT READY";
                    case Code.Ready:
                        return "READY";
                    case Code.Start:
                        return "STARTED";
                    case Code.Verify:
                        return "VERIFYING";
                    case Code.ReadyToCapture:
                        return "READY TO CAPTURE";
                    case Code.Captured:
                        return "CAPTURED";
                    case Code.Completed:
                        return "COMPLETED";
                    case Code.Failed:
                        return "FAILED";
                    default:
                        return "UNDEFINED";
                }
            }
        }

        public static MonthEndStatus getStatus(int monthEndStatusId)
        {
            if (monthEndStatusId == Code.NotReady.GetHashCode()) return MonthEndStatus.NOTREADY;
            else if (monthEndStatusId == Code.Ready.GetHashCode()) return MonthEndStatus.READY;
            else if (monthEndStatusId == Code.Start.GetHashCode()) return MonthEndStatus.START;
            else if (monthEndStatusId == Code.Verify.GetHashCode()) return MonthEndStatus.VERIFY;
            else if (monthEndStatusId == Code.ReadyToCapture.GetHashCode()) return MonthEndStatus.READYTOCAPTURE;
            else if (monthEndStatusId == Code.Captured.GetHashCode()) return MonthEndStatus.CAPTURED;
            else if (monthEndStatusId == Code.Completed.GetHashCode()) return MonthEndStatus.COMPLETED;
            else if (monthEndStatusId == Code.Failed.GetHashCode()) return MonthEndStatus.FAILED;
            else return MonthEndStatus.NOTREADY; ;
        }

        public static ArrayList getCollectionValues()
        {
            ArrayList list = new ArrayList();

            list.Add(MonthEndStatus.NOTREADY);
            list.Add(MonthEndStatus.READY);
            list.Add(MonthEndStatus.START);
            list.Add(MonthEndStatus.VERIFY);
            list.Add(MonthEndStatus.READYTOCAPTURE);
            list.Add(MonthEndStatus.CAPTURED);
            list.Add(MonthEndStatus.COMPLETED);
            list.Add(MonthEndStatus.FAILED);
            return list;
        }
    }
}
