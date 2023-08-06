using BaseProject.Data.Base;
using System.ComponentModel.DataAnnotations;

namespace BaseProject.Models
{
    public class ProductModel : IEntityBase
    {
        //상품명, 가격, 소모되는 자재량, 사진, 상태, 수정날짜
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public int Price { get; set; }        
        public string ImgUrl { get; set; }

        public string Status { get; set; }
        public DateTime CreateTime { get; set; }

        public List<ProductUseMetrailModel> ProductUseMetrailModels { get; set; }
        public List<ProductEditLogModel> ProductEditLogModels { get; set; }
    }
}
