using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GamingSessionApp.DataAccess;

namespace GamingSessionApp.BusinessLogic
{
    public interface IBusinessLogic<T>
    {
        Task<List<T>> GetAll();

        T GetById(int id);

        Task<T> GetByIdAsync(int id);
    }
}
