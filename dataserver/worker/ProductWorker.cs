using System;
using System.Collections;
using System.Data;
using com.next.common.datafactory.worker;
using com.next.common.domain.types;
using com.next.common.domain;
using com.next.infra.persistency.dataaccess;
using com.next.infra.persistency.transactions;
using com.next.isam.dataserver.model.common;
using com.next.isam.dataserver.model.product;
using com.next.isam.domain.common;
using com.next.isam.domain.product;
using com.next.isam.domain.types;

namespace com.next.isam.dataserver.worker
{
	public class ProductWorker : Worker
	{
        private static ProductWorker _instance;
		private CommonWorker commonWorker;

		protected ProductWorker()
		{
			commonWorker = CommonWorker.Instance;
		}

		public static ProductWorker Instance
		{
			get 
			{
				if (_instance == null)
				    _instance = new ProductWorker();
				return _instance;
			}
		}	

		public ProductDef getProductByKey(int key) 
		{
		    IDataSetAdapter ad = getDataSetAdapter("ProductApt", "GetProductByKey");
		    ad.SelectCommand.Parameters["@ProductId"].Value = key.ToString();

		    ProductDs dataSet = new ProductDs();
		    int recordsAffected = ad.Fill(dataSet);

		    if (recordsAffected < 1) return null;

		    ProductDef def = new ProductDef();
		    ProductMapping(dataSet.Product[0], def);
            return def;
		}

        public ProductRef getProductRefByKey(int key)
        {
            IDataSetAdapter ad = getDataSetAdapter("ProductApt", "GetProductByKey");
            ad.SelectCommand.Parameters["@ProductId"].Value = key.ToString();

            ProductDs dataSet = new ProductDs();
            int recordsAffected = ad.Fill(dataSet);

            if (recordsAffected < 1) return null;

            ProductRef rf = new ProductRef();
            ProductMapping(dataSet.Product[0], rf);
            return rf;
        }

        public ProductDef getProductByItemNo(string ItemNo) 
		{
			IDataSetAdapter ad = getDataSetAdapter("ProductApt", "GetProductByItemNo");
			ad.SelectCommand.Parameters["@ItemNo"].Value = ItemNo.ToString();

			ProductDs dataSet = new ProductDs();
			int recordsAffected = ad.Fill(dataSet);			

			ArrayList list = new ArrayList();
			
			if (recordsAffected < 1) return null;

			ProductDef def = new ProductDef();
			ProductMapping(dataSet.Product[0], def);
			return def;
		}

		public ArrayList getProductListByItemNo(string ItemNo) 
		{
			IDataSetAdapter ad = getDataSetAdapter("ProductApt", "GetProductListByItemNo");
			ad.SelectCommand.Parameters["@ItemNo"].Value = ItemNo.ToString();

			ProductDs dataSet = new ProductDs();

			ad.SelectCommand.DbCommand.CommandTimeout = 120;
			int recordsAffected = ad.Fill(dataSet);			

			ArrayList list = new ArrayList();
			
			foreach(ProductDs.ProductRow row in dataSet.Product)
			{
				ProductDef def = new ProductDef();
				ProductMapping(row, def);
				list.Add(def);
			}
			return list;
		}

		public ArrayList getTop20ProductListByItemNo(string ItemNo) 
		{
			IDataSetAdapter ad = getDataSetAdapter("ProductApt", "GetTop20ProductListByItemNo");
			ad.SelectCommand.Parameters["@ItemNo"].Value = ItemNo.ToString();

			ProductDs dataSet = new ProductDs();

			ad.SelectCommand.DbCommand.CommandTimeout = 120;
			int recordsAffected = ad.Fill(dataSet);			

			ArrayList list = new ArrayList();
			
			foreach(ProductDs.ProductRow row in dataSet.Product)
			{
				ProductDef def = new ProductDef();
				ProductMapping(row, def);
				list.Add(def);
			}
			return list;
		}

		public ArrayList getProductListByDesignRef(string designRef) 
		{
			IDataSetAdapter ad = getDataSetAdapter("ProductApt", "GetProductListByDesignRef");
			ad.SelectCommand.Parameters["@DesignRef"].Value = designRef.ToString();

			ProductDs dataSet = new ProductDs();

			ad.SelectCommand.DbCommand.CommandTimeout = 120;
			int recordsAffected = ad.Fill(dataSet);			

			ArrayList list = new ArrayList();
			
			foreach(ProductDs.ProductRow row in dataSet.Product)
			{
				ProductDef def = new ProductDef();
				ProductMapping(row, def);
				list.Add(def);
			}
			return list;
		}

