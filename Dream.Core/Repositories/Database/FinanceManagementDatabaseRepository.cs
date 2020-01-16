using Dream.IO.Database;
using Dream.IO.Database.Contexts;
using Dream.IO.Database.Entities.FinanceManagement;
using Dream.IO.Excel.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;

namespace Dream.Core.Repositories.Database
{
    public class FinanceManagementDatabaseRepository : DatabaseRepository
    {
        public override DbContext DatabaseContext => DatabaseContextRetrieiver.GetFinanceManagementContext();

        private Dictionary<string, int> _ledgerAccounts;
        public Dictionary<string, int> LedgerAccounts
        {
            get
            {
                // Cache database list of ledger accounts
                if (_ledgerAccounts == null)
                {
                    using (var financeManagementContext = DatabaseContext as FinanceManagementContext)
                    {
                        var ledgerAccountEntities = financeManagementContext.LedgerAccountDimEntities.ToList();
                        _ledgerAccounts = ledgerAccountEntities.ToDictionary(e => e.LedgerAccount, e => e.LedgerAccountKey);
                    }
                }

                return _ledgerAccounts;
            }
        }

        private Dictionary<string, int> _costCenters;
        public Dictionary<string, int> CostCenters
        {
            get
            {
                // Cache database list of cost centers
                if (_costCenters == null)
                {
                    using (var financeManagementContext = DatabaseContext as FinanceManagementContext)
                    {
                        var costCenterEntities = financeManagementContext.CostCenterDimEntities.ToList();
                        _costCenters = costCenterEntities.ToDictionary(e => e.CostCenter, e => e.CostCenterKey);
                    }
                }

                return _costCenters;
            }
        }

        private Dictionary<string, int> _productLines;
        public Dictionary<string, int> ProductLines
        {
            get
            {
                // Cache database list of product lines
                if (_productLines == null)
                {
                    using (var financeManagementContext = DatabaseContext as FinanceManagementContext)
                    {
                        var productLineEntities = financeManagementContext.ProductLineDimEntities.ToList();
                        _productLines = productLineEntities.ToDictionary(e => e.ProductLine, e => e.ProductLineKey);
                    }
                }

                return _productLines;
            }
        }

        private Dictionary<string, int> _regions;
        public Dictionary<string, int> Regions
        {
            get
            {
                // Cache database list of regions
                if (_regions == null)
                {
                    using (var financeManagementContext = DatabaseContext as FinanceManagementContext)
                    {
                        var regionEntities = financeManagementContext.RegionDimEntities.ToList();
                        _regions = regionEntities.ToDictionary(e => e.Region, e => e.RegionKey);
                    }
                }

                return _regions;
            }
        }

        public FinanceManagementDatabaseRepository() : base()
        {
            _ledgerAccounts = LedgerAccounts;
            _costCenters = CostCenters;
            _productLines = ProductLines;
            _regions = Regions;
        }

        public FinanceManagementDatabaseRepository(DateTime cutOffDate) : base(cutOffDate)
        {
            _ledgerAccounts = LedgerAccounts;
            _costCenters = CostCenters;
            _productLines = ProductLines;
            _regions = Regions;
        }

        public void LoadOrUpdateLegderAccountAggregationGroups(string aggregationGroup, List<FinanceManagementMappingRecord> financeManagementMappingRecords)
        {
            using (var financeManagementContext = DatabaseContext as FinanceManagementContext)
            {
                var distinctLedgerAccountAggregationGroups = financeManagementContext
                    .LedgerAccountAggregationGroupEntities
                    .Where(e => e.IsAggregationGroupActive)
                    .Select(e => e.LedgerAccountAggregationGroupDescription)
                    .Distinct().ToList();

                // If where is already an active grouping with the same name, update the active record to be inactive first
                if (distinctLedgerAccountAggregationGroups.Contains(aggregationGroup))
                {
                    var activeLedgerAccountAggregationGroup = financeManagementContext
                        .LedgerAccountAggregationGroupEntities
                        .SingleOrDefault(e => e.IsAggregationGroupActive 
                                           && e.LedgerAccountAggregationGroupDescription == aggregationGroup);

                    activeLedgerAccountAggregationGroup.IsAggregationGroupActive = false;
                    financeManagementContext.SaveChanges();
                }

                // Add the new account aggregation group and save the context to capture its group Id
                var newLedgerAccountAggregationGroup = new LedgerAccountAggregationGroupEntity
                {
                    LedgerAccountAggregationGroupDescription = aggregationGroup,
                    IsAggregationGroupActive = true
                };

                financeManagementContext.LedgerAccountAggregationGroupEntities.Add(newLedgerAccountAggregationGroup);
                financeManagementContext.SaveChanges();

                var newLedgerAccountAggregationGroupId = newLedgerAccountAggregationGroup.LedgerAccountAggregationGroupId;
                var listOfLedgerAccountAggregationGroupMappingEntities = new List<LedgerAccountAggregationGroupMappingEntity>();

                // Add new mappings for each record provided
                foreach (var financeManagementMappingRecord in financeManagementMappingRecords)
                {
                    if (LedgerAccounts.ContainsKey(financeManagementMappingRecord.Description))
                    {
                        var newLedgerAccountAggregationGroupMappingEntity = new LedgerAccountAggregationGroupMappingEntity
                        {
                            LedgerAccountAggregationGroupId = newLedgerAccountAggregationGroupId,
                            LedgerAccountKey = LedgerAccounts[financeManagementMappingRecord.Description],
                            LedgerAccountAggregationGroupIdentifier = financeManagementMappingRecord.MappingIdentifier
                        };

                        listOfLedgerAccountAggregationGroupMappingEntities.Add(newLedgerAccountAggregationGroupMappingEntity);
                    }
                    else
                    {
                        throw new Exception(string.Format("ERROR: An account named '{0}' does not exist. Please check your mapping file input or make a request to add this account.",
                            financeManagementMappingRecord.Description));
                    }
                }

                financeManagementContext.LedgerAccountAggregationGroupMappingEntities.AddRange(listOfLedgerAccountAggregationGroupMappingEntities);
                financeManagementContext.SaveChanges();
            }
        }

