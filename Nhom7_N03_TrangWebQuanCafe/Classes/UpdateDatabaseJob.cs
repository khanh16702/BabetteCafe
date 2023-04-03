using Nhom7_N03_TrangWebQuanCafe.Models;
using Quartz;

namespace Nhom7_N03_TrangWebQuanCafe.Classes
{
    public class UpdateDatabaseJob : IJob
    {
        CafeWebsiteContext _dbContext = new CafeWebsiteContext();
        public Task Execute(IJobExecutionContext context)
        {
            return Task.FromResult(true);
        }
    }
}
