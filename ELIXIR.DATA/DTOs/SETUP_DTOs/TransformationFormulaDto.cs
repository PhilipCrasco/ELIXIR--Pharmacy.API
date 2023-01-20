using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIR.DATA.DTOs.SETUP_DTOs
{
    public class TransformationFormulaDto
    {
        public int Id { get; set; }
        public int TransformationFormulaId { get; set; }
        public string ItemCode { get; set; }
        public string ItemDescription { get; set; }
        public int Version { get; set; }
        public decimal Quantity { get; set; }
        public string DateAdded { get; set; }
        public string AddedBy { get; set; }
        public bool IsActive { get; set; }
        public string Reason { get; set; }
        public bool CountFormula { get; set; }
        public string Uom { get; set; }

        public decimal CountQuantity { get; set; }




    }
}
