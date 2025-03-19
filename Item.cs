using System;
using System.Collections.Generic;

namespace ToDoApi;

/// <summary>
/// Represents an item in the ToDo application.
/// </summary>
public partial class Item
{

     /// <summary>
    /// Gets or sets the unique identifier for the item.
    /// </summary>
    public int Id { get; set; }

        /// <summary>
    /// Gets or sets the name of the item.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the item is complete.
    /// </summary>
    public bool? IsComplete { get; set; }
}
