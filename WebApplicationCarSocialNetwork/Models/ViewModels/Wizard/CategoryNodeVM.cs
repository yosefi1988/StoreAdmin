using System.Collections.Generic;

namespace WebApplicationCar.Models.ViewModels.Wizard
{
    public class CategoryNodeVM
    {
        public int CategoryId { get; set; }
        public string NameFa { get; set; }
        public string CategoryType { get; set; }
        public int? FK_ParentId { get; set; }
        public List<CategoryNodeVM> Children { get; set; }
    }
}