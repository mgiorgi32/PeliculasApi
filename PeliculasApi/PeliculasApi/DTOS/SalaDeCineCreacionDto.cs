﻿using NetTopologySuite.Geometries;
using System.ComponentModel.DataAnnotations;

namespace PeliculasApi.DTOS
{
    public class SalaDeCineCreacionDto
    {
        [Required]
        [StringLength(120)]
        public string Nombre { get; set; }
        [Range(-90, 90)]
        public double Latitud { get; set; }
        [Range(-180, 180)]
        public double Longitud { get; set; }
        public Point Ubicacion { get; set; }
    }
}