		public ArrayList getProductRefListByDesignRefProductTeamId(string designRef, int productTeamId, int seasonId, int vendorId) 
		{
			IDataSetAdapter ad = getDataSetAdapter("ProductApt", "GetProductListByDesignRefProductTeamIdSeasonId");
			ad.SelectCommand.Parameters["@DesignRef"].Value = designRef.ToString();
			ad.SelectCommand.Parameters["@ProductTeamId"].Value = productTeamId;
			ad.SelectCommand.Parameters["@SeasonId"].Value = seasonId;
			ad.SelectCommand.Parameters["@VendorId"].Value = vendorId;

			ProductDs dataSet = new ProductDs();

			ad.SelectCommand.DbCommand.CommandTimeout = 120;
			int recordsAffected = ad.Fill(dataSet);			

			ArrayList list = new ArrayList();
			
			foreach(ProductDs.ProductRow row in dataSet.Product)
			{
				ProductRef pRef = new ProductRef();
				ProductMapping(row, pRef);
				list.Add(pRef);
			}
			return list;
		}

		public ArrayList getSplitProductRefListByDesignRefProductTeamId(string designRef, int productTeamId, int seasonId, int vendorId) 
		{
			IDataSetAdapter ad = getDataSetAdapter("SplitProductApt", "GetSplitProductListByDesignRefProductTeamIdSeasonId");
			ad.SelectCommand.Parameters["@DesignRef"].Value = designRef.ToString();
			ad.SelectCommand.Parameters["@ProductTeamId"].Value = productTeamId;
			ad.SelectCommand.Parameters["@SeasonId"].Value = seasonId;
			ad.SelectCommand.Parameters["@VendorId"].Value = vendorId;

			ProductDs dataSet = new ProductDs();

			ad.SelectCommand.DbCommand.CommandTimeout = 120;
			int recordsAffected = ad.Fill(dataSet);			

			ArrayList list = new ArrayList();
			
			foreach(ProductDs.ProductRow row in dataSet.Product)
			{
				SplitProductDef pRef = new SplitProductDef();
				SplitProductMapping(row, pRef);
				list.Add(pRef);
			}
			return list;
		}


		/// <summary>
		/// Testing, do not use
		/// </summary>
		/// <param name="officeId"></param>
		/// <param name="productTeamId"></param>
		/// <param name="seasonId"></param>
		/// <param name="designRef"></param>
		/// <returns></returns>
		public ArrayList getProductListByCriteria(int iTop, int userId, int officeId, int deptId, TypeCollector productCodeIds, int seasonId, int vendorId, string designRefNo, TypeCollector workflowStatusList, TypeCollector phaseIdList, TypeCollector customerIdList, int designSourceId, string orderType, int withKnitwearComponent, TypeCollector destinationIdList)
		{
			IDataSetAdapter ad;
			
			if (iTop == 5)
				ad = getDataSetAdapter("ProductApt", "GetTop5DesignRefListByCriteria");
			else if (iTop == 10)
				ad = getDataSetAdapter("ProductApt", "GetTop10DesignRefListByCriteria");
			else
				ad = getDataSetAdapter("ProductApt", "GetProductListByCriteria");

			ad.SelectCommand.Parameters["@OfficeId"].Value = officeId;
			ad.SelectCommand.Parameters["@DeptId"].Value = deptId;
			
			//ad.SelectCommand.Parameters["@ProductGroupId"].Value = productGroupId;
			//ad.SelectCommand.Parameters["@ProductTeamId"].Value = productTeamId;

			ad.SelectCommand.CustomParameters["@ProductCodeList"] = CustomDataParameter.parse(productCodeIds.IsInclusive, productCodeIds.Values);

			ad.SelectCommand.Parameters["@SeasonId"].Value = seasonId;
			ad.SelectCommand.Parameters["@VendorId"].Value = vendorId;
			ad.SelectCommand.Parameters["@DesignRef"].Value = designRefNo;
			ad.SelectCommand.CustomParameters["@WorkflowStatusList"] = CustomDataParameter.parse(workflowStatusList.IsInclusive, workflowStatusList.Values);
			ad.SelectCommand.CustomParameters["@PhaseIdList"] = CustomDataParameter.parse(phaseIdList.IsInclusive, phaseIdList.Values);
			ad.SelectCommand.CustomParameters["@CustomerIdList"] = CustomDataParameter.parse(customerIdList.IsInclusive, customerIdList.Values);
			ad.SelectCommand.Parameters["@DesignSourceId"].Value = designSourceId;
			ad.SelectCommand.Parameters["@OrderType"].Value = orderType;

			ad.SelectCommand.Parameters["@WithKnitwearComponent"].Value = withKnitwearComponent;

			ad.SelectCommand.CustomParameters["@DestinationIdList"] = CustomDataParameter.parse(destinationIdList.IsInclusive, destinationIdList.Values);
			

			ProductDs dataSet = new ProductDs();

			ad.SelectCommand.DbCommand.CommandTimeout = 120;
			int recordsAffected = ad.Fill(dataSet);

			ArrayList list = new ArrayList();
			
			foreach(ProductDs.ProductRow row in dataSet.Product)
			{
				ProductDef pDef = new ProductDef();
				ProductMapping(row, pDef);
				list.Add(pDef);
			}
			return list;
		}



/*
		/// <summary>
		/// Testing, do not use
		/// </summary>
		/// <param name="officeId"></param>
		/// <param name="productTeamId"></param>
		/// <param name="seasonId"></param>
		/// <param name="designRef"></param>
		/// <returns></returns>
		public ArrayList getUnqiueDesignRefProductListByCriteria(int officeId, int deptId, int productTeamId, int seasonId, int vendorId, string designRefNo, TypeCollector workflowStatusList, TypeCollector phaseIdList, TypeCollector customerIdList)
		{
			IDataSetAdapter ad = getDataSetAdapter("ProductApt", "GetUnqiueDesignRefProductListByCriteria");
			ad.SelectCommand.Parameters["@OfficeId"].Value = officeId;
			ad.SelectCommand.Parameters["@DeptId"].Value = deptId;
			ad.SelectCommand.Parameters["@ProductTeamId"].Value = productTeamId;
			ad.SelectCommand.Parameters["@SeasonId"].Value = seasonId;
			ad.SelectCommand.Parameters["@VendorId"].Value = vendorId;
			ad.SelectCommand.Parameters["@DesignRef"].Value = designRefNo;
			ad.SelectCommand.CustomParameters["@WorkflowStatusList"] = CustomDataParameter.parse(workflowStatusList.IsInclusive, workflowStatusList.Values);
			ad.SelectCommand.CustomParameters["@PhaseIdList"] = CustomDataParameter.parse(phaseIdList.IsInclusive, phaseIdList.Values);
			ad.SelectCommand.CustomParameters["@CustomerIdList"] = CustomDataParameter.parse(customerIdList.IsInclusive, customerIdList.Values);

			ProductDs dataSet = new ProductDs();

			ad.SelectCommand.DbCommand.CommandTimeout = 120;
			int recordsAffected = ad.Fill(dataSet);

			ArrayList list = new ArrayList();
			
			foreach(ProductDs.ProductRow row in dataSet.Product)
			{
				ProductDef pDef = new ProductDef();
				ProductMapping(row, pDef);
				list.Add(pDef);
			}
			return list;
		}
*/

