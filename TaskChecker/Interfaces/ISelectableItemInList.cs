using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskChecker.Interfaces
{
    public interface ISelectableItemInList
    {
        /// <summary>
        /// Gets or sets a value indicating whether this instance is selected.
        /// </summary>
        bool IsSelected { get; set; }

        /// <summary>
        /// Gets the selectable itemslist.
        /// </summary>
        IEnumerable<ISelectableItemInList> SelectableItemslist { get; }
    }
}