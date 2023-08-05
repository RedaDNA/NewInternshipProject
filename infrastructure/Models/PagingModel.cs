﻿namespace Infrastructure.Models
{
    public class PagingModel<T> where T : class
    {
    public PagingModel() { }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int RowsPerPage { get; set; }
        public int TotalRows { get; set; }
        public int OrdersPerPage { get; set; }
        public IEnumerable<T> Results { get; set; } = Array.Empty<T>();

    }
}
