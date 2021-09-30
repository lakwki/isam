using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.next.common.domain.epicor;
using com.next.isam.domain.account;

namespace com.next.isam.dataserver.worker
{
    public class NonTradeEpicorInterfaceWorker
    {
        private static NonTradeEpicorInterfaceWorker _instance;
        public EpicorInterfaceFile GLInterfaceFile;
        public EpicorInterfaceFile ARInvoiceInterfaceFile;
        public EpicorInterfaceFile APInvoiceInterfaceFile;
        public EpicorInterfaceFile ReceiptInterfaceFile;
        public EpicorInterfaceFile PaymentInterfaceFile;

        protected NonTradeEpicorInterfaceWorker()
        {

        }

        public static NonTradeEpicorInterfaceWorker Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new NonTradeEpicorInterfaceWorker();
                return _instance;
            }
        }

        public void initialize(int queueId, int sunInterfaceTypeId)
        {
            GLInterfaceFile = new EpicorInterfaceFile(EpicorInterfaceTypeEnum.GL, SourceTypeEnum.NON_TRADE, false, queueId, SunInterfaceTypeRef.isAutoPost(sunInterfaceTypeId));
            ARInvoiceInterfaceFile = new EpicorInterfaceFile(EpicorInterfaceTypeEnum.ARInvoice, SourceTypeEnum.NON_TRADE, false, queueId, SunInterfaceTypeRef.isAutoPost(sunInterfaceTypeId));
            APInvoiceInterfaceFile = new EpicorInterfaceFile(EpicorInterfaceTypeEnum.APInvoice, SourceTypeEnum.NON_TRADE, false, queueId, SunInterfaceTypeRef.isAutoPost(sunInterfaceTypeId));
            ReceiptInterfaceFile = new EpicorInterfaceFile(EpicorInterfaceTypeEnum.Receipt, SourceTypeEnum.NON_TRADE, false, queueId, SunInterfaceTypeRef.isAutoPost(sunInterfaceTypeId));
            PaymentInterfaceFile = new EpicorInterfaceFile(EpicorInterfaceTypeEnum.Payment, SourceTypeEnum.NON_TRADE, false, queueId, SunInterfaceTypeRef.isAutoPost(sunInterfaceTypeId));
        }

        public void dispose()
        {
            GLInterfaceFile = null;
            ARInvoiceInterfaceFile = null;
            APInvoiceInterfaceFile = null;
            ReceiptInterfaceFile = null;
            PaymentInterfaceFile = null;
        }

    }
}
