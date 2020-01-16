using Dream.Common;
using Dream.IO.Database.Entities.FinanceManagement;
using Dream.IO.Database.Mappings.FinanceManagement;
using System.Data.Entity;

namespace Dream.IO.Database.Contexts
{
    public class FinanceManagementContext : DbContext
    {
        public FinanceManagementContext(string connectionString) : base(connectionString)
        {
            System.Data.Entity.Database.SetInitializer<FinanceManagementContext>(null);
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema(Constants.FinanceManagementSchemaName);

            modelBuilder.Configurations.Add(new CalendarDimMapping());
            modelBuilder.Configurations.Add(new CompanyDimMapping());
            modelBuilder.Configurations.Add(new CostCenterAggregationGroupMapping());
            modelBuilder.Configurations.Add(new CostCenterAggregationGroupMappingMapping());
            modelBuilder.Configurations.Add(new CostCenterDimMapping());
            modelBuilder.Configurations.Add(new EmployeeDimMapping());
            modelBuilder.Configurations.Add(new EmployeeFactMapping());
            modelBuilder.Configurations.Add(new JournalLineFactMapping());
            modelBuilder.Configurations.Add(new LedgerAccountAggregationGroupMapping());
            modelBuilder.Configurations.Add(new LedgerAccountAggregationGroupMappingMapping());
            modelBuilder.Configurations.Add(new LedgerAccountDimMapping());
            modelBuilder.Configurations.Add(new LedgerAccountTypeDimMapping());
            modelBuilder.Configurations.Add(new LoanFactMapping());
            modelBuilder.Configurations.Add(new LoanLevelDimMapping());
            modelBuilder.Configurations.Add(new LoanProductDimMapping());
            modelBuilder.Configurations.Add(new ProductLineAggregationGroupMapping());
            modelBuilder.Configurations.Add(new ProductLineAggregationGroupMappingMapping());
            modelBuilder.Configurations.Add(new ProductLineDimMapping());
            modelBuilder.Configurations.Add(new RecordTypeDimMapping());
            modelBuilder.Configurations.Add(new RegionAggregationGroupMapping());
            modelBuilder.Configurations.Add(new RegionAggregationGroupMappingMapping());
            modelBuilder.Configurations.Add(new RegionDimMapping());
            modelBuilder.Configurations.Add(new RevenueCategoryDimMapping());
            modelBuilder.Configurations.Add(new SourceSystemDimMapping());
            modelBuilder.Configurations.Add(new SpendCategoryDimMapping());
        }

        public virtual DbSet<CalendarDimEntity> CalendarDimEntities { get; set; }
        public virtual DbSet<CompanyDimEntity> CompanyDimEntities { get; set; }
        public virtual DbSet<CostCenterAggregationGroupEntity> CostCenterAggregationGroupEntities { get; set; }
        public virtual DbSet<CostCenterAggregationGroupMappingEntity> CostCenterAggregationGroupMappingEntities { get; set; }
        public virtual DbSet<CostCenterDimEntity> CostCenterDimEntities { get; set; }
        public virtual DbSet<EmployeeDimEntity> EmployeeDimEntities { get; set; }
        public virtual DbSet<EmployeeFactEntity> EmployeeFactEntities { get; set; }
        public virtual DbSet<JournalLineFactEntity> JournalLineFactEntities { get; set; }
        public virtual DbSet<LedgerAccountAggregationGroupEntity> LedgerAccountAggregationGroupEntities { get; set; }
        public virtual DbSet<LedgerAccountAggregationGroupMappingEntity> LedgerAccountAggregationGroupMappingEntities { get; set; }
        public virtual DbSet<LedgerAccountDimEntity> LedgerAccountDimEntities { get; set; }
        public virtual DbSet<LedgerAccountTypeDimEntity> LedgerAccountTypeDimEntities { get; set; }
        public virtual DbSet<LoanFactEntity> LoanFactEntities { get; set; }
        public virtual DbSet<LoanLevelDimEntity> LoanLevelDimEntities { get; set; }
        public virtual DbSet<LoanProductDimEntity> LoanProductDimEntities { get; set; }
        public virtual DbSet<ProductLineAggregationGroupEntity> ProductLineAggregationGroupEntities { get; set; }
        public virtual DbSet<ProductLineAggregationGroupMappingEntity> ProductLineAggregationGroupMappingEntities { get; set; }
        public virtual DbSet<ProductLineDimEntity> ProductLineDimEntities { get; set; }
        public virtual DbSet<RecordTypeDimEntity> RecordTypeDimEntities { get; set; }
        public virtual DbSet<RegionAggregationGroupEntity> RegionAggregationGroupEntities { get; set; }
        public virtual DbSet<RegionAggregationGroupMappingEntity> RegionAggregationGroupMappingEntities { get; set; }
        public virtual DbSet<RegionDimEntity> RegionDimEntities { get; set; }
        public virtual DbSet<RevenueCategoryDimEntity> RevenueCategoryDimEntities { get; set; }
        public virtual DbSet<SourceSystemDimEntity> SourceSystemDimEntities { get; set; }
        public virtual DbSet<SpendCategoryDimEntity> SpendCategoryDimEntities { get; set; }
    }
}
