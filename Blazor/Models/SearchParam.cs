namespace Blazor.Models
{
    public enum filterRangeType { Pieces, Price}
    public enum orderByType { PieceAmount, Price, CreatedAt }

    public enum inOrderType { ASC, DESC }
    public class SearchParam
    {
        public string SearchText { get; set; } = null;
        public orderByType OrderBy = orderByType.CreatedAt;
        public inOrderType InOrder = inOrderType.DESC;
        public filterRangeType FilterRange = filterRangeType.Pieces;
        public int? RangeMin { get; set; } = null;
        public int? RangeMax { get; set; }
    }
}
