namespace Blazor.Models
{
    public enum filterRangeType { Pieces, Price}
    public enum orderByType { Pieces, Price, CreatedAt }

    public enum inOrderType { asc, desc }
    public class SearchParam
    {
        public string SearchText { get; set; } = null;
        public orderByType OrderBy = orderByType.CreatedAt;
        public inOrderType InOrder = inOrderType.desc;
        public filterRangeType FilterRange = filterRangeType.Pieces;
        public int RangeMin { get; set; }
        public int? RangeMax { get; set; }
    }
}
