using System;
using System.Collections.Generic;

#nullable disable

namespace RentCollection.NetAPI.Models
{
    public partial class InvoiceItemCategory
    {
        public int InvoiceItemCategoryId { get; set; }
        public int UserId { get; set; }
        public string Code { get; set; }

        public virtual User User { get; set; }
    }
}
