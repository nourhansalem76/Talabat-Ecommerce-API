using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Talabat.Core.Entities;
using Talabat.Core.Repositries;
using Talabat.Core.Specifications.Employee_Spec;

namespace Talabat.APIs.Controllers
{

    public class EmployeeController : ApiBaseController
    {
        private readonly IGenericRepository<Employee> _employeeRepo;

        public EmployeeController(IGenericRepository<Employee> EmployeeRepo)
        {
            _employeeRepo = EmployeeRepo;
        }

        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<Employee>>> GetAllEmployeesSpecAsync()
        {
            var Spec = new EmployeeWithDepartmentSpecification();
            var Employees = await _employeeRepo.GetEntityAsyncSpec(Spec);
            return Ok(Employees);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Employee>> GetEmployeeById(int id)
        {
            var Spec = new EmployeeWithDepartmentSpecification(id);
            var Employee = await _employeeRepo.GetEntityAsyncSpec(Spec);
            return Ok(Employee);
        }

    }
}
