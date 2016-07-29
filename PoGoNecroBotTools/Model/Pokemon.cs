using System.Globalization;

namespace PoGoNecroBotTools.Model
{
    public class Pokemon
    {
        #region Fields

        private readonly string _name;

        #endregion

        #region Constructors

        public Pokemon(string name, byte level, ushort cp, double iv)
        {
            _name = name;
            Level = level;
            CP = cp;
            IV = iv;
        }

        #endregion

        #region Properties

        // ReSharper disable once InconsistentNaming
        public ushort CP { get; }

        // ReSharper disable once InconsistentNaming
        public double IV { get; }

        public byte Level { get; }

        public string Name => CultureInfo.CurrentCulture.TwoLetterISOLanguageName == "fr" ? ToFrenchPokemonName(_name) : _name;

        #endregion

        #region Methods

        private string ToFrenchPokemonName(string name)
        {
            switch (name)
            {
                case "Bulbasaur":
                    return "Bulbizarre";
                case "Ivysaur":
                    return "Herbizarre";
                case "Venusaur":
                    return "Florizarre";
                case "Charmander":
                    return "Salamèche";
                case "Charmeleon":
                    return "Reptincel";
                case "Charizard":
                    return "Dracaufeu";
                case "Squirtle":
                    return "Carapuce";
                case "Wartortle":
                    return "Carabaffe";
                case "Blastoise":
                    return "Tortank";
                case "Caterpie":
                    return "Chenipan";
                case "Metapod":
                    return "Chrysacier";
                case "Butterfree":
                    return "Papilusion";
                case "Weedle":
                    return "Aspicot";
                case "Kakuna":
                    return "Coconfort";
                case "Beedrill":
                    return "Dardargnan";
                case "Pidgey":
                    return "Roucool";
                case "Pidgeotto":
                    return "Roucoups";
                case "Pidgeot":
                    return "Roucarnage";
                case "Rattata":
                    return "Rattata";
                case "Raticate":
                    return "Rattatac";
                case "Spearow":
                    return "Piafabec";
                case "Fearow":
                    return "Rapasdepic";
                case "Ekans":
                    return "Abo";
                case "Arbok":
                    return "Arbok";
                case "Pikachu":
                    return "Pikachu";
                case "Raichu":
                    return "Raichu";
                case "Sandshrew":
                    return "Sabelette";
                case "Sandslash":
                    return "Sablaireau";
                case "NidoranFemale":
                    return "Nidoran(Femelle)";
                case "Nidorina":
                    return "Nidorina";
                case "Nidoqueen":
                    return "Nidoqueen";
                case "NidoranMale":
                    return "Nidoran(Mâle)";
                case "Nidorino":
                    return "Nidorino";
                case "Nidoking":
                    return "Nidoking";
                case "Clefairy":
                    return "Mélofée";
                case "Clefable":
                    return "Mélodelfe";
                case "Vulpix":
                    return "Goupix";
                case "Ninetales":
                    return "Feunard";
                case "Jigglypuff":
                    return "Rondoudou";
                case "Wigglytuff":
                    return "Grodoudou";
                case "Zubat":
                    return "Nosferapti";
                case "Golbat":
                    return "Nosferalto";
                case "Oddish":
                    return "Mystherbe";
                case "Gloom":
                    return "Ortide";
                case "Vileplume":
                    return "Rafflesia";
                case "Paras":
                    return "Paras";
                case "Parasect":
                    return "Parasect";
                case "Venonat":
                    return "Mimitoss";
                case "Venomoth":
                    return "Aéromite";
                case "Diglett":
                    return "Taupiqueur";
                case "Dugtrio":
                    return "Triopikeur";
                case "Meowth":
                    return "Miaouss";
                case "Persian":
                    return "Persian";
                case "Psyduck":
                    return "Psykokwak";
                case "Golduck":
                    return "Akwakwak";
                case "Mankey":
                    return "Férosinge";
                case "Primeape":
                    return "Colossinge";
                case "Growlithe":
                    return "Caninos";
                case "Arcanine":
                    return "Arcanin";
                case "Poliwag":
                    return "Ptitard";
                case "Poliwhirl":
                    return "Têtarte";
                case "Poliwrath":
                    return "Tartard";
                case "Abra":
                    return "Abra";
                case "Kadabra":
                    return "Kadabra";
                case "Alakazam":
                    return "Alakazam";
                case "Machop":
                    return "Machoc";
                case "Machoke":
                    return "Machopeur";
                case "Machamp":
                    return "Mackogneur";
                case "Bellsprout":
                    return "Chétiflor";
                case "Weepinbell":
                    return "Boustiflor";
                case "Victreebel":
                    return "Empiflor";
                case "Tentacool":
                    return "Tentacool";
                case "Tentacruel":
                    return "Tentacruel";
                case "Geodude":
                    return "Racaillou";
                case "Graveler":
                    return "Gravalanch";
                case "Golem":
                    return "Grolem";
                case "Ponyta":
                    return "Ponyta";
                case "Rapidash":
                    return "Galopa";
                case "Slowpoke":
                    return "Ramoloss";
                case "Slowbro":
                    return "Flagadoss";
                case "Magnemite":
                    return "Magnéti";
                case "Magneton":
                    return "Magnéton";
                case "Farfetch'd":
                    return "Canarticho";
                case "Doduo":
                    return "Doduo";
                case "Dodrio":
                    return "Dodrio";
                case "Seel":
                    return "Otaria";
                case "Dewgong":
                    return "Lamantine";
                case "Grimer":
                    return "Tadmorv";
                case "Muk":
                    return "Grotadmorv";
                case "Shellder":
                    return "Kokiyas";
                case "Cloyster":
                    return "Crustabri";
                case "Gastly":
                    return "Fantominus";
                case "Haunter":
                    return "Spectrum";
                case "Gengar":
                    return "Ectoplasma";
                case "Onix":
                    return "Onix";
                case "Drowzee":
                    return "Soporifik";
                case "Hypno":
                    return "Hypnomade";
                case "Krabby":
                    return "Krabby";
                case "Kingler":
                    return "Krabboss";
                case "Voltorb":
                    return "Voltorbe";
                case "Electrode":
                    return "Électrode";
                case "Exeggcute":
                    return "Nœunœuf";
                case "Exeggutor":
                    return "Noadkoko";
                case "Cubone":
                    return "Osselait";
                case "Marowak":
                    return "Ossatueur";
                case "Hitmonlee":
                    return "Kicklee";
                case "Hitmonchan":
                    return "Tygnon";
                case "Lickitung":
                    return "Excelangue";
                case "Koffing":
                    return "Smogo";
                case "Weezing":
                    return "Smogogo";
                case "Rhyhorn":
                    return "Rhinocorne";
                case "Rhydon":
                    return "Rhinoféros";
                case "Chansey":
                    return "Leveinard";
                case "Tangela":
                    return "Saquedeneu";
                case "Kangaskhan":
                    return "Kangourex";
                case "Horsea":
                    return "Hypotrempe";
                case "Seadra":
                    return "Hypocéan";
                case "Goldeen":
                    return "Poissirène";
                case "Seaking":
                    return "Poissoroy";
                case "Staryu":
                    return "Stari";
                case "Starmie":
                    return "Staross";
                case "Mr.Mime":
                    return "M.Mime";
                case "Scyther":
                    return "Insécateur";
                case "Jynx":
                    return "Lippoutou";
                case "Electabuzz":
                    return "Élektek";
                case "Magmar":
                    return "Magmar";
                case "Pinsir":
                    return "Scarabrute";
                case "Tauros":
                    return "Tauros";
                case "Magikarp":
                    return "Magicarpe";
                case "Gyarados":
                    return "Léviator";
                case "Lapras":
                    return "Lokhlass";
                case "Ditto":
                    return "Métamorph";
                case "Eevee":
                    return "Évoli";
                case "Vaporeon":
                    return "Aquali";
                case "Jolteon":
                    return "Voltali";
                case "Flareon":
                    return "Pyroli";
                case "Porygon":
                    return "Porygon";
                case "Omanyte":
                    return "Amonita";
                case "Omastar":
                    return "Amonistar";
                case "Kabuto":
                    return "Kabuto";
                case "Kabutops":
                    return "Kabutops";
                case "Aerodactyl":
                    return "Ptéra";
                case "Snorlax":
                    return "Ronflex";
                case "Articuno":
                    return "Artikodin";
                case "Zapdos":
                    return "Électhor";
                case "Moltres":
                    return "Sulfura";
                case "Dratini":
                    return "Minidraco";
                case "Dragonair":
                    return "Draco";
                case "Dragonite":
                    return "Dracolosse";
                case "Mewtwo":
                    return "Mewtwo";
                case "Mew":
                    return "Mew";
                default:
                    return name;
            }
        }

        #endregion
    }
}