using GodsUnchained_Deck_Tracker.Controller;
using GodsUnchained_Deck_Tracker.Model.Enums;
using System;

namespace GodsUnchained_Deck_Tracker.Model.Entities
{
    public class Card : IComparable<Card>
    {

        public Card(string name) {
            Prototype = new Prototype(name);
        }

        public Card(Card card) {
            Prototype = card.Prototype;
        }

        public Card(string name, bool getPrototype) {
            if (getPrototype) {
                Prototype = CardPrototypeController.GetPrototypeByName(name);
            }
        }

        //TODO: make this constructor better
        public Card(string name, string prototypeId, bool getPrototype) {
            if (getPrototype) {
                Prototype = CardPrototypeController.GetPrototypeById(prototypeId);
            }
        }

        public User User { get; set; }

        public CardQuality Quality { get; set; }

        public Prototype Prototype { get; set; }

        public int CompareTo(Card other) {
            // TODO: Handle x or y being null, or them not having names
            return Prototype.Name.CompareTo(other.Prototype.Name);
        }

        public override int GetHashCode() => Prototype.Name?.GetHashCode() ?? 0;

        public override bool Equals(object other) => (other as Card)?.Prototype.Name == Prototype.Name;
    }
}
