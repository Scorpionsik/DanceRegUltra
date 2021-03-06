﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DanceRegUltra.Models.Categories
{
    public class IdTitle : IComparable<IdTitle>
    {
        public int Id { get; private set; }
        public string Title { get; private set; }

        public IdTitle(int id, string title)
        {
            this.Id = id;
            this.Title = title;
        }

        public int CompareTo(IdTitle other)
        {
            return this.Title.CompareTo(other.Title);
        }
    }
}
