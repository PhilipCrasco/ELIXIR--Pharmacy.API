﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS.SETUP_MODEL
{
    public class LotName : BaseEntity
    {
        public string LotNameCode {
            get; 
            set;
        }
        public LotCategory LotCategory { 
            get;
            set; 
        }
        public int LotCategoryId { 
            get; 
            set;
        }
        public string SectionName {
            get;
            set;
        }
        public DateTime DateAdded {
            get; 
            set;
        }
        public string AddedBy { 
            get;
            set;
        }
        public bool IsActive {
            get;
            set;
        }
        public string Reason {
            get; 
            set; 
        }

    }
}
