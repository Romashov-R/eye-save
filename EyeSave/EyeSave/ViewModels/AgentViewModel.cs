using EyeSave.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Documents;

namespace EyeSave.ViewModels
{
    public class AgentViewModel : ViewModelBase
    {
        private Agent _agent;
        private List<AgentType> _agentTypes;

        private bool _isNew = false;

        public Agent Agent
        {
            get => _agent;
            set => Set(ref _agent, value, nameof(Agent));
        }
        public List<AgentType> AgentTypes
        {
            get => _agentTypes;
            set => Set(ref _agentTypes, value, nameof(AgentTypes));
        }

        private string _searchValue;

        public string SearchValue
        {
            get => _searchValue;
            set => Set(ref _searchValue, value, nameof(SearchValue));
        }

        private ProductSale _selectedProductSale;

        public ProductSale SelectedProductSale
        {
            get => _selectedProductSale;
            set => Set(ref _selectedProductSale, value, nameof(SelectedProductSale));
        }

        // Если agentId null, то это означает, что мы 
        // СОЗДАЕМ нового агента
        public AgentViewModel(int? agentId)
        {
            using (ApplicationDbContext context = new())
            {
                AgentTypes = context.AgentTypes.ToList();
            }

            if (agentId == null)
            {
                _isNew = true;
                Agent = new Agent();
                return;
            }

            Agent = GetAgent((int)agentId);
        }

        public void DeleteSelectedProductSale()
        {
            using (ApplicationDbContext context = new())
            {
                context.ProductSales.Remove(SelectedProductSale);
                context.SaveChanges();
            }
            SelectedProductSale = null;
            Agent = GetAgent(Agent.Id);            
        }

        private Agent GetAgent(int agentId)
        {
            using (ApplicationDbContext context = new())
            {
                return context.Agents
                    .Include(a => a.AgentType)
                    .Include(a => a.ProductSales)
                    .ThenInclude(s => s.Product)
                    .Single(a => a.Id == agentId);
            }
        }
    }
}
