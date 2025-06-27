using System;
using System.Collections.Generic;

namespace BusinessObjects;

public partial class CosmeticCategory
{
    public string CategoryId { get; set; } = null!;

    public string CategoryName { get; set; } = null!;

    public string UsagePurpose { get; set; } = null!;

    public string FormulationType { get; set; } = null!;

    public virtual ICollection<CosmeticInformation> CosmeticInformations { get; set; } = new List<CosmeticInformation>();
}
