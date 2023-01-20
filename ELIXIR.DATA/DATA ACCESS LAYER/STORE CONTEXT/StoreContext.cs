using ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS;
using ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS.IMPORT_MODEL;
using ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS.INVENTORY_MODEL;
using ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS.ORDERING_MODEL;
using ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS.QC_MODEL;
using ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS.SETUP_MODEL;
using ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS.TRANSFORMATION_MODEL;
using ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS.USER_MODEL;
using ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS.WAREHOUSE_MODEL;
using Microsoft.EntityFrameworkCore;

namespace ELIXIR.DATA.DATA_ACCESS_LAYER.STORE_CONTEXT
{
    public class StoreContext : DbContext
    {
        public StoreContext(DbContextOptions<StoreContext> options) : base(options) { }

        public virtual DbSet<User> Users {
            get; 
            set;
        }
        public virtual DbSet<UserRole> Roles { 
            get; 
            set; 
        }
        public virtual DbSet<Department> Departments {
            get;
            set;
        }
        public virtual DbSet<Module> Modules {
            get; 
            set; 
        }
        public virtual DbSet<UserRole_Modules> RoleModules {
            get; 
            set;
        }
        public virtual DbSet<MainMenu> MainMenus {
            get;
            set; 
        }
        public virtual DbSet<UOM> UOMS { 
            get;
            set; 
        }
        public virtual DbSet<Supplier> Suppliers {
            get;
            set; 
        }
        public virtual DbSet<LotName> LotNames { 
            get;
            set;
        }
        public virtual DbSet<LotCategory> LotCategories { 
            get; 
            set;
        }
        public virtual DbSet<Customer> Customers {
            get; 
            set;
        }
        public virtual DbSet<FarmType> Farms {
            get;
            set;
        }
        public virtual DbSet<RawMaterial> RawMaterials {
            get; 
            set; 
        }
        public virtual DbSet<ItemCategory> ItemCategories {
            get;
            set; 
        }
        public virtual DbSet<Reason> Reasons { 
            get; 
            set;
        }
        public virtual DbSet<TransformationFormula> Formulas {
            get;
            set; 
        }
        public virtual DbSet<TransformationRequirement> FormulaRequirements {
            get;
            set;
        }
        public virtual DbSet<ImportPOSummary> POSummary { 
            get; 
            set;
        }
        public virtual DbSet<PO_Receiving> QC_Receiving {
            get; 
            set; 
        }
        public virtual DbSet<PO_Reject> QC_Reject {
            get; 
            set; 
        }
        public virtual DbSet<WarehouseReceiving> WarehouseReceived { 
            get;
            set;
        }
        public virtual DbSet<Warehouse_Reject> Warehouse_Reject {
            get; 
            set;
        }
        public virtual DbSet<TransformationPlanning> Transformation_Planning {
            get;
            set;
        }
        public virtual DbSet<TransformationRequest> Transformation_Request {
            get; 
            set;
        }
        public virtual DbSet<TransformationReject> Transformation_Reject { 
            get;
            set; 
        }
        public virtual DbSet<TransformationPreparation> Transformation_Preparation {
            get;
            set; 
        }
        public virtual DbSet<Ordering> Orders
        {
            get;
            set;
        }
        public virtual DbSet<MoveOrder> MoveOrders { 
            get;
            set; 
        }
        public virtual DbSet<GenerateOrderNo> GenerateOrderNos { 
            get; 
            set; 
        }
        public virtual DbSet<TransactMoveOrder> TransactMoveOrder {
            get;
            set; 
        }
        public virtual DbSet<MiscellaneousReceipt> MiscellaneousReceipts { 
            get;
            set; 
        }
        public virtual DbSet<MiscellaneousIssue> MiscellaneousIssues {
            get;
            set; 
        }
        public virtual DbSet<MiscellaneousIssueDetails> MiscellaneousIssueDetails {
            get;
            set;
        }

        public virtual DbSet<Transaction> Transactions
        {
            get;
            set;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
} 
