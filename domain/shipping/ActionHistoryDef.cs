using System;
using com.next.common.domain;
using com.next.isam.domain.common;
using com.next.isam.domain.types;
using com.next.common.appserver;

namespace com.next.isam.domain.shipping
{
    [Serializable()]
    public class ActionHistoryDef : DomainData
    {
        private int actionHistoryId;
        private int shipmentId;
        private int splitShipmentId;
        private ActionHistoryType actionHistoryType;
        private AmendmentType amendmentType;
        private string remark;
        private DateTime actionDate;
        private int status;
        private UserRef user;

        public ActionHistoryDef() {}

        public ActionHistoryDef(int shpmtId, int splitShpmtId, ActionHistoryType actionType, AmendmentType amendType, string rmk, int userId)
        {
            // for data amendment
            actionHistoryId = -1;
            shipmentId = shpmtId;
            splitShipmentId = splitShpmtId;
            actionHistoryType = actionType;
            remark = rmk;
            amendmentType = amendType;
            actionDate = DateTime.Now;
            status = GeneralCriteria.TRUE;
            user = GeneralManager.Instance.getUserByKey(userId);
        }

        public ActionHistoryDef(int shpmtId, int splitShpmtId, ActionHistoryType actionType, string rmk, int s, int userId)
        {
            // just for logging the result of a action.
            actionHistoryId = -1;
            shipmentId = shpmtId;
            splitShipmentId = splitShpmtId;
            actionHistoryType = actionType;
            amendmentType = null;
            remark = rmk;
            actionDate = DateTime.Now;
            status = s;
            user = GeneralManager.Instance.getUserByKey(userId);
        }

        public int ActionHistoryId
        {
            get { return actionHistoryId; }
            set { actionHistoryId = value; }
        }

        public int ShipmentId
        {
            get { return shipmentId; }
            set { shipmentId = value; }
            
        }

        public int SplitShipmentId
        {
            get { return splitShipmentId; }
            set { splitShipmentId = value; }

        }

        public ActionHistoryType ActionHistoryType
        {
            get { return actionHistoryType; }
            set { actionHistoryType = value; }
        }

        public AmendmentType AmendmentType
        {
            get { return amendmentType; }
            set { amendmentType = value; }
        }

        public string Remark
        {
            get { return remark; }
            set { remark = value; }
        }

        public DateTime ActionDate
        {
            get { return actionDate; }
            set { actionDate = value; }
        }

        public int Status
        {
            get { return status; }
            set { status = value; }
        }

        public UserRef User
        {
            get { return user; }
            set { user = value; }
        }

    }
}
