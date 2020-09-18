﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlazorFormSample.Shared
{
    public class Item
    {
        [Key]
        [Column("ItemId")]
        public Guid Id { get; set; }

        [Required]
        [RegularExpression("^(?! )[A-Za-z0-9 \']*(?<! )$")]
        public string Name { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public decimal Weight { get; set; }

        [Required]
        public GameSystem GameSystem { get; set; }
    }
}
