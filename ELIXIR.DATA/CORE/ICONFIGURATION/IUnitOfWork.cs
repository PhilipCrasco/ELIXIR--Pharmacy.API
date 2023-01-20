using ELIXIR.DATA.CORE.INTERFACES;
using ELIXIR.DATA.CORE.INTERFACES.IMPORT_INTERFACE;
using ELIXIR.DATA.CORE.INTERFACES.INVENTORY_INTERFACE;
using ELIXIR.DATA.CORE.INTERFACES.ORDERING_INTERFACE;
using ELIXIR.DATA.CORE.INTERFACES.QC_INTERFACE;
using ELIXIR.DATA.CORE.INTERFACES.REPORT_INTERFACE;
using ELIXIR.DATA.CORE.INTERFACES.SETUP_INTERFACE;
using ELIXIR.DATA.CORE.INTERFACES.TRANSFORMATION_INTERFACE;
using ELIXIR.DATA.CORE.INTERFACES.WAREHOUSE_INTERFACE;
using System.Threading.Tasks;

namespace ELIXIR.DATA.CORE.ICONFIGURATION
{
    public interface IUnitOfWork
    {
        //User
        IUserRepository Users { get; }
        IModuleRepository Modules { get; }
        IRoleRepository Roles { get; }

        //Setup
        IUomRepository Uoms { get; }
        ISupplierRepository Suppliers { get; }
        ILotNameAndCategoriesRepository Lots { get; }
        ICustomerRepository Customers { get; }
        IRawMaterialRepository RawMaterials { get; }
        IReasonRepository Reasons { get; }
        ITransformationRepository Transforms { get; }
        ITransactionRepository Transactions { get; }


      //Import
        IImportRepository Imports { get;  }

      //Receiving
        IReceivingRepository Receives { get; }

        //Warehouse
        IWarehouseReceive Warehouse { get; }


        //Transformation Planning
        ITransformationPlanning Planning { get; }

        //Transformation Preparation
        ITransformationPreparation Preparation { get; }


        //Ordering
        IOrdering Order { get; }

        //Inventory
        IRawMaterialInventory Inventory { get; }
        IMiscellaneous Miscellaneous { get; }

        IReportRepository Report { get;  }



        Task CompleteAsync();


    }
}
