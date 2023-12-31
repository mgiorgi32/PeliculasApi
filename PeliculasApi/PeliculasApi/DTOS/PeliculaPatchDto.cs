﻿using System.ComponentModel.DataAnnotations;

namespace PeliculasApi.DTOS
{
    public class PeliculaPatchDto
    {
        [Required]
        [StringLength(300)]
        public string Titulo { get; set; }
        public bool EnCines { get; set; }
        public DateTime FechaEstreno { get; set; }
    }
}
