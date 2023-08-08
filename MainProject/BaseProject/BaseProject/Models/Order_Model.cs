﻿using BaseProject.Data.Base;
using System.ComponentModel.DataAnnotations;

namespace BaseProject.Models
{
    public class Order_Model : IEntityBase
    {
        [Key]
        public int Id { get; set; }
        
        public string Customer{ get; set; }
        // 현황
        public string Status { get; set; }

        public int[] ProductId { get; set; }
        public int[] Quantity { get; set; }
        // 등록날짜
        public DateTime RegisterDate { get; set; }

        // 마감날짜
        public DateTime EndDate { get; set; }

        public List<Order_Product_Model> OrderProducts { get; } = new List<Order_Product_Model>();
        public List<Order_Edit_Log_Model> OrderEditLogModels { get; } = new List<Order_Edit_Log_Model>();
    }
}
