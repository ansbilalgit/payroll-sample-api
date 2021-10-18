using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Payroll_Api.Models
{
    public class Employees
    {
        [Key]
        public int EmployeeId
        {
            get;
            set;
        }
        public string EmployeeName
        {
            get;
            set;
        }
        public decimal Salary
        {
            get;
            set;
        }
        public int Age { get; set; }
        public DateTime PaymentDate { get; set; }
        public string Department { get; set; }
        public string JobType { get; set; }
        public int PaymentStatusId { get; set; }
    }
    public class PaymentStatus
    {
        [Key]
        public int PaymentStatusId
        {
            get;
            set;
        }
        public string StatusName
        {
            get;
            set;
        }
    }
}
