namespace REAL_STATE.Models.VM
{
    public class DashboardVM
    {
        
            // Property cards
            public int TotalProperties { get; set; }
            public int AvailableProperties { get; set; }
            public int FeaturedProperties { get; set; }
            public int ForSale { get; set; }
            public int ForRent { get; set; }

            // People cards
            public int TotalDevelopers { get; set; }
            public int ActiveDevelopers { get; set; }
            public int TotalAgents { get; set; }
            public int ActiveAgents { get; set; }
            public int TotalUsers { get; set; }
            public int LockedUsers { get; set; }

            // Panels
            public List<Property> RecentProperties { get; set; } = new();
            public List<StatusCount> PropertiesByStatus { get; set; } = new();
            public List<StatusCount> PropertiesByType { get; set; } = new();
        

            public class StatusCount
            {
                public string Label { get; set; } = string.Empty;
                public int Count { get; set; }
            }
    }
}
