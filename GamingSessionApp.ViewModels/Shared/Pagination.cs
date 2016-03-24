using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamingSessionApp.ViewModels.Shared
{
    public class Pagination
    {
        public int TotalCount { get; set; }

        public int PageNo { get; set; }

        public int PageSize { get; set; }
    }
}
