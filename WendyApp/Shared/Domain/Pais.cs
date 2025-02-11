﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

#nullable disable

namespace WendyApp.Shared.Domain
{
    public class Pais
    {

        public int PaisId { get; set; }
        public string Nombre { get; set; }
        public virtual List<PaisProveedor> Proveedores { get; set; }
        public virtual List<Sucursal> Sucursales { get; set; }
    }
}
