//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан по шаблону.
//
//     Изменения, вносимые в этот файл вручную, могут привести к непредвиденной работе приложения.
//     Изменения, вносимые в этот файл вручную, будут перезаписаны при повторном создании кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace VsInsideManagement
{
    using System;
    using System.Collections.Generic;
    
    public partial class Packaging
    {
        public int packaging_id { get; set; }
        public Nullable<int> order_id { get; set; }
        public Nullable<int> product_id { get; set; }
    
        public virtual SetOrder SetOrder { get; set; }
        public virtual Product Product { get; set; }
    }
}
