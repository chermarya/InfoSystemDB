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
    
    public partial class Making
    {
        public int making_id { get; set; }
        public Nullable<int> supply_id { get; set; }
        public Nullable<int> product_id { get; set; }
        public int quantity { get; set; }
    
        public virtual Product Product { get; set; }
        public virtual Supply Supply { get; set; }
    }
}
