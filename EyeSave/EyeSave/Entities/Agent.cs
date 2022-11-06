using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace EyeSave.Entities
{
    public partial class Agent
    {
        private string? _logo;

        public Agent()
        {
            AgentPriorityHistories = new HashSet<AgentPriorityHistory>();
            ProductSales = new HashSet<ProductSale>();
            Shops = new HashSet<Shop>();
        }

        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public int AgentTypeId { get; set; }
        public string? Address { get; set; }
        public string Inn { get; set; } = null!;
        public string? Kpp { get; set; }
        public string? DirectorName { get; set; }
        public string Phone { get; set; } = null!;
        public string? Email { get; set; }
        public string? Logo 
        {
            get => (_logo == null) || (_logo == string.Empty)
                ? $"\\resources\\picture.png"
                : $"\\resources{_logo}";
            set => _logo = value; 
        }
        public int Priority { get; set; }

        public virtual AgentType AgentType { get; set; } = null!;
        public virtual ICollection<AgentPriorityHistory> AgentPriorityHistories { get; set; }
        public virtual ICollection<ProductSale> ProductSales { get; set; }
        public virtual ICollection<Shop> Shops { get; set; }

        [NotMapped]
        public int SalesPerLastYear
        { 
            get
            {
                var sales = 0;                

                if (ProductSales == null)
                    return sales;                

                var now = DateTime.Now;
                var yearAgo = now.AddYears(-10);

                sales = ProductSales.Where(ps => (now >= ps.SaleDate) && (ps.SaleDate > yearAgo)).Count();

                return sales;
            }
        }

        // [0-10000) -> 0%, [10000-50000) -> 5%, [50000-150000) -> 10%, [150000-500000) -> 20%, [500000-...] -> 25%
        [NotMapped]
        public int Discount
        {
            get
            {
                decimal sum = 0M;

                if (ProductSales == null)
                    return 0;

                sum = ProductSales.Sum(ps => ps.Product != null ? ps.ProductCount * ps.Product.MinCostForAgent : 0);

                return SumToDiscount(sum);
            }
        }

        private int SumToDiscount(decimal d) => d switch
        {
            < 10000 => 0,
            < 50000 => 5,
            < 150000 => 10,
            < 500000 => 20,
            _ => 25
        };
    }
}
