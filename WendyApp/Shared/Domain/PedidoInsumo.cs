﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace WendyApp.Shared.Domain
{
    [Table("PedidosInsumos")]
    public class PedidoInsumo
    {
        public int Id { get; set; }
        [ForeignKey(nameof(Insumo))]
        public int InsumoId { get; set; }
        public int Cantidad { get; set; }
        public decimal Precio { get; set; }
        public virtual Insumo Insumo { get; set; }

        [ForeignKey(nameof(Pedido))]
        public int PedidoId { get; set; }
        public virtual Pedido Pedido { get; set; }

    }
}