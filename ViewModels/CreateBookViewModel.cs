﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using IndyBooks.Models;

namespace IndyBooks.ViewModels
{
    public class CreateBookViewModel
    {
        public long BookId { get; set; }
        public string Title { get; set; }
        public string SKU { get; set; }
        [DataType(DataType.Currency)]
        public decimal Price { get; set; }
        [Display(Name = "OR enter new Author")]
        public string Name { get; set; }

        //Add properties to support a Writer's SelectList (Id and Writers)
        public long AuthorId { get; set; }
        public IEnumerable<Writer> Writers { get; set; }
    }
}