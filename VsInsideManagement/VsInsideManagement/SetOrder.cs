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
    
    public partial class SetOrder
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public SetOrder()
        {
            this.Packaging = new HashSet<Packaging>();
        }
    
        public int order_id { get; set; }
        public System.DateTime ddate { get; set; }
        public Nullable<int> delivery_id { get; set; }
        public int summ { get; set; }
        public Nullable<int> discount_id { get; set; }
        public int prepay { get; set; }
        public int amount_due { get; set; }
        public string invoice { get; set; }
        public string stat { get; set; }
        public string note { get; set; }
        public Nullable<int> shop_id { get; set; }
    
        public virtual Delivery Delivery { get; set; }
        public virtual Discount Discount { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Packaging> Packaging { get; set; }
        public virtual Shop Shop { get; set; }
    }
}
