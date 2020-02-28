using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EFCoreTest.Models
{
    public class BookDto : ReponseReSource
    {
        public int BookId { get; set; }
        public string Name { get; set; }
        public int AuthorId { get; set; }
    }
}