        public void LoadOrUpdateCostCenterAggregationGroups(string aggregationGroup, List<FinanceManagementMappingRecord> financeManagementMappingRecords)
        {
            using (var financeManagementContext = DatabaseContext as FinanceManagementContext)
            {
                var distinctCostCenterAggregationGroups = financeManagementContext
                    .CostCenterAggregationGroupEntities
                    .Where(e => e.IsAggregationGroupActive)
                    .Select(e => e.CostCenterAggregationGroupDescription)
                    .Distinct().ToList();

                // If where is already an active grouping with the same name, update the active record to be inactive first
                if (distinctCostCenterAggregationGroups.Contains(aggregationGroup))
                {
                    var activeCostCenterAggregationGroup = financeManagementContext
                        .CostCenterAggregationGroupEntities
                        .SingleOrDefault(e => e.IsAggregationGroupActive
                                           && e.CostCenterAggregationGroupDescription == aggregationGroup);

                    activeCostCenterAggregationGroup.IsAggregationGroupActive = false;
                    financeManagementContext.SaveChanges();
                }

                // Add the new account aggregation group and save the context to capture its group Id
                var newCostCenterAggregationGroup = new CostCenterAggregationGroupEntity
                {
                    CostCenterAggregationGroupDescription = aggregationGroup,
                    IsAggregationGroupActive = true
                };

                financeManagementContext.CostCenterAggregationGroupEntities.Add(newCostCenterAggregationGroup);
                financeManagementContext.SaveChanges();

                var newCostCenterAggregationGroupId = newCostCenterAggregationGroup.CostCenterAggregationGroupId;
                var listOfCostCenterAggregationGroupMappingEntities = new List<CostCenterAggregationGroupMappingEntity>();

                // Add new mappings for each record provided
                foreach (var financeManagementMappingRecord in financeManagementMappingRecords)
                {
                    if (CostCenters.ContainsKey(financeManagementMappingRecord.Description))
                    {
                        var newCostCenterAggregationGroupMappingEntity = new CostCenterAggregationGroupMappingEntity
                        {
                            CostCenterAggregationGroupId = newCostCenterAggregationGroupId,
                            CostCenterKey = CostCenters[financeManagementMappingRecord.Description],
                            CostCenterAggregationGroupIdentifier = financeManagementMappingRecord.MappingIdentifier
                        };

                        listOfCostCenterAggregationGroupMappingEntities.Add(newCostCenterAggregationGroupMappingEntity);
                    }
                    else
                    {
                        throw new Exception(string.Format("ERROR: A cost center named '{0}' does not exist. Please check your mapping file input or make a request to add this account.",
                            financeManagementMappingRecord.Description));
                    }
                }

                financeManagementContext.CostCenterAggregationGroupMappingEntities.AddRange(listOfCostCenterAggregationGroupMappingEntities);
                financeManagementContext.SaveChanges();
            }
        }

