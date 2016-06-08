namespace Live.Log.Extractor.Web.Helper.Mappers
{
    using System;
    using Live.Log.Extractor.Domain;
    using Live.Log.Extractor.Web.Models;

    /// <summary>
    /// Mapper class to map view model to data model and vice versa.
    /// </summary>
    public static class Mapper
    {
        /// <summary>
        /// Maps the login detais to data model.
        /// </summary>
        /// <param name="vm">The vm.</param>
        /// <param name="dataModel">The data model.</param>
        internal static void MapLoginDetaisToDataModel(LoginDetailsViewModel vm, LogDataModel dataModel)
        {
            dataModel.UserId = vm.UserId;
            dataModel.Password = vm.Password;
            dataModel.Region = vm.LoginRegion;
            dataModel.SqlQuery = vm.LoginQuery;
            dataModel.SqlQuery = HelperClass.ReplaceKey(dataModel.SqlQuery, "Region", dataModel.Region);
        }

        /// <summary>
        /// Maps the error detail view model to data model.
        /// </summary>
        /// <param name="halErrorDetailViewModel">The hal error detail view model.</param>
        /// <param name="logDataModel">The log data model.</param>
        internal static void MapErrorDetailViewModelToDataModel(HalErrorDetailViewModel vm, LogDataModel dataModel)
        {
            if (!string.IsNullOrEmpty(vm.ErrorCode))
            {
                dataModel.ErrorCode = vm.ErrorCode;
            }
            
            dataModel.Region = vm.ExceedRegion.CurrentRegion;
            dataModel.SqlQuery = vm.DetailSqlQuery;
            dataModel.SqlQuery = HelperClass.ReplaceKey(dataModel.SqlQuery, "Region", dataModel.Region);
            dataModel.SqlQuery = HelperClass.ReplaceKey(dataModel.SqlQuery, "ErrorRef", dataModel.ErrorCode);
        }

        /// <summary>
        /// Maps the view stat to data model.
        /// </summary>
        /// <param name="vm">The vm.</param>
        /// <param name="dataModel">The data model.</param>
        internal static void MapViewStatToDataModel(HalErrorDetailViewModel vm, LogDataModel dataModel)
        {
            MapErrorDetailViewModelToDataModel(vm, dataModel);

            dataModel.SqlQuery = HelperClass.CreateUnionQuery(dataModel.SqlQuery);
            //dataModel.SqlQuery = HelperClass.ReplaceKey(dataModel.SqlQuery, "FailTs", HelperClass.convertToUKFormat(DateTime.Now.AddMonths(-5).Year, DateTime.Now.AddMonths(-5).Month));
        }

        /// <summary>
        /// Maps the data model to cart view model.
        /// </summary>
        /// <param name="logDataModel">The log data model.</param>
        /// <param name="vm">The vm.</param>
        internal static void MapDataModelToCartViewModel(LogDataModel logDataModel, CartViewModel vm)
        {
            vm.UserId = logDataModel.UserId;
        }

        /// <summary>
        /// Maps the Print initial data to data model.
        /// </summary>
        /// <param name="vm">The vm.</param>
        /// <param name="dataModel">The data model.</param>
        internal static void MapPrintInitialDataToDataModel(PrintViewModel vm, LogDataModel dataModel)
        {
            dataModel.Region = vm.ExceedRegion.CurrentRegion;
            dataModel.SqlQuery = vm.SqlQuery;
            dataModel.SqlQuery = HelperClass.ReplaceKey(dataModel.SqlQuery, "Region", dataModel.Region);

            if (!string.IsNullOrEmpty(vm.PolicyNumber))
            {
                dataModel.PolicyNumber = vm.PolicyNumber;
                dataModel.SqlQuery = HelperClass.ReplaceKey(dataModel.SqlQuery, "PolicyNbr", dataModel.PolicyNumber);
            }

            if (!string.IsNullOrEmpty(vm.PolicyID))
            {
                dataModel.PolicyId = vm.PolicyID;
                dataModel.SqlQuery = HelperClass.ReplaceKey(dataModel.SqlQuery, "PolicyId", dataModel.PolicyId);
            }
        }

        /// <summary>
        /// Maps the EDD initial data to data model.
        /// </summary>
        /// <param name="vm">The vm.</param>
        /// <param name="dataModel">The data model.</param>
        internal static void MapEDDInitialDataToDataModel(EDDViewModel vm, LogDataModel dataModel)
        {
            dataModel.Region = vm.ExceedRegion.CurrentRegion;
            dataModel.SqlQuery = vm.SqlQuery;
            dataModel.SqlQuery = HelperClass.ReplaceKey(dataModel.SqlQuery, "Region", dataModel.Region);

            if (!string.IsNullOrEmpty(vm.PolicyNumber))
            {
                dataModel.PolicyNumber = vm.PolicyNumber;
                dataModel.SqlQuery = HelperClass.ReplaceKey(dataModel.SqlQuery, "PolicyNbr", dataModel.PolicyNumber);
            }

            if (!string.IsNullOrEmpty(vm.PolicyID))
            {
                dataModel.PolicyId = vm.PolicyID;
                dataModel.SqlQuery = HelperClass.ReplaceKey(dataModel.SqlQuery, "PolicyId", dataModel.PolicyId);
            }
        }

        ///// <summary>
        ///// Maps the Print initial data to data model.
        ///// </summary>
        ///// <param name="vm">The vm.</param>
        ///// <param name="dataModel">The data model.</param>
        //internal static void MapPrintInitialDataToDataModel(PrintViewModel vm, LogDataModel dataModel)
        //{
        //    dataModel.Region = vm.ExceedRegion.CurrentRegion;
        //    dataModel.SqlQuery = vm.SqlQuery;
        //    dataModel.SqlQuery = HelperClass.ReplaceKey(dataModel.SqlQuery, "Region", dataModel.Region);

        //    if (!string.IsNullOrEmpty(vm.PolicyNumber))
        //    {
        //        dataModel.PolicyNumber = vm.PolicyNumber;
        //        dataModel.SqlQuery = HelperClass.ReplaceKey(dataModel.SqlQuery, "PolicyNbr", dataModel.PolicyNumber);
        //    }

        //    if (!string.IsNullOrEmpty(vm.PolicyID))
        //    {
        //        dataModel.PolicyId = vm.PolicyID;
        //        dataModel.SqlQuery = HelperClass.ReplaceKey(dataModel.SqlQuery, "PolicyId", dataModel.PolicyId);
        //    }
        //}

        /// <summary>
        /// Maps the EDDIO data to data model.
        /// </summary>
        /// <param name="vm">The vm.</param>
        /// <param name="dataModel">The data model.</param>
        internal static void MapEDDKeyToDataModel(EDDViewModel vm, LogDataModel dataModel)
        {
            dataModel.EddKey = vm.EddKey;
        }

        /// <summary>
        /// Maps the PrintIO data to data model.
        /// </summary>
        /// <param name="vm">The vm.</param>
        /// <param name="dataModel">The data model.</param>
        internal static void MapPrintKeyToDataModel(PrintViewModel vm, LogDataModel dataModel)
        {
            dataModel.EddKey = vm.EddKey;
        }

        /// <summary>
        /// Maps the LOB code.
        /// </summary>
        /// <param name="vm">The vm.</param>
        /// <param name="logDataModel">The log data model.</param>
        internal static void MapLOBCode(EDDViewModel vm, LogDataModel dataModel)
        {
            dataModel.LOBCD = vm.LOBCD;
        }

        /// <summary>
        /// Maps the LOB code.
        /// </summary>
        /// <param name="vm">The vm.</param>
        /// <param name="logDataModel">The log data model.</param>
        internal static void MapLOBCode(PrintViewModel vm, LogDataModel dataModel)
        {
            dataModel.LOBCD = vm.LOBCD;
        }
        /// <summary>
        /// Maps the EDD polaris I ds to data model.
        /// </summary>
        /// <param name="eDDViewModel">The e DD view model.</param>
        /// <param name="logDataModel">The log data model.</param>
        internal static void MapEDDPolarisIDsToDataModel(EDDViewModel vm, LogDataModel dataModel)
        {
            string polarisIds = string.Empty;
            if (vm.PolarisIds != null && vm.PolarisIds.Count > 0)
            {
                vm.PolarisIds.ForEach(x => polarisIds += "'" + x + "',");
                dataModel.PolarisIds = polarisIds.Substring(0, polarisIds.Length - 1);
            }
        }

        /// <summary>
        /// Maps the Print polaris I ds to data model.
        /// </summary>
        /// <param name="PrintViewModel">The Print view model.</param>
        /// <param name="logDataModel">The log data model.</param>
        internal static void MapPrintPolarisIDsToDataModel(PrintViewModel vm, LogDataModel dataModel)
        {
            string polarisIds = string.Empty;
            if (vm.PolarisIds != null && vm.PolarisIds.Count > 0)
            {
                vm.PolarisIds.ForEach(x => polarisIds += "'" + x + "',");
                dataModel.PolarisIds = polarisIds.Substring(0, polarisIds.Length - 1);
            }
        }

        /// <summary>
        /// Maps the EDD rating query to data model.
        /// </summary>
        /// <param name="vm">The vm.</param>
        /// <param name="logDataModel">The log data model.</param>
        internal static void MapEDDRatingQueryToDataModel(EDDViewModel vm, LogDataModel dataModel)
        {
            dataModel.SqlQuery = vm.SqlQuery;
            dataModel.SqlQuery = HelperClass.ReplaceKey(dataModel.SqlQuery, "Region", dataModel.Region);
            dataModel.SqlQuery = HelperClass.ReplaceKey(dataModel.SqlQuery, "LOBCODE", dataModel.LOBCD);
            dataModel.SqlQuery = HelperClass.ReplaceKey(dataModel.SqlQuery, "IOCHAR", dataModel.EddKey.Split('-')[1]);
            dataModel.SqlQuery = HelperClass.ReplaceKey(dataModel.SqlQuery, "PolarisIds", dataModel.PolarisIds);
        }

        /// <summary>
        /// Maps the Print rating query to data model.
        /// </summary>
        /// <param name="vm">The vm.</param>
        /// <param name="logDataModel">The log data model.</param>
        internal static void MapPrintRatingQueryToDataModel(PrintViewModel vm, LogDataModel dataModel)
        {
            dataModel.SqlQuery = vm.SqlQuery;
            dataModel.SqlQuery = HelperClass.ReplaceKey(dataModel.SqlQuery, "Region", dataModel.Region);
            dataModel.SqlQuery = HelperClass.ReplaceKey(dataModel.SqlQuery, "LOBCODE", dataModel.LOBCD);
            dataModel.SqlQuery = HelperClass.ReplaceKey(dataModel.SqlQuery, "IOCHAR", dataModel.EddKey.Split('-')[1]);
            dataModel.SqlQuery = HelperClass.ReplaceKey(dataModel.SqlQuery, "PolarisIds", dataModel.PolarisIds);
        }


        /// <summary>
        /// Maps the EDDLOBCD query to data model.
        /// </summary>
        /// <param name="vm">The vm.</param>
        /// <param name="dataModel">The data model.</param>
        internal static void MapEDDLOBCDQueryToDataModel(EDDViewModel vm, LogDataModel dataModel)
        {
            dataModel.SqlQuery = vm.SqlQuery;
            dataModel.SqlQuery = HelperClass.ReplaceKey(dataModel.SqlQuery, "Region", dataModel.Region);
            dataModel.SqlQuery = HelperClass.ReplaceKey(dataModel.SqlQuery, "PolicyId", dataModel.PolicyId);
        }

        /// <summary>
        /// Maps the PrintLOBCD query to data model.
        /// </summary>
        /// <param name="vm">The vm.</param>
        /// <param name="dataModel">The data model.</param>
        internal static void MapPrintLOBCDQueryToDataModel(PrintViewModel vm, LogDataModel dataModel)
        {
            dataModel.SqlQuery = vm.SqlQuery;
            dataModel.SqlQuery = HelperClass.ReplaceKey(dataModel.SqlQuery, "Region", dataModel.Region);
            dataModel.SqlQuery = HelperClass.ReplaceKey(dataModel.SqlQuery, "PolicyId", dataModel.PolicyId);
        }


        internal static void MapTopTenErrorCodeQueryToDataModel(HalErrorDetailViewModel vm, LogDataModel dataModel)
        {
            dataModel.SqlQuery = vm.DetailSqlQuery;
            dataModel.Region = vm.ExceedRegion.CurrentRegion;
            dataModel.SqlQuery = HelperClass.ReplaceKey(dataModel.SqlQuery, "Region", dataModel.Region);
            dataModel.SqlQuery = HelperClass.ReplaceKey(dataModel.SqlQuery, "MinFailTs", HelperClass.convertToUKFormat(DateTime.Now.AddMonths(-5).Year, DateTime.Now.AddMonths(-5).Month));
        }
    }
}