		public SplitProductDef getSplitProductByKey(int key) 
		{
			IDataSetAdapter ad = getDataSetAdapter("SplitProductApt", "GetSplitProductByKey");
			ad.SelectCommand.Parameters["@ProductId"].Value = key.ToString();

			ProductDs dataSet = new ProductDs();
			int recordsAffected = ad.Fill(dataSet);

			if (recordsAffected < 1) return null;

			SplitProductDef def = new SplitProductDef();
			SplitProductMapping(dataSet.Product[0], def);
			return def;
		}
		
		public SplitProductDef getSplitProductByItemNo(string ItemNo) 
		{
			IDataSetAdapter ad = getDataSetAdapter("SplitProductApt", "GetSplitProductByItemNo");
			ad.SelectCommand.Parameters["@ItemNo"].Value = ItemNo.ToString();

			ProductDs dataSet = new ProductDs();

			ad.SelectCommand.DbCommand.CommandTimeout = 120;
			int recordsAffected = ad.Fill(dataSet);			

			ArrayList list = new ArrayList();
			
			if (recordsAffected < 1) return null;

			SplitProductDef def = new SplitProductDef();
			SplitProductMapping(dataSet.Product[0], def);
			return def;
		}

		public ArrayList getSplitProductListByItemNo(string ItemNo) 
		{
			IDataSetAdapter ad = getDataSetAdapter("SplitProductApt", "GetSplitProductListByItemNo");
			ad.SelectCommand.Parameters["@ItemNo"].Value = ItemNo.ToString();

			ProductDs dataSet = new ProductDs();

			ad.SelectCommand.DbCommand.CommandTimeout = 120;
			int recordsAffected = ad.Fill(dataSet);			

			ArrayList list = new ArrayList();
			
			foreach(ProductDs.ProductRow row in dataSet.Product)
			{
				SplitProductDef def = new SplitProductDef();
				SplitProductMapping(row, def);
				list.Add(def);
			}
			return list;
		}

