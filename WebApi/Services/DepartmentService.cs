using Domain.Entities;
using Domain.Interfaces;

namespace WebApi.Services
{
    public class DepartmentService : IDepartmentService
    {
        private readonly IRepository<Department> _departmentRepository;

        public DepartmentService(IRepository<Department> departmentRepository)
        {
            _departmentRepository = departmentRepository;
        }

        public async Task<IEnumerable<Department>> GetAllDepartmentsAsync()
        {
            return await _departmentRepository.GetAllAsync();
        }

        public async Task<Department> GetDepartmentByIdAsync(int id)
        {
            return await _departmentRepository.GetByIdAsync(id);
        }

        public async Task AddDepartmentAsync(Department department)
        {
            await _departmentRepository.AddAsync(department);
            await _departmentRepository.SaveChangesAsync();
        }

        public async Task UpdateDepartmentAsync(Department department)
        {
            await _departmentRepository.UpdateAsync(department);
            await _departmentRepository.SaveChangesAsync();
        }

        public async Task DeleteDepartmentAsync(int id)
        {
            await _departmentRepository.DeleteAsync(id);
            await _departmentRepository.SaveChangesAsync();
        }

        public async Task<Department> GetDepartmentExistsByNameAsync(string departmentName)
        {
            var departments = await _departmentRepository.FindAsync(d => d.Name == departmentName);

            return departments.FirstOrDefault();
        }

    }

}
