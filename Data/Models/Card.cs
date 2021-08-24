using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyTCGCollection.Data.Models
{
    [Table("Cards")]
    public class Card
    {
        [Key]
        [Required]

        public int Id { get; set; }
        public string CardNumber { get; set; }
        public string CardName { get; set; }
        public string CardRarity { get; set; }
        public int Quantity { get; set; }
        public string CardValue { get; set; }
        [ForeignKey(nameof(Game))]
        public int GameId { get; set; }
        public string Expansion { get; set; }


    }
}
