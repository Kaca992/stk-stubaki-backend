//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace StkStubaki.DatabaseModel
{
    using System;
    using System.Collections.Generic;
    
    public partial class SetMec
    {
        public int IdSet { get; set; }
        public int RedniBrSet { get; set; }
        public int PoenDom { get; set; }
        public int PoenGost { get; set; }
        public int IdMec { get; set; }
    
        public virtual Mec Mec { get; set; }
    }
}
