﻿using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

#nullable disable

namespace WendyApp.Server.Models
{
    public class InsumoDTO
    {
        public int InsumoId { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public DateTime FechaCreacion { get; set; }

        
        public virtual List<InsumoCategoriaDTO> Categorias { get; set; }
        
        public virtual List<PedidoInsumoDTO> Pedidos { get; set; }
        
        public virtual List<ProveedorInsumoDTO> Proveedores { get; set; }
    }
}
