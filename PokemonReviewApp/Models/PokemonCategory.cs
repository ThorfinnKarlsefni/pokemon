using System;
namespace PokemonReviewApp.Models
{
    public class PokemonCategory
    {
        public Pokemon PokemonId { get; set; }
        public Category CategoryId { get; set; }
        public Pokemon Pokemon { get; set; }
        public Category Category { get; set; }
    }
}

