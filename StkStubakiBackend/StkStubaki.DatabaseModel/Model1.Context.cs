﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class StkStubakiEntities : DbContext
    {
        public StkStubakiEntities()
            : base("name=StkStubakiEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Igrac> Igracs { get; set; }
        public virtual DbSet<IgraZza> IgraZzas { get; set; }
        public virtual DbSet<Korisnici> Korisnicis { get; set; }
        public virtual DbSet<Mec> Mecs { get; set; }
        public virtual DbSet<Momcad> Momcads { get; set; }
        public virtual DbSet<Natjece> Natjeces { get; set; }
        public virtual DbSet<Par> Pars { get; set; }
        public virtual DbSet<SetMec> SetMecs { get; set; }
        public virtual DbSet<SetPar> SetPars { get; set; }
        public virtual DbSet<Sezona> Sezonas { get; set; }
        public virtual DbSet<Utakmica> Utakmicas { get; set; }
    }
}
