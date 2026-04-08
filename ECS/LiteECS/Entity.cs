using System;

namespace LiteECS
{
    public readonly struct Entity : IEquatable<Entity>
    {
        public int Id { get; }
        public int Generation { get; }

        internal Entity(int id, int generation)
        {
            Id = id;
            Generation = generation;
        }

        public static Entity Null => new Entity(-1, -1);
        public bool IsValid => Id >= 0;

        public bool Equals(Entity other) => Id == other.Id && Generation == other.Generation;
        public override bool Equals(object obj) => obj is Entity other && Equals(other);
        public override int GetHashCode() => HashCode.Combine(Id, Generation);
        public static bool operator ==(Entity left, Entity right) => left.Equals(right);
        public static bool operator !=(Entity left, Entity right) => !left.Equals(right);
    }
}