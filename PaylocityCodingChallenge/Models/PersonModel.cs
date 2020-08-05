using PaylocityCodingChallenge.Code;
using System;

namespace PaylocityCodingChallenge.Models
{
    public abstract class PersonModel : IUniqueId
    {
        public Guid Id { get; private set; }
        public abstract PersonTypeEnum PersonType { get; }
        public string Name { get; set; }

        public PersonModel()
        {
            Id = Guid.NewGuid();
        }
    }
}
