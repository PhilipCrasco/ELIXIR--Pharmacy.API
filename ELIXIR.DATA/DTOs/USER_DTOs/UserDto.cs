﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIR.DATA.DTOs
{
    public class UserDto
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public bool Status { get; set; }
        public int UserRoleId { get; set; }
        public string UserRole { get; set; }
        public int DepartmentId { get; set; }
        public string Department { get; set; }
        public string DateAdded { get; set; }
        public string AddedBy { get; set; }
        public string ModifiedBy { get; set; }
        public string Reason { get; set; }
      


    }
}
