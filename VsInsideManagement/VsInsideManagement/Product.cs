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
    
    public partial class Product
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Product()
        {
            this.Making = new HashSet<Making>();
            this.Packaging = new HashSet<Packaging>();
        }
    
        public int product_id { get; set; }
        public int prodtype_id { get; set; }
        public string title { get; set; }
        public Nullable<int> size_id { get; set; }
        public Nullable<int> color_id { get; set; }
        public int price { get; set; }
        public int quantity { get; set; }
    
        public virtual Color Color { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Making> Making { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Packaging> Packaging { get; set; }
        public virtual ProdType ProdType { get; set; }
        public virtual Size Size { get; set; }
    }
}