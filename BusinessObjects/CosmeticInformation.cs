using System;
using System.Collections.Generic;

namespace BusinessObjects;

public partial class CosmeticInformation
{
    public string CosmeticId { get; set; } = null!;

    public string CosmeticName { get; set; } = null!;

    public string SkinType { get; set; } = null!;

    public string ExpirationDate { get; set; } = null!;

    public string CosmeticSize { get; set; } = null!;

    public decimal DollarPrice { get; set; }

    public string? CategoryId { get; set; }

    public virtual CosmeticCategory? Category { get; set; }
}
