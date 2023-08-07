﻿using BaseProject.Data.Base;
using System.ComponentModel.DataAnnotations;

namespace BaseProject.Models
{
    public class Order_Model : IEntityBase
    {
        [Key]
        public int Id { get; set; }
        public List<Order_Product_Model> OrderProducts { get; } = new List<Order_Product_Model>();
        public string Customer{ get; set; }
        // 현황
        public string Status { get; set; }
        // 등록날짜
        public string RegisterDate { get; set; }

        // 마감날짜
        public string EndDate { get; set; }

        public List<Order_Edit_Log_Model> OrderEditLogModels { get; } = new List<Order_Edit_Log_Model>();
    }
}