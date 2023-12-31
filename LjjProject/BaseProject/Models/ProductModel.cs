﻿using BaseProject.Data.Base;
using System.ComponentModel.DataAnnotations;

namespace BaseProject.Models
{
    public class ProductModel
    {
        //상품명, 가격, 소모되는 자재량, 사진, 상태, 수정날짜
        [Key]
        public int Id { get; set; }
        public string name { get; set; }
        public List<MaterialModel> materialModels { get; set; }
        public string ImgUrl { get; set; }
        public DateTime CreateTime { get; set; }
    }
}
