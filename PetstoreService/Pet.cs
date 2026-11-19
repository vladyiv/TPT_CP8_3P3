using System;
using System.Collections.Generic;
using System.Text;

namespace PetstoreService
{
    public class Pet
    {
        public long Id { get; set; }
        public Category Category { get; set; }
        public string Name { get; set; }
        public List<string> PhotoUrls { get; set; }
        public List<Tag> Tags { get; set; }
        public string Status { get; set; }
    }

    public class Category
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public Category(long id, string name)
        {
            Id = id;
            Name = name;
        }
    }

    public class Tag
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public Tag(long id, string name)
        {
            Id = id;
            Name = name;
        }
    }

}
