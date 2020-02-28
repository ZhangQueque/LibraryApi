using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace EFCoreTest.Entities
{
    public class Book
    {
        [Key]
        public int BookId { get; set; }
        public string Name { get; set; }
        [ForeignKey("AuthorId")]
        public Author Author { get; set; }
        public int AuthorId { get; set; }
    }
}
