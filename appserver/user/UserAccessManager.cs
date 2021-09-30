using System;
using System.Collections;

using com.next.infra.util;
using com.next.common.domain;
using com.next.common.domain.module;
using com.next.common.domain.types;
using com.next.common.security;
using com.next.common.datafactory.worker;

namespace com.next.isam.appserver.user
{
	/// <summary>
	/// Summary description for UserAccessManager.
	/// </summary>
	public class UserAccessManager
	{
		private static UserAccessManager _instance;
		private GeneralWorker gworker;
		private SecurityManager smanager;

		public UserAccessManager()
		{
			gworker = GeneralWorker.Instance;
			smanager = SecurityManager.Instance;
		}

		public static UserAccessManager Instance
		{
			get 
			{
				if (_instance == null)
				{
					_instance = new UserAccessManager();
				}
				return _instance;
			}
		}

		public ArrayList getAccessOfficeByUserId(int userId)
		{
			ArrayList pDepts = gworker.getProductDepartmentByUserId(userId);
			ArrayList offices = new ArrayList();

			if (pDepts != null)
			{
				ArrayList officeIds = new ArrayList();

				foreach (ProductDepartmentRef pdDef in pDepts)
				{
					if (!officeIds.Contains(pdDef.OfficeId))
					{
						OfficeRef oDef = gworker.getOfficeRefByKey(pdDef.OfficeId);

						if (oDef != null)
						{
							offices.Add(oDef);
							officeIds.Add(oDef.OfficeId);
						}
					}
				}
			}
			else
			{
				UserRef uDef = gworker.getUserByKey(userId);

				if (uDef != null && uDef.Department != null && uDef.Department.Office != null)
					offices.Add(uDef.Department.Office);
			}

			if (offices.Count > 0)
				offices.Sort(new ArrayListHelper.Sorter("Description"));

			return (offices.Count > 0)? offices : null;
		}

		public ArrayList getAccessProductDeptByUserId(int userId, int officeId, bool isAdvUser)
		{
			ArrayList pDepts = gworker.getProductDepartmentByUserId(userId);
			ArrayList pds = new ArrayList();

			if (pDepts != null)
			{
				foreach (ProductDepartmentRef pdDef in pDepts)
				{
					if (officeId == GeneralCriteria.ALL ||
						officeId == pdDef.OfficeId)
					{
						pds.Add(pdDef);
					}
				}
			}
			else
			{
				//Advanced User
				if (isAdvUser)
				{
					UserRef uDef = gworker.getUserByKey(userId);

					if (uDef != null && uDef.Department != null && uDef.Department.Office != null)
					{
						ArrayList userProductDepts = gworker.getProductDepartmentList(uDef.Department.Office.OfficeId);
						
						if (userProductDepts != null)
							pds.AddRange(userProductDepts);
					}
				}
			}

			if (pds.Count > 0)
				pds.Sort(new ArrayListHelper.Sorter("Description"));

			return (pds.Count > 0)? pds : null;
		}

		public UserLocation getUserLocationByUserId(int userId)
		{
			int appId = AppId.ELAB.Code;

			if (smanager.isAuthenticated(userId, appId, AccessMapper.ELAB.OutsideHKLocation.Id, AccessMapper.ELAB.OutsideHKLocation.Shenzhen))
				return UserLocation.Shenzhen;
			else if (smanager.isAuthenticated(userId, appId, AccessMapper.ELAB.OutsideHKLocation.Id, AccessMapper.ELAB.OutsideHKLocation.Shanghai))
				return UserLocation.Shanghai;
			else
				return UserLocation.HK;
		}
	}

	public enum UserLocation
	{
		HK,
		Shenzhen,
		Shanghai
	}
}
