using System;
using System.Collections.Generic;

namespace ToDoApi;

/// <summary>
/// Represents a user in the ToDo application.
/// </summary>
public partial class User
{
     /// <summary>
    /// Gets or sets the unique identifier for the user.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the name of the user.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Gets or sets the password for the user.
    /// </summary>
    public string? Password { get; set; }
}