        public void LoadOrUpdateProductLineAggregationGroups(string aggregationGroup, List<FinanceManagementMappingRecord> financeManagementMappingRecords)
        {
            using (var financeManagementContext = DatabaseContext as FinanceManagementContext)
            {
                var distinctProductLineAggregationGroups = financeManagementContext
                    .ProductLineAggregationGroupEntities
                    .Where(e => e.IsAggregationGroupActive)
                    .Select(e => e.ProductLineAggregationGroupDescription)
                    .Distinct().ToList();

                // If where is already an active grouping with the same name, update the active record to be inactive first
                if (distinctProductLineAggregationGroups.Contains(aggregationGroup))
                {
                    var activeProductLineAggregationGroup = financeManagementContext
                        .ProductLineAggregationGroupEntities
                        .SingleOrDefault(e => e.IsAggregationGroupActive
                                           && e.ProductLineAggregationGroupDescription == aggregationGroup);

                    activeProductLineAggregationGroup.IsAggregationGroupActive = false;
                    financeManagementContext.SaveChanges();
                }

                // Add the new account aggregation group and save the context to capture its group Id
                var newProductLineAggregationGroup = new ProductLineAggregationGroupEntity
                {
                    ProductLineAggregationGroupDescription = aggregationGroup,
                    IsAggregationGroupActive = true
                };

                financeManagementContext.ProductLineAggregationGroupEntities.Add(newProductLineAggregationGroup);
                financeManagementContext.SaveChanges();

                var newProductLineAggregationGroupId = newProductLineAggregationGroup.ProductLineAggregationGroupId;
                var listOfProductLineAggregationGroupMappingEntities = new List<ProductLineAggregationGroupMappingEntity>();

                // Add new mappings for each record provided
                foreach (var financeManagementMappingRecord in financeManagementMappingRecords)
                {
                    if (ProductLines.ContainsKey(financeManagementMappingRecord.Description))
                    {
                        var newProductLineAggregationGroupMappingEntity = new ProductLineAggregationGroupMappingEntity
                        {
                            ProductLineAggregationGroupId = newProductLineAggregationGroupId,
                            ProductLineKey = ProductLines[financeManagementMappingRecord.Description],
                            ProductLineAggregationGroupIdentifier = financeManagementMappingRecord.MappingIdentifier
                        };

                        listOfProductLineAggregationGroupMappingEntities.Add(newProductLineAggregationGroupMappingEntity);
                    }
                    else
                    {
                        throw new Exception(string.Format("ERROR: An product line named '{0}' does not exist. Please check your mapping file input or make a request to add this account.",
                            financeManagementMappingRecord.Description));
                    }
                }

                financeManagementContext.ProductLineAggregationGroupMappingEntities.AddRange(listOfProductLineAggregationGroupMappingEntities);
                financeManagementContext.SaveChanges();
            }
        }

        public void LoadOrUpdateRegionAggregationGroups(string aggregationGroup, List<FinanceManagementMappingRecord> financeManagementMappingRecords)
        {
            using (var financeManagementContext = DatabaseContext as FinanceManagementContext)
            {
                var distinctRegionAggregationGroups = financeManagementContext
                    .RegionAggregationGroupEntities
                    .Where(e => e.IsAggregationGroupActive)
                    .Select(e => e.RegionAggregationGroupDescription)
                    .Distinct().ToList();

                // If where is already an active grouping with the same name, update the active record to be inactive first
                if (distinctRegionAggregationGroups.Contains(aggregationGroup))
                {
                    var activeRegionAggregationGroup = financeManagementContext
                        .RegionAggregationGroupEntities
                        .SingleOrDefault(e => e.IsAggregationGroupActive
                                           && e.RegionAggregationGroupDescription == aggregationGroup);

                    activeRegionAggregationGroup.IsAggregationGroupActive = false;
                    financeManagementContext.SaveChanges();
                }

                // Add the new account aggregation group and save the context to capture its group Id
                var newRegionAggregationGroup = new RegionAggregationGroupEntity
                {
                    RegionAggregationGroupDescription = aggregationGroup,
                    IsAggregationGroupActive = true
                };

                financeManagementContext.RegionAggregationGroupEntities.Add(newRegionAggregationGroup);
                financeManagementContext.SaveChanges();

                var newRegionAggregationGroupId = newRegionAggregationGroup.RegionAggregationGroupId;
                var listOfRegionAggregationGroupMappingEntities = new List<RegionAggregationGroupMappingEntity>();

                // Add new mappings for each record provided
                foreach (var financeManagementMappingRecord in financeManagementMappingRecords)
                {
                    if (Regions.ContainsKey(financeManagementMappingRecord.Description))
                    {
                        var newRegionAggregationGroupMappingEntity = new RegionAggregationGroupMappingEntity
                        {
                            RegionAggregationGroupId = newRegionAggregationGroupId,
                            RegionKey = Regions[financeManagementMappingRecord.Description],
                            RegionAggregationGroupIdentifier = financeManagementMappingRecord.MappingIdentifier
                        };

                        listOfRegionAggregationGroupMappingEntities.Add(newRegionAggregationGroupMappingEntity);
                    }
                    else
                    {
                        throw new Exception(string.Format("ERROR: A region named '{0}' does not exist. Please check your mapping file input or make a request to add this account.",
                            financeManagementMappingRecord.Description));
                    }
                }

                financeManagementContext.RegionAggregationGroupMappingEntities.AddRange(listOfRegionAggregationGroupMappingEntities);
                financeManagementContext.SaveChanges();
            }
        }
    }
}
