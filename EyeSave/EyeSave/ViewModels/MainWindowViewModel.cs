using EyeSave.Entities;
using EyeSave;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System;

namespace EyeSave.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {        
        private List<Agent> _agents;
        private Agent _selectedAgent;

        private string _selectedSort;
        private string _selectedFilter;
        private string _searchValue;
        private List<Agent> _displayingAgents;

        public List<Agent> DisplayingAgents
        { 
            get => _displayingAgents;
            set => Set(ref _displayingAgents, value, nameof(DisplayingAgents)); 
        }

        public Agent SelectedAgent
        {
            get => _selectedAgent;
            set => Set(ref _selectedAgent, value, nameof(SelectedAgent));
        }

        public string SelectedSort
        {
            get => _selectedSort;
            set
            {
                if (Set(ref _selectedSort, value, nameof(SelectedSort)))
                    DisplayAgents();
            }
        }
        public string SelectedFilter
        {
            get => _selectedFilter;
            set
            {
                if(Set(ref _selectedFilter, value, nameof(SelectedFilter)))
                    DisplayAgents();
            }
        }
        public string SearchValue
        {
            get => _searchValue;
            set
            {
                if(Set(ref _searchValue, value, nameof(Search)))
                    DisplayAgents();
            }
        }

        public List<string> SortList { get; } = new List<string>
        {
            "Без сортировки",
            "Наименование (возр)",
            "Наименование (уб)",
            "Размер скидки (возр)",
            "Размер скидки (уб)",
            "Приоритет (возр)",
            "Приоритет (уб)"
        };
        public List<string> FilterList { get; } = new List<string>
        {
            "Все типы"
        };

        #region Paging

        public record Page(int pageNum);
        private const int PageSize = 10;
        private List<Page> _pages;
        private Page _selectedPage;

        public List<Page> Pages
        {
            get => _pages;
            set => Set(ref _pages, value, nameof(Pages));
        }
        public Page SelectedPage
        {
            get => _selectedPage;
            set
            {
                if (Set(ref _selectedPage, value, nameof(SelectedPage)))
                    DisplayAgents();
            }
        }

        private List<Page> GetPages(int itemCount)
        {
            double pageCount = Math.Ceiling((double)itemCount / PageSize);            
            var list = new List<Page>((int)pageCount);
            list.Add(new Page(1));
            for (int i = 1; i < pageCount; i++)
                list.Add(new Page(i + 1));
            return list;
        }

        #endregion

        public MainWindowViewModel()
        {
            using (ApplicationDbContext context = new())
            {
                FilterList.AddRange(context.AgentTypes.Select(at => at.Title));
                
                _agents = context.Agents.AsNoTracking()
                    .Include(a => a.AgentType)
                    .Include(a => a.ProductSales)
                    .ThenInclude(ps => ps.Product)
                    .OrderBy(a => a.Id)
                    .ToList();
            }
            _displayingAgents = new List<Agent>(_agents);
            _selectedFilter = FilterList[0];
            _selectedSort = SortList[0];
            _pages = GetPages(_displayingAgents.Count);
            _selectedPage = _pages[0];
        }

        private void DisplayAgents()
        {            
            var list = Sort(Search(Filter(_agents))).ToList();
            // Получаем страницы на основе количества элементов после
            // фильтрации, поиска и сортировки
            Pages = GetPages(list.Count);
            // SelectedPage может быть null, если мы были на 5 странице,
            // а потом применили фильтр и их стало меньше
            var pageNum = SelectedPage == null
                ? 1
                : SelectedPage.pageNum;

            DisplayingAgents = Paging(list, pageNum, PageSize).ToList();

            SelectedPage ??= Pages[0];
        }

        private IEnumerable<Agent> Sort(IEnumerable<Agent> agents)
        {
            var sort = SelectedSort;

            if (sort == SortList[1])
                return agents.OrderBy(a => a.Title);
            else if (sort == SortList[2])
                return agents.OrderByDescending(a => a.Title);
            else if (sort == SortList[3])
                return agents.OrderBy(a => a.Discount);
            else if (sort == SortList[4])
                return agents.OrderByDescending(a => a.Discount);
            else if (sort == SortList[5])
                return agents.OrderBy(a => a.Priority);
            else if (sort == SortList[6])
                return agents.OrderByDescending(a => a.Priority);
            else
                return agents;
        }

        private IEnumerable<Agent> Filter(IEnumerable<Agent> agents)
        {
            var filter = SelectedFilter;

            if (filter == FilterList[0])
                return agents;
            else
                return agents.Where(a => a.AgentType.Title == filter);
        }

        private IEnumerable<Agent> Search(IEnumerable<Agent> agents)
        {
            var search = SearchValue == null
                ? string.Empty
                : SearchValue.ToLower();

            return (search == string.Empty) || (search == null)
                ? agents
                : agents.Where(a => (a.Title.Contains(search) || a.Phone.Contains(search) || a.Email.Contains(search)));
        }

        private IEnumerable<Agent> Paging(IEnumerable<Agent> agents, int pageNum, int pageSize)
        {            
            if (pageNum > 0)
                agents = agents.Skip((pageNum - 1) * pageSize);

            return agents.Take(pageSize);
        }        
    }
}