		public ArrayList getSplitProductListByDesignRef(string DesignRef) 
		{
			IDataSetAdapter ad = getDataSetAdapter("SplitProductApt", "GetSplitProductListByDesignRef");
			ad.SelectCommand.Parameters["@DesignRef"].Value = DesignRef.ToString();

			ProductDs dataSet = new ProductDs();

			ad.SelectCommand.DbCommand.CommandTimeout = 120;
			int recordsAffected = ad.Fill(dataSet);			

			ArrayList list = new ArrayList();
			
			foreach(ProductDs.ProductRow row in dataSet.Product)
			{
				SplitProductDef def = new SplitProductDef();
				SplitProductMapping(row, def);
				list.Add(def);
			}
			return list;
		}

		public ArrayList getSplitProductListByParentId(int ParentId) 
		{
			IDataSetAdapter ad = getDataSetAdapter("SplitProductApt", "GetSplitProductListByParentId");
			ad.SelectCommand.Parameters["@ParentId"].Value = ParentId.ToString();

			ProductDs dataSet = new ProductDs();

			ad.SelectCommand.DbCommand.CommandTimeout = 120;
			int recordsAffected = ad.Fill(dataSet);			

			ArrayList list = new ArrayList();
			
			foreach(ProductDs.ProductRow row in dataSet.Product)
			{
				SplitProductDef def = new SplitProductDef();
				SplitProductMapping(row, def);
				list.Add(def);
			}
			return list;
		}


		public ArrayList getSplitDesignRefListByParentIdList(TypeCollector productIdList) 
		{
			IDataSetAdapter ad = getDataSetAdapter("ProductPlannerInfoApt", "GetSplitDesignRefListByParentIdList");
			ad.SelectCommand.CustomParameters["@ParentIdList"] = CustomDataParameter.parse(productIdList.IsInclusive, productIdList.Values);

			DataSet dataSet = new DataSet();

			ad.SelectCommand.DbCommand.CommandTimeout = 120;
			int recordsAffected = ad.Fill(dataSet);			

			ArrayList list = new ArrayList();
			
			foreach(DataRow row in dataSet.Tables[0].Rows)
			{
				if (!Convert.IsDBNull(row[0])) list.Add((string)row[0]);
			}
			return list;
		}


		public int getMaxProductId() 
		{
			IDataSetAdapter ad = getDataSetAdapter("ProductApt", "GetMaxProductId");
			DataSet dataSet = new DataSet();
			int recordsAffected = ad.Fill(dataSet);

			if (recordsAffected < 1) return 0;
			
			if (Convert.IsDBNull(dataSet.Tables[0].Rows[0][0]))
				return 0;
			else
				return (int)(dataSet.Tables[0].Rows[0][0]);
		}

		public int getMaxSplitProductId() 
		{
			IDataSetAdapter ad = getDataSetAdapter("SplitProductApt", "GetMaxProductId");
			DataSet dataSet = new DataSet();
			int recordsAffected = ad.Fill(dataSet);

			if (recordsAffected < 1) return 0;
			
			if (Convert.IsDBNull(dataSet.Tables[0].Rows[0][0]))
				return 0;
			else
				return (int)(dataSet.Tables[0].Rows[0][0]);
		}

		public void updateProductDef(ProductDef productDef, int userId)
		{
            int maxId;
			if (productDef == null || userId == 0)
				return;

			TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.Required);
			try
			{
				ctx.Enter();
				TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();


				ProductDs dataSet =  new ProductDs ();
				ProductDs.ProductRow row = null;
				
				IDataSetAdapter ad = getDataSetAdapter("ProductApt", "GetProductByKey");
				ad.SelectCommand.Parameters["@ProductId"].Value = productDef.ProductId.ToString();

				ad.PopulateCommands();

				int recordsAffected = ad.Fill(dataSet);

				if (productDef.ProductId > 0) 
				{
					row=dataSet.Product[0];
					ProductMapping(productDef, row);
					sealStamp(dataSet.Product[0], userId, Stamp.UPDATE);
				} 
				else 
				{
                    maxId = getMaxProductId();
					row = dataSet.Product.NewProductRow();
					maxId++;
					productDef.ProductId=maxId;
					row.ProductId=maxId;
					productDef.Status = GeneralStatus.ACTIVE.Code;
					row.Status = GeneralStatus.ACTIVE.Code;
					ProductMapping(productDef, row);
					sealStamp(row, userId, Stamp.INSERT);
					dataSet.Product.AddProductRow(row);
				}
				
				recordsAffected = ad.Update(dataSet);
				if (recordsAffected < 1) 
					throw new DataAccessException("Update Product ERROR");
			
				ctx.VoteCommit();
			}
			catch(Exception e) 
			{
				Console.WriteLine("ERROR " + e.Message);
				ctx.VoteRollback();
				throw e;
			}
			finally 
			{
				ctx.Exit();
			}			
		}

