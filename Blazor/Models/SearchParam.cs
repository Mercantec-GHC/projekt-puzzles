/// <summary>
/// Specifies the range type for filtering search results.
/// </summary>
public enum filterRangeType { Pieces, Price }

/// <summary>
/// Specifies the field by which to order search results.
/// </summary>
public enum orderByType { PieceAmount, Price, CreatedAt }

/// <summary>
/// Specifies the order direction for sorting search results.
/// </summary>
public enum inOrderType { ASC, DESC }

/// <summary>
/// Represents the parameters used for searching and filtering puzzles.
/// </summary>
public class SearchParam
{
    /// <summary>
    /// Gets or sets the search text used to filter results.
    /// </summary>
    public string SearchText { get; set; } = null;

    /// <summary>
    /// Specifies the field by which to order the results.
    /// </summary>
    public orderByType OrderBy = orderByType.CreatedAt;

    /// <summary>
    /// Specifies the direction in which to order the results.
    /// </summary>
    public inOrderType InOrder = inOrderType.DESC;

    /// <summary>
    /// Specifies the range type for filtering results.
    /// </summary>
    public filterRangeType FilterRange = filterRangeType.Pieces;

    /// <summary>
    /// Gets or sets the minimum value for the filter range.
    /// </summary>
    public int? RangeMin { get; set; } = null;

    /// <summary>
    /// Gets or sets the maximum value for the filter range.
    /// </summary>
    public int? RangeMax { get; set; }
}
namespace Blazor.Models
{
    public enum filterRangeType { Pieces, Price}
    public enum orderByType { PieceAmount, Price, CreatedAt }

    public enum inOrderType { ASC, DESC }

    // class used for the search parameters
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
