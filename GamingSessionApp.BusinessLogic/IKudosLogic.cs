using GamingSessionApp.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GamingSessionApp.BusinessLogic
{
    public interface IKudosLogic
    {
        Task<ValidationResult> AddKudosPoints(string userId, int value);
        Task<List<Kudos>> KudosLeadboard();
    }
}