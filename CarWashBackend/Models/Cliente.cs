﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace CarWashBackend.Models;

public partial class Cliente
{
    public string id { get; set; }

    public string nombre { get; set; }

    public string correo { get; set; }

    public string telefono { get; set; }

    public string genero { get; set; }

    public bool? activo { get; set; }

    public DateTime? created_at { get; set; }

    public DateTime? updated_at { get; set; }

    public virtual ICollection<registro_servicio> registro_servicios { get; set; } = new List<registro_servicio>();

    public virtual ICollection<Vehiculo> vehiculos { get; set; } = new List<Vehiculo>();
}