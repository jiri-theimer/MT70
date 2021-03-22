using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BO
{
    public enum x20EntryModeENUM
    {
        Combo = 1,
        InsertUpdateWithoutCombo = 2,
        ExternalByWorkflow = 3
    }

    public enum x20GridColumnENUM
    {
        EntityColumn = 1,
        CategoryColumn = 2,
        Both = 3,
        _None = 4
    }

    public enum x20EntityPageENUM
    {
        Label = 1,
        Hyperlink = 2,
        HyperlinkPlusNew = 3,
        NotUsed = 9
    }
    public class x20EntiyToCategory:BaseBO
    {
        public int x20ID { get; set; }
        public int x18ID { get; set; }
        public int x29ID { get; set; }
        public string x20Name { get; set; }
        public bool x20IsMultiSelect { get; set; }
        public bool x20IsEntryRequired { get; set; }
        public int x20EntityTypePID { get; set; }
        public int x29ID_EntityType { get; set; }
        public x20EntryModeENUM x20EntryModeFlag { get; set; } = x20EntryModeENUM.Combo;
        public x20GridColumnENUM x20GridColumnFlag { get; set; } = x20GridColumnENUM.EntityColumn;

        public string EntityTypeAlias { get; set; }   // pomocný atribut - není v SQL
        public bool x20IsClosed { get; set; }
        public int x20Ordinary { get; set; }
        public x20EntityPageENUM x20EntityPageFlag { get; set; } = x20EntityPageENUM.Label;
    }
}
