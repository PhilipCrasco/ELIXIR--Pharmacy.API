using ELIXIR.DATA.CORE.INTERFACES.IMPORT_INTERFACE;
using ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS.IMPORT_MODEL;
using ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS.SETUP_MODEL;
using ELIXIR.DATA.DATA_ACCESS_LAYER.STORE_CONTEXT;
using ELIXIR.DATA.DTOs.IMPORT_DTOs;
using ELIXIR.DATA.DTOs.SETUP_DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ELIXIR.DATA.DATA_ACCESS_LAYER.REPOSITORIES.IMPORT_REPOSITORY
{
    public class ImportRepository : IImportRepository

    {
        private readonly StoreContext _context;
        public ImportRepository(StoreContext context) 
        {
            _context = context;
        }

        //----PO SUMMARY-------


        public async Task<bool> AddNewPoSummary(ImportPOSummary posummary)
        {




            await _context.POSummary.AddAsync(posummary);

            return true;
        }

        public List<ImportPOSummary> InsertListOfPOSummary(List<ImportPOSummary> summarys)
        {
             _context.BulkInsert(summarys);
       
            return summarys; 
        }
        public async Task<bool> AddNewPORequest(ImportPOSummary posummary)
        {

            posummary.PR_Date = Convert.ToDateTime(posummary.PR_Date);
            posummary.PO_Date = Convert.ToDateTime(posummary.PO_Date);

            posummary.ImportDate = DateTime.Now;

            posummary.IsActive = true;

            var existingInfo = await _context.RawMaterials.Where(x => x.ItemCode == posummary.ItemCode)
                                                          .FirstOrDefaultAsync();

            if (existingInfo == null)
                return false;


            posummary.ItemDescription = existingInfo.ItemDescription;

            await _context.POSummary.AddAsync(posummary);
            return true;
        }

        public async Task<bool> ItemCodeExist(string itemcode)
        {
           await _context.RawMaterials.AnyAsync(x => x.ItemCode == itemcode);
           return true;
        }

        public async Task<bool> ValidateExcelTemplate(IFormFile formfile)
        {
            using (var stream = new MemoryStream())
            {

                await formfile.CopyToAsync(stream);

                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

                using (var package = new ExcelPackage(stream))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                    var columnCount = worksheet.Dimension.Columns;

                    if (columnCount != 12)
                        return false;
                }
            }

            return true;

        }
        public async Task<bool> ValidateFileIfNull(IFormFile formfile)
        {
            using (var stream = new MemoryStream())
            {

                await formfile.CopyToAsync(stream);

                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

                using (var package = new ExcelPackage(stream))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                    var rowCount = worksheet.Dimension;
                  
                    if (rowCount == null)
                        return false;
                }
            }

            return true;
        }

        public async Task<bool> ValidateExcelPerRow(IFormFile formfile, int rowExcel)
        {
            using (var stream = new MemoryStream())
            {

                await formfile.CopyToAsync(stream);

                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

                using (var package = new ExcelPackage(stream))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                    var rowCount = worksheet.Dimension.Rows;

                    for (int x = 1; x <=12; x++)
                    {
                        if (worksheet.Cells[rowExcel, x].Value == null)
                            return false;
                    }
                }
            }

            return true;
        }

        public async Task<bool> ValidateItemCode(string itemcode)
        {
           return await _context.RawMaterials.AnyAsync(x => x.ItemCode == itemcode);
        }

        public async Task<bool> CheckItemCode(string rawmaterial)
        {
            var validate = await _context.RawMaterials.Where(x => x.ItemCode == rawmaterial)
                                                      .Where(x => x.IsActive == true)
                                                      .FirstOrDefaultAsync();

            if (validate == null)
                return false;

            return true;
        }

        public async Task<bool> CheckUomCode(string uom)
        {
            var validate = await _context.UOMS.Where(x => x.UOM_Description == uom)
                                              .Where(x => x.IsActive == true)
                                              .FirstOrDefaultAsync();

            if (validate == null)
                return false;

            return true;
        }

        public async Task<bool> CheckSupplier(string supplier)
        {
            var validate = await _context.Suppliers.Where(x => x.SupplierName == supplier)
                                                   .Where(x => x.IsActive == true)
                                                   .FirstOrDefaultAsync();
            if (validate == null)
                return false;

            return true;
        }

        public async Task<bool> CheckPRNumber(string prnumber)
        {
            return await _context.POSummary.AnyAsync(x => x.PR_Number == Convert.ToInt32(prnumber));
        }

        public async Task<bool> ValidatePOAndItemcode(string ponumber, string itemcode)
        {
            var validate = await _context.POSummary.Where(x => x.PO_Number == Convert.ToInt32(ponumber))
                                                   .Where(x => x.ItemCode == itemcode)
                                                   .FirstOrDefaultAsync();

            if (validate == null)
                return false;

            return true;
        }

        public Task<bool> ValidateExcelColumnNumber(string number)
        {
     
            var validate =  int.TryParse(number, out int value);

             if(validate == true)
                return Task.FromResult(true);


            return Task.FromResult(false);
        }

        public Task<bool> ValidateExcelColumnDecimal(string decimalnum)
        {
            var validate = decimal.TryParse(decimalnum, out decimal value);

            if (validate == true)
                return Task.FromResult(true);


            return Task.FromResult(false);

        }

        //----RAW MATERIAL------

        public List<RawMaterial> InsertListOfRawMaterial(List<RawMaterial> rawmaterials)
        {
            _context.BulkInsert(rawmaterials);

            return rawmaterials;
        }

        public async Task<int> CheckItemCategory(string category)
        {
            var validate = await _context.ItemCategories.Where(x => x.ItemCategoryName == category)
                                                        .Where(x => x.IsActive == true)
                                                        .FirstOrDefaultAsync();

            var validateId = validate.Id;

            return validateId;
        }

        public async Task<int> CheckUom(string uom)
        {
            var validate = await _context.UOMS.Where(x => x.UOM_Description == uom)
                                              .Where(x => x.IsActive == true)
                                              .FirstOrDefaultAsync();

            var validateId = validate.Id;

            return validateId;
        }

        public async Task<bool> CheckItemCodeInRawMaterial(string itemcode)
        {

            var validate = await _context.RawMaterials.Where(x => x.ItemCode == itemcode)
                                                      .Where(x => x.IsActive == true)
                                                      .FirstOrDefaultAsync();
             
            if (validate == null)
                return false;

            return true;
        }

        public async Task<bool> ValidateExcelTemplateRawMaterial(IFormFile formfile)
        {
            using (var stream = new MemoryStream())
            {

                await formfile.CopyToAsync(stream);

                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

                using (var package = new ExcelPackage(stream))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                    var columnCount = worksheet.Dimension.Columns;

                    if (columnCount != 5)
                        return false;
                }
            }

            return true;
        }

        public async Task<bool> ValidateExcelPerRowRawMaterial(IFormFile formfile, int rowExcel)
        {
            using (var stream = new MemoryStream())
            {

                await formfile.CopyToAsync(stream);

                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

                using (var package = new ExcelPackage(stream))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                    var rowCount = worksheet.Dimension.Rows;

                    for (int x = 1; x <= 5; x++)
                    {
                        if (worksheet.Cells[rowExcel, x].Value == null)
                            return false;
                    }
                }
            }

            return true;
        }

        public async Task<bool> ValidateItemCategory(string category)
        {
            var validate = await _context.ItemCategories.Where(x => x.ItemCategoryName == category)
                                                        .Where(x => x.IsActive == true)
                                                        .FirstOrDefaultAsync();
            if (validate == null)
                return false;

            return true;

        }
        public async Task<bool> ValidateUom(string uom)
        {
            var validate = await _context.UOMS.Where(x => x.UOM_Description == uom)
                                              .Where(x => x.IsActive == true)
                                              .FirstOrDefaultAsync();
            if (validate == null)
                return false;

            return true;
        }

        public async Task<bool> ValidateFormulaCode(string code, string description, string version)
        {
            var validate = await _context.Formulas.Where(x => x.ItemCode == code)
                                                  .Where(x => x.ItemDescription == description)
                                                  .Where(x => x.Version == Convert.ToInt32(version))
                                                  .Where(x => x.IsActive == true)   
                                                  .FirstOrDefaultAsync();

            if (validate == null)
                return false;

            return true;

        }

        public async Task<bool> ValidateRawMaterialAndDescription(string itemcode, string description)
        {
            var validate = await _context.RawMaterials.Where(x => x.ItemCode == itemcode)
                                                      .Where(x => x.ItemDescription == description)
                                                      .Where(x => x.IsActive == true)
                                                      .FirstOrDefaultAsync();

            if (validate == null)
                return false;

            return true;
        }

        public async Task<int> CheckFormulaCodeId(string code, string description, string version)
        {
            var validate = await _context.Formulas.Where(x => x.ItemCode == code)
                                                  .Where(x => x.ItemDescription == description)
                                                  .Where(x => x.Version == Convert.ToInt32(version))
                                                  .Where(x => x.IsActive == true)
                                                  .FirstOrDefaultAsync();

            var validateId = validate.Id;

            return validateId;
        }

        public async Task<int> CheckRawMaterialId(string itemcode, string description)
        {
            var validate = await _context.RawMaterials.Where(x => x.ItemCode == itemcode)
                                                      .Where(x => x.ItemDescription == description)
                                                      .Where(x => x.IsActive == true)
                                                      .FirstOrDefaultAsync();

            var validateId = validate.Id;

            return validateId;
        }

        public List<TransformationRequirement> InsertListOfFormulaCode(List<TransformationRequirement> formulaCode)
        {
            _context.BulkInsert(formulaCode);

            return formulaCode;
        }

        public async Task<bool> ValidateFormulaAndRawMaterial(int formula, int rawmaterial)
        {
            var validate = await _context.FormulaRequirements.Where(x => x.TransformationFormulaId == formula)
                                                             .Where(x => x.RawMaterialId == rawmaterial)
                                                             .FirstOrDefaultAsync();

            if (validate == null)
                return false;

            return true;
        }

        public async Task<bool> ValidateExcelTemplateFormula(IFormFile formfile)
        {
            using (var stream = new MemoryStream())
            {

                await formfile.CopyToAsync(stream);

                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

                using (var package = new ExcelPackage(stream))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                    var columnCount = worksheet.Dimension.Columns;

                    if (columnCount != 6)
                        return false;
                }
            }

            return true;
        }

        public async Task<bool> ValidateExcelPerRowFormula(IFormFile formfile, int rowExcel)
        {
            using (var stream = new MemoryStream())
            {

                await formfile.CopyToAsync(stream);

                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

                using (var package = new ExcelPackage(stream))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                    var rowCount = worksheet.Dimension.Rows;

                    for (int x = 1; x <= 6; x++)
                    {
                        if (worksheet.Cells[rowExcel, x].Value == null)
                            return false;
                    }
                }
            }

            return true;
        }

        public async Task<bool> ValidatePOAndItemcodeManual(int ponumber, string itemcode)
        {
              var validate = await _context.POSummary.Where(x => x.PO_Number == ponumber)
                                                 .Where(x => x.ItemCode == itemcode)
                                                   .FirstOrDefaultAsync();

            if (validate == null)
                return false;

            return true;
        }

        public async Task<IReadOnlyList<PO_SummaryDto>> GetAllPoSummary()
        {
            var posummary = (from po in _context.POSummary
                             join itemcode in _context.RawMaterials on po.ItemCode equals itemcode.ItemCode
                             join uom in _context.UOMS on po.UOM equals uom.UOM_Description
                             join supplier in _context.Suppliers on po.VendorName equals supplier.SupplierName
                             select new PO_SummaryDto
                             {
                                 Id = po.Id,
                                 PR_Number = po.PR_Number,
                                 PR_Date = (po.PR_Date).ToString("MM/dd/yyyy"),
                                 PO_Number = po.PO_Number,
                                 PO_Date = (po.PO_Date).ToString("MM/dd/yyyy"),
                                 ItemCode = itemcode.ItemCode,
                                 ItemDescription = itemcode.ItemDescription,
                                 Ordered = po.Ordered,
                                 Delivered = po.Delivered,
                                 Billed = po.Billed,
                                 UOM = uom.UOM_Description,
                                 UnitPrice = po.UnitPrice,
                                 VendorName = supplier.SupplierName
                             });
            return await posummary.ToListAsync();
        }

        public async Task<PO_SummaryDto> GetPoSummaryById(int id)
        {
            var posummary = (from po in _context.POSummary
                             join itemcode in _context.RawMaterials on po.ItemCode equals itemcode.ItemCode
                             join uom in _context.UOMS on po.UOM equals uom.UOM_Description
                             join supplier in _context.Suppliers on po.VendorName equals supplier.SupplierName
                             select new PO_SummaryDto
                             {
                                 Id = po.Id,
                                 PR_Number = po.PR_Number,
                                 PR_Date = (po.PR_Date).ToString("MM/dd/yyyy"),
                                 PO_Number = po.PO_Number,
                                 PO_Date = (po.PO_Date).ToString("MM/dd/yyyy"),
                                 ItemCode = itemcode.ItemCode,
                                 ItemDescription = itemcode.ItemDescription,
                                 Ordered = po.Ordered,
                                 Delivered = po.Delivered,
                                 Billed = po.Billed,
                                 UOM = uom.UOM_Description,
                                 UnitPrice = po.UnitPrice,
                                 VendorName = supplier.SupplierName
                             });
            return await posummary.Where(x => x.Id == id)
                                  .FirstOrDefaultAsync();
        }

        public async Task<bool> AddNewRawMaterialSummary(RawMaterial materials)
        {

            materials.IsActive = true;
            materials.DateAdded = DateTime.Now;

            await _context.RawMaterials.AddAsync(materials);
            return true;
        }

        public async Task<bool> AddNewFormulaSummary(TransformationRequirement requirement)
        {
            await _context.FormulaRequirements.AddAsync(requirement);

            return true;
        }

        public async Task<UomDto> GetAllUomByItemCode(string uom)
        {
            var rawmats = _context.UOMS.Select(uoms => new UomDto
            {
                Id = uoms.Id,
                UOM_Code = uoms.UOM_Code,
                UOM_Description = uoms.UOM_Description,
                IsActive = uoms.IsActive
            });


            return await rawmats.Where(x => x.UOM_Code == uom)
                                .Where(x => x.IsActive == true)
                                .FirstOrDefaultAsync();

                      
        }

        public Task<RawMaterialDto> GetAllRawMaterialByItemCode(string material)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> ValidateFormulaQuantity(TransformationRequirement requirement)
        {
            var formula = await (from formulacode in _context.Formulas
                           join requirements in _context.FormulaRequirements
                           on formulacode.Id equals requirements.TransformationFormulaId into leftJ
                           from requirements in leftJ.DefaultIfEmpty()

                           select new TransformationRequirementDto
                           {

                               Id = formulacode.Id,
                               FormulaCode = formulacode.ItemCode,
                               FormulaDescription = formulacode.ItemDescription,
                               FormulaVersion = formulacode.Version,
                               FormulaQuantity = formulacode.Quantity,
                               RequirementCode = requirements.RawMaterial.ItemCode,
                               RequirementDescription = requirements.RawMaterial.ItemDescription,
                               RequirementQuantity = requirements.Quantity

                           }).Where(x => x.Id == requirement.TransformationFormulaId)
                             .ToListAsync();
            return false;

        }

        public async Task<decimal> GetFormulaCodeQuantity(int id)
        {
            var totalquantity = await _context.Formulas.Where(x => x.Id == id)
                                                       .FirstOrDefaultAsync();

            return Convert.ToDecimal(totalquantity.Quantity);

        }

        public async Task<bool> AddNewSupplierSummary(Supplier supply)
        {

            supply.IsActive = true;
            supply.DateAdded = DateTime.Now;

            await _context.Suppliers.AddAsync(supply);

            return true;

        }
    }
}
