using ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS.IMPORT_MODEL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS.SETUP_MODEL;
using ELIXIR.DATA.DTOs.IMPORT_DTOs;
using ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS.QC_MODEL;
using ELIXIR.DATA.DTOs.SETUP_DTOs;

namespace ELIXIR.DATA.CORE.INTERFACES.IMPORT_INTERFACE
{
    public interface IImportRepository
    {
       //---PO SUMMARY--------
        List<ImportPOSummary> InsertListOfPOSummary(List<ImportPOSummary> summarys);
        Task<bool> AddNewPoSummary(ImportPOSummary posumamry);
        Task<bool> ValidateExcelTemplate(IFormFile file);
        Task<bool> ValidateFileIfNull(IFormFile file);
        Task<bool> ValidateExcelPerRow(IFormFile file, int rowExcel);
        Task<bool> CheckPRNumber(string prnumber);
        Task<bool> CheckItemCode(string rawmaterial);
        Task<bool> CheckUomCode(string uom);
        Task<bool> CheckSupplier(string supplier);
        Task<bool> ValidatePOAndItemcode(string ponumber, string itemcode);
        Task<bool> ValidatePOAndItemcodeManual(int ponumber, string itemcode);
        Task<bool>ValidateExcelColumnNumber (string number);
        Task<bool> ValidateExcelColumnDecimal(string decimalnum);
        Task<bool> AddNewPORequest(ImportPOSummary posummary);

        //-----PO SUMARRY GET------ 
        Task<IReadOnlyList<PO_SummaryDto>> GetAllPoSummary();
        Task<PO_SummaryDto> GetPoSummaryById(int id);

        //------RAW MATERIALS----------
        List<RawMaterial> InsertListOfRawMaterial(List<RawMaterial> rawmaterials);
        Task<int> CheckItemCategory(string category);
        Task<bool> ValidateItemCategory(string category);
        Task<int> CheckUom(string uom);
        Task<bool> ValidateUom(string uom);
        Task<bool> CheckItemCodeInRawMaterial(string itemcode);
        Task<bool> ValidateExcelTemplateRawMaterial(IFormFile file);
        Task<bool> ValidateExcelPerRowRawMaterial(IFormFile file, int rowExcel);


        //-----FORMULA-------
        List<TransformationRequirement> InsertListOfFormulaCode(List<TransformationRequirement> formulaCode);
        Task <bool> ValidateFormulaCode(string code, string description, string version);
        Task<bool> ValidateRawMaterialAndDescription(string itemcode, string description);
        Task<int> CheckFormulaCodeId(string code, string description, string version);
        Task<int> CheckRawMaterialId(string itemcode, string description);
        Task<bool> ValidateFormulaAndRawMaterial(int formula, int rawmaterial);
        Task<bool> ValidateExcelTemplateFormula(IFormFile file);
        Task<bool> ValidateExcelPerRowFormula(IFormFile file, int rowExcel);



        Task<bool> AddNewRawMaterialSummary(RawMaterial materials);


        Task<bool> AddNewFormulaSummary(TransformationRequirement requirement);


        Task<RawMaterialDto> GetAllRawMaterialByItemCode(string material);

        Task<bool> ValidateFormulaQuantity(TransformationRequirement requirement);

        Task<decimal> GetFormulaCodeQuantity(int id);

        Task<bool> AddNewSupplierSummary(Supplier  supply);

    }
}
