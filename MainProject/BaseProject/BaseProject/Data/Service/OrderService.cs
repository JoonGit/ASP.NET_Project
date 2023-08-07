using BaseProject.Data.Base;
using BaseProject.Models;

namespace BaseProject.Data.Service
{
    public class OrderService : EntityBaseRepository<Order_Model>
    {
        public OrderService(BaseDbContext context) : base(context)
        {
        }
    }
}
