using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyTCGCollection.Data.Models
{
    [Table("Games")]
    public class Game
    {
        [Key]
        [Required]
        public int Id { get; set; }

        public string GameName { get; set; }
        public virtual List<Card> Cards { get; set; }

    }
}
