//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан по шаблону.
//
//     Изменения, вносимые в этот файл вручную, могут привести к непредвиденной работе приложения.
//     Изменения, вносимые в этот файл вручную, будут перезаписаны при повторном создании кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace InfoSystemDB
{
    using System;
    using System.Collections.Generic;
    
    public partial class Orders
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Orders()
        {
            this.Packaging = new HashSet<Packaging>();
        }
    
        public int id { get; set; }
        public Nullable<System.DateTime> ddate { get; set; }
        public Nullable<int> del { get; set; }
        public Nullable<int> amount { get; set; }
        public Nullable<int> discount { get; set; }
        public Nullable<int> prepay { get; set; }
        public Nullable<int> amount_due { get; set; }
        public string invoice { get; set; }
        public string stat { get; set; }
        public string note { get; set; }
        public Nullable<int> shop { get; set; }
    
        public virtual Delivery Delivery { get; set; }
        public virtual Discounts Discounts { get; set; }
        public virtual Shops Shops { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Packaging> Packaging { get; set; }
    }
}
