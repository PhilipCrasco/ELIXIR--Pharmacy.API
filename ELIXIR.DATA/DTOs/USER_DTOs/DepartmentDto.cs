﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIR.DATA.DTOs.USER_DTOs
{
    public class DepartmentDto
    {
        public int Id { get; set; }
        public string DepartmentName { get; set; }
        public string AddedBy { get; set; }
        public string DateAdded { get; set; }
        public bool IsActive { get; set; }

    }
}
