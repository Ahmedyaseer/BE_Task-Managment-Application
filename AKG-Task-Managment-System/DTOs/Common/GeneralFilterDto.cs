namespace AKG_Task_Managment_System.DTOs.Common
{
    public class GeneralFilterDto
    {
        public int Page { get; set; } = 0;
        public int Size { get; set; } = 10;
        public string SortBy { get; set; } = "Id";
        public string SortDirection { get; set; } = "desc";
        public string? GlobalSearch { get; set; }
        public List<FilterCriteria> Filters { get; set; } = new();
    }

    public class FilterCriteria
    {
        public string Field { get; set; } = string.Empty;
        public string Operator { get; set; } = "EQUALS";
        public string Value { get; set; } = string.Empty;
    }
}
