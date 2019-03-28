using SQLite.Net.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StartFinance.Models
{
    public class ShoppingList
    {
        [PrimaryKey, AutoIncrement]
        public int ShoppingItemID { get; set; }

        [NotNull]
        public string NameOfItem { get; set; }

        [NotNull]
        public double PriceQuoted { get; set; }

        [NotNull]
        public string ShopName { get; set; }

        [NotNull]
        public String ShoppingDate { get; set; }
    }
}
