using System;
using System.Collections.Generic;
using System.Text;
using SQLite;

namespace Noted
{
    public class NotedModel
    {
        [PrimaryKey]
        public string NoteName { get; set; }
        public string Note { get; set; }

        public NotedModel(string noteName, string note)
        {
            NoteName = noteName;
            Note = note;
        }

        public NotedModel()
        {
        }
    }
}