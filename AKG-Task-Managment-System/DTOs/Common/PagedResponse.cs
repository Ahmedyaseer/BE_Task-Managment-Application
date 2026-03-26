namespace AKG_Task_Managment_System.DTOs.Common
{
    public class PagedResponse<T>
    {
        public List<T> Content { get; set; } = new();
        public int TotalElements { get; set; }
        public int TotalPages { get; set; }
        public int Size { get; set; }
        public int Page { get; set; }
    }
}
