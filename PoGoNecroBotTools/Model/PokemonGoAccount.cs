using System.Collections.ObjectModel;

namespace PoGoNecroBotTools.Model
{
    public class PokemonGoAccount
    {
        #region Fields

        private readonly ObservableCollection<Pokemon> _pokemons = new ObservableCollection<Pokemon>();

        #endregion

        #region Constructors

        public PokemonGoAccount(string accountName)
        {
            Name = accountName;
            Pokemons = new ReadOnlyObservableCollection<Pokemon>(_pokemons);
        }

        #endregion

        #region Properties

        public string Name { get; }

        public ReadOnlyCollection<Pokemon> Pokemons { get; }

        #endregion

        #region Methods

        public void AddPokemon(Pokemon pokemon)
        {
            _pokemons.Add(pokemon);
        }

        public void ClearPokemons()
        {
            _pokemons.Clear();
        }

        #endregion
    }
}