		public void updateProductList(ICollection ProductBatchDefs, int userId)
		{
			if (ProductBatchDefs == null || ProductBatchDefs.Count== 0 || userId == 0)
				return;

			TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.Required);

			try
			{
				ctx.Enter();
				TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();

				int maxId=getMaxProductId();

				foreach (ProductDef def in ProductBatchDefs) 
				{
					ProductDs dataSet =  new ProductDs ();
					ProductDs.ProductRow row = null;
					
					IDataSetAdapter ad = getDataSetAdapter("ProductApt", "GetProductByKey");
					ad.SelectCommand.Parameters["@ProductId"].Value = def.ProductId.ToString();

					ad.PopulateCommands();

					int recordsAffected = ad.Fill(dataSet);

					if (def.ProductId > 0) 
					{
						row=dataSet.Product[0];
						ProductMapping(def, row);
						sealStamp(dataSet.Product[0], userId, Stamp.UPDATE);
					} 
					else 
					{
						row = dataSet.Product.NewProductRow();
						maxId++;
						def.ProductId=maxId;
						row.ProductId=maxId;

						def.Status = GeneralStatus.ACTIVE.Code;
						row.Status = GeneralStatus.ACTIVE.Code;

						ProductMapping(def, row);
						sealStamp(row, userId, Stamp.INSERT);
						sealStamp(row, userId, Stamp.UPDATE);
						dataSet.Product.AddProductRow(row);
					}
					
					recordsAffected = ad.Update(dataSet);
					if (recordsAffected < 1) 
						throw new DataAccessException("Update Product ERROR");
				}
				ctx.VoteCommit();
			}
			catch(Exception e) 
			{
				Console.WriteLine("ERROR " + e.Message);
				ctx.VoteRollback();
				throw e;
			}
			finally 
			{
				ctx.Exit();
			}			
		}

		public void updateSplitProductDef(SplitProductDef productDef, int userId)
		{
			if (productDef == null || userId == 0)
				return;

			TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.Required);

			try
			{
				ctx.Enter();
				TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();
				int maxId=getMaxSplitProductId();
				ProductDs dataSet =  new ProductDs ();
				ProductDs.ProductRow row = null;
				IDataSetAdapter ad = getDataSetAdapter("SplitProductApt", "GetSplitProductByKey");
				ad.SelectCommand.Parameters["@ProductId"].Value = productDef.ProductId.ToString();

				ad.PopulateCommands();

				int recordsAffected = ad.Fill(dataSet);

				if (productDef.ProductId > 0) 
				{
					row=dataSet.Product[0];
					SplitProductMapping(productDef, row);
					sealStamp(dataSet.Product[0], userId, Stamp.UPDATE);
				} 
				else 
				{
					row = dataSet.Product.NewProductRow();
					maxId++;
					productDef.ProductId=maxId;
					row.ProductId=maxId;
					productDef.Status = GeneralStatus.ACTIVE.Code;
					row.Status = GeneralStatus.ACTIVE.Code;
					SplitProductMapping(productDef, row);
					sealStamp(row, userId, Stamp.INSERT);
					dataSet.Product.AddProductRow(row);
				}

				recordsAffected = ad.Update(dataSet);
				if (recordsAffected < 1) 
					throw new DataAccessException("Update Product ERROR");
				
				ctx.VoteCommit();
			}
			catch(Exception e) 
			{
				Console.WriteLine("ERROR " + e.Message);
				ctx.VoteRollback();
				throw e;
			}
			finally 
			{
				ctx.Exit();
			}			
		}

		public void updateSplitProductList(ICollection SplitProductBatchDefs, int userId)
		{
			if (SplitProductBatchDefs == null || SplitProductBatchDefs.Count== 0 || userId == 0)
				return;

			TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.Required);

			try
			{
				ctx.Enter();
				TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();

				int maxId = getMaxSplitProductId();

				foreach (SplitProductDef def in SplitProductBatchDefs) 
				{
					ProductDs dataSet =  new ProductDs ();
					ProductDs.ProductRow row = null;
					
					IDataSetAdapter ad = getDataSetAdapter("SplitProductApt", "GetSplitProductByKey");
					ad.SelectCommand.Parameters["@ProductId"].Value = def.ProductId.ToString();

					ad.PopulateCommands();

					int recordsAffected = ad.Fill(dataSet);

					if (def.ProductId > 0) 
					{
						row=dataSet.Product[0];
						SplitProductMapping(def, row);
						sealStamp(dataSet.Product[0], userId, Stamp.UPDATE);
					} 
					else 
					{
						row = dataSet.Product.NewProductRow();
						maxId++;
						def.ProductId=maxId;
						row.ProductId=maxId;

						def.Status = GeneralStatus.ACTIVE.Code;
						row.Status = GeneralStatus.ACTIVE.Code;

						SplitProductMapping(def, row);
						sealStamp(row, userId, Stamp.INSERT);
						sealStamp(row, userId, Stamp.UPDATE);
						dataSet.Product.AddProductRow(row);
					}

					recordsAffected = ad.Update(dataSet);
					if (recordsAffected < 1) 
						throw new DataAccessException("Update Product ERROR");
				}
				ctx.VoteCommit();
			}
			catch(Exception e) 
			{
				Console.WriteLine("ERROR " + e.Message);
				ctx.VoteRollback();
				throw e;
			}
			finally 
			{
				ctx.Exit();
			}			
		}

		#region Mapping
	
		private void ProductMapping(Object source, Object target)
		{
			if (source.GetType() == typeof(ProductDs.ProductRow) &&
				target.GetType() == typeof(ProductDef))
			{
				ProductDs.ProductRow row = (ProductDs.ProductRow) source;
				ProductDef def = (ProductDef) target;

				def.ProductId = row.ProductId;
				def.ItemNo = row.ItemNo;

                if (!row.IsMasterDesc1Null())
                    def.MasterDescription1 = row.MasterDesc1;
                else
                    def.MasterDescription1 = String.Empty;

                if (!row.IsMasterDesc2Null())
                    def.MasterDescription2 = row.MasterDesc2;
                else
                    def.MasterDescription2 = String.Empty;

                if (!row.IsMasterDesc3Null())
                    def.MasterDescription3 = row.MasterDesc3;
                else
                    def.MasterDescription3 = String.Empty;

                if (!row.IsMasterDesc4Null())
                    def.MasterDescription4 = row.MasterDesc4;
                else
                    def.MasterDescription4 = String.Empty;

                if (!row.IsMasterDesc5Null())
                    def.MasterDescription5 = row.MasterDesc5;
                else
                    def.MasterDescription5 = String.Empty;

                if (!row.IsDesc1Null()) 
					def.Description1 = row.Desc1; 
				else
                    def.Description1 = String.Empty;
				
				if (!row.IsDesc2Null()) 
					def.Description2 = row.Desc2;
				else
                    def.Description2 = String.Empty;
				
				if (!row.IsDesc3Null()) 
					def.Description3 = row.Desc3;
				else
                    def.Description3 = String.Empty;
				
				if (!row.IsDesc4Null()) 
					def.Description4 = row.Desc4;
				else
                    def.Description4 = String.Empty;

				if (!row.IsDesc5Null()) 
					def.Description5 = row.Desc5;
				else
                    def.Description5 = String.Empty;

				if (!row.IsShortDescNull()) 
					def.ShortDesc = row.ShortDesc;
				else
                    def.ShortDesc = String.Empty;

				if (!row.IsColourNull()) 
					def.Colour = row.Colour;
				else
                    def.Colour = String.Empty;
				
				if (!row.IsDesignRefNull()) def.StyleDesignRef = row.DesignRef;
				if (!row.IsDesignSourceIdNull()) def.DesignSource = GeneralWorker.Instance.getDesignSourceByKey(row.DesignSourceId);
				if (!row.IsCartonTypeIdNull()) def.CartonType = commonWorker.getCartonTypeByKey(row.CartonTypeId);
				if (!row.IsQtyPerCartonNull()) def.QtyPerCarton = row.QtyPerCarton;

                if (!row.IsGarmentWashNull()) def.GarmentWash = row.GarmentWash; else def.GarmentWash = String.Empty;

				if (!row.IsDesignerIdNull()) def.DesignerId = row.DesignerId; else def.DesignerId = int.MinValue;

                if (!row.IsProductDesignStyleIdNull())
                    def.ProductDesignStyleId = row.ProductDesignStyleId;
                else
                    def.ProductDesignStyleId = -1;
                if (!row.IsRetailDescriptionNull())
                    def.RetailDescription = row.RetailDescription;
                else
                    def.RetailDescription = String.Empty;

                def.OriginalItemNo = def.ItemNo;
				def.OriginalDescription1 = def.Description1;
				def.OriginalDescription2 = def.Description2;
				def.OriginalDescription3 = def.Description3;
				def.OriginalDescription4 = def.Description4;
				def.OriginalDescription5 = def.Description5;
				def.OriginalColour = def.Colour;
				def.Status = row.Status;
			} 
			if (source.GetType() == typeof(ProductDs.ProductRow) &&
				target.GetType() == typeof(ProductRef))
			{
				ProductDs.ProductRow row = (ProductDs.ProductRow) source;
				ProductRef pRef = (ProductRef) target;

				pRef.ProductId = row.ProductId;
				pRef.ItemNo = row.ItemNo;

                if (!row.IsMasterDesc1Null())
                    pRef.MasterDescription1 = row.MasterDesc1;
                else
                    pRef.MasterDescription1 = String.Empty;

                if (!row.IsMasterDesc2Null())
                    pRef.MasterDescription2 = row.MasterDesc2;
                else
                    pRef.MasterDescription2 = String.Empty;

                if (!row.IsMasterDesc3Null())
                    pRef.MasterDescription3 = row.MasterDesc3;
                else
                    pRef.MasterDescription3 = String.Empty;

                if (!row.IsMasterDesc4Null())
                    pRef.MasterDescription4 = row.MasterDesc4;
                else
                    pRef.MasterDescription4 = String.Empty;

                if (!row.IsMasterDesc5Null())
                    pRef.MasterDescription5 = row.MasterDesc5;
                else
                    pRef.MasterDescription5 = String.Empty;

                if (!row.IsDesc1Null()) 
					pRef.Description1 = row.Desc1; 
				else
                    pRef.Description1 = String.Empty;
				
				if (!row.IsDesc2Null()) 
					pRef.Description2 = row.Desc2;
				else
                    pRef.Description2 = String.Empty;
				
				if (!row.IsDesc3Null()) 
					pRef.Description3 = row.Desc3;
				else
                    pRef.Description3 = String.Empty;
				
				if (!row.IsDesc4Null()) 
					pRef.Description4 = row.Desc4;
				else
                    pRef.Description4 = String.Empty;

				if (!row.IsDesc5Null()) 
					pRef.Description5 = row.Desc5;
				else
                    pRef.Description5 = String.Empty;

				if (!row.IsShortDescNull()) 
					pRef.ShortDesc = row.ShortDesc;
				else
                    pRef.ShortDesc = String.Empty;

				if (!row.IsColourNull()) 
					pRef.Colour = row.Colour;
				else
                    pRef.Colour = String.Empty;

                if (!row.IsProductDesignStyleIdNull())
                    pRef.ProductDesignStyleId = row.ProductDesignStyleId;
                else
                    pRef.ProductDesignStyleId = -1;

                if (!row.IsRetailDescriptionNull())
                    pRef.RetailDescription = row.RetailDescription;
                else
                    pRef.RetailDescription = String.Empty;

			}
			else if (source.GetType() == typeof(ProductDef) &&
				target.GetType() == typeof(ProductDs.ProductRow))
			{
				ProductDef def = (ProductDef) source;
				ProductDs.ProductRow row = (ProductDs.ProductRow) target;

				row.ProductId = def.ProductId;
				row.ItemNo = def.ItemNo;
				row.SetParentIdNull();
				row.SetSplitSuffixNull();

                if (def.MasterDescription1 != String.Empty)
                    row.MasterDesc1 = def.MasterDescription1;
                else
                    row.SetMasterDesc1Null();
                
                if (def.MasterDescription2 != String.Empty)
                    row.MasterDesc2 = def.MasterDescription2;
                else
                    row.SetMasterDesc2Null();

                if (def.MasterDescription3 != String.Empty)
                    row.MasterDesc3 = def.MasterDescription3;
                else
                    row.SetMasterDesc3Null();

                if (def.MasterDescription4 != String.Empty)
                    row.MasterDesc4 = def.MasterDescription4;
                else
                    row.SetMasterDesc4Null();

                if (def.MasterDescription5 != String.Empty)
                    row.MasterDesc5 = def.MasterDescription5;
                else
                    row.SetMasterDesc5Null();

                if (def.Description1 != String.Empty)
					row.Desc1 = def.Description1;
				else
					row.SetDesc1Null();

                if (def.Description2 != String.Empty)
					row.Desc2 = def.Description2;
				else
					row.SetDesc2Null();

                if (def.Description3 != String.Empty)
					row.Desc3 = def.Description3;
				else
					row.SetDesc3Null();

                if (def.Description4 != String.Empty)
					row.Desc4 = def.Description4;
				else
					row.SetDesc4Null();

                if (def.Description5 != String.Empty)
					row.Desc5 = def.Description5;
				else
					row.SetDesc5Null();

                if (def.ShortDesc != String.Empty)
					row.ShortDesc = def.ShortDesc;
				else
					row.SetShortDescNull();

                if (def.Colour != String.Empty)
					row.Colour = def.Colour;
				else
					row.SetColourNull();

                if (def.StyleDesignRef != String.Empty)
					row.DesignRef = def.StyleDesignRef;
				else
					row.SetDesignRefNull();

				if (def.DesignSource != null)
					row.DesignSourceId = def.DesignSource.DesignSourceId;
				else
					row.SetDesignSourceIdNull();

				if (def.CartonType != null)
					row.CartonTypeId = def.CartonType.CartonTypeId;
				else
					row.SetCartonTypeIdNull();
				
				if (def.QtyPerCarton > 0)
					row.QtyPerCarton = def.QtyPerCarton;
				else
					row.SetQtyPerCartonNull();

				row.Status = def.Status;

				row.GarmentWash = def.GarmentWash;
			
				if (def.DesignerId > 0)
					row.DesignerId = def.DesignerId;
				else
					row.SetDesignerIdNull();

                if (def.ProductDesignStyleId > 0)
                    row.ProductDesignStyleId = def.ProductDesignStyleId;
                else
                    row.SetProductDesignStyleIdNull();

                if (def.RetailDescription != null)
                    row.RetailDescription = def.RetailDescription;
                else
                    row.SetRetailDescriptionNull();
			}
		}
	
		private void SplitProductMapping(Object source, Object target)
		{
			if (source.GetType() == typeof(ProductDs.ProductRow) &&
				target.GetType() == typeof(SplitProductDef))
			{
				ProductDs.ProductRow row = (ProductDs.ProductRow) source;
				SplitProductDef def = (SplitProductDef) target;

				def.ProductId = row.ProductId;
				def.ItemNo = row.ItemNo;
				def.SplitSuffix = row.SplitSuffix;
				def.ParentProduct = getProductByKey(row.ParentId);
				if (!row.IsDesc1Null()) 
					def.Description1 = row.Desc1;
				else
                    def.Description1 = String.Empty;
				if (!row.IsDesc2Null()) 
					def.Description2 = row.Desc2;
				else
                    def.Description2 = String.Empty;
				if (!row.IsDesc3Null()) 
					def.Description3 = row.Desc3;
				else
                    def.Description3 = String.Empty;
				if (!row.IsDesc4Null()) 
					def.Description4 = row.Desc4;
				else
                    def.Description4 = String.Empty;
				if (!row.IsDesc5Null()) 
					def.Description5 = row.Desc5;
				else
                    def.Description5 = String.Empty;

				if (!row.IsShortDescNull()) 
					def.ShortDesc = row.ShortDesc;
				else
                    def.ShortDesc = String.Empty;

				if (!row.IsColourNull()) 
					def.Colour = row.Colour;
				else
                    def.Colour = String.Empty;
				if (!row.IsDesignRefNull()) def.StyleDesignRef = row.DesignRef;
				if (!row.IsDesignSourceIdNull()) def.DesignSource = GeneralWorker.Instance.getDesignSourceByKey(row.DesignSourceId);

				if (!row.IsDesignerIdNull()) def.DesignerId = row.DesignerId; else def.DesignerId = int.MinValue;

				def.OriginalDescription1 = def.Description1;
				def.OriginalDescription2 = def.Description2;
				def.OriginalDescription3 = def.Description3;
				def.OriginalDescription4 = def.Description4;
				def.OriginalDescription5 = def.Description5;
				def.OriginalShortDesc = def.ShortDesc;
				def.OriginalColour = def.Colour;
				def.Status = row.Status;
			}
			else if (source.GetType() == typeof(SplitProductDef) &&
				target.GetType() == typeof(ProductDs.ProductRow))
			{
				SplitProductDef def = (SplitProductDef) source;
				ProductDs.ProductRow row = (ProductDs.ProductRow) target;

				row.ProductId = def.ProductId;
				row.ItemNo = def.ItemNo;
				row.ParentId = def.ParentProduct.ProductId;
				row.SplitSuffix = def.SplitSuffix;

                if (def.Description1 != String.Empty)
					row.Desc1 = def.Description1;
				else
					row.SetDesc1Null();

                if (def.Description2 != String.Empty)
					row.Desc2 = def.Description2;
				else
					row.SetDesc2Null();

                if (def.Description3 != String.Empty)
					row.Desc3 = def.Description3;
				else
					row.SetDesc3Null();

                if (def.Description4 != String.Empty)
					row.Desc4 = def.Description4;
				else
					row.SetDesc4Null();

                if (def.Description5 != String.Empty)
					row.Desc5 = def.Description5;
				else
					row.SetDesc5Null();

				row.ShortDesc = def.ShortDesc;

                if (def.Colour != String.Empty)
					row.Colour = def.Colour;
				else
					row.SetColourNull();

                if (def.StyleDesignRef != String.Empty)
					row.DesignRef = def.StyleDesignRef;
				else
					row.SetDesignRefNull();

				if (def.DesignSource != null)
					row.DesignSourceId = def.DesignSource.DesignSourceId;
				else
					row.SetDesignSourceIdNull();
			
				if (def.DesignerId > 0)
					row.DesignerId = def.DesignerId;
				else
					row.SetDesignerIdNull();

				row.Status = def.Status;
			}
		}

		#endregion
	}
}
