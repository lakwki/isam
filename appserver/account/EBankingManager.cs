using System;
using System.IO;
using System.Collections;
using com.next.infra.util;
using com.next.isam.dataserver.worker;
using com.next.common.domain.types;
using com.next.isam.domain.account;

namespace com.next.isam.appserver.account
{
	public class EBankingManager
	{
		public static EBankingManager _instance;
        private eBankingWorker ebWorker;
		private CommonWorker _commonWorker;        
        //private static int appId =  AccessMapper.ACC.Id;
			
		public EBankingManager()
		{
			ebWorker = eBankingWorker.Instance;
			_commonWorker = CommonWorker.Instance;
		}

		public static EBankingManager Instance
		{
			get 
			{
				if (_instance == null)
				{
					_instance = new EBankingManager();
				}
				return _instance;
			}
		}

        public int genPaymentImportFile(ConvertPaymentFileRequestDef request, int userId)
        {

            if (!File.Exists(request.FileName))
                return -1;

            ebWorker.updateConvertPaymentFileRequest(request);
                        
            WebConfig.getValue("appSettings", "UPLOAD_EBANKING_TEMP_Folder");
            string sourceFile = request.FileName.Substring(request.FileName.LastIndexOf("\\") + 1);
            string destinationPath;

            if (request.Bank == 1)
                destinationPath = WebConfig.getValue("appSettings", "UPLOAD_EBANKING_HSBC_Folder");
            else
                destinationPath = WebConfig.getValue("appSettings", "UPLOAD_EBANKING_SCB_Folder");

            string archivePath = destinationPath + @"archive\" + request.FileName.Substring(request.FileName.LastIndexOf("\\") + 1).ToUpper().Replace(".CSV", "")
                + request.Bank + DateTime.Now.ToString("yyyyMMdd_hhmm") + ".csv";

            destinationPath += request.FileName.Substring(request.FileName.LastIndexOf("\\") + 1).ToUpper().Replace(".CSV", "") 
                + request.Bank + DateTime.Now.ToString("yyyyMMdd_hhmm") + ".csv";

            File.Copy(request.FileName, archivePath);

            return ebWorker.ExportBankFile(userId, request, destinationPath);            
        }

	}
}
