using ELIXIR.DATA.CORE.ICONFIGURATION;
using ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS.IMPORT_MODEL;
using ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS.SETUP_MODEL;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace ELIXIR.API.Controllers.IMPORT_CONTROLLER
{
    public class ImportController : BaseApiController
    {
        private IUnitOfWork _unitOfWork;
        public ImportController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpPost]
        [Route("UploadPOSummaryReport")]
        public async Task<ActionResult<List<ImportPOSummary>>> ImportPoExcelFile
                    (IFormFile formfile,
                     CancellationToken cancellationToken)
        {

            var validateFile = await _unitOfWork.Imports.ValidateFileIfNull(formfile);

            if (validateFile == false)
                return BadRequest("Upload Failed, You're trying to upload blank excel file!");

            if (formfile == null || formfile.Length <= 0)
                return BadRequest("Upload Failed, Please select file!");

            if (!Path.GetExtension(formfile.FileName).Equals(".xlsx", StringComparison.OrdinalIgnoreCase))
                return BadRequest("Not supported File!, Please try to upload a right file.");

            var validateTemplate = await _unitOfWork.Imports.ValidateExcelTemplate(formfile);

            if (validateTemplate == false)
                return BadRequest("You're using a wrong template, Please check your template first");

            var posummary = new List<ImportPOSummary>();

            using (var stream = new MemoryStream())
            {

                await formfile.CopyToAsync(stream, cancellationToken);

                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

                using (var package = new ExcelPackage(stream))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                    var rowCount = worksheet.Dimension.Rows;
                    var columnCount = worksheet.Dimension.Columns;

                    for (int row = 2; row <= rowCount; row++)
                    {

                        var validateRow = await _unitOfWork.Imports.ValidateExcelPerRow(formfile, row);
                        if (validateRow == false)
                            return BadRequest("Some of the columns are null, Please check your data!");

                        var prnumber = worksheet.Cells[row, 1].Value.ToString().Trim();
                        var validateprnumber = await _unitOfWork.Imports.ValidateExcelColumnNumber(prnumber);
                        if (validateprnumber == false)
                            return BadRequest("Please check your data in column PR Number!");

                        var prdate = worksheet.Cells[row, 2].Value.ToString().Trim();


                        var ponumber = worksheet.Cells[row, 3].Value.ToString().Trim();
                        var validateponumber = await _unitOfWork.Imports.ValidateExcelColumnNumber(prnumber);
                        if (validateprnumber == false)
                            return BadRequest("Please check your data in column PO Number!");

                        var podate = worksheet.Cells[row, 4].Value.ToString().Trim();
                        var itemcode = worksheet.Cells[row, 5].Value.ToString().Trim();

                        var validatePoandItem = await _unitOfWork.Imports.ValidatePOAndItemcode(ponumber, itemcode);
                        if (validatePoandItem == true)
                            return BadRequest("PO Number with Item code already exist!");

                        var validateItemcode = await _unitOfWork.Imports.CheckItemCode(itemcode);
                        if (validateItemcode == false)
                            return BadRequest("Some of the ItemCode not exist, Please input data first!");

                        var itemdescription = worksheet.Cells[row, 6].Value.ToString().Trim();

                        var quantityordered = worksheet.Cells[row, 7].Value.ToString().Trim();
                        var validatequantity = await _unitOfWork.Imports.ValidateExcelColumnNumber(quantityordered);
                        if (validatequantity == false)
                            return BadRequest("Please check your data in column Quantity Ordered!");

                        var quantitydelivered = worksheet.Cells[row, 8].Value.ToString().Trim();
                        var validatedelivered = await _unitOfWork.Imports.ValidateExcelColumnNumber(quantitydelivered);
                        if (validatedelivered == false)
                            return BadRequest("Please check your data in column Quantity Delivered!");

                        var quantitybilled = worksheet.Cells[row, 9].Value.ToString().Trim();
                        var validatebilled = await _unitOfWork.Imports.ValidateExcelColumnNumber(quantitybilled);
                        if (validatebilled == false)
                            return BadRequest("Please check your data in column Quantity Billed!");

                        var uom = worksheet.Cells[row, 10].Value.ToString().Trim();
                        var validateUom = await _unitOfWork.Imports.CheckUomCode(uom);
                        if (validateUom == false)
                            return BadRequest("Some of the Uom Code not exist, Please input data first!");

                        var unitprice = worksheet.Cells[row, 11].Value.ToString().Trim();
                        var validateunitprice = await _unitOfWork.Imports.ValidateExcelColumnDecimal(unitprice);
                        if (validateunitprice == false)
                            return BadRequest("Please check your data in column Unit Price!");

                        var vendorname = worksheet.Cells[row, 12].Value.ToString().Trim();
                        var validateSupplier = await _unitOfWork.Imports.CheckSupplier(vendorname);
                        if (validateSupplier == false)
                            return BadRequest("Some of the Supplier not exist, Please input data first!");

                        posummary.Add(new ImportPOSummary
                        {
                            PR_Number = Convert.ToInt32(prnumber),
                            PR_Date = Convert.ToDateTime(prdate),
                            PO_Number = Convert.ToInt32(ponumber),
                            PO_Date = Convert.ToDateTime(podate),
                            ItemCode = itemcode,
                            ItemDescription = itemdescription,
                            Ordered = Convert.ToInt32(quantityordered),
                            Delivered = Convert.ToInt32(quantitydelivered),
                            Billed = Convert.ToInt32(quantitybilled),
                            UOM = uom,
                            UnitPrice = Decimal.Parse(unitprice),
                            VendorName = vendorname,
                            IsActive = true
                        });
                    }

                    _unitOfWork.Imports.InsertListOfPOSummary(posummary);
                    await _unitOfWork.CompleteAsync();
                }

            }

            return posummary;
        }

        [HttpPost]
        [Route("UploadRawMaterialsReport")]
        public async Task<ActionResult<List<RawMaterial>>> ImportRawMaterialExcelFile
                         (IFormFile formfile,
                          CancellationToken cancellationToken)
        {

            var validateFile = await _unitOfWork.Imports.ValidateFileIfNull(formfile);

            if (validateFile == false)
                return BadRequest("Upload Failed, You're trying to upload blank excel file!");

            if (formfile == null || formfile.Length <= 0)
                return BadRequest("Upload Failed, Please select file!");

            if (!Path.GetExtension(formfile.FileName).Equals(".xlsx", StringComparison.OrdinalIgnoreCase))
                return BadRequest("Not supported File!, Please try to upload a right file.");

            var validateTemplate = await _unitOfWork.Imports.ValidateExcelTemplateRawMaterial(formfile);

            if (validateTemplate == false)
                return BadRequest("You're using a wrong template, Please check your template first");

            var rawmaterialSummary = new List<RawMaterial>();

            using (var stream = new MemoryStream())
            {

                await formfile.CopyToAsync(stream);

                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

                using (var package = new ExcelPackage(stream))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                    var rowCount = worksheet.Dimension.Rows;

                    for (int row = 2; row <= rowCount; row++)
                    {
                        var validateRow = await _unitOfWork.Imports.ValidateExcelPerRowRawMaterial(formfile, row);
                        if (validateRow == false)
                            return BadRequest("Some of the columns are null, Please check your data!");

                        var itemcode = worksheet.Cells[row, 1].Value.ToString().Trim();
                        var checkitemcode = await _unitOfWork.Imports.CheckItemCodeInRawMaterial(itemcode);
                        if (checkitemcode == true)
                            return BadRequest("Item Code is already exist!");

                        var itemdescription = worksheet.Cells[row, 2].Value.ToString().Trim();

                        var itemcategory = worksheet.Cells[row, 3].Value.ToString().Trim();
                        var validateitemCategory = await _unitOfWork.Imports.ValidateItemCategory(itemcategory);
                        if (validateitemCategory == false)
                            return BadRequest("Some of the Item Category not exist, Please input data first!");
                        var category = await _unitOfWork.Imports.CheckItemCategory(itemcategory);
                        var categoryId = category;

                        var uom = worksheet.Cells[row, 4].Value.ToString().Trim();
                        var validateuom = await _unitOfWork.Imports.ValidateUom(uom);
                        if (validateuom == false)
                            return BadRequest("Some of the Uom not exist, Please input data first!");
                        var validateUom = await _unitOfWork.Imports.CheckUom(uom);
                        var validateUomId = validateUom;

                        var buffer = worksheet.Cells[row, 5].Value.ToString().Trim();
                        var validatebuffer = await _unitOfWork.Imports.ValidateExcelColumnDecimal(buffer);
                        if (validatebuffer == false)
                            return BadRequest("Please check your data in column Unit Price!");

                        rawmaterialSummary.Add(new RawMaterial
                        {
                            ItemCode = itemcode,
                            ItemDescription = itemdescription,
                            ItemCategoryId = Convert.ToInt32(categoryId),
                            UomId = Convert.ToInt32(validateUomId),
                            BufferLevel = Decimal.Parse(buffer),
                            DateAdded = DateTime.Now,
                            AddedBy = "Admin",
                            IsActive = true
                        });
                    }

                    _unitOfWork.Imports.InsertListOfRawMaterial(rawmaterialSummary);
                    await _unitOfWork.CompleteAsync();

                }
            }

            return rawmaterialSummary;
        }

        [HttpPost]
        [Route("UploadFormulaReport")]
        public async Task<ActionResult<List<TransformationRequirement>>> ImportFormulaExcelFile
                         (IFormFile formfile,
                          CancellationToken cancellationToken)
        {
            var validateFile = await _unitOfWork.Imports.ValidateFileIfNull(formfile);

            if (validateFile == false)
                return BadRequest("Upload Failed, You're trying to upload blank excel file!");

            if (formfile == null || formfile.Length <= 0)
                return BadRequest("Upload Failed, Please select file!");

            if (!Path.GetExtension(formfile.FileName).Equals(".xlsx", StringComparison.OrdinalIgnoreCase))
                return BadRequest("Not supported File!, Please try to upload a right file.");

            var validateTemplate = await _unitOfWork.Imports.ValidateExcelTemplateFormula(formfile);

            if (validateTemplate == false)
                return BadRequest("You're using a wrong template, Please check your template first");

            using (var stream = new MemoryStream())
            {

                await formfile.CopyToAsync(stream);

                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

                var formulaSummary = new List<TransformationRequirement>();

                using (var package = new ExcelPackage(stream))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                    var rowCount = worksheet.Dimension.Rows;

                    for (int row = 2; row <= rowCount; row++)
                    {

                        var validateRow = await _unitOfWork.Imports.ValidateExcelPerRowFormula(formfile, row);
                        if (validateRow == false)
                            return BadRequest("Some of the columns are null, Please check your data!");

                        var formulacode = worksheet.Cells[row, 1].Value.ToString().Trim();
                        var formulaname = worksheet.Cells[row, 2].Value.ToString().Trim();
                        var version = worksheet.Cells[row, 3].Value.ToString().Trim();

                        var validateFormula = await _unitOfWork.Imports.ValidateFormulaCode(formulacode, formulaname, version);

                        if (validateFormula == false)
                            return BadRequest("Formula Code and Version not exist, Please check your data!");

                        var formula = await _unitOfWork.Imports.CheckFormulaCodeId(formulacode, formulaname, version);
                        var formulaid = formula;

                        var itemcode = worksheet.Cells[row, 4].Value.ToString().Trim();
                        var itemdescription = worksheet.Cells[row, 5].Value.ToString().Trim();

                        var validateRawmaterial = await _unitOfWork.Imports.ValidateRawMaterialAndDescription(itemcode, itemdescription);

                        if (validateRawmaterial == false)
                            return BadRequest("Raw Material with Description not exist, Please check your data!");

                        var rawMaterial = await _unitOfWork.Imports.CheckRawMaterialId(itemcode, itemdescription);
                        var rawMaterialid = rawMaterial;

                        var formulaAndrawmaterial = await _unitOfWork.Imports.ValidateFormulaAndRawMaterial(formulaid, rawMaterialid);

                        if (formulaAndrawmaterial == true)
                            return BadRequest("Formula with requirements already exist!");

                        var quantity = worksheet.Cells[row, 6].Value.ToString().Trim();
                        var validatequantity = await _unitOfWork.Imports.ValidateExcelColumnDecimal(quantity);
                        if (validatequantity == false)
                            return BadRequest("Please check your data in column Unit Price!");

                        formulaSummary.Add(new TransformationRequirement
                        {
                            TransformationFormulaId = Convert.ToInt32(formulaid),
                            RawMaterialId = Convert.ToInt32(rawMaterialid),
                            ItemDescription = itemdescription,
                            Quantity = Decimal.Parse(quantity),
                            AddedBy = "Admin"

                        });

                    }

                    _unitOfWork.Imports.InsertListOfFormulaCode(formulaSummary);

                    await _unitOfWork.CompleteAsync();
                }
                return formulaSummary;
            }

        }

        [HttpPost]
        [Route("AddNewPOManual")]
        public async Task<ActionResult> CreateNewPo([FromBody] ImportPOSummary[] posummary)
        {

            if (ModelState.IsValid)
            {

                List<ImportPOSummary> duplicateList = new List<ImportPOSummary>();
                List<ImportPOSummary> availableImport = new List<ImportPOSummary>();
                List<ImportPOSummary> supplierNotExist = new List<ImportPOSummary>();
                List<ImportPOSummary> itemcodeNotExist = new List<ImportPOSummary>();
                List<ImportPOSummary> uomCodeNotExist = new List<ImportPOSummary>();

                foreach (ImportPOSummary items in posummary)
                {

                    var validateSupplier = await _unitOfWork.Imports.CheckSupplier(items.VendorName);
                    var validateItemCode = await _unitOfWork.Imports.CheckItemCode(items.ItemCode);
                    var validatePoandItem = await _unitOfWork.Imports.ValidatePOAndItemcodeManual(items.PO_Number, items.ItemCode);
                    var validateUom = await _unitOfWork.Imports.CheckUomCode(items.UOM);

                    if (validatePoandItem == true)
                    {
                        duplicateList.Add(items);
                    }

                    else if (validateSupplier == false)
                    {
                        supplierNotExist.Add(items);
                    }

                    else if (validateUom == false)
                    {
                        uomCodeNotExist.Add(items);
                    }

                    else if (validateItemCode == false)
                    {
                        itemcodeNotExist.Add(items);
                    }

                    else
                        availableImport.Add(items);

                    await _unitOfWork.Imports.AddNewPORequest(items);

                }

                var resultList = new
                {
                    availableImport,
                    duplicateList,
                    supplierNotExist,
                    itemcodeNotExist,
                    uomCodeNotExist
                };

                if (duplicateList.Count == 0 && supplierNotExist.Count == 0 && itemcodeNotExist.Count == 0 && uomCodeNotExist.Count == 0)
                {
                    await _unitOfWork.CompleteAsync();
                    return Ok("Successfully Add!");
                }

                else
                {

                    return BadRequest(resultList);
                }
            }
            return new JsonResult("Something went Wrong!") { StatusCode = 500 };
        }

        [HttpGet]
        [Route("GetAllPoSummary")]
        public async Task<IActionResult> GetAllPoSummary()
        {
            var posummary = await _unitOfWork.Imports.GetAllPoSummary();

            return Ok(posummary);
        }

        [HttpGet]
        [Route("GetPoSummaryById/{id}")]
        public async Task<IActionResult> GetPoSummaryById(int id)
        {
            var posummary = await _unitOfWork.Imports.GetPoSummaryById(id);

            return Ok(posummary);
        }

        [HttpPost]
        [Route("AddNewRawMaterialManual")]
        public async Task<IActionResult> AddNewRawMaterialManual([FromBody] RawMaterial[] material)
        {
            if (ModelState.IsValid)
            {

                foreach (RawMaterial items in material)
                {
                    var itemCategoryId = await _unitOfWork.RawMaterials.ValidateItemCategoryId(items.ItemCategoryId);
                    var uomId = await _unitOfWork.RawMaterials.ValidateUOMId(items.UomId);

                    if (itemCategoryId == false)
                        return BadRequest("Item Category doesn't exist, Please add data first!");

                    if (uomId == false)
                        return BadRequest("UOM doesn't exist, Please add data first!");

                    if (await _unitOfWork.RawMaterials.ItemCodeExist(items.ItemCode))
                        return BadRequest("Item Code already Exist!, Please try something else!");

                    await _unitOfWork.Imports.AddNewRawMaterialSummary(items);

                }

                await _unitOfWork.CompleteAsync();

            }
            return Ok("Successfully import raw materials!");
        }

        [HttpPost]
        [Route("AddNewFormulaManual")]
        public async Task<IActionResult> AddNewFormulaManual([FromBody] TransformationRequirement[] requirement)
        {

            if (ModelState.IsValid)
            {
                decimal totalamount = 0;
                int transformId = 0;
                int rawmats = 0;
                int formulacode = 0;

                foreach (TransformationRequirement items in requirement)
                {

                    totalamount = totalamount + items.Quantity;
                    transformId = items.TransformationFormulaId;

                    if (formulacode == items.TransformationFormulaId && rawmats == items.RawMaterialId)
                        return BadRequest("Import failed! There is duplicate data in your excel.");


                    if (formulacode != items.TransformationFormulaId && formulacode != 0)
                        return BadRequest("Import failed! Cannot import different formula code at the same time.");

                    var validateRawMaterial = await _unitOfWork.Transforms.ValidateRawMaterial(items.RawMaterialId);

                    var verifytagrequirement = await _unitOfWork.Transforms.ValidateTagRequirements(items);

                    var validateformula = await _unitOfWork.Transforms.ValidateFormulaIfActive(items.TransformationFormulaId);


                    if (validateRawMaterial == false)
                        return BadRequest("Raw Material doesn't exist, Please add data first!");

                    if (verifytagrequirement == false)
                        return BadRequest("Raw Material already added in the formula!");

                    if (validateformula == false)
                        return BadRequest("Formulation not exist, Please check your data!");

                    await _unitOfWork.Imports.AddNewFormulaSummary(items);

                    formulacode = items.TransformationFormulaId;
                    rawmats = items.RawMaterialId;

                }

                var validateQuantity = await _unitOfWork.Imports.GetFormulaCodeQuantity(transformId);
                var requirementId = validateQuantity;

                if (requirementId != totalamount)
                    return BadRequest("Upload failed! requirement quantity doesn't meet the formula needed.");


                await _unitOfWork.CompleteAsync();

            }

            return Ok("Successfully import formula requirements!");

        }

        [HttpPost]
        [Route("AddNewSupplierSummary")]
        public async Task<IActionResult> AddNewSupplierSummary([FromBody] Supplier[] supply)
        {

            if (ModelState.IsValid)
            {
                foreach (Supplier items in supply)
                {

                    if (await _unitOfWork.Suppliers.SupplierCodeExist(items.SupplierCode))
                        return BadRequest("Supplier name already exist, please try something else!");

                    await _unitOfWork.Imports.AddNewSupplierSummary(items);

                }

                await _unitOfWork.CompleteAsync();
            }

            return Ok("Successfully import supplier!");

        }


    }
